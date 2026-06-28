package com.tawala.web.projectmanager.alldataimport;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class DisplayExportAllDataDescriptionController implements Controller {
	public static final String PROJECT_ID_PARAMETER = "id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		long projectId = Long.parseLong(request
				.getParameter(PROJECT_ID_PARAMETER));

		UserProject userProject = ProjectsHibernateImpl.getUserProjectById(
				UserInfoPreparationInterceptor.getSessionUser(request),
				projectId);
		if (userProject == null) {
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return null;
		}

		ModelAndView result = new ModelAndView(
				"projectmanager.export.all.data.description");

		result.addObject("userProject", userProject);

		return result;
	}
}
