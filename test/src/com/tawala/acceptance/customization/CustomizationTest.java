package com.tawala.acceptance.customization;

import java.util.Collections;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.springframework.mail.javamail.JavaMailSenderImpl;
import org.xml.sax.SAXException;

import com.meterware.httpunit.HTMLElement;
import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.WebLink;
import com.meterware.httpunit.WebRequest;
import com.meterware.httpunit.WebResponse;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.email.Emailer;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.ProjectsTest;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ForEachBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.theme.CommonTheme;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.CloneAndCustomizeController;
import com.tawala.web.projectmanager.ChangeProjectThemeController;
import com.tawala.web.projectmanager.SaveDuringCustomizationController;
import com.tawala.web.projectmanager.SendLinksByEmailController;

import fake.smtp.FakeSmtpMessage;
import fake.smtp.FakeSmtpServer;

public class CustomizationTest extends AcceptanceTestCase {
	private static final String CUSTOMIZATION_USER_NAME = "customizationuser";
	private String CUSTOMIZABLE_PROJECT_NAME = "Customizable Test";
	private String CATEGORY_NAME = "Customization Test";
	private String CUSTOMIZATION_FORM_NAME = Project.SETUP_WIZARD_FORM_NAMES
			.iterator().next();
	private LibraryProject libraryProject;

	public CustomizationTest() {
		setCategoryNamesToDelete(CATEGORY_NAME);
		setProjectNamesToDelete(CUSTOMIZABLE_PROJECT_NAME);
		addUserNameToDelete(CUSTOMIZATION_USER_NAME);
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder customizationForm = projectBuilder
				.addForm(CUSTOMIZATION_FORM_NAME);
		customizationForm.addText("some text on the customization form");
		customizationForm.addFib("Project name:", "projectName", 50);

		FormBuilder userForm = projectBuilder.addForm("UserForm");
		userForm.addTextWithFields("Enter something for ", "<<ProjectName>>");
		userForm.addFib("Enter something:", "userentry", 40);

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("Pre Process");
		processBlockBuilder.addGet("records", CUSTOMIZATION_FORM_NAME);
		ForEachBuilder forEachBuilder = processBlockBuilder.addForEach(
				"record", "records");
		forEachBuilder.addSet("ProjectName", OperandType.FIELD, "record:"
				+ CUSTOMIZATION_FORM_NAME + ":projectName");

		userForm.setPreProcess(processBlockBuilder);

		FormBuilder adminForm = projectBuilder
				.addForm(Project.ADMINISTRATION_FORM_NAMES.iterator().next());
		adminForm.addText("some text on the administration form");

		Project project = projectBuilder.build();

		Category category = new Category(ProjectLibrary.SYSTEM_LIBRARY,
				CATEGORY_NAME, "Customization test");
		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject userProject = new UserProject(project, projectOwner,
				CUSTOMIZABLE_PROJECT_NAME);
		world.domain().projects().put(userProject);

		libraryProject = new LibraryProject(projectOwner.getId(), userProject);
		libraryProject.setFeatured(true);
		libraryProject.setFeaturedOrder(Integer.valueOf(-1));
		libraryProject
				.setShortDescription("Short description of the customization test project");
		libraryProject
				.setLongDescription("Long description of the customization test project");
		libraryProject.setCategory(category);

		ProjectLibraryService.onProjectSubmission(libraryProject, userProject);
	}

	private void navigateToCustomizationPage() throws RobotException {
		bot.go(WellKnown.urls.getHomePage());
		assertContains(libraryProject.getShortDescription(), bot.getPageText());

		bot.followLink("projectDescription" + libraryProject.getId());

		assertContains("setPageTitle(\"Project Details\")", bot.getPageText());
		assertContains(libraryProject.getShortDescription(), bot.getPageText());
		assertContains(libraryProject.getLongDescription(), bot.getPageText());

		bot.followLink("startCustomization");

		assertContains("setPageTitle(\"Customize ", bot.getPageText());
	}

