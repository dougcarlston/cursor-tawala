package com.tawala.web.library;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.springframework.web.servlet.ModelAndView;

import com.scissor.Log;
import com.tawala.UsersPersistentBunchImpl;
import com.tawala.World;
import com.tawala.domain.EmailAddress;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.UserProject;
import com.tawala.project.commands.Send;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.project.ProjectTestDriveController;

public abstract class TestDrivePreparationController extends
		ProjectVersionController {
	protected UserProject prepareTestWorld(HttpServletRequest request,
			HttpServletResponse response, LibraryProject project,
			LibraryProjectVersion version) throws IOException {
		HttpSession session = request.getSession(true);
		if (session == null) {
			Log.warn(this, "Unable to create a new session");
			LibraryNavigation.navigateToProjectDetailPage(response, project
					.getId());
			return null;
		}

		// Test if we already have this version of the project - it might be
		// trying to access another form and we would like to preserve the
		// submissions.
		World testDriveWorld = (World) session
				.getAttribute(ProjectTestDriveController.TESTWORLD_ATTRIBUTE_NAME);
		if (testDriveWorld != null) {
			UserProject runnableProject = testDriveWorld.domain().projects()
					.get("test", project.getName());
			if (runnableProject != null
					&& runnableProject.getProject().getId() == version
							.getProject().getId()) {
				return runnableProject;
			}
		}

		User sessionUser = UserInfoPreparationInterceptor
				.getSessionUser(request);

		EmailAddress emailAddress = sessionUser == null
				|| sessionUser.getEmail() == null ? new EmailAddress(
				Send.DEFAULT_FROM_ADDRESS) : sessionUser.getEmail();

		User user = new User("test", "test", "test", emailAddress, "none");
		UserProject runnableProject = new UserProject(version.getProject(),
				user, project.getName());
		runnableProject.setUniqueRandomId(String.valueOf(version.getId()));

		testDriveWorld = new World(WorldInitializer.getRealPath(),
				new UsersPersistentBunchImpl());
		testDriveWorld.domain().projects().put(runnableProject);

		testDriveWorld.domain().users().addOrSave(user);

		TestDriveSupport.copyExistingSubmissionsToTestWorld(version
				.getProject(), testDriveWorld);

		session.setAttribute(
				ProjectTestDriveController.TESTWORLD_ATTRIBUTE_NAME,
				testDriveWorld);

		ProjectLibraryService.onProjectTestDrive(project);

		return runnableProject;
	}

	@Override
	final public ModelAndView doHandle(HttpServletRequest request,
			HttpServletResponse response, LibraryProject project,
			LibraryProjectVersion version) throws Exception {
		UserProject userProject = prepareTestWorld(request, response, project,
				version);

		EventService.createEvent(new Event("ProjectLibaryTestDrive", request,
				project.getName()));

		return handlePreparedProject(userProject, project, version, request,
				response);
	}

	protected abstract ModelAndView handlePreparedProject(
			UserProject runnableProject, LibraryProject project,
			LibraryProjectVersion version, HttpServletRequest request,
			HttpServletResponse response) throws IOException;
}
