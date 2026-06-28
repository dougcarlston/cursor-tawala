package com.tawala.web.projectmanager;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONObject;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class SaveHideSelectedFormsStateController implements Controller {
	public static final String PROJECT_ID_PARAMETER = "project_id";
	public static final String STATUS_PARAMETER = "state";

	public ModelAndView handleRequest(final HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		ProjectsHibernateImpl.hideSelectedFormsInProjectManager(
				UserInfoPreparationInterceptor.getSessionUser(request), Long
						.parseLong(request.getParameter(PROJECT_ID_PARAMETER)),
				Boolean.parseBoolean(request.getParameter(STATUS_PARAMETER)));

		JSONObject responseObject = new JSONObject();
		responseObject.put("result", Boolean.TRUE);

		response.setContentType("text/plain");
		response.getWriter().write(responseObject.toString());

		return null;
	}

}
