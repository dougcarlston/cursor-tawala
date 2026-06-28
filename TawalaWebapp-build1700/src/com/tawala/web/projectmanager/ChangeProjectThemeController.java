package com.tawala.web.projectmanager;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.web.WorldInitializer;

// --- TODO: more elaborate error handling. Currently there is no indication to the client if something went wrong.

public class ChangeProjectThemeController implements Controller {
	public final static String PARAMETER_PROJECT_ID = "project_id";

	public final static String PARAMETER_THEME_PATH = "project.theme.themeId";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		String projectId = request.getParameter(PARAMETER_PROJECT_ID);
		String theme = request.getParameter(PARAMETER_THEME_PATH);
		if(theme == null) {
			//--- TODO: something changed in Spring 2.5 that makes <form:select> work differently than 2.0
			theme = request.getParameter("project." + PARAMETER_THEME_PATH);
		}
		if (theme == null) {
			Log.error(this, "Unable to find the theme in the request.");
		} else {
			WorldInitializer.getDefaultWorld().domain().projects()
					.changeProjectTheme(projectId, theme);
			response.getOutputStream().print("false");
		}
		return null;
	}
}
