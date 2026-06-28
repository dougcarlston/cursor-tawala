package com.tawala.web.user;

import java.io.UnsupportedEncodingException;

import javax.servlet.http.Cookie;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.mail.MailException;
import org.springframework.util.StringUtils;
import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.VisitorTrackerInterceptor;
import com.tawala.web.general.LandingPageController;

public class UserRegistrationController extends BaseUserAccountEditController {
	public UserRegistrationController() {
		setFormView("registration");
		setSuccessView("registration.confirmation");
	}

	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		return new UserForm();
	}

	protected void onBindAndValidate(HttpServletRequest request,
			Object command, BindException errors) throws Exception {

		UserForm form = (UserForm) command;
		
		updateUserWithOriginalDomainInformation(request, form.getUser());

		commonUserValidation(errors, form);

		if (!StringUtils.hasText(form.getPassword())) {
			errors.rejectValue("password", "user.password.is.empty");
		} else {
			validateNonEmptyPasswords(errors, form);
		}
	}

	private void updateUserWithOriginalDomainInformation(HttpServletRequest request, User user) {
		Cookie cookie = LandingPageController.getOriginalDomainCookie(request);
		if(cookie != null) {
			user.setOriginalDomain(cookie.getValue());
		}
		user.setOriginalVisitorId(VisitorTrackerInterceptor.getVisitorId(request));
	}

	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {

		UserForm form = (UserForm) command;
		checkForDuplicateIds(errors, form);
		if(errors.hasErrors()) {
			return showForm(request, response, errors);
		}

		registerUser(errors, form);
		if(errors.hasErrors()) {
			return showForm(request, response, errors);
		}
		
		postUserRegistration(request, response, form.getUser());
		
		return super.onSubmit(request, response, command, errors);
	}

	protected void registerUser(BindException errors, UserForm form) throws UnsupportedEncodingException {
		// TODO: catch DuplicateKeyException or whatever it's called to detect
		// duplicate id creation.
		try {
			WorldInitializer.getDefaultWorld().domain().users()
					.onUserRegistration(form.getUser());
		} catch (MailException e) {
			Log.warn(this, "Failed to send an email to " + form.getUser().getEmail()
					+ ":", e);
			errors.reject("user.failed.to.send.email", new Object[] { form.getUser()
					.getEmail().toString() }, "Failed to send an email.");
		}

		Log.info(this, "Successfully processed user registration for '"
				+ form.getUser().getId() + "'.");
	}

	protected void checkForDuplicateIds(BindException errors, UserForm form) {
		if (WorldInitializer.getDefaultWorld().domain().users().get(
				form.getUser().getId()) != null) {
			errors
					.rejectValue("user.id", "user.duplicate.ids",
							new Object[] { form.getUser().getId() },
							// There is no method that would let you not pass
							// the default message.
							"User name \"{0}\" already exists: choose a different one.");
		}
	}

	protected void postUserRegistration(HttpServletRequest request,
			HttpServletResponse response, User user) {
		// --- Do nothing; supposed to be overwritten by extending classes.
	}
}
