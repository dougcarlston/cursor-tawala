package com.tawala.web.library;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.library.ProjectLibraryService;

public class CustomizablesPageController implements Controller {

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		ModelAndView result = new ModelAndView("library.customizables");
		result.addObject("projectsMap", ProjectLibraryService.getReadyToRunProjects());
		return result;
	}
}
