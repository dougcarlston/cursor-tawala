package com.tawala.web.tags;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.jsp.JspException;
import javax.servlet.jsp.JspTagException;
import javax.servlet.jsp.JspWriter;
import javax.servlet.jsp.tagext.TagSupport;

import com.scissor.Log;
import com.tawala.web.controller.UserAccessTicketInterceptor;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.user.LoginController;

public class LoginLinkTag extends TagSupport {
	private static final long serialVersionUID = 1L;

	@Override
	public int doStartTag() throws JspException {
		HttpServletRequest httpServletRequest = ((HttpServletRequest) pageContext
				.getRequest());
		try {
			JspWriter out = pageContext.getOut();
			out.write(constructLoginURL(httpServletRequest));
		} catch (Exception e) {
			throw new JspTagException(e.getMessage());
		}

		return SKIP_BODY;
	}

	public static String constructLoginURL(HttpServletRequest httpServletRequest) {
		StringBuilder result = new StringBuilder(WellKnown.urls.getLogin());
		if (httpServletRequest.getMethod().equals("GET")) {
			String requestURL = (String) httpServletRequest
					.getAttribute(UserAccessTicketInterceptor.CURRENT_PAGE_URI_ATTRIBUTE);
			if (requestURL != null) {
				try {
					String encodedURL = URLEncoder.encode(requestURL, "UTF-8");

					result.append('?');
					result.append(LoginController.REDIRECT_TO_PARAMETER);
					result.append('=');
					result.append(encodedURL);
				} catch (UnsupportedEncodingException e) {
					Log.error(LoginLinkTag.class, "Unable to encode " + requestURL);
				}
			}
		}
		return result.toString();
	}
}