	public void testValidateCorrectLinkageToLibraryProject() throws Exception {
		navigateToCustomizationPage();

		WebForm form = bot.getForm("themeChangeForm");
		String uniqueProjectId = form.getParameterValue(ChangeProjectThemeController.PARAMETER_PROJECT_ID);

		LinkToUserProject linkToProject = world.domain().projects()
				.getWithProjectRuntime(uniqueProjectId);
		UserProject userProject = linkToProject.getProject();
		assertNotNull("user project", userProject);

		assertEquals("version id", Long.valueOf(libraryProject
				.getLatestVersion().getId()), userProject
				.getOriginalLibraryProjectVersionId());
	}

	public void testChangeTheme() throws Exception {
		for (int i = 2; i < CommonTheme.ALL_THEMES.length && i < 6; i++) {
			doTestChangeTheme(CommonTheme.ALL_THEMES[i]);
		}
	}

	private void doTestChangeTheme(CommonTheme theme) throws RobotException {
		navigateToCustomizationPage();

		WebForm form = bot.getForm("themeChangeForm");
		String uniqueProjectId = form.getParameterValue(ChangeProjectThemeController.PARAMETER_PROJECT_ID);
		
		WebRequest request = form.newUnvalidatedRequest();
		assertNotNull(request.getParameter("project.theme.themeId"));
		request.setParameter("project.theme.themeId", theme.getPath());
		
		bot.go(request);

		LinkToUserProject linkToProject = world.domain().projects()
		.getWithProjectRuntime(uniqueProjectId);

		assertEquals(theme.getPath(), linkToProject.getProject().getProject().getTheme().getThemeId());
	}

	//--- TODO: check if still useful and rework to support new customization.
	public void TODOtestValidateLinksOnCustomizationPage() throws RobotException,
			SAXException {
		navigateToCustomizationPage();

		HTMLElement customizationIframe = bot.lastResponse().getElementWithID(
				"customizeContent");
		assertNotNull("customization iframe", customizationIframe);
		assertTrue("IFRAME".equalsIgnoreCase(customizationIframe.getTagName()));
		String iframeSource = customizationIframe.getAttribute("src");

		HTMLElement linkToUserForm = bot.lastResponse().getElementWithID(
				"linkToUserForm");
		assertNotNull("link to user form", linkToUserForm);
		String urlToUserForm = linkToUserForm.getAttribute("href");

		HTMLElement invitationLink = bot.lastResponse().getElementWithID(
				"invitationLink");
		String mailtoURL = invitationLink.getAttribute("href");
		assertContains(urlToUserForm, mailtoURL);
		// --- Making sure that the email template contains \n\r and not just
		// \n.
		assertTrue("number of line breaks",
				mailtoURL.split("%0D%0A").length > 3);

		HTMLElement linkToAdminForm = bot.lastResponse().getElementWithID(
				"linkToAdminForm");
		assertNotNull("link to admin form", linkToAdminForm);

		// --- Validate that the customization form is displayed properly and
		// set a customization variable.
		bot.go(iframeSource);
		assertContains("some text on the customization form", bot.getPageText());
		bot.setParameter("projectName", "My Project");
		bot.submit();

		// --- Validate that the link to the user form is correct.
		bot.go(urlToUserForm);
		assertContains("Enter something for My Project", bot.getPageText());

		// --- Validate admin link.
		bot.go(linkToAdminForm.getAttribute("href"));
		assertContains("some text on the administration form", bot
				.getPageText());
	}

	public void testLoginDuringCustomization() throws Exception {
		bot.logInAs(projectOwner);
		navigateToCustomizationPage();
		WebLink linkToLoginDialog = bot.getLink("linkToLoginDialog");
		assertNull("link to login dialog", linkToLoginDialog);

		bot.logOut();
		navigateToCustomizationPage();
		linkToLoginDialog = bot.getLink("linkToLoginDialog");
		assertNotNull("link to login dialog", linkToLoginDialog);

		WebResponse responseToCustomization = bot.lastResponse();

		WebForm loginForm = bot.getForm("loginDuringCustomizationForm");
		assertNotNull("login form", loginForm);

		loginForm.setParameter("userName", projectOwner.getId());
		loginForm.setParameter("password", projectOwner.getPassword());

		bot.submit(loginForm);

		//--- Save the project
		WebForm saveForm = responseToCustomization.getFormWithID("saveForm");
		assertNotNull("save form", saveForm);
		saveForm.setParameter(
				SaveDuringCustomizationController.PROJECT_NAME_PARAMETER,
				"My Customized Project");
		bot.submit(saveForm);

		UserProject userProject = WorldInitializer.getDefaultWorld().domain()
				.projects().get(projectOwner.getId(), "My Customized Project");
		assertNotNull("user project", userProject);

		validateLinkToProjectManager(responseToCustomization);
	}

