package com.tawala.acceptance.library;

import java.io.IOException;
import java.util.Date;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.xml.sax.SAXException;

import com.meterware.httpunit.HttpUnitOptions;
import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.WebRequest;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.acceptance.ApiTest;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.ViewProjectDetailsController;

public class AddProjectVersionTest extends AcceptanceTestCase {
	private static final String PROJECT_IS_VETTED_MESSAGE = "The project you tried to update is vetted and only administrators are allowed to modify it.";

	private static final String PROJECT_NAME = "Simple Poll";

	private static final String ANOTHER_PROJECT_NAME = "Another project";

	private static final String POLLS_CATEGORY_NAME = "polls";

	private User administrator = UserTest.aUser("another tester");

	public AddProjectVersionTest() {
		setCategoryNamesToDelete(new String[] { POLLS_CATEGORY_NAME });
		setProjectNamesToDelete(new String[] { PROJECT_NAME,
				ANOTHER_PROJECT_NAME });
		addUserNameToDelete(administrator.getId());
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		administrator.setAdministrator(true);
		world.domain().users().addOrSave(administrator);

		projectOwner.setStatus(Status.REGISTERED);
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(
				projectOwner);
	}

	public void testAddNewVersionToAnUnrelatedProject() throws Exception {
		Category POLLS = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				POLLS_CATEGORY_NAME, "Projects that...");
		ProjectLibraryService.createCategory(POLLS, projectOwner);

		createAndDeployProject(POLLS, PROJECT_NAME, projectOwner);

		bot.logInAs(projectOwner.getId(), projectOwner.getPassword());
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		LibraryProject anotherProject = createAndDeployProject(
				ProjectLibraryService
						.getOrCreateDefaultCategory(ProjectLibrary.COMMUNITY_LIBRARY),
				ANOTHER_PROJECT_NAME, administrator);

		navigateToAddVersionPage();

		WebForm form = bot.getForm("addVersionForm");
		assertNotNull(form);
		form.setParameter("versionDescription", "New version");

		bot.submit(form, "save");

		assertContains("Project must be selected", bot.getPageText());

		form = bot.getForm("addVersionForm");
		WebRequest request = form.newUnvalidatedRequest(form
				.getSubmitButton("save"));
		request.setParameter("libraryProjectId", Long.toString(anotherProject
				.getId()));

		bot.go(request);

		assertContains(ANOTHER_PROJECT_NAME, bot.getPageText());
		assertContains("has been updated with a new version", bot.getPageText());

		anotherProject = ProjectLibraryService.findProjectById(anotherProject
				.getId());
		bot.go(WellKnown.urls.getLibraryProjectDetailView() + "?"
				+ ViewProjectDetailsController.PARAMETER_ID + "="
				+ anotherProject.getId());

		bot.followLink("downloadVersion"
				+ anotherProject.getVersions().get(0).getId());

		assertEquals("content type", "application/octet-stream", bot
				.lastResponse().getContentType());

		UserProject deployedProject = world.domain().projects()
				.getWithProjectRuntime(projectOwner.getId(), PROJECT_NAME);

