package com.tawala.web.projectmanager;

import javax.servlet.http.HttpServletRequest;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class LastClonedLibraryProjectCustomizationController extends
		CustomizationController {
	public static final String ATTRIBUTE_CUSTOMIZATION_CONTEXT = "tawala.customization.context";

	@Override
	protected CustomizationContext getCustomizationContext(
			HttpServletRequest request) {
		ClonedLibraryProjectCustomizationContext customizationContext = (ClonedLibraryProjectCustomizationContext) request
				.getSession().getAttribute(ATTRIBUTE_CUSTOMIZATION_CONTEXT);
		if (customizationContext == null) {
			Log
					.warn(this,
							"Project id attribute is not set - the session must have expired");
			return null;
		}

		LinkToUserProject linkToProject = WorldInitializer.getDefaultWorld()
				.domain().projects().getWithProjectRuntime(
						customizationContext.getUniqueProjectId());
		if (linkToProject == null) {
			Log.warn(this,
					"Unable to find the project - it must have been deleted");
			return null;
		}
		UserProject userProject = linkToProject.getProject();

		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		boolean projectIsSaved = user != null
				&& userProject.getUser().equals(user);

		CustomizationContext result = new CustomizationContext(userProject,
				projectIsSaved ? userProject.getName() : customizationContext
						.getLibraryProjectName(), projectIsSaved);
		return result;
	}
}
