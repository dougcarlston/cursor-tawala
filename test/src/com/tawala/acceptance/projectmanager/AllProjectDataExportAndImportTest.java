package com.tawala.acceptance.projectmanager;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.protocol.UploadFileSpec;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.Reference;
import com.tawala.project.data.DataImporter;
import com.tawala.project.data.ProjectToExcelExporter;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.alldataimport.AllDataImportController;

public class AllProjectDataExportAndImportTest extends AcceptanceTestCase {
	private static final String FIRST_FORM_NAME = "Form 1";
	private static final String SECOND_FORM_NAME = "Form 2";
	private static final String FIRST_FIELD_NAME = "field1";
	private static final String SECOND_FIELD_NAME = "field2";
	private static final String THIRD_FIELD_NAME = "field3";
	private static final String DECLARED_FIELD_NAME = "declared_field";
	private static final String FORTH_FIELD_NAME = "field4";

	private static final String FORM_NAME = "uploadForm";

	private Project project;
	private UserProject userProject;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		WorldInitializer.getDefaultWorld().domain().users()
				.onUserUpgradeToFullyRegistered(projectOwner);

		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm(FIRST_FORM_NAME);
		FibBuilder fibBuilder = formBuilder.addFib("My field:");
		fibBuilder.addBlank(FIRST_FIELD_NAME);
		fibBuilder.addBlank(SECOND_FIELD_NAME);
		fibBuilder.addBlank(THIRD_FIELD_NAME);
		formBuilder.addDeclaredFields(DECLARED_FIELD_NAME);

		formBuilder = builder.addForm(SECOND_FORM_NAME);
		fibBuilder = formBuilder.addFib();
		fibBuilder.addBlank(FORTH_FIELD_NAME);

		project = builder.build();
		userProject = new UserProject(project, projectOwner, "test");

