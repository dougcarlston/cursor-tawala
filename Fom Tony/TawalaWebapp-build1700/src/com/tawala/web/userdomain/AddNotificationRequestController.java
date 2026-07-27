package com.tawala.web.userdomain;

import java.util.Date;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.util.StringUtils;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.userdomain.NotificationRequest;
import com.tawala.userdomain.UserDomainStorage;

public class AddNotificationRequestController implements Controller {
	public static final String DOMAIN_PARAMETER = "domain";
	public static final String EMAIL_PARAMETER = "email";

	public ModelAndView handleRequest(HttpServletRequest request, HttpServletResponse response) throws Exception {
		NotificationRequest notificationRequest = new NotificationRequest();
		notificationRequest.setCreatedDate(new Date());
		notificationRequest.setDomainName(request.getParameter(DOMAIN_PARAMETER));
		notificationRequest.setEmail(request.getParameter(EMAIL_PARAMETER));

		boolean requestIsValid = true;
		if(! StringUtils.hasText(notificationRequest.getDomainName())) {
			Log.error(this, "Domain name is empty.");
			requestIsValid = false;
		}
		if(! StringUtils.hasText(notificationRequest.getEmail())) {
			Log.error(this, "Email is empty.");
			requestIsValid = false;
		}
		
		if(requestIsValid) {
			UserDomainStorage.createNotificationRequest(notificationRequest);
		}
		
		return null;
	}
}
