package com.tawala.web.library;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.UserProject;
import com.tawala.project.library.Constants;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.LastClonedLibraryProjectCustomizationController;
import com.tawala.web.projectmanager.ClonedLibraryProjectCustomizationContext;

public class CloneAndCustomizeController implements Controller {
	public static final String PARAMETER_PROJECT_ID = "app_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		String projectIdParameter = request.getParameter(PARAMETER_PROJECT_ID);
		long projectId = Long.parseLong(projectIdParameter);

		LibraryProject libraryProject = ProjectLibraryService
				.findProjectById(projectId);
		if (libraryProject == null) {
			throw new IllegalStateException("Unable to find project by id="
					+ projectId);
		}

		boolean makeProjectNameUnique = true;
		User user = WorldInitializer.getDefaultWorld().domain().users().get(
				Constants.ANONYMOUS_USER_ID);
		if (user == null) {
			throw new IllegalStateException("Unable to find anonymous user.");
		}

		UserProject userProject = ProjectLibraryService
				.cloneProjectToUserAccount(libraryProject, user,
						makeProjectNameUnique, null, null, "Copy of version "
								+ libraryProject.getLatestVersionNumber()
								+ " of \"" + libraryProject.getName() + "\".");

		EventService.createEvent(new Event("LibraryProjectCloning", request,
				libraryProject.getName()));

		HttpSession session = request.getSession();

		session
				.setAttribute(
						LastClonedLibraryProjectCustomizationController.ATTRIBUTE_CUSTOMIZATION_CONTEXT,
						new ClonedLibraryProjectCustomizationContext(
								userProject, libraryProject));

		response.sendRedirect(WellKnown.urls.getCustomizeLastClonedProject());
		return null;
	}
}
