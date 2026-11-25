package com.tawala.web.controller;

import java.io.IOException;

import javax.servlet.Filter;
import javax.servlet.FilterChain;
import javax.servlet.FilterConfig;
import javax.servlet.ServletException;
import javax.servlet.ServletRequest;
import javax.servlet.ServletResponse;
import javax.servlet.http.HttpServletRequest;

/**
 * The address https://teacherexams.org/ is the home page. Tomcat does not hand
 * that exact path to the Spring home controller, so this shows the same page
 * while leaving the address unchanged.
 */
public class RootHomeFilter implements Filter {
	public void init(FilterConfig config) {
	}

	public void destroy() {
	}

	public void doFilter(ServletRequest request, ServletResponse response,
			FilterChain chain) throws IOException, ServletException {
		if (request instanceof HttpServletRequest) {
			String uri = ((HttpServletRequest) request).getRequestURI();
			if (uri == null || uri.length() == 0 || "/".equals(uri)) {
				request.getRequestDispatcher("/home").forward(request, response);
				return;
			}
		}
		chain.doFilter(request, response);
	}
}
