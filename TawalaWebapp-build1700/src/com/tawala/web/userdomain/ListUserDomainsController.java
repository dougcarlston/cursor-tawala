package com.tawala.web.userdomain;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.userdomain.UserDomainStorage;

public class ListUserDomainsController implements Controller {

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		
		ModelAndView result = new ModelAndView("list.user.domains");
		result.addObject("domains", UserDomainStorage.getAllDomains());
		return result;
	}
}
