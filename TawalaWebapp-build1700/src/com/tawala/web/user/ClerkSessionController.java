package com.tawala.web.user;

import java.io.BufferedReader;
import java.math.BigInteger;
import java.net.HttpURLConnection;
import java.net.URI;
import java.net.URL;
import java.nio.charset.Charset;
import java.security.KeyFactory;
import java.security.PublicKey;
import java.security.SecureRandom;
import java.security.Signature;
import java.security.spec.RSAPublicKeySpec;
import java.util.Locale;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONArray;
import org.json.JSONObject;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.domain.EmailAddress;
import com.tawala.domain.User;
import com.tawala.web.WorldInitializer;

/**
 * Clerk owns sign-up and login. This accepts a Clerk session token, checks
 * Clerk's signature, and opens the existing Tawala session for that person.
 * A Clerk user whose signed token includes a user name opens that existing
 * Tawala account. The name is taken from the token, not from the browser.
 * A Clerk user with no matching Tawala account becomes a new account.
 */
public class ClerkSessionController implements Controller {
	private static final String ISSUER = "https://rare-bat-6337.clerk.accounts.dev";
	private static final String JWKS_URL = ISSUER + "/.well-known/jwks.json";
	private static final Charset UTF8 = Charset.forName("UTF-8");
	private static final SecureRandom RANDOM = new SecureRandom();
	private static final long CLOCK_SKEW_SECONDS = 60;
	private static volatile String cachedJwks = null;
	private static volatile long cachedJwksAt = 0;

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		if (!"POST".equals(request.getMethod())) {
			response.sendError(HttpServletResponse.SC_METHOD_NOT_ALLOWED);
			return null;
		}

		JSONObject body = readBody(request);
		if (body == null) {
			response.sendError(HttpServletResponse.SC_BAD_REQUEST);
			return null;
		}
		String token = body.optString("token", "");
		JSONObject claims;
		try {
			claims = verify(token);
		} catch (IllegalArgumentException e) {
			Log.info(this, "Rejected Clerk session: " + e.getMessage());
			response.sendError(HttpServletResponse.SC_UNAUTHORIZED);
			return null;
		}

		String clerkId = claims.getString("sub");
		if (!allowedOrigin(request, claims.optString("azp", ""))) {
			Log.info(this, "Rejected Clerk session for another site.");
			response.sendError(HttpServletResponse.SC_UNAUTHORIZED);
			return null;
		}

		User user = existingAccount(claims);
		if (user == null) {
			user = WorldInitializer.getDefaultWorld().domain().users().get(clerkId);
		}
		if (user == null) {
			user = createAccount(clerkId, body);
		}
		if (user == null || user.isSuspended() || !user.getStatus().isAllowedToLogOn()) {
			response.sendError(HttpServletResponse.SC_FORBIDDEN);
			return null;
		}

