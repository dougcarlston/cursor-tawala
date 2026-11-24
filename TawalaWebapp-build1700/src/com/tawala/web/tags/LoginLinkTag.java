package com.tawala.web.tags;

import java.io.UnsupportedEncodingException;
import java.net.URI;
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
		String server = httpServletRequest.getServerName();
		boolean publicSite = server != null
				&& ("teacherexams.org".equalsIgnoreCase(server) || "www.teacherexams.org"
						.equalsIgnoreCase(server));
		StringBuilder result = new StringBuilder(publicSite ? "/app/login.html"
				: WellKnown.urls.getLogin());
		if (httpServletRequest.getMethod().equals("GET")) {
			String requestURL = (String) httpServletRequest
					.getAttribute(UserAccessTicketInterceptor.CURRENT_PAGE_URI_ATTRIBUTE);
			String target = publicSite ? sameSitePath(requestURL) : requestURL;
			if (target != null && target.length() > 0) {
				try {
					String encodedURL = URLEncoder.encode(target, "UTF-8");

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

	private static String sameSitePath(String requestURL) {
		if (requestURL == null || requestURL.length() == 0) {
			return null;
		}
		String path = requestURL;
		String query = null;
		if (requestURL.indexOf("://") >= 0) {
			try {
				URI uri = new URI(requestURL);
				path = uri.getRawPath();
				query = uri.getRawQuery();
			} catch (Exception e) {
				return null;
			}
		}
		if (path == null || !path.startsWith("/") || path.startsWith("//")
				|| path.indexOf('\\') >= 0 || path.indexOf("://") >= 0) {
			return null;
		}
		if ("/login".equals(path) || "/app/login.html".equals(path)
				|| "/app/signup.html".equals(path)) {
			return "/home";
		}
		if (query != null && query.length() > 0) {
			return path + "?" + query;
		}
		return path;
	}
}
