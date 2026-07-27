package com.tawala.web.user;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class UpgradeToNextLevelController implements Controller {

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		User user = WorldInitializer.getDefaultWorld().domain().users()
				.onUserUpgradeToFullyRegistered(
						UserInfoPreparationInterceptor.getSessionUser(request));
		request.getSession().setAttribute("user", user);

		EventService.createEvent(new Event("RegistrationUpgrade", request, user.getId()));

		return new ModelAndView("registration.nextlevel.confirmation");
	}

}
