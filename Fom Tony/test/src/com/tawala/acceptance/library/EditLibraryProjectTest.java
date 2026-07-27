package com.tawala.acceptance.library;

import java.io.IOException;
import java.util.Date;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.ModifyProjectController;

public class EditLibraryProjectTest extends AcceptanceTestCase {
	private static final String CATEGORY_NAME = "polls";
	private static final String PROJECT_NAME = "Simple poll";

	public EditLibraryProjectTest() {
		setProjectNamesToDelete(new String[] { PROJECT_NAME });
		setCategoryNamesToDelete(new String[] { CATEGORY_NAME });
	}

	public void testEditAuthorId() throws RobotException, IOException {
		Category POLLS = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				CATEGORY_NAME, "Projects that...");
		ProjectLibraryService.createCategory(POLLS, projectOwner);

		UserProject deployedProject = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), projectOwner, "Simple poll");
		world.domain().projects().put(deployedProject);

		LibraryProject project = new LibraryProject(projectOwner.getId(),
				deployedProject);
		project
				.setLongDescription("Some very long description submitted by John Smith.");
		project.setShortDescription(PROJECT_NAME);
		project.setSubmittedDate(new Date());
		project.setCategory(POLLS);
		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		// --- Test non-admin doesn't have access
		bot.logInAs(projectOwner);
		String editProjectUrl = WellKnown.urls.getLibraryEditProject() + "?"
				+ ModifyProjectController.PARAMETER_PROJECT_ID + "="
				+ project.getId();
		bot.go(editProjectUrl);

		WebForm form = bot.getForm("editProjectForm");
		assertFalse(form.hasParameterNamed("project.authorId"));
		bot.logOut();

		// --- Make owner an admin and try again
		projectOwner.setAdministrator(true);
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(
				projectOwner);

		bot.logInAs(projectOwner);
		bot.go(editProjectUrl);
		form = bot.getForm("editProjectForm");
		assertEquals(projectOwner.getId(), form
				.getParameterValue("project.authorId"));
		form.setParameter("project.authorId", "admin");
		bot.submit(form);

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals("admin", project.getAuthorId());

	}
}