	public void testSendEmailWithProjectLinks() throws Exception {
		bot.go(WellKnown.urls.getLibraryCustomizeAndDeploy() + "?"
				+ CloneAndCustomizeController.PARAMETER_PROJECT_ID + "="
				+ libraryProject.getId());

		FakeSmtpServer server = new FakeSmtpServer();

		try {
			JavaMailSenderImpl senderImpl = new JavaMailSenderImpl();
			senderImpl.setPort(server.getPort());
			senderImpl.setHost("127.0.0.1");
			new Emailer().setSender(senderImpl);

			WebForm form = bot.getForm("sendEmailWithProjectLinksForm");
			assertNotNull("form to send email", form);

			form.setParameter(SendLinksByEmailController.EMAIL_PARAMETER,
					"test@example.com");
			bot.submit(form);
		} finally {
			server.shutDown();
		}

		assertContains("sendResponse = ", bot.getPageText());
		assertContains("\"success\":true", bot.getPageText());

		FakeSmtpMessage message = server.getMessage(0);
		assertContains(libraryProject.getName(), message.getBody());
		assertEquals(Collections.singletonList("test@example.com"), message
				.getRecipients());
		assertEquals("text/plain; charset=UTF-8", message.getHeader("Content-Type"));

		// --- TODO: add a more comprehensive check for all links.
		assertMatches(ProjectsTest.getUrlMatchingRE(CUSTOMIZATION_FORM_NAME), message.getBody());
	}

	private void validateLinkToProjectManager(WebResponse response)
			throws RobotException, SAXException {
		HTMLElement linkToProjectManager = response
				.getElementWithID("linkToProjectManager");
		bot.go(linkToProjectManager.getAttribute("href"));
		assertContains("<h1 id=\"pageTitle\">My Tawala</h1>", bot.getPageText());
	}

	public void testSignupDuringCustomization() throws Exception {
		bot.logInAs(projectOwner);
		navigateToCustomizationPage();
		WebLink linkToSignupDialog = bot.getLink("linkToSignupDialog");
		assertNull("link to signup dialog", linkToSignupDialog);

		bot.logOut();
		navigateToCustomizationPage();
		linkToSignupDialog = bot.getLink("linkToSignupDialog");
		assertNotNull("link to signup dialog", linkToSignupDialog);

		WebResponse responseToCustomization = bot.lastResponse();

		WebForm signupForm = bot.getForm("signupDuringCustomizationForm");
		assertNotNull("signup form", signupForm);

		signupForm.setParameter("emailAddress", "test@example.com");
		signupForm.setParameter("user.id", CUSTOMIZATION_USER_NAME);
		signupForm.setParameter("password", "abc");
		signupForm.setParameter("repeatedPassword", "abc");

		bot.submit(signupForm);

		WebForm saveForm = responseToCustomization.getFormWithID("saveForm");
		assertNotNull("save form", saveForm);
		saveForm.setParameter(
				SaveDuringCustomizationController.PROJECT_NAME_PARAMETER,
				"My Customized Project");
		bot.submit(saveForm);

		UserProject userProject = WorldInitializer.getDefaultWorld().domain()
				.projects().get(CUSTOMIZATION_USER_NAME,
						"My Customized Project");
		assertNotNull("user project", userProject);

		validateLinkToProjectManager(responseToCustomization);
	}

	public void XXXtestTestDriveDuringCustomization() throws Exception {
		bot.go(WellKnown.urls.getLibraryCustomizeAndDeploy() + "?"
				+ CloneAndCustomizeController.PARAMETER_PROJECT_ID + "="
				+ libraryProject.getId());

		WebLink link = bot.getLink("testDriveLink_UserForm");
		String onClickCode = link.getAttribute("onclick");

		Pattern pattern = Pattern.compile("Tawala\\.Customize\\.launchTestDrive\\('([^']+)'.*");
		Matcher matcher = pattern.matcher(onClickCode);
		assertTrue(matcher.matches());
		String url = matcher.group(1);
		bot.go(url);
		assertContains("Enter something for", bot.getPageText());
	}
}
