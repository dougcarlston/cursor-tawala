package com.tawala.web.email;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class ViewAllUserProjectEmailsController implements Controller {
	public static final String PROJECT_ID_PARAMETER = "project_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		UserProject userProject = ProjectsHibernateImpl.getUserProjectById(
				UserInfoPreparationInterceptor.getSessionUser(request), Long
						.parseLong(request.getParameter(PROJECT_ID_PARAMETER)));
		if (userProject == null) {
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return null;
		} else {
			ModelAndView result = new ModelAndView("view.all.project.emails");
			result.addObject("project", userProject);
			return result;
		}
	}
}
