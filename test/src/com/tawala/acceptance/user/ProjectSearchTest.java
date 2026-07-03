package com.tawala.acceptance.user;

import java.io.IOException;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.admin.ProjectSearchController;
import com.tawala.web.controller.WellKnown;

public class ProjectSearchTest extends AcceptanceTestCase {
	private static final String REGULAR_USER_NAME = "tester1";
	private static final String ADMIN_USER_NAME = "tester2";

	private User regularUser;
	private User adminUser;

	public ProjectSearchTest() {
		setUserNamesToDelete(new String[] { REGULAR_USER_NAME, ADMIN_USER_NAME });
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		regularUser = UserTest.aUser(REGULAR_USER_NAME);
		regularUser.setStatus(Status.REGISTERED);
		regularUser.setAdministrator(false);
		regularUser.setFirstName("Joe");
		regularUser.setLastName("UniqueLastName");

		world.domain().users().addOrSave(regularUser);

		adminUser = UserTest.aUser(ADMIN_USER_NAME);
		adminUser.setStatus(Status.REGISTERED);
		adminUser.setAdministrator(true);

		world.domain().users().addOrSave(adminUser);
	}

	public void testAdminOnlyLogic() throws Exception {
		bot.logInAs(regularUser.getId(), regularUser.getPassword());

		bot.go(WellKnown.urls.getAdminSearchProject());
		assertEquals(WellKnown.urls.getSportsHome(), bot.getPath().localPart());

		bot.logOut();

		bot.logInAs(adminUser.getId(), adminUser.getPassword());
		bot.go(WellKnown.urls.getAdminSearchProject());
		assertEquals(WellKnown.urls.getAdminSearchProject(), bot.getPath()
				.localPart());
	}

	public void testWebInferface() throws RobotException, IOException {
		Project project = ProjectBuilder.buildMinimalisticProject();
		UserProject userProject = new UserProject(project, regularUser, "TestOfProjectSearch");
		userProject = world.domain().projects().put(userProject);
		
		bot.logInAs(adminUser.getId(), adminUser.getPassword());
		bot.go(WellKnown.urls.getAdminSearchProject());

		//--- Test missed search.
		WebForm form = bot.getForm("searchByIdForm");
		assertNotNull(form);
		form.setParameter(ProjectSearchController.PARAMETER_PROJECT_ID, "junk");

		bot.submit(form);

		assertContains("Unable to find a project with this id!", bot.getPageText());
		assertContains("<input type=\"text\" id=\"projectId\" name=\""
				+ ProjectSearchController.PARAMETER_PROJECT_ID
				+ "\" value=\"junk\" size=\"20\" />", bot.getPageText());
		
		//--- Test successful search.
		form = bot.getForm("searchByIdForm");
		assertNotNull(form);
		form.setParameter(ProjectSearchController.PARAMETER_PROJECT_ID, userProject.getUniqueRandomId());

		bot.submit(form);

		assertDoesntContain("Unable to find a project with this id!", bot.getPageText());
		assertContains(regularUser.getId(), bot.getPageText());
		assertContains(regularUser.getFirstName(), bot.getPageText());
		assertContains(regularUser.getLastName(), bot.getPageText());
		
		assertContains(userProject.getName(), bot.getPageText());
		
		bot.followLink("viewUserDetailLink");
		assertContains(regularUser.getId(), bot.getPageText());
	}
}
