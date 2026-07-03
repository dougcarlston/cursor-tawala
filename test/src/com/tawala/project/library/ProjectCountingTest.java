package com.tawala.project.library;

import java.io.IOException;
import java.util.Date;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;

public class ProjectCountingTest extends ProjectLibraryTestCase {
	private static final String TEST_PROJECT_NAME = "Test project";
	private String[] categoryNames;

	private User projectOwner = UserTest.aUser("tester");

	public ProjectCountingTest() {
		categoryNames = new String[20];
		for (int i = 0; i < categoryNames.length; i++) {
			categoryNames[i] = "Level " + i;
		}

		setCategoryNamesToDelete(categoryNames);
		setProjectNamesToDelete(new String[] { TEST_PROJECT_NAME });

		addUserNameToDelete(projectOwner.getId());
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(
				projectOwner);
	}

	public void testVeryDeepProjectCounting() throws IOException {
		Category previous = null;
		for (int i = 0; i < categoryNames.length; i++) {
			Category next = previous == null ? new Category(
					ProjectLibrary.COMMUNITY_LIBRARY, categoryNames[i],
					"Some description") : new Category(previous,
					categoryNames[i], "Some description");
			previous = next;
			ProjectLibraryService.createCategory(next, projectOwner);
		}

		UserProject deployedProject = new UserProject(new ConfigElement(
				"<project name=\"" + TEST_PROJECT_NAME
						+ "\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		project.setLongDescription("Another project");
		project.setShortDescription("Something else");
		project.setSubmittedDate(new Date());
		project.setCategory(previous);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		Category next = ProjectLibraryService.findProjectById(project.getId())
				.getCategory();
		while (next != null) {
			assertEquals("Project count for " + next.getName(), 1, next
					.getProjectCount());

			next = next.getParent();
		}
	}
}
