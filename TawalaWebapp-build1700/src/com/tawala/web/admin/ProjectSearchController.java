package com.tawala.web.admin;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.LinkToUserProject;
import com.tawala.web.WorldInitializer;

public class ProjectSearchController implements Controller {
	public final static String PARAMETER_PROJECT_ID = "project_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		ModelAndView result = new ModelAndView("admin.search.project");

		String id = request.getParameter(PARAMETER_PROJECT_ID);
		if(id != null) {
			result.addObject("projectId", id);
			LinkToUserProject link = WorldInitializer.getDefaultWorld().domain().projects().getWithProjectRuntime(id);
			if(link == null) {
				result.addObject("searchFailed", Boolean.TRUE);
			} else {
				result.addObject("link", link);
			}
		}
		
		return result;
	}

}
