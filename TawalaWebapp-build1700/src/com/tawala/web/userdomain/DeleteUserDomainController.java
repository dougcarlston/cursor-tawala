package com.tawala.web.userdomain;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.userdomain.UserDomainStorage;
import com.tawala.web.controller.WellKnown;

public class DeleteUserDomainController implements Controller {
	public static final String DOMAIN_ID_PARAMETER = "domain_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		UserDomainStorage.deleteDomainById(Long.parseLong(request.getParameter(DOMAIN_ID_PARAMETER)));
		
		response.sendRedirect(WellKnown.urls.getListUserDomains());
		return null;
	}
}
