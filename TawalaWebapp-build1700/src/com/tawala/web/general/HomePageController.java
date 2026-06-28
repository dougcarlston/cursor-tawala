package com.tawala.web.general;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.library.ProjectLibraryService;
import com.tawala.userdomain.UserDomainStorage;

public class HomePageController implements Controller {
	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		ModelAndView result = new ModelAndView("home.page");
		result.addObject("domain", UserDomainStorage.getDefaultDomain());
		result.addObject("featuredProjects", ProjectLibraryService
				.getFeaturedProjects());
		result.addObject("domains", UserDomainStorage.getAllDomains());
		
		LandingPageController.setOriginalDomainCookie(request, response, "default");
		return result;
	}
}
