package com.tawala.web.controller;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

public class HomeController implements Controller {
	public static final String TAWALA_USER_SEEN = "user_seen";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

        /*
		Cookie[] cookies = request.getCookies();
        
		boolean cookieFound = false;
		if (cookies != null) {
			for (Cookie cookie : cookies) {
				if (cookie.getName().equals(TAWALA_USER_SEEN)) {
					cookieFound = true;
					break;
				}
			}
		}
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		if (user != null) {
			response.sendRedirect(WellKnown.urls.getRegHome());
		} else {
			response.sendRedirect(WellKnown.urls.getNewHome());
		}

        */
		
		response.sendRedirect(WellKnown.urls.getSportsHome());
		return null;
	}

}
