package com.tawala.web.general;

import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.userdomain.UserDomain;
import com.tawala.userdomain.UserDomainStorage;

public class LandingPageController implements Controller {
	public static final String ORIGINAL_LANDING_DOMAIN_COOKIE_NAME = "original_domain";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		String[] chunks = request.getRequestURI().split("/");
		String domainName = chunks[chunks.length - 1];

		UserDomain domain = UserDomainStorage.getDomainNamed(domainName);
		if (domain == null) {
			Log.warn(this, "Can't find domain '" + domainName + "'.");
			domain = UserDomainStorage.getDefaultDomain();
		} else {
			setOriginalDomainCookie(request, response, domainName);
		}

		ModelAndView result = new ModelAndView("home.page");
		result.addObject("domain", domain);
		result.addObject("featuredProjects", domain.getFeaturedProjects());
		return result;
	}

	public static void setOriginalDomainCookie(HttpServletRequest request,
			HttpServletResponse response, String domainName) {
		if (getOriginalDomainCookie(request) == null) {
			Cookie cookie = new Cookie(ORIGINAL_LANDING_DOMAIN_COOKIE_NAME,
					domainName);
			cookie.setMaxAge(Integer.MAX_VALUE);
			cookie.setPath("/");
			response.addCookie(cookie);
		}
	}

	public static Cookie getOriginalDomainCookie(HttpServletRequest request) {
		Cookie[] cookies = request.getCookies();
		if (cookies != null) {
			for (int i = 0; i < cookies.length; i++) {
				Cookie cookie = cookies[i];
				if (cookie.getName()
						.equals(ORIGINAL_LANDING_DOMAIN_COOKIE_NAME)) {
					return cookie;
				}
			}
		}
		return null;
	}
}
