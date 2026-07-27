package com.tawala.web.projectmanager;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class HowToIncludeInAnotherWebSiteController implements Controller {
	public static final String PROJECT_NAME_PARAMETER = "project_name";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		String projectName = request.getParameter(PROJECT_NAME_PARAMETER);

		UserProject userProject = WorldInitializer.getDefaultWorld().domain()
				.projects().getWithProjectRuntime(
						UserInfoPreparationInterceptor.getSessionUser(request)
								.getId(), projectName);
		ModelAndView result = new ModelAndView(
				"projectmanager.how.to.include.in.another.page");
		
		result.addObject("project", userProject);
		
		return result;
	}

}
