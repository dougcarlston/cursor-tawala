package com.tawala.acceptance.user;

import java.io.IOException;
import java.io.UnsupportedEncodingException;

import org.springframework.mail.javamail.JavaMailSenderImpl;
import org.springframework.mock.web.MockHttpServletRequest;
import org.springframework.mock.web.MockHttpServletResponse;
import org.springframework.web.servlet.ModelAndView;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.email.Emailer;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.admin.ViewUserDetailController;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.user.ManageUsersController;

import fake.smtp.FakeSmtpMessage;
import fake.smtp.FakeSmtpServer;

public class ManageUsersTest extends AcceptanceTestCase {
	private static final String REGULAR_USER_NAME = "tester1";
	private static final String ADMIN_USER_NAME = "tester2";

	private User regularUser;
	private User adminUser;

	public ManageUsersTest() {
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

		bot.go(WellKnown.urls.getAdminManageUsers());
		assertEquals(WellKnown.urls.getSportsHome(), bot.getPath().localPart());

		bot.logOut();

		bot.logInAs(adminUser.getId(), adminUser.getPassword());
		bot.go(WellKnown.urls.getAdminManageUsers());
		assertEquals(WellKnown.urls.getAdminManageUsers(), bot.getPath()
				.localPart());
	}

	public void testWebInferface() throws RobotException, IOException {
		bot.logInAs(adminUser.getId(), adminUser.getPassword());
		bot.go(WellKnown.urls.getAdminManageUsers());

		// -- Test search by query
		WebForm form = bot.getForm("searchByQueryForm");
		form.setParameter(ManageUsersController.PARAMETER_QUERY, regularUser
				.getLastName());

		bot.submit(form);

		assertContains("<td class=\"id\"><a href=\""
				+ WellKnown.urls.getAdminViewUserInfo() + "?"
				+ ViewUserDetailController.USER_ID_PARAMETER + "="
				+ regularUser.getDatabaseId() + "\" id=\"linkToUserDetails"
				+ regularUser.getDatabaseId() + "\">" + regularUser.getId()
				+ "</a></td>", bot.lastResponse().getText());

		// -- Test search by registration date
		form = bot.getForm("searchByRegistrationDateForm");
		form.setParameter(
				ManageUsersController.PARAMETER_REGISTRATION_DAYS_BACK, "3");

		bot.submit(form);

		assertContains("<input name=\""
				+ ManageUsersController.PARAMETER_REGISTRATION_DAYS_BACK
				+ "\" value=\"3\"", bot.lastResponse().getText());

		assertContains("<td class=\"id\"><a href=\""
				+ WellKnown.urls.getAdminViewUserInfo() + "?"
				+ ViewUserDetailController.USER_ID_PARAMETER + "="
				+ regularUser.getDatabaseId() + "\" id=\"linkToUserDetails"
				+ regularUser.getDatabaseId() + "\">" + regularUser.getId()
				+ "</a></td>", bot.lastResponse().getText());
	}

	public void testSwitchingUsers() throws RobotException {
		Project project = ProjectBuilder.buildMinimalisticProject();
		UserProject userProject = new UserProject(project, regularUser, "unique project name");
		
		world.domain().projects().put(userProject);
		
		bot.logInAs(adminUser);
		bot.go(WellKnown.urls.getAdminManageUsers());

		WebForm form = bot.getForm("searchByQueryForm");
		form.setParameter(ManageUsersController.PARAMETER_QUERY, regularUser
				.getLastName());
		bot.submit(form);

		bot.followLink("linkToUserDetails" + regularUser.getDatabaseId());
		//--- Validate that the regular user project appears on the page
		assertContains(userProject.getName(), bot.getPageText());
		
		bot.followLink("switchUserLink");

		assertContains("Welcome back, <span class=\"userName\">"
				+ regularUser.getId() + "</span>", bot.getPageText());
		//--- Validate we are seeing the right list of projects.
		assertContains(userProject.getName(), bot.getPageText());
		
		bot.followLink("restoreOriginalUserLink");
		assertContains("Welcome back, <span class=\"userName\">"
				+ adminUser.getId() + "</span>", bot.getPageText());
	}

