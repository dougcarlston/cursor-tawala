package com.tawala.web.projectmanager;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONObject;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class DeleteMultipleVersionsController implements Controller {
	public static final String PARAMETER_VERSION_ID = "version_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		String[] parameters = request.getParameterValues(PARAMETER_VERSION_ID);
		if (parameters != null) {
			for (int i = 0; i < parameters.length; i++) {
				long projectVersionId = Long.parseLong(parameters[i]);

				WorldInitializer.getDefaultWorld().domain().projects()
						.deleteProjectVersion(
								UserInfoPreparationInterceptor
										.getSessionUser(request),
								projectVersionId);
			}
		}

		JSONObject responseObject = new JSONObject();
		responseObject.append("success", Boolean.TRUE);

		return null;
	}
}
