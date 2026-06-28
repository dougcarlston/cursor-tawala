package com.tawala.web.general;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.library.ProjectLibraryService;

public class NewHomePageController implements Controller {

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		ModelAndView result = new ModelAndView("home.new");
		result.addObject("exampleProjectsMap", ProjectLibraryService.getExampleProjects());
		return result;
	}
}
