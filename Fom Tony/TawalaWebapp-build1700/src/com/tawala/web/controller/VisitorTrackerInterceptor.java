package com.tawala.web.controller;

import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.ModelAndView;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.domain.Visitor;
import com.tawala.event.Event;
import com.tawala.web.WorldInitializer;

public class VisitorTrackerInterceptor implements HandlerInterceptor {

	public static final String USER_PREFIX = "b";
	public static final String VISITOR_PREFIX = "a";
	private static final String SEPARATOR = ":";
	
	public static final String TAWALA_TOKEN_COOKIE_NAME = "token";

	public void postHandle(HttpServletRequest request,
			HttpServletResponse response, Object handler,
			ModelAndView modelAndView) throws Exception {
		// --- Do nothing
	}

	public void afterCompletion(HttpServletRequest request,
			HttpServletResponse response, Object handler, Exception ex)
			throws Exception {
		// --- Do nothing
	}

	public boolean preHandle(HttpServletRequest request,
			HttpServletResponse response, Object handler) throws Exception {
		if (getTrackerCookie(request) != null) {
			return true;
		}

		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		if (user != null) {
			return true;
		}

		Visitor visitor = new Visitor();
		visitor.setLandedOn(request.getRequestURI());
		visitor.setReferrer(request.getHeader("Referer"));
		visitor.setRemoteHost(request.getRemoteHost());
		visitor.setUserAgent(request.getHeader("User-Agent"));

		try {
			WorldInitializer.getDefaultWorld().domain().users().createVisitor(
					visitor);
			Cookie cookie = createCookieForVisitor(visitor);

			response.addCookie(cookie);
		} catch (Throwable e) {
			Log.error(this, "Unable to create a visitor:", e);
		}

		return true;
	}

	private static Cookie createCookieForVisitor(Visitor visitor) {
		return createCookie(VISITOR_PREFIX + SEPARATOR + visitor.getId());
	}

	public static Cookie createCookieForUser(User user) {
		return createCookie(USER_PREFIX + SEPARATOR + user.getDatabaseId());
	}

	private static Cookie createCookie(String cookieValue) {
		Cookie cookie = new Cookie(TAWALA_TOKEN_COOKIE_NAME, cookieValue);
		cookie.setPath("/");
		cookie.setMaxAge(Integer.MAX_VALUE);
		return cookie;
	}

	private static Cookie getTrackerCookie(HttpServletRequest request) {
		Cookie[] cookies = request.getCookies();
		if (cookies == null) {
			return null;
		}
		for (int i = 0; i < cookies.length; i++) {
			if (cookies[i].getName().equals(TAWALA_TOKEN_COOKIE_NAME)) {
				return cookies[i];
			}
		}
		return null;
	}

	public static void populateEvent(Event event, HttpServletRequest request) {
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		if(user != null) {
			event.setUserId(user.getDatabaseId());
			event.setVisitorId(user.getOriginalVisitorId());
			return;
		}
		
		Cookie cookie = getTrackerCookie(request);
		if(cookie == null) {
			return;
		}

		CookieParser cookieParser = new CookieParser(cookie);
		event.setVisitorId(cookieParser.visitorId);
		event.setUserId(cookieParser.userId);
	}

	public static Long getVisitorId(HttpServletRequest request) {
		Cookie cookie = getTrackerCookie(request);
		if(cookie == null) {
			return null;
		}

		return new CookieParser(cookie).visitorId;
	}
	
	private static class CookieParser {
		Long userId;
		Long visitorId;
		
		public CookieParser(Cookie cookie) {
			String [] pieces = cookie.getValue().split(SEPARATOR);
			if(pieces.length != 2) {
				Log.warn(VisitorTrackerInterceptor.class, "Unable to parse visitor cookie: '" + cookie.getValue() + "'.");
				return;
			}

			long id;
			try {
				id = Long.parseLong(pieces[1]);
			} catch (NumberFormatException e) {
				Log.warn(VisitorTrackerInterceptor.class, "Unable to parse id value: '" + cookie.getValue() + "'.");
				return;
			}
			
			if(pieces[0].equals(VISITOR_PREFIX)) {
				visitorId = id;
			} else if(pieces[0].equals(USER_PREFIX)) {
				userId = id;
			} else {
				Log.warn(VisitorTrackerInterceptor.class, "Unrecognized prefix: '" + cookie.getValue() + "'.");
			}
		}
	}
}