		world.domain().projects().put(userProject);
	}

	protected void navigateToBackupPage() throws RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		// --- Extract the link to the description page. 
		// --- It's stored in a JavaScript variable.
		Pattern pattern = Pattern.compile("var linkToExportAll = '([^']*)'",
				Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());

		String link = matcher.group(1);
		bot.go(link);

		// --- Extract the link to the export itself. 
		// --- It's stored in a JavaScript variable.
		pattern = Pattern.compile("\\s*parent.exportURL = '([^']*)'",
				Pattern.MULTILINE);
		matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());

		link = matcher.group(1);
		bot.go(link);
	}

	protected void navigateToFirstRestorePage() throws RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		// --- Extract the link to the first import page. 
		// --- It's stored in a JavaScript variable.
		Pattern pattern = Pattern.compile("var linkToImportAll = '([^']*)'",
				Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());

		String link = matcher.group(1);
		bot.go(link);
	}

	public void testBasicBackup() throws RobotException, IOException {
		recordData();
		navigateToBackupPage();

		assertEquals("application/vnd.ms-excel", bot.getContentType());
		assertMatches("attachment; filename=\"" + userProject.getName()
				+ "-\\d\\d\\d\\d\\d\\d\\d\\d_\\d\\d\\d\\d\\.xls\";", bot
				.lastResponse().getHeaderField("Content-Disposition"));
		HSSFWorkbook workbook = new HSSFWorkbook(bot.lastResponse()
				.getInputStream());
		assertEquals(3, workbook.getNumberOfSheets());

		// --- Verify data.
		HSSFSheet formSheet = workbook.getSheet(FIRST_FORM_NAME);
		assertEquals(2, formSheet.getLastRowNum());
		HSSFRow row = formSheet.getRow(1);
		assertEquals(1., row.getCell((short) 0).getNumericCellValue());
		assertEquals(2., row.getCell((short) 1).getNumericCellValue());
		assertEquals(3., row.getCell((short) 2).getNumericCellValue());

		row = formSheet.getRow(2);
		assertEquals(11., row.getCell((short) 0).getNumericCellValue());
		assertEquals(22., row.getCell((short) 1).getNumericCellValue());
		assertEquals(33., row.getCell((short) 2).getNumericCellValue());

		formSheet = workbook.getSheet(SECOND_FORM_NAME);
		assertEquals(1, formSheet.getLastRowNum());
		row = formSheet.getRow(1);
		assertEquals(4., row.getCell((short) 0).getNumericCellValue());
	}

	public void testRestoreFromHomePage() throws RobotException, IOException {
		recordData();

		navigateToBackupPage();
		HSSFWorkbook workbook = new HSSFWorkbook(bot.lastResponse()
				.getInputStream());
		navigateToFirstRestorePage();

		// --- File submission step
		WebForm form = bot.getForm(FORM_NAME);
		
		//--- Test empty submission
		bot.submit(form, AllDataImportController.PARAM_TARGET + "1");
		assertContains("The backup file was not uploaded. Please try again.", bot.getPageText());

		form = bot.getForm(FORM_NAME);
		ByteArrayOutputStream workbookData = new ByteArrayOutputStream();
		workbook.write(workbookData);

		form.setParameter("data",
				new UploadFileSpec[] { new UploadFileSpec(
						"c:\\temp\\test-backup.xls", new ByteArrayInputStream(
								workbookData.toByteArray()),
						"application/vnd.ms-excel") });

		bot.submit(form, AllDataImportController.PARAM_TARGET + "1");

		assertContains("I understand. Please proceed.", bot.getPageText());

		// --- Approval step
		form = bot.getForm(FORM_NAME);
		form.setCheckbox("confirmDataDeletion", true);
		bot.submit(form, AllDataImportController.PARAM_FINISH);

		// --- confirmation page
		assertContains("Data has been imported.", bot.getPageText());

		// --- Verify restored data
		List<FormSubmission> submissions = world.domain().storedData()
				.responsesFor(userProject.getProject(), FIRST_FORM_NAME);
		assertEquals(2, submissions.size());
		FormSubmission submission = submissions.get(0);
		assertEquals("1", submission.getValue(new Reference(FIRST_FIELD_NAME))
				.toString());
		assertEquals("2", submission.getValue(new Reference(SECOND_FIELD_NAME))
				.toString());
		assertEquals("3", submission.getValue(new Reference(THIRD_FIELD_NAME))
				.toString());

		submission = submissions.get(1);
		assertEquals("11", submission.getValue(new Reference(FIRST_FIELD_NAME))
				.toString());
		assertEquals("22", submission
				.getValue(new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("33", submission.getValue(new Reference(THIRD_FIELD_NAME))
				.toString());

		submissions = world.domain().storedData().responsesFor(
				userProject.getProject(), SECOND_FORM_NAME);
		assertEquals(1, submissions.size());
		submission = submissions.get(0);
		assertEquals("4", submission.getValue(new Reference(FORTH_FIELD_NAME))
				.toString());
	}

	public void testRestoreFromTheBackupLink() throws RobotException,
			IOException {
		navigateToBackupPage();
		HSSFWorkbook workbook = new HSSFWorkbook(bot.lastResponse()
				.getInputStream());

		HSSFCell cell = DataImporter.getNamedCell(workbook,
				ProjectToExcelExporter.LINK_TO_PROJECT_RESTORE_NAME);
		assertNotNull(cell);
		String formula = cell.getCellFormula();

		Pattern pattern = Pattern.compile("HYPERLINK\\(\"([^\"]*)\".*");
		Matcher matcher = pattern.matcher(formula);
		assertTrue(matcher.matches());
		String url = matcher.group(1);

		bot.go(url);

		// --- File submission step
		WebForm form = bot.getForm(FORM_NAME);
		assertNotNull(form);

		ByteArrayOutputStream workbookData = new ByteArrayOutputStream();
		workbook.write(workbookData);

		form.setParameter("data",
				new UploadFileSpec[] { new UploadFileSpec(
						"c:\\temp\\test-backup.xls", new ByteArrayInputStream(
								workbookData.toByteArray()),
						"application/vnd.ms-excel") });

		bot.submit(form, AllDataImportController.PARAM_TARGET + "1");

		assertContains("I understand. Please proceed.", bot.getPageText());

		// --- Approval step
		form = bot.getForm(FORM_NAME);
		form.setCheckbox("confirmDataDeletion", true);
		bot.submit(form, AllDataImportController.PARAM_FINISH);

		// --- confirmation page
		assertContains("Data has been imported.", bot.getPageText());
	}

	private void recordData() throws RobotException {
		bot.go(userProject, userProject.getProject().getForm(FIRST_FORM_NAME));
		WebForm form = bot.getForm(0);
		form.setParameter(FIRST_FIELD_NAME, "1");
		form.setParameter(SECOND_FIELD_NAME, "2");
		form.setParameter(THIRD_FIELD_NAME, "3");
		bot.submit(form);

		bot.go(userProject, userProject.getProject().getForm(FIRST_FORM_NAME));
		form = bot.getForm(0);
		form.setParameter(FIRST_FIELD_NAME, "11");
		form.setParameter(SECOND_FIELD_NAME, "22");
		form.setParameter(THIRD_FIELD_NAME, "33");
		bot.submit(form);

		bot.go(userProject, userProject.getProject().getForm(SECOND_FORM_NAME));
		form = bot.getForm(0);
		form.setParameter(FORTH_FIELD_NAME, "4");
		bot.submit(form);
	}
}
