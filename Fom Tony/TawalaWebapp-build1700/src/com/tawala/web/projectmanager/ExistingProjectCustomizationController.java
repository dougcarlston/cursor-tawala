package com.tawala.web.projectmanager;

import javax.servlet.http.HttpServletRequest;

import com.scissor.Log;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class ExistingProjectCustomizationController extends
		CustomizationController {
	public static final String PARAMETER_PROJECT_NAME = "project_name";

	@Override
	protected CustomizationContext getCustomizationContext(
			HttpServletRequest request) {
		String projectName = request.getParameter(PARAMETER_PROJECT_NAME);

		if (projectName == null) {
			Log.warn(this, "Project id parameter is not set.");
			return null;
		}

		UserProject userProject = WorldInitializer.getDefaultWorld().domain()
				.projects().getWithProjectRuntime(
						UserInfoPreparationInterceptor.getSessionUser(request)
								.getId(), projectName);
		if (userProject == null) {
			Log.warn(this,
					"Unable to find the project - it must have been deleted");
			return null;
		}
		CustomizationContext result = new CustomizationContext(userProject,
				userProject.getName(), true);

		EventService.createEvent(new Event("ExistingProjectRecustomization",
				request, userProject.getName()));

		return result;
	}
}
