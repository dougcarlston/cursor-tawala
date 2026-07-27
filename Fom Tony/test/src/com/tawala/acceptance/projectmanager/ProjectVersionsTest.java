package com.tawala.acceptance.projectmanager;

import java.io.IOException;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;

public class ProjectVersionsTest extends AcceptanceTestCase {
	private static final String PROJECT_NAME = "test";

	private UserProject userProject;

	private Project firstVersion;

	private Project secondVersion;

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		WorldInitializer.getDefaultWorld().domain().users()
				.onUserUpgradeToFullyRegistered(projectOwner);

		firstVersion = ProjectBuilder.buildMinimalisticProject();
		userProject = new UserProject(firstVersion, projectOwner, PROJECT_NAME);

		world.domain().projects().put(userProject);

		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm("Another form");
		formBuilder.addText("This is the second version");
		secondVersion = builder.build();

		userProject = new UserProject(secondVersion, projectOwner, PROJECT_NAME);
		world.domain().projects().put(userProject);

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), PROJECT_NAME);

		bot.logInAs(projectOwner);
		navigateToProjectDetailsPage();
	}

	private void navigateToProjectDetailsPage() throws RobotException {
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");
		assertContains(WellKnown.urls.getProjectManagerProjectDetailView(), bot
				.getPath().localPart());
	}

	public void testDownload() throws RobotException, IOException {
		bot.followLink("download" + userProject.getVersions().get(0).getId());
		assertEquals(secondVersion.getProjectXmlDefinition(), bot
				.lastResponse().getText());

		navigateToProjectDetailsPage();
		bot.followLink("download" + userProject.getVersions().get(1).getId());
		assertEquals(firstVersion.getProjectXmlDefinition(), bot.lastResponse()
				.getText());
	}

	public void testDeploy() throws RobotException, IOException {
		assertEquals(2, userProject.getDeployedVersion().getVersionNumber());

		assertContains("id=\"deployedVersion"
				+ userProject.getVersions().get(0).getId() + '"', bot
				.lastResponse().getText());

		bot.followLink("deploy" + userProject.getVersions().get(1).getId());

		assertContains("id=\"deployedVersion"
				+ userProject.getVersions().get(1).getId() + '"', bot
				.lastResponse().getText());

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), PROJECT_NAME);

		assertEquals(1, userProject.getDeployedVersion().getVersionNumber());
	}

	public void testDelete() throws RobotException, IOException {
		assertEquals(2, userProject.getDeployedVersion().getVersionNumber());

		WebForm form = bot.getForm("deleteVersion"
				+ userProject.getVersions().get(0).getId());
		// --- We don't to have the form for the deployed version.
		assertNull("form", form);

		form = bot.getForm("deleteVersion"
				+ userProject.getVersions().get(1).getId());
		assertNotNull("form", form);
		bot.submit(form);

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), PROJECT_NAME);

		assertEquals(1, userProject.getVersions().size());
	}
}
