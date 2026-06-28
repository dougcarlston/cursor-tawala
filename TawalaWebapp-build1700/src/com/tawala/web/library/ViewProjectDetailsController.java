package com.tawala.web.library;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class ViewProjectDetailsController implements Controller {
	public static final String PARAMETER_ID = "id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		String idParameter = request.getParameter(PARAMETER_ID);

		long projectId = 0;
		try {
			projectId = Long.parseLong(idParameter);
		} catch (Throwable e) {
			Log.warn(this, "Unable to get id of the project.", e);
			return navigateOnFailureToLoadProject(request, response);
		}

		LibraryProject project = ProjectLibraryService
				.findProjectById(projectId);
		if (project == null) {
			return navigateOnFailureToLoadProject(request, response);
		}

		User currentUser = UserInfoPreparationInterceptor
				.getSessionUser(request);
		boolean allowedToEdit = ProjectLibraryService
				.getLibrariesUpdatableByUser(currentUser).contains(
						project.getCategory().getLibrary());

		boolean allowedToDownload = currentUser != null
				&& currentUser.getStatus().isAllowedToViewDesigner()
				&& (currentUser.isAdministrator() || !(project.isVetted() && project
						.getLatestVersion().getProject().isCustomizable()));

		ModelAndView result = new ModelAndView("library.detail");
		result.addObject("project", project);
		result.addObject("currentUserAllowedToEdit", allowedToEdit);
		result.addObject("currentUserAllowedToDownload", allowedToDownload);

		EventService.createEvent(new Event("LibraryProjectViewed", request,
				project.getName()));

		return result;
	}

	/**
	 * @param request
	 * @param response
	 * @return
	 * @throws Exception
	 */
	private ModelAndView navigateOnFailureToLoadProject(
			HttpServletRequest request, HttpServletResponse response)
			throws Exception {
		response.sendRedirect(WellKnown.urls.getLibrarySearch());
		return null;
	}
}
