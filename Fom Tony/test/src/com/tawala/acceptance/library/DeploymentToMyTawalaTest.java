package com.tawala.acceptance.library;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.ViewProjectManagerDetailsController;

public class DeploymentToMyTawalaTest extends AcceptanceTestCase {
	private static final String PROJECT_NAME = "Deployment Test";
	private static final String CATEGORY_NAME = "Youth Sports";
	private static final String FORM_NAME = "main";
	private LibraryProject libraryProject;

	public DeploymentToMyTawalaTest() {
		setCategoryNamesToDelete(CATEGORY_NAME);
		setProjectNamesToDelete(PROJECT_NAME);
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		projectOwner.setAdministrator(true);
		world.domain().users().addOrSave(projectOwner);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder form = projectBuilder.addForm(FORM_NAME);
		form.addFib("Your name:", "name", 50);

		Project project = projectBuilder.build();

		Category category = new Category(ProjectLibrary.PAID_PROJECT_LIBRARY,
				CATEGORY_NAME, "Deployment test");
		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject userProject = new UserProject(project, projectOwner,
				PROJECT_NAME);
		world.domain().projects().put(userProject);

		libraryProject = new LibraryProject(projectOwner.getId(), userProject);
		libraryProject.setFeatured(false);
		libraryProject.setShortDescription("Short description");
		libraryProject.setLongDescription("Long description");
		libraryProject.setCategory(category);

		ProjectLibraryService.onProjectSubmission(libraryProject, userProject);
	}

	private void navigateToLibraryProjectDetailsPage() throws RobotException {
		bot.logInAs(projectOwner);

		bot.go(WellKnown.urls.getLibrarySearch());
		bot.followLink("selectLibraryLink"
				+ ProjectLibrary.PAID_PROJECT_LIBRARY.getId());
		bot.followLink("projectDetailsLink" + libraryProject.getId());
		assertContains(libraryProject.getShortDescription(), bot.getPageText());

		bot.followLink("deployToMyTawalaLink");
		assertContains("setPageTitle(\"Deploy to My Tawala\")", bot
				.getPageText());
	}

	public void testDeployment() throws Exception {
		navigateToLibraryProjectDetailsPage();

		WebForm form = bot.getForm("deployForm");
		form.setParameter("projectName", "Deployed Project");
		form.setParameter("themePath", "basicblue");

		bot.submit(form);

		UserProject project = world.domain().projects().getWithVersions(
				projectOwner.getId(), "Deployed Project");
		assertNotNull(project);
		assertEquals((Long) libraryProject.getLatestVersion().getId(), project
				.getOriginalLibraryProjectVersionId());

		assertEquals("Deployed version 1 of \"" + PROJECT_NAME + "\".", project
				.getDeployedVersion().getDescription());
	}

	public void testDeploymentAndSubsequentUpdates() throws Exception {
		String deployedProjectName = "DeployedProject";

		navigateToLibraryProjectDetailsPage();

		WebForm form = bot.getForm("deployForm");
		form.setParameter("projectName", deployedProjectName);
		form.setParameter("themePath", "basicblue");

		bot.submit(form);

		Project project = ProjectBuilder.buildMinimalisticProject();
		// --- Add another version of the project to the library.
		UserProject userProject = new UserProject(project, projectOwner,
				"some other project");
		userProject = world.domain().projects().put(userProject);
		LibraryProjectVersion libraryProjectVersion = new LibraryProjectVersion(
				libraryProject, projectOwner.getId(), userProject.getProject());
		libraryProjectVersion.setText("Second version of the library project");
		ProjectLibraryService.onAddingProjectVersion(userProject,
				libraryProject, libraryProjectVersion);

		bot.go(WellKnown.urls.getProjectManagerProjectDetailView() + '?'
				+ ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME
				+ '=' + deployedProjectName);
		assertContains(
				"The library has a newer version of the original project", bot
						.getPageText());
		bot.followLink("upgradeToNewerLibraryVersionLink");

		assertContains("setPageTitle(\"Upgrade Project\");", bot.getPageText());
		assertContains("Second version of the library project", bot
				.getPageText());
		form = bot.getForm("upgradeToNewerVersionForm");
		form.setParameter("newLibraryVersionNumber", "2");
		form.setParameter("versionDescription", "Upgrade to version 2.");
		bot.submit(form, "submit");

		assertDoesntContain("has a newer version", bot.getPageText());

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), deployedProjectName);
		assertEquals(2, userProject.getVersions().size());
		assertEquals(2, userProject.getDeployedVersion().getVersionNumber());
		assertEquals("Upgrade to version 2.", userProject.getDeployedVersion()
				.getDescription());

		userProject = world.domain().projects().getWithProjectRuntime(
				projectOwner.getId(), deployedProjectName);
		assertEquals(project.getProjectXmlDefinition(), userProject
				.getProject().getProjectXmlDefinition());
		assertEquals("basicblue", userProject.getProject().getTheme()
				.getThemeId());
	}

	public void testAdminToolToPushUpdatesToMultipleProjects() throws Exception {
		String deployedProjectName = "DeployedProject";

		navigateToLibraryProjectDetailsPage();

		WebForm form = bot.getForm("deployForm");
		form.setParameter("projectName", deployedProjectName);
		form.setParameter("themePath", "basicblue");

		bot.submit(form);

		// --- Add another version of the project to the library.
		Project project = ProjectBuilder.buildMinimalisticProject();
		UserProject userProject = new UserProject(project, projectOwner,
				"some other project");
		userProject = world.domain().projects().put(userProject);
		LibraryProjectVersion libraryProjectVersion = new LibraryProjectVersion(
				libraryProject, projectOwner.getId(), userProject.getProject());
		libraryProjectVersion.setText("Second version of the library project");
		ProjectLibraryService.onAddingProjectVersion(userProject,
				libraryProject, libraryProjectVersion);

		// --- Go to the report page
		UserProject deployedProject = world.domain().projects().get(
				projectOwner.getId(), deployedProjectName);
		assertNotNull(deployedProject);
		bot.go(WellKnown.urls.getAdminUpgradeProjectsWithNewerVersion());
		String expectedCheckbox = "<input type=\"checkbox\" name=\"projectIds\" value=\""
						+ deployedProject.getId() + "\"/>";
		assertContains(expectedCheckbox, bot.getPageText());

		form = bot.getForm("upgrade-projects-form");
		form.setCheckbox("projectIds", String.valueOf(deployedProject.getId()), true);
		bot.submit(form);

		assertContains("Upgrade Projects", bot.getPageText());
		assertDoesntContain(expectedCheckbox, bot.getPageText());

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), deployedProjectName);
		assertEquals(2, userProject.getVersions().size());
		assertEquals(2, userProject.getDeployedVersion().getVersionNumber());
		assertEquals("Upgraded to version 2.", userProject.getDeployedVersion()
				.getDescription());

		userProject = world.domain().projects().getWithProjectRuntime(
				projectOwner.getId(), deployedProjectName);
		assertEquals(project.getProjectXmlDefinition(), userProject
				.getProject().getProjectXmlDefinition());
		assertEquals("basicblue", userProject.getProject().getTheme()
				.getThemeId());
	}
}