		assertEquals("project", deployedProject.getProject()
				.getProjectXmlDefinition(), bot.lastResponse().getText());
	}

	private LibraryProject createAndDeployProject(Category category,
			String projectName, User user) throws IOException {
		UserProject userProject = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user, projectName);
		world.domain().projects().put(userProject);

		LibraryProject project = new LibraryProject(user.getId(), userProject);
		project.setLongDescription("Some very long description submitted by "
				+ user.getId() + ".");
		project.setShortDescription(projectName + " description");
		project.setSubmittedDate(new Date());
		project.setCategory(ProjectLibraryService.findCategoryById(category
				.getId()));

		ProjectLibraryService.onProjectSubmission(project, userProject);
		return project;
	}

	/* TODO: - currently unused
	 * 
	 */
	public void XXXtestAddNewVersionToALinkedProject() throws Exception {
		Category POLLS = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				POLLS_CATEGORY_NAME, "Projects that...");
		ProjectLibraryService.createCategory(POLLS, projectOwner);

		UserProject userProject = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), projectOwner, PROJECT_NAME);
		world.domain().projects().put(userProject);

		LibraryProject project = new LibraryProject(projectOwner.getId(),
				userProject);
		project.setLongDescription("Some very long description submitted by "
				+ projectOwner.getId() + ".");
		project.setShortDescription(PROJECT_NAME);
		project.setSubmittedDate(new Date());
		project.setCategory(POLLS);
		ProjectLibraryService.onProjectSubmission(project, userProject);

		bot.logInAs(projectOwner.getId(), projectOwner.getPassword());
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		// --- Emulate conversion to a vetted project
		project = ProjectLibraryService.findProjectById(project.getId());
		Category defaultCategoryForSystemLibrary = ProjectLibraryService
				.getOrCreateDefaultCategory(ProjectLibrary.SYSTEM_LIBRARY);
		project.setCategory(defaultCategoryForSystemLibrary);
		ProjectLibraryService.onProjectUpdate(project, administrator);

		// --- Deploy a new version using Client API.
		postProject("<project name=\"" + PROJECT_NAME
				+ "\" themePath=\"NEW THEME\" format=\"1.3\" />", projectOwner
				.getId(), projectOwner.getPassword());

		userProject = world.domain().projects().getWithProjectRuntime(
				projectOwner.getId(), PROJECT_NAME);

		navigateToAddVersionPage();

		WebForm form = bot.getForm("addVersionForm");
		WebRequest request = form.newUnvalidatedRequest(form
				.getSubmitButton("save"));

		request.setParameter("projectVersion.text", "New version");
		request
				.setParameter("libraryProjectId", Long
						.toString(project.getId()));
		bot.go(request);

		assertContains(PROJECT_IS_VETTED_MESSAGE, bot.getPageText());
		// --- Emulate conversion to an un-vetted project
		project = ProjectLibraryService.findProjectById(project.getId());
		Category defaultCategoryForCommunityLibrary = ProjectLibraryService
				.getOrCreateDefaultCategory(ProjectLibrary.COMMUNITY_LIBRARY);
		project.setCategory(defaultCategoryForCommunityLibrary);
		ProjectLibraryService.onProjectUpdate(project, administrator);

		form = bot.getForm("addVersionForm");
		request = form.newUnvalidatedRequest(form.getSubmitButton("save"));
		request.setParameter("projectVersion.text", "New version");
		request
				.setParameter("libraryProjectId", Long
						.toString(project.getId()));
		bot.go(request);

		assertContains("has been updated with a new version", bot.getPageText());

		project = ProjectLibraryService.findProjectById(project.getId());

		bot.go(WellKnown.urls.getLibraryProjectDetailView() + "?"
				+ ViewProjectDetailsController.PARAMETER_ID + "="
				+ project.getId());

		bot
				.followLink("downloadVersion"
						+ project.getVersions().get(0).getId());

		assertEquals("content type", "application/octet-stream", bot
				.lastResponse().getContentType());
		assertEquals("project", userProject.getProject()
				.getProjectXmlDefinition(), bot.lastResponse().getText());
	}

	private void navigateToAddVersionPage() throws RobotException {
		HttpUnitOptions.setScriptingEnabled(false);

		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		// --- Extract the link from the page. It's stored in a JavaScript
		// variable.
		Pattern pattern = Pattern.compile(
				"var linkToPublishAsVersion = '([^']*)'", Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());

		String link = matcher.group(1);
		link = link.replace("\\", ""); //--- this removes Javascript escaping.
		bot.go(link);
	}

	private void postProject(String contents, String userId, String password)
			throws IOException, SAXException {
		getResponse(ApiTest.postXml("<request type=\"" + "uploadProject"
				+ "\" protocol=\"1.0\">\n" + "<credentials user=\"" + userId
				+ "\" password=\"" + password + "\" />\n" + contents
				+ "</request>"), client);
		assertEquals(200, response.getResponseCode());
	}
}