	public void testDisplayOnly() throws Exception {
		MockHttpServletRequest request = new MockHttpServletRequest();
		MockHttpServletResponse response = new MockHttpServletResponse();

		ModelAndView modelAndView = new ManageUsersController().handleRequest(
				request, response);

		assertEquals("view name", "admin.manage.users", modelAndView
				.getViewName());
		assertNull("Users", modelAndView.getModel().get("users"));
	}

	// --- TODO: fix it. Needs to be more elaborate with user counts.
	public void XXXtestApprove() throws Exception {
		FakeSmtpServer server = new FakeSmtpServer();

		JavaMailSenderImpl senderImpl = new JavaMailSenderImpl();
		senderImpl.setPort(server.getPort());
		senderImpl.setHost("127.0.0.1");
		new Emailer().setSender(senderImpl);

		try {
			MockHttpServletRequest request = new MockHttpServletRequest();
			request.addParameter(
					ViewUserDetailController.PARAMETER_ACTION_APPROVE, "true");
			request.addParameter(ManageUsersController.PARAMETER_USER_ID,
					regularUser.getId());

			MockHttpServletResponse response = new MockHttpServletResponse();

			ModelAndView modelAndView = new ManageUsersController()
					.handleRequest(request, response);

			assertEquals("view name", "admin.manage.users", modelAndView
					.getViewName());
			assertNotNull("Users", modelAndView.getModel().get("users"));

			User updatedUser = world.domain().users().get(regularUser.getId());
			assertEquals("user status", Status.REGISTERED, updatedUser
					.getStatus());

			server.waitForAllConnectionsToClose();
			assertEquals(1, server.getMessageCount());
			validateApprovalEmail(
					world.domain().users().get(REGULAR_USER_NAME), server
							.getMessage(0));
		} finally {
			server.shutDown();
		}
	}

	public void testSuspendAndUnsuspend() throws Exception {
		MockHttpServletRequest request = new MockHttpServletRequest();
		request.addParameter(ViewUserDetailController.PARAMETER_ACTION,
				ViewUserDetailController.PARAMETER_ACTION_SUSPEND);
		request.addParameter(ViewUserDetailController.USER_ID_PARAMETER, Long
				.toString(regularUser.getDatabaseId()));

		MockHttpServletResponse response = new MockHttpServletResponse();

		new ViewUserDetailController().handleRequest(request, response);

		User updatedUser = world.domain().users().get(regularUser.getId());
		assertTrue("user status", updatedUser.isSuspended());

		// --- Unsuspend
		request = new MockHttpServletRequest();
		request.addParameter(ViewUserDetailController.PARAMETER_ACTION,
				ViewUserDetailController.PARAMETER_ACTION_RELEASE);
		request.addParameter(ViewUserDetailController.USER_ID_PARAMETER, Long
				.toString(regularUser.getDatabaseId()));

		response = new MockHttpServletResponse();

		new ViewUserDetailController().handleRequest(request, response);

		updatedUser = world.domain().users().get(regularUser.getId());
		assertTrue("user status", !updatedUser.isSuspended());
	}

	public void testDelete() throws Exception {
		MockHttpServletRequest request = new MockHttpServletRequest();
		request.addParameter(ViewUserDetailController.PARAMETER_ACTION,
				ViewUserDetailController.PARAMETER_ACTION_DELETE);
		request.addParameter(ViewUserDetailController.USER_ID_PARAMETER, Long
				.toString(regularUser.getDatabaseId()));

		MockHttpServletResponse response = new MockHttpServletResponse();

		ModelAndView modelAndView = new ViewUserDetailController()
				.handleRequest(request, response);

		assertNull(modelAndView);
		assertEquals(WellKnown.urls.getAdminManageUsers(), response
				.getRedirectedUrl());

		User updatedUser = world.domain().users().get(regularUser.getId());
		assertNull("user", updatedUser);
	}

	private void validateApprovalEmail(User user, FakeSmtpMessage message)
			throws UnsupportedEncodingException {
		assertNotNull("user", user);
		assertNotNull("message", message);

		String body = message.getBody();
		assertContains("Dear " + user.getFirstName(), body);
		assertContains("Thank you for joining the Tawala community!", body);
		assertContains("http", body);
		assertContains("<strong>" + user.getId() + "</strong>", body);

		assertEquals(user.getEmail().toString(), message.getRecipients().get(0));
	}
}
