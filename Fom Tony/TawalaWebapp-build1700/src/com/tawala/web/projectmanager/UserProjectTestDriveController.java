package com.tawala.web.projectmanager;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.servlet.http.HttpSession;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.UsersPersistentBunchImpl;
import com.tawala.World;
import com.tawala.domain.EmailAddress;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.Form;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.UserProject;
import com.tawala.project.commands.Send;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.TestDriveSupport;
import com.tawala.web.project.ProjectTestDriveController;

public class UserProjectTestDriveController implements Controller {
	public static final String PARAMETER_FORM_NAME = "form";
	public static final String PARAMETER_PROJECT_RANDOM_ID = "id";
	public static final String PARAMETER_PROJECT_FORCE_REINITIALIZATION = "r";

	protected UserProject prepareTestWorld(HttpServletRequest request,
			HttpServletResponse response, UserProject userProject)
			throws IOException {
		HttpSession session = request.getSession(true);
		if (session == null) {
			Log.warn(this, "Unable to create a new session");
			response.sendRedirect(WellKnown.urls.getHome());

			return null;
		}

		UserProject runnableProject = null;

		if (request.getParameter(PARAMETER_PROJECT_FORCE_REINITIALIZATION) == null) {
			runnableProject = getExistingMatchingTestDrivenProject(userProject,
					session);
			if (runnableProject != null) {
				return runnableProject;
			}
		}

		EmailAddress emailAddress = userProject.getUser().getEmail() == null ? new EmailAddress(
				Send.DEFAULT_FROM_ADDRESS)
				: userProject.getUser().getEmail();

		User user = new User("test", "test", "test", emailAddress, "none");
		runnableProject = new UserProject(userProject.getProject(), user,
				userProject.getName());
		runnableProject.setUniqueRandomId(userProject.getUniqueRandomId());
		runnableProject.setId(userProject.getId());

		World testDriveWorld = new World(WorldInitializer.getRealPath(),
				new UsersPersistentBunchImpl());
		testDriveWorld.domain().projects().put(runnableProject);
		testDriveWorld.domain().users().addOrSave(user);

		TestDriveSupport.copyExistingSubmissionsToTestWorld(userProject
				.getProject(), testDriveWorld);

		session.setAttribute(
				ProjectTestDriveController.TESTWORLD_ATTRIBUTE_NAME,
				testDriveWorld);

		return runnableProject;
	}

	private UserProject getExistingMatchingTestDrivenProject(
			UserProject userProject, HttpSession session) {
		// Test if we already running this the project - it might be
		// trying to access another form and we would like to preserve the
		// submissions.
		World testDriveWorld = (World) session
				.getAttribute(ProjectTestDriveController.TESTWORLD_ATTRIBUTE_NAME);
		if (testDriveWorld != null) {
			UserProject runnableProject = testDriveWorld.domain().projects()
					.get("test", userProject.getName());
			if (runnableProject != null
					&& runnableProject.getProject().getId() == userProject
							.getProject().getId()) {
				return runnableProject;
			}
		}
		return null;
	}

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		String projectId = request.getParameter(PARAMETER_PROJECT_RANDOM_ID);
		LinkToUserProject link = WorldInitializer.getDefaultWorld().domain()
				.projects().getWithProjectRuntime(projectId);
		UserProject originalProject = link.getProject();
		UserProject projectReadyForTestDrive = prepareTestWorld(request,
				response, originalProject);

		String formName = request.getParameter(PARAMETER_FORM_NAME);
		if (formName == null) {
			throw new IllegalStateException("Unable to find parameter '"
					+ PARAMETER_FORM_NAME + "'");
		}
		Form form = projectReadyForTestDrive.getProject().getForm(formName);
		if (form == null) {
			throw new IllegalStateException("Unable to find form '" + formName
					+ "'");
		}

		String url = projectReadyForTestDrive.getEntryPointURLs(
				UserProject.EntryPointType.TEST_DRIVE).get(form);
		response.sendRedirect(url);

		EventService.createEvent(new Event("UserProjectTestDrive", request,
				originalProject.getName()));
		return null;
	}
}
