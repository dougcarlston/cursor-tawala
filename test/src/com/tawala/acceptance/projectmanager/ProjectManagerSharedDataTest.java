package com.tawala.acceptance.projectmanager;

import java.io.IOException;
import java.util.List;

import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.WebRequest;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.ViewProjectManagerDataController;

/*
 * TODO: fix broken tests
 */

public class ProjectManagerSharedDataTest extends AcceptanceTestCase {
	private static final String DATASOURCE_NAME = "My Datasource";
	private static final String PROJECT_NAME = "MyProject";
	private static final String FORM_NAME = "Form1";

	private UserProject userProject;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm(FORM_NAME);
		formBuilder.addFib("Fib").addBlank("Field1");
		formBuilder.addMcWithAlternateLabel("MCQField", "MCQ 1", new String[] {
				"a", "b", "c", "d", "e", "f" });

		formBuilder.setExternalDataSource(DATASOURCE_NAME);

		userProject = new UserProject(builder.build(), projectOwner,
				PROJECT_NAME);
		world.domain().projects().put(userProject);

		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), PROJECT_NAME);

		recordSubmission("abc", new String[] { "a" });
		recordSubmission("xxxx", new String[] { "b", "c", "d" });

		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("linkToViewSharedData1");
		assertContains(DATASOURCE_NAME, bot.getPageText());
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
		bot.followLink("view1");
		assertContains("Field1", bot.lastResponse().getText());
		assertContains("abc", bot.lastResponse().getText());
		assertContains("xxxx", bot.lastResponse().getText());
	}

	public void testExportCSV() throws RobotException, IOException {
		bot.followLink("exportCSV1");
		assertEquals("application/octet-stream", bot.getContentType());
		assertContains("MCQField", bot.lastResponse().getText());
		assertContains("\"b,c,d\"", bot.lastResponse().getText());
	}

	public void testExportExcel() throws RobotException, IOException {
		bot.followLink("exportExcel1");
		assertEquals("application/vnd.ms-excel", bot.getContentType());
		
		HSSFWorkbook workbook = new HSSFWorkbook(bot.lastResponse().getInputStream());
		HSSFSheet sheet = workbook.getSheet(DATASOURCE_NAME);
		assertNotNull(sheet);
		
		HSSFRow headerRow = sheet.getRow(0);
		assertEquals("MCQField", headerRow.getCell((short)1).getRichStringCellValue().toString());
		
		HSSFRow firstDataRow = sheet.getRow(1);
		assertContains("a", firstDataRow.getCell((short)1).getRichStringCellValue().toString());
		
		HSSFRow secondDataRow = sheet.getRow(2);
		assertContains("b,c,d", secondDataRow.getCell((short)1).getRichStringCellValue().toString());
	}

	public void testViewData() throws RobotException, IOException {
		bot.followLink("view1");
		assertContains("MCQField", bot.lastResponse().getText());
		assertContains("[\"b\",\"c\",\"d\"]", bot.lastResponse().getText());
	}

	public void testDeleteData() throws RobotException, IOException {
		List<FormSubmission> submissions = world.domain().storedData()
				.responsesFor(userProject.getUser().getSharedStorage(),
						DATASOURCE_NAME);
		assertEquals(2, submissions.size());

		FormSubmission tobeDeleted = submissions.get(0);

		bot.followLink("view1");

		WebForm form = bot.getForm("deleteSubmissionForm");
		WebRequest request = form.newUnvalidatedRequest();
		request.setParameter(ViewProjectManagerDataController.SUBMISSSION_ID,
				String.valueOf(tobeDeleted.getDatabaseId()));
		bot.go(request);

		submissions = world.domain().storedData().responsesFor(
				userProject.getUser().getSharedStorage(), DATASOURCE_NAME);
		assertEquals(1, submissions.size());
		assertFalse(submissions.contains(tobeDeleted));

		tobeDeleted = submissions.get(0);
		form = bot.getForm("deleteSubmissionForm");
		request = form.newUnvalidatedRequest();
		request.setParameter(ViewProjectManagerDataController.SUBMISSSION_ID,
				String.valueOf(tobeDeleted.getDatabaseId()));
		bot.go(request);

		submissions = world.domain().storedData().responsesFor(
				userProject.getUser().getSharedStorage(), DATASOURCE_NAME);
		assertEquals(0, submissions.size());
		assertFalse(submissions.contains(tobeDeleted));

	}

	public void testDeleteProject() throws RobotException, IOException {
		WebForm form = bot.getForm("confirmDeleteForm1");
		bot.submit(form);

		Project sharedStorage = world.domain().users().getSharedStorageForUser(
				projectOwner);
		assertEquals(0, sharedStorage.getDataSources().size());

		assertEquals(0, WorldInitializer.getDefaultWorld().domain()
				.storedData().responseCount(sharedStorage));
	}
}
