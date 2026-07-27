package com.tawala.acceptance.projectmanager;

import java.io.IOException;
import java.net.MalformedURLException;
import java.util.Collections;
import java.util.List;

import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.xml.sax.SAXException;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.WebLink;
import com.meterware.httpunit.WebRequest;
import com.scissor.webrobot.RobotException;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.form.DynamicMultiChoiceDataProvider;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.theme.CommonTheme;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.ChangeOnlineStatusController;
import com.tawala.web.projectmanager.ViewProjectManagerDataController;

public class ProjectManagerTest extends AcceptanceTestCase {
	private static final String FORM_NAME = "Form1";

	private static final String PROJECT_NAME = "MyProject";

	private UserProject userProject;

	private void addDefaultProjectAndNavigateToIt() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm(FORM_NAME);
		formBuilder.addFib("Fib").addBlank("Field1");
		formBuilder.addMcWithAlternateLabel("MCQField", "MCQ 1", new String[] {
				"a", "b", "c", "d", "e", "f" });

		userProject = new UserProject(builder.build(), projectOwner,
				PROJECT_NAME);
		world.domain().projects().put(userProject);

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), PROJECT_NAME);

		recordSubmission("abc", new String[] { "a" });
		recordSubmission("xxxx", new String[] { "b", "c", "d" });
		recordSubmission("123", new String[] { "a" });
		recordSubmission("123.45", new String[] { "a" });

		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");
	}

	private void recordSubmission(String fieldValue, String[] mcqValues)
			throws RobotException {
		bot.go(userProject);
		WebForm form = bot.getForm(0);
		form.setParameter("Field1", fieldValue);
		form.setParameter("MCQField", mcqValues);
		bot.submit(form);
	}

	public void testView() throws RobotException, IOException {
		addDefaultProjectAndNavigateToIt();

		bot.followLink("view1");
		assertContains("Field1", bot.lastResponse().getText());
		assertContains("abc", bot.lastResponse().getText());
		assertContains("xxxx", bot.lastResponse().getText());
	}

	public void testSummary() throws RobotException, IOException {
		addDefaultProjectAndNavigateToIt();

		bot.followLink("summary1");
		assertContains("MCQField", bot.lastResponse().getText());
	}

	public void testExportCSV() throws RobotException, IOException {
		addDefaultProjectAndNavigateToIt();

		bot.followLink("exportCSV1");
		assertEquals("application/octet-stream", bot.getContentType());
		assertContains("MCQField", bot.lastResponse().getText());
		assertContains("\"b,c,d\"", bot.lastResponse().getText());
	}

	public void testExportExcel() throws RobotException, IOException {
		addDefaultProjectAndNavigateToIt();

		bot.followLink("exportExcel1");
		assertEquals("application/vnd.ms-excel", bot.getContentType());

		HSSFWorkbook workbook = new HSSFWorkbook(bot.lastResponse()
				.getInputStream());
		HSSFSheet sheet = workbook.getSheet(FORM_NAME);
		assertNotNull(sheet);

		HSSFRow headerRow = sheet.getRow(0);
		assertEquals("MCQField", headerRow.getCell((short) 1)
				.getRichStringCellValue().getString());

		HSSFRow firstDataRow = sheet.getRow(1);
		assertContains("a", firstDataRow.getCell((short) 1)
				.getRichStringCellValue().getString());

		HSSFRow secondDataRow = sheet.getRow(2);
		assertContains("b,c,d", secondDataRow.getCell((short) 1)
				.getRichStringCellValue().getString());

		HSSFRow thirdDataRow = sheet.getRow(3);
		assertEquals(123, (int) thirdDataRow.getCell((short) 0)
				.getNumericCellValue());

		HSSFRow forthDataRow = sheet.getRow(4);
		assertEquals(123.45, forthDataRow.getCell((short) 0)
				.getNumericCellValue());

	}

	public void testBasicViewData() throws RobotException, IOException {
		addDefaultProjectAndNavigateToIt();

		bot.followLink("view1");
		assertContains("\"MCQField\"", bot.lastResponse().getText());
		assertContains("[\"b\",\"c\",\"d\"]", bot.lastResponse().getText());
	}

	public void testViewDataWithDynamicMCQ() throws RobotException, IOException {
		ProjectBuilder builder = new ProjectBuilder();

		String optionFormName = "Option";
		String optionField = "Value";

		ComponentBuilder dynamicMCQBuilder = new ComponentBuilder(
				new DynamicMultiChoiceDataProvider());
		dynamicMCQBuilder.addPreformattedParameter(
				DynamicMultiChoiceDataProvider.DISPLAY_EXPRESSION_PARAMETER,
				"<field name=\"" + RecordSelector.DEFAULT_RECORD_LIST_NAME
						+ ":" + optionFormName + ":" + optionField + "\" />");
		dynamicMCQBuilder.addPreformattedParameter(
				DynamicMultiChoiceDataProvider.VALUE_EXPRESSION_PARAMETER,
				"<field name=\"" + RecordSelector.DEFAULT_RECORD_LIST_NAME
						+ ":" + optionFormName + ":" + optionField + "\" />");

		String condition = "<mcDoesNotContain field=\"" + optionFormName + ":"
				+ optionField + "\">\n"
				+ "<string field=\"Record:Team:Division\"/>\n"
				+ "</mcDoesNotContain>";
		dynamicMCQBuilder.addConditionsParameter(
				DynamicMultiChoiceDataProvider.CONDITIONS_PARAMETER,
				Collections
						.singletonList(new Object[] { optionFormName, false }),
				condition);

		FormBuilder formBuilder = builder.addForm(FORM_NAME);
		formBuilder.addFib("Fib:", "Field1", 30);
		formBuilder.addMcWithCustomDataProvider("MCQField", "MCQ 1", false,
				false, dynamicMCQBuilder);

		FormBuilder optionForm = builder.addForm(optionFormName);
		optionForm.addFib("Add a new value:", optionField, 30);

		userProject = new UserProject(builder.build(), projectOwner,
				PROJECT_NAME);
		world.domain().projects().put(userProject);

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), PROJECT_NAME);

		// -- Add options data
		bot.go(userProject, optionFormName);
		bot.setParameter(optionField, "123");
		bot.submit();

		recordSubmission("something", new String[] { "123" });

		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		bot.followLink("view1");
		assertContains("\"MCQField\"", bot.lastResponse().getText());
		assertContains("\"123\"", bot.lastResponse().getText());
	}

	public void testDeleteSubmissionData() throws RobotException, IOException,
			SAXException {
		addDefaultProjectAndNavigateToIt();

		bot.followLink("view1");

		List<FormSubmission> submissions = WorldInitializer.getDefaultWorld()
				.domain().storedData().responsesFor(userProject.getProject(),
						userProject.getProject().defaultForm());
		int originalCount = submissions.size();
		FormSubmission firstOne = submissions.get(0);
		WebForm form = bot.getForm("deleteSubmissionForm");
		assertNotNull(form);
		WebRequest request = form.newUnvalidatedRequest();
		request.setParameter(ViewProjectManagerDataController.SUBMISSSION_ID,
				String.valueOf(firstOne.getDatabaseId()));
		bot.go(request);

		submissions = WorldInitializer.getDefaultWorld().domain().storedData()
				.responsesFor(userProject.getProject(),
						userProject.getProject().defaultForm());
		assertEquals("Number of submissions", originalCount - 1, submissions
				.size());
		assertFalse(submissions.contains(firstOne));

	}

	public void testViewInvitations() throws RobotException, IOException {
		addDefaultProjectAndNavigateToIt();

		bot.followLink("linkToInvitation");
		WebLink link = bot.getLink("mailToLink1");
		assertNotNull("mailto link", link);
		String[] parameters = link.getParameterValues("body");
		assertNotNull(parameters);

		String bodyOfEmail = parameters[0];
		assertContains(userProject.getUrlToForm(EntryPointType.REAL_PROJECT,
				userProject.getProject().getForm(FORM_NAME)), bodyOfEmail);
		assertContains(userProject.getUser().getFirstName() + ' '
				+ userProject.getUser().getLastName(), bodyOfEmail);
	}

	public void testCustomizationOfExistingProjects() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder
				.addForm(Project.SETUP_WIZARD_FORM_NAMES.iterator().next());
		formBuilder.addFib("Fib").addBlank("Field1");
		formBuilder.addMcWithAlternateLabel("MCQField", "MCQ 1", new String[] {
				"a", "b", "c", "d", "e", "f" });

		builder.addForm("UserForm");

		userProject = new UserProject(builder.build(), projectOwner,
				PROJECT_NAME);
		world.domain().projects().put(userProject);

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), PROJECT_NAME);

		recordSubmission("abc", new String[] { "a" });
		recordSubmission("xxxx", new String[] { "b", "c", "d" });

		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		bot.followLink("customizationLink");

		assertContains("This project has been saved under your <a href=\""
				+ WellKnown.urls.getProjectManagerView()
				+ "\" id=\"linkToProjectManager\">My Tawala</a> account as \""
				+ PROJECT_NAME + "\".", bot.getPageText());
	}

	public void testChangeTheme() throws RobotException, MalformedURLException,
			IOException, SAXException {
		addDefaultProjectAndNavigateToIt();
		boolean testedAtLeastOne = false;
		for (int i = 2; i < CommonTheme.ALL_THEMES.length && i < 6; i++) {
			doTestChangeTheme(CommonTheme.ALL_THEMES[i]);
			testedAtLeastOne = true;
		}
		assertTrue(testedAtLeastOne);
	}

	private void doTestChangeTheme(CommonTheme theme) throws RobotException,
			MalformedURLException, IOException, SAXException {
		WebForm form = bot.getForm("themeChangeForm");

		WebRequest request = form.getRequest();
		request.setParameter("project.project.theme.themeId", theme.getPath());
		client.getResponse(request);

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), PROJECT_NAME);
		assertEquals(theme.getPath(), userProject.getProject().getTheme()
				.getThemeId());

		Project reconstructedProject = new Project(new ConfigElement(
				userProject.getProject().getProjectXmlDefinition()));
		assertEquals(theme.getPath(), reconstructedProject.getTheme()
				.getThemeId());
	}

	public void testTakingProjectOffline() throws RobotException {
		addDefaultProjectAndNavigateToIt();

		WebForm form = bot.getForm("changeOnlineStatusForm");
		WebRequest request = form.newUnvalidatedRequest();
		request.setParameter(
				ChangeOnlineStatusController.PARAMETER_TAKE_OFFLINE, "true");

		bot.go(request);

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), PROJECT_NAME);
		assertTrue("offline", userProject.isOffline());

		bot.go(userProject);
		assertContains("Project Is Not Available", bot.getPageText());

		// --- Test putting it back online
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		form = bot.getForm("changeOnlineStatusForm");
		request = form.newUnvalidatedRequest();
		request.setParameter(
				ChangeOnlineStatusController.PARAMETER_TAKE_OFFLINE, "false");

		bot.go(request);

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), PROJECT_NAME);
		assertFalse("offline", userProject.isOffline());

		bot.go(userProject);
		assertContains("MCQ 1", bot.getPageText());
	}
}
