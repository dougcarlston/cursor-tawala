package com.tawala.web.controller;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;
import org.springframework.web.util.UrlPathHelper;

public class URLToViewNameDisplayController implements Controller {
	private final String overridingURL;
	
	public URLToViewNameDisplayController() {
		this(null);
	}
	
	public URLToViewNameDisplayController(String overridingURL) {
		this.overridingURL = overridingURL;
	}
	
	private UrlPathHelper urlPathHelper = new UrlPathHelper();
	{
		urlPathHelper.setAlwaysUseFullPath(true);
	}

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		String viewName = overridingURL == null ? urlPathHelper.getLookupPathForRequest(request) : overridingURL;
		return new ModelAndView(viewName);
	}
}
