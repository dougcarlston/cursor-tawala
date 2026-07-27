package com.tawala.web.user;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;

public class InitialRegistrationController extends
		UserRegistrationController {
	public InitialRegistrationController() {
		setFormView("registration.initial");
		setSuccessView("registration.initial.confirmation");
	}
	
	
	@Override
	protected void postUserRegistration(HttpServletRequest request, HttpServletResponse response, User user) {
		LoginController.onUserLogin(request, response, user, false);
		EventService.createEvent(new Event("InitialRegistration", request, user.getId()));
	}
}
