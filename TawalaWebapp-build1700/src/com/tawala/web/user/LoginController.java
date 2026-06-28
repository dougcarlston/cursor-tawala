package com.tawala.web.user;

import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.apache.log4j.NDC;
import org.springframework.util.StringUtils;
import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.web.controller.BaseAuthenticationInterceptor;
import com.tawala.web.controller.VisitorTrackerInterceptor;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.jforum.ForumIntegrationToken;

public class LoginController extends SimpleFormController {
	public static final String REDIRECT_TO_PARAMETER = "r";

	public LoginController() {
		setSessionForm(false);
		setCommandName("login");
		setFormView("login.view");
		setSuccessView("none - see onSubmit");
		setValidator(new LoginValidator());
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see org.springframework.web.servlet.mvc.SimpleFormController#onSubmit(javax.servlet.http.HttpServletRequest,
	 *      javax.servlet.http.HttpServletResponse, java.lang.Object,
	 *      org.springframework.validation.BindException)
	 */
	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		LoginForm form = (LoginForm) command;

		onUserLogin(request, response, form.getUser(), form.isKeepSignedIn());

		EventService.createEvent(new Event("Login", request, form.getUser()
				.getId()));

		Log.info(this, "successful login for '" + form.getUserName()
				+ "' from " + request.getRemoteAddr());
		Log.info(this, "Using browser: " + request.getHeader("User-Agent"));

		if (form.getUser().isRequirePasswordReset()) {
			response.sendRedirect(WellKnown.urls.getUserPasswordChange());
		} else {
			if(StringUtils.hasText(form.getRedirectTo())) {
				response.sendRedirect(form.getRedirectTo());
			} else {
				response.sendRedirect(WellKnown.urls.getHome());
			}
		}

		return null;
	}

	public static void onUserLogin(HttpServletRequest request,
			HttpServletResponse response, User user, boolean keepSignedIn) {
		setupUserSession(request, response, user, true);
		if (keepSignedIn) {
			response.addCookie(createUserAccessCookie(user));
		} else {
			response.addCookie(removeUserAccessCookie());
		}
	}

	public static void setupUserSession(HttpServletRequest request,
			HttpServletResponse response, User user, boolean createNewSession) {
		HttpSession session = null;
		if (createNewSession) {
			session = request.getSession(false);
			if (session != null) {
				session.invalidate();
			}
		}

		session = request.getSession(true);
		session.setAttribute("user", user);

		NDC.clear();
		NDC.push(user.getId());

		response.addCookie(new ForumIntegrationToken(user.getId()).asCookie());
		response.addCookie(VisitorTrackerInterceptor.createCookieForUser(user));
	}

	public static Cookie removeUserAccessCookie() {
		Cookie cookie = new Cookie(
				BaseAuthenticationInterceptor.USER_ACCESS_TOKEN_COOKIE_NAME, "");
		cookie.setPath("/");
		cookie.setMaxAge(0);
		return cookie;
	}

	private static Cookie createUserAccessCookie(User user) {
		UserAccessTicket accessTicket = UsersHibernateImpl
				.generateUserAccessTicket(user);
		Cookie cookie = new Cookie(
				BaseAuthenticationInterceptor.USER_ACCESS_TOKEN_COOKIE_NAME,
				accessTicket.getAccessToken());
		cookie.setPath("/");
		cookie.setMaxAge(Integer.MAX_VALUE);
		return cookie;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see org.springframework.web.servlet.mvc.AbstractFormController#formBackingObject(javax.servlet.http.HttpServletRequest)
	 */
	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		LoginForm form = new LoginForm();
		String redirectTo = request.getParameter(REDIRECT_TO_PARAMETER);
		form.setRedirectTo(redirectTo);
		return form;
	}

	public static Cookie removeForumIntegrationCookie() {
		Cookie forumCookie = new Cookie(ForumIntegrationToken.COOKIE_ID, "");
		forumCookie.setPath("/");
		forumCookie.setMaxAge(0);
		return forumCookie;
	}
}
