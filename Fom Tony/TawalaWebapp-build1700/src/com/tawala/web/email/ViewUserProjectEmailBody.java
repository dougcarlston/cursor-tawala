package com.tawala.web.email;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.email.EmailService;
import com.tawala.email.UserProjectEmail;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class ViewUserProjectEmailBody implements Controller {
	public final static String EMAIL_ID_PARAMETER = "email_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		UserProjectEmail email = EmailService.getUserProjectEmailById(
				UserInfoPreparationInterceptor.getSessionUser(request), Long
						.parseLong(request.getParameter(EMAIL_ID_PARAMETER)));

		response.setContentType(email.getType().getContentType());
		
		//-- TODO: verify that this is good enough for all character encoding.
		response.getOutputStream().write(email.getMessageText().getBytes());
		
		return null;
	}
}
