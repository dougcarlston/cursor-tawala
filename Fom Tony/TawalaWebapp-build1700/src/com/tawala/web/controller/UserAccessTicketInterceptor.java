package com.tawala.web.controller;

import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.ModelAndView;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.User;
import com.tawala.web.WorldInitializer;
import com.tawala.web.user.LoginController;
import com.tawala.web.user.UserAccessTicket;

public class UserAccessTicketInterceptor implements HandlerInterceptor {

	public static final String CURRENT_PAGE_URI_ATTRIBUTE = "com.tawala.current_page_uri";
	
	public boolean preHandle(HttpServletRequest request,
			HttpServletResponse response, Object handler) throws Exception {
		boolean sessionIsActive = false;
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		if (user == null) {
			UserAccessTicket ticket = getUserAccessTicket(request);
			if (ticket != null) {
				user = ticket.getUser();
				if (user.getStatus().isAllowedToLogOn()) {
					LoginController.setupUserSession(request, response, user,
							false);
					WorldInitializer.getDefaultWorld().domain().users()
							.onLogin(user);
					UsersHibernateImpl.updateLastTimeAccessTicketUsed(ticket);
					Log.info(this,
							"Created user session based on existing ticket. Connected from "
									+ request.getRemoteAddr());
					Log.info(this, "Using browser: "
							+ request.getHeader("User-Agent"));
					sessionIsActive = true;
				} else {
					Log
							.info(
									this,
									"User status ("
											+ user.getStatus()
											+ " prevented from automatically logging in based on existing ticket: "
											+ user.getId());
					sessionIsActive = false;
				}
			}
		} else {
			sessionIsActive = true;
		}
		
		if(! sessionIsActive) {
			if(request.getMethod().equals("GET")) {
				StringBuffer url = request.getRequestURL();
				if(request.getQueryString() != null && request.getQueryString().length() > 0) {
					url.append('?').append(request.getQueryString());
				}
				request.setAttribute(CURRENT_PAGE_URI_ATTRIBUTE, url.toString());
			}
		}
		
		return true;
	}

	public void postHandle(HttpServletRequest request,
			HttpServletResponse response, Object handler,
			ModelAndView modelAndView) throws Exception {
		// --- Do nothing

	}

	public void afterCompletion(HttpServletRequest request,
			HttpServletResponse response, Object handler, Exception ex)
			throws Exception {
	}

	public static UserAccessTicket getUserAccessTicket(
			HttpServletRequest request) {
		Cookie[] cookies = request.getCookies();
		if (cookies == null) {
			return null;
		}

		Cookie cookie = null;
		for (Cookie nextCookie : cookies) {
			if (nextCookie
					.getName()
					.equals(
							BaseAuthenticationInterceptor.USER_ACCESS_TOKEN_COOKIE_NAME)) {
				cookie = nextCookie;
				break;
			}
		}

		if (cookie == null || cookie.getValue().length() == 0) {
			return null;
		}

		return UsersHibernateImpl.retrieveAccessTicket(cookie.getValue());
	}
}
