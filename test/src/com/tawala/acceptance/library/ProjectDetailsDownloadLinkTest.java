package com.tawala.acceptance.library;

import java.util.Date;

import com.meterware.httpunit.WebLink;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.ViewProjectDetailsController;

public class ProjectDetailsDownloadLinkTest extends AcceptanceTestCase {
	private static final String CATEGORY_NAME = "polls";
	private static final String PROJECT_NAME = "Simple poll";
	private static final String USER_NAME = "tester";

	public ProjectDetailsDownloadLinkTest() {
		setProjectNamesToDelete(new String[] { PROJECT_NAME });
		setCategoryNamesToDelete(new String[] { CATEGORY_NAME });
		setUserNamesToDelete(new String[] { USER_NAME });
	}

	public void testNoUser() throws Exception {
		doTest(
		// --- Link should be present
				false,
				// --- Library
				ProjectLibrary.SYSTEM_LIBRARY,
				// --- Admin
				false,
				// --- User Status
				null,
				// --- Customizable
				false);
	}

	public void testAdminAndCustomizable() throws Exception {
		doTest(
		// --- Link should be present
				true,
				// --- Library
				ProjectLibrary.SYSTEM_LIBRARY,
				// --- Admin
				true,
				// --- User Status
				Status.REGISTERED,
				// --- Customizable
				false);
	}

	public void testRegularUserAndCustomizable() throws Exception {
		doTest(
		// --- Link should be present
				false,
				// --- Library
				ProjectLibrary.SYSTEM_LIBRARY,
				// --- Admin
				false,
				// --- User Status
				Status.REGISTERED,
				// --- Customizable
				true);
	}

	public void testRegularUserAndCustomizableInUserLibrary() throws Exception {
		doTest(
		// --- Link should be present
				true,
				// --- Library
				ProjectLibrary.COMMUNITY_LIBRARY,
				// --- Admin
				false,
				// --- User Status
				Status.REGISTERED,
				// --- Customizable
				true);
	}

	private void doTest(boolean linkPresent, ProjectLibrary library,
			boolean admin, Status userStatus, boolean customizable)
			throws Exception {
		Category POLLS = new Category(library, CATEGORY_NAME,
				"Projects that...");
		ProjectLibraryService.createCategory(POLLS, projectOwner);

		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm(customizable ? Project.SETUP_WIZARD_FORM_NAMES
				.iterator().next() : "Form1");
		Project runnableProject = builder.build();

		UserProject deployedProject = new UserProject(runnableProject,
				projectOwner, "Simple poll");
		world.domain().projects().put(deployedProject);

		LibraryProject project = new LibraryProject(projectOwner.getId(),
				deployedProject);
		project
				.setLongDescription("Some very long description submitted by John Smith.");
		project.setShortDescription(PROJECT_NAME);
		project.setSubmittedDate(new Date());
		project.setCategory(POLLS);
		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		bot.logOut();
		if (userStatus != null) {
			User user = new User(userStatus);
			user.setFirstName("Joe");
			user.setLastName("Tester");
			user.setId(USER_NAME);
			user.setAdministrator(admin);
			user.setPassword("abc");

			world.domain().users().addOrSave(user);

			bot.logInAs(user);
		}

		bot.go(WellKnown.urls.getLibraryProjectDetailView() + "?"
				+ ViewProjectDetailsController.PARAMETER_ID + "="
				+ project.getId());

		WebLink downloadLink = bot.getLink("downloadLink");
		assertEquals("Link present", linkPresent, downloadLink != null);
	}
}
