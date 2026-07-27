package com.tawala.acceptance.library;

import java.io.IOException;
import java.util.Date;

import org.xml.sax.SAXException;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.WebRequest;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.LibrarySearchController;

public class ProjectLibrarySearchTest extends AcceptanceTestCase {
	private static final String CATEGORY_NAME = "polls";
	private static final String PROJECT_NAME = "Simple poll";

	public ProjectLibrarySearchTest() {
		setProjectNamesToDelete(new String[] { PROJECT_NAME });
		setCategoryNamesToDelete(new String[] { CATEGORY_NAME });
	}

	public void testSearch() throws RobotException, IOException, SAXException {
		Category POLLS = new Category(ProjectLibrary.SYSTEM_LIBRARY,
				CATEGORY_NAME, "Projects that...");
		ProjectLibraryService.createCategory(POLLS, projectOwner);

		UserProject deployedProject = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), projectOwner, PROJECT_NAME);

		world.domain().projects().put(deployedProject);

		LibraryProject project = new LibraryProject(projectOwner.getId(),
				deployedProject);
		project.setLongDescription("Some very long description.");
		project.setShortDescription(PROJECT_NAME + " description");
		project.setSubmittedDate(new Date());
		project.setCategory(POLLS);
		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		bot.logInAs(projectOwner);
		// --- Load the page.
		bot.go(WellKnown.urls.getLibrarySearch());

		WebForm form = bot.getForm("searchForm");
		form.setParameter(LibrarySearchController.PARAMETER_QUERY, "poll");
		bot.submit(form);

		assertContains("<input class=\"text\" type=text name=\""
				+ LibrarySearchController.PARAMETER_QUERY
				+ "\" id=\"query\" value=\"poll\"", bot.lastResponse()
				.getText());
		assertContains("id=\"projectDetailsLink" + project.getId() + "\"", bot
				.lastResponse().getText());
	}

	public void testCategoryDisplay() throws RobotException, IOException,
			SAXException {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				CATEGORY_NAME, "Projects that...");
		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), projectOwner, PROJECT_NAME);

		world.domain().projects().put(deployedProject);

		LibraryProject project = new LibraryProject(projectOwner.getId(),
				deployedProject);
		project.setLongDescription("Some very long description.");
		project.setShortDescription(PROJECT_NAME + " description");
		project.setSubmittedDate(new Date());
		project.setCategory(category);
		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		// --- Load the page.
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getLibrarySearch());

		WebForm form = bot.getForm("searchByCategoryForm");
		WebRequest request = form.newUnvalidatedRequest();
		request.setParameter(
				LibrarySearchController.CATEGORY_ID_PARAMETER,
				Long.toString(category.getId()));
		bot.go(request);

		assertContains("All projects in \"" + category.getName()
				+ "\" category", bot.lastResponse().getText());
		assertContains("id=\"projectDetailsLink" + project.getId() + "\"", bot
				.lastResponse().getText());
	}

	public void testFailedSearch() throws RobotException, IOException,
			SAXException {
		Category POLLS = new Category(ProjectLibrary.SYSTEM_LIBRARY,
				CATEGORY_NAME, "Projects that...");
		ProjectLibraryService.createCategory(POLLS, projectOwner);

		UserProject deployedProject = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), projectOwner, PROJECT_NAME);

		world.domain().projects().put(deployedProject);

		LibraryProject project = new LibraryProject(projectOwner.getId(),
				deployedProject);
		project.setLongDescription("Some very long description.");
		project.setShortDescription(PROJECT_NAME + " description");
		project.setSubmittedDate(new Date());
		project.setCategory(POLLS);
		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		bot.logInAs(projectOwner);
		// --- Load the page.
		bot.go(WellKnown.urls.getLibrarySearch());

		WebForm form = bot.getForm("searchForm");
		form.setParameter(LibrarySearchController.PARAMETER_QUERY, "{}");
		bot.submit(form);

		assertContains("An error occured while searching the project library.",
				bot.lastResponse().getText());
	}
}
