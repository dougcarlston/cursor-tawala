package com.tawala.web;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.ArrayList;
import java.util.List;

import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.scissor.ImpossibleException;
import com.tawala.World;
import com.tawala.project.UserProject;
import com.tawala.web.admin.UrgentMessage;

public abstract class Response {
	private List<Cookie> cookies = null;

	public final void process(HttpServletRequest request,
			HttpServletResponse response, World world) throws IOException {
		if (cookies != null) {
			addCookies(response, this.cookies);
		}

		addP3PPolicyHeader(response);

		handle(request, response, world);

	}

	abstract public void handleUrgentNotificationMessage(UrgentMessage urgentMessage);
	
	public static void addP3PPolicyHeader(HttpServletResponse response) {
		response
				.addHeader(
						"P3P",
						"policyref=\"http://" +
						UserProject.getWebsiteHostName() +
						"/w3c/p3p.xml\", CP=\"NOI CURa ADMa DEVa TAIa OUR BUS IND UNI COM NAV\"");
	}

	private void addCookies(HttpServletResponse response, List<Cookie> cookies) {
		if (cookies != null) {
			for (Cookie cookie : cookies) {
				response.addCookie(cookie);
			}
		}
	}

	protected abstract void handle(HttpServletRequest request,
			HttpServletResponse response, World world) throws IOException;

	protected String urlForPath(HttpServletRequest request, String path) {
		String fullUrl;
		try {
			URL currentUrl = new URL(request.getRequestURL().toString());
			URL desiredUrl = new URL(currentUrl, path);
			fullUrl = desiredUrl.toExternalForm();
		} catch (MalformedURLException e) {
			throw new ImpossibleException(e);
		}
		return fullUrl;
	}

	public void setCookie(Cookie cookie) {
		if (cookies == null)
			cookies = new ArrayList<Cookie>();
		cookies.add(cookie);
	}

	public void removeCookie(String cookieId) {
		Cookie cookie = new Cookie(cookieId, "");
		cookie.setMaxAge(0);
		setCookie(cookie);
	}
}