		LoginController.setupUserSession(request, response, user, true);
		response.setStatus(HttpServletResponse.SC_OK);
		response.setContentType("text/plain");
		response.getWriter().write("ok");
		return null;
	}

	private static User existingAccount(JSONObject claims) {
		String name = claims.optString("username", "");
		if (!name.matches("[A-Za-z0-9_]{1,40}")) {
			return null;
		}
		return WorldInitializer.getDefaultWorld().domain().users().get(name);
	}

	private User createAccount(String clerkId, JSONObject body) {
		String password = randomPassword();
		User created = new User(clerkId, clip(body.optString("firstName", ""), 20),
				clip(body.optString("lastName", ""), 30), emailOrNull(body
						.optString("email", "")), password);
		try {
			WorldInitializer.getDefaultWorld().domain().users()
					.onUserRegistration(created);
		} catch (DataIntegrityViolationException already) {
			return WorldInitializer.getDefaultWorld().domain().users().get(clerkId);
		} catch (Exception e) {
			User existing = WorldInitializer.getDefaultWorld().domain().users()
					.get(clerkId);
			if (existing != null) {
				return existing;
			}
			Log.warn(this, "Could not open a Tawala account for a Clerk user.", e);
			return null;
		}
		return WorldInitializer.getDefaultWorld().domain().users().get(clerkId);
	}

	private static JSONObject verify(String token) throws Exception {
		if (token == null || token.length() < 20 || token.length() > 8000) {
			throw new IllegalArgumentException("missing token");
		}
		String[] parts = token.split("\\.");
		if (parts.length != 3) {
			throw new IllegalArgumentException("malformed token");
		}
		JSONObject header = new JSONObject(new String(base64Url(parts[0]), UTF8));
		if (!"RS256".equals(header.optString("alg"))) {
			throw new IllegalArgumentException("unexpected algorithm");
		}
		String kid = header.optString("kid", "");
		PublicKey key = keyFor(kid);
		if (key == null) {
			throw new IllegalArgumentException("unknown signing key");
		}
		Signature signature = Signature.getInstance("SHA256withRSA");
		signature.initVerify(key);
		signature.update((parts[0] + "." + parts[1]).getBytes(UTF8));
		if (!signature.verify(base64Url(parts[2]))) {
			throw new IllegalArgumentException("bad signature");
		}

		JSONObject claims = new JSONObject(new String(base64Url(parts[1]), UTF8));
		if (!ISSUER.equals(claims.optString("iss"))) {
			throw new IllegalArgumentException("unexpected issuer");
		}
		String sub = claims.optString("sub", "");
		if (!sub.matches("user_[A-Za-z0-9]{8,34}") || sub.length() > 40) {
			throw new IllegalArgumentException("unexpected account id");
		}
		long now = System.currentTimeMillis() / 1000L;
		if (claims.optLong("exp", 0) < now - CLOCK_SKEW_SECONDS) {
			throw new IllegalArgumentException("expired");
		}
		if (claims.has("nbf")
				&& claims.optLong("nbf", 0) > now + CLOCK_SKEW_SECONDS) {
			throw new IllegalArgumentException("not yet valid");
		}
		return claims;
	}

	private static boolean allowedOrigin(HttpServletRequest request, String azp) {
		if (azp == null || azp.length() == 0) {
			return false;
		}
		try {
			String host = new URI(azp).getHost();
			if (host == null) {
				return false;
			}
			host = host.toLowerCase(Locale.US);
			String server = request.getServerName() == null ? "" : request
					.getServerName().toLowerCase(Locale.US);
			return host.equals(server) || "teacherexams.org".equals(host)
					|| "localhost".equals(host) || "127.0.0.1".equals(host);
		} catch (Exception e) {
			return false;
		}
	}

	private static PublicKey keyFor(String kid) throws Exception {
		if (kid == null || kid.length() == 0) {
			return null;
		}
		PublicKey key = keyFromJwks(jwks(false), kid);
		if (key == null) {
			key = keyFromJwks(jwks(true), kid);
		}
		return key;
	}

	private static String jwks(boolean force) throws Exception {
		long now = System.currentTimeMillis();
		if (!force && cachedJwks != null && now - cachedJwksAt < 10 * 60 * 1000L) {
			return cachedJwks;
		}
		HttpURLConnection connection = (HttpURLConnection) new URL(JWKS_URL)
				.openConnection();
		connection.setConnectTimeout(5000);
		connection.setReadTimeout(5000);
		connection.setRequestMethod("GET");
		try {
			if (connection.getResponseCode() != 200) {
				throw new IllegalStateException("Clerk signing keys were unavailable.");
			}
			BufferedReader reader = new BufferedReader(
					new java.io.InputStreamReader(connection.getInputStream(), UTF8));
			StringBuilder body = new StringBuilder();
			String line;
			while ((line = reader.readLine()) != null) {
				body.append(line);
			}
			reader.close();
			cachedJwks = body.toString();
			cachedJwksAt = now;
			return cachedJwks;
		} finally {
			connection.disconnect();
		}
	}

	private static PublicKey keyFromJwks(String jwks, String kid) throws Exception {
		JSONArray keys = new JSONObject(jwks).getJSONArray("keys");
		for (int i = 0; i < keys.length(); i++) {
			JSONObject key = keys.getJSONObject(i);
			if (!kid.equals(key.optString("kid"))) {
				continue;
			}
			if (!"RSA".equals(key.optString("kty"))
					|| !"RS256".equals(key.optString("alg"))) {
				return null;
			}
			BigInteger modulus = new BigInteger(1, base64Url(key.getString("n")));
			BigInteger exponent = new BigInteger(1, base64Url(key.getString("e")));
			return KeyFactory.getInstance("RSA").generatePublic(
					new RSAPublicKeySpec(modulus, exponent));
		}
		return null;
	}

	private static JSONObject readBody(HttpServletRequest request) throws Exception {
		BufferedReader reader = request.getReader();
		StringBuilder body = new StringBuilder();
		char[] buf = new char[1024];
		int total = 0;
		int n;
		while ((n = reader.read(buf)) >= 0) {
			total += n;
			if (total > 12000) {
				return null;
			}
			body.append(buf, 0, n);
		}
		if (body.length() == 0) {
			return null;
		}
		return new JSONObject(body.toString());
	}

	private static EmailAddress emailOrNull(String email) {
		if (email == null) {
			return null;
		}
		email = email.trim();
		if (email.length() < 3 || email.length() > 100 || email.indexOf(' ') >= 0
				|| email.indexOf("..") >= 0 || email.indexOf('@') < 1
				|| email.indexOf('@') != email.lastIndexOf('@')) {
			return null;
		}
		return new EmailAddress(email);
	}

	private static String clip(String value, int max) {
		if (value == null) {
			return "";
		}
		value = value.trim();
		if (value.length() > max) {
			return value.substring(0, max);
		}
		return value;
	}

	private static byte[] base64Url(String value) {
		String padded = value.replace('-', '+').replace('_', '/');
		int pad = padded.length() % 4;
		if (pad != 0) {
			padded = padded + "====".substring(pad);
		}
		return javax.xml.bind.DatatypeConverter.parseBase64Binary(padded);
	}

	private static String randomPassword() {
		byte[] bytes = new byte[18];
		RANDOM.nextBytes(bytes);
		return javax.xml.bind.DatatypeConverter.printBase64Binary(bytes);
	}
}
