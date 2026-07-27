package com.tawala.acceptance.projectmanager;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.Iterator;
import java.util.List;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFRichTextString;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.protocol.UploadFileSpec;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.FormSubmission;
import com.tawala.project.commands.Reference;
import com.tawala.web.projectmanager.dataimport.ImportDataController;

public abstract class DataImportTestSupport extends AcceptanceTestCase {
	protected static final String DECLARED_FIELD_NAME = "declaredField1";
	protected static final String PROJECT_FORM_NAME = "First Form";
	protected static final String FIRST_FIELD_NAME = "FirstField";
	protected static final String SECOND_FIELD_NAME = "SecondField";
	protected static final String THIRD_FIELD_NAME = "ThirdField";
	protected static final String FORM_NAME = "uploadForm";

	abstract void navigateToFirstImportPage() throws RobotException;

	public void testCancelButton() throws RobotException {
		navigateToFirstImportPage();

		WebForm form = bot.getForm(FORM_NAME);
		assertNotNull(form);

		bot.submit(form, ImportDataController.PARAM_CANCEL);

		assertEquals(getExpectedCancelDestination(), bot.getPath().localPart());
	}

	abstract protected String getExpectedCancelDestination();

	public void testUpload() throws RobotException, IOException {
		navigateToFirstImportPage();

		// ------- Page 1 - data upload

		// Test no data added error handling
		WebForm form = bot.getForm(FORM_NAME);
		assertTrue(form.isSubmitAsMime());

		bot.submit(form, ImportDataController.PARAM_TARGET + "1");
		assertContains("The file was not uploaded.", bot.lastResponse()
				.getText());

		// Continue with the normal flow.
		form = bot.getForm(FORM_NAME);
		String data = "a1, a2, a3\n" + "b1, b2, b3";
		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\mydata.dat", new ByteArrayInputStream(data
						.getBytes()), "text/csv") });
		form.setParameter("excelSpreadsheet", "false");

		bot.submit(form, ImportDataController.PARAM_TARGET + "1");

		// ------- Page 2 - mapping
		form = bot.getForm(FORM_NAME);
		form.setParameter("mapping[1]", FIRST_FIELD_NAME);

		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		// ------- Page 3 - approval
		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_FINISH);

		// ------- Confirmation Page
		assertContains("Data has been imported", bot.lastResponse().getText());

		List<FormSubmission> formSubmissions = getResponses();
		assertEquals(2, formSubmissions.size());
		Iterator<FormSubmission> iterator = formSubmissions.iterator();

		FormSubmission currentFormsubmission = iterator.next();
		assertEquals("a2", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("b2", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
	}

	abstract protected List<FormSubmission> getResponses();

	public void testSkippedFirstRow() throws RobotException, IOException {
		navigateToFirstImportPage();

		WebForm form = bot.getForm(FORM_NAME);
		String data = "Header1, Header2, Header3\n" + "a1, a2, a3\n"
				+ "b1, b2, b3";
		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\mydata.dat", new ByteArrayInputStream(data
						.getBytes()), "text/csv") });
		form.setParameter("excelSpreadsheet", "false");

		bot.submit(form, ImportDataController.PARAM_TARGET + "1");

		// ------- Page 2 - mapping
		form = bot.getForm(FORM_NAME);
		form.setParameter("mapping[1]", FIRST_FIELD_NAME);
		form.setCheckbox("skipFirstRow", true);

		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		// ------- Page 3 - approval
		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_FINISH);

		// ------- Confirmation Page
		assertContains("Data has been imported", bot.lastResponse().getText());

		List<FormSubmission> formSubmissions = getResponses();
		assertEquals(2, formSubmissions.size());
		Iterator<FormSubmission> iterator = formSubmissions.iterator();

		FormSubmission currentFormsubmission = iterator.next();
		assertEquals("a2", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("b2", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());

	}

	public void testDeletePreviousRecords() throws RobotException, IOException {
		// --- Create previous record
		createARecord();

		List<FormSubmission> submissions = getResponses();
		assertEquals(1, submissions.size());

		// --- Go through the import steps.
		navigateToFirstImportPage();

		WebForm form = bot.getForm(FORM_NAME);
		String data = "Header1, Header2, Header3\n" + "a1, a2, a3\n"
				+ "b1, b2, b3";
		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\mydata.dat", new ByteArrayInputStream(data
						.getBytes()), "text/csv") });
		form.setParameter("excelSpreadsheet", "false");

		bot.submit(form, ImportDataController.PARAM_TARGET + "1");

		// ------- Page 2 - mapping
		form = bot.getForm(FORM_NAME);
		form.setParameter("mapping[1]", FIRST_FIELD_NAME);
		form.setCheckbox("skipFirstRow", true);

		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		// ------- Page 3 - approval
		form = bot.getForm(FORM_NAME);
		form.setCheckbox("deleteOldData", true);
		bot.submit(form, ImportDataController.PARAM_FINISH);

		// ------- Confirmation Page
		assertContains("Data has been imported", bot.lastResponse().getText());

		List<FormSubmission> formSubmissions = getResponses();
		assertEquals(2, formSubmissions.size());
		Iterator<FormSubmission> iterator = formSubmissions.iterator();

		FormSubmission currentFormsubmission = iterator.next();
		assertEquals("a2", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("b2", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
	}

	abstract void createARecord() throws RobotException;

	public void testUploadPlainExcel() throws RobotException, IOException {
		HSSFWorkbook workbook = createExcelWorkbook(new String[][] {
				{ "a1", "a2", "a3" }, { "b1", "b2", "b3" } });

		ByteArrayOutputStream out = new ByteArrayOutputStream();
		workbook.write(out);

		navigateToFirstImportPage();

		// ------- Page 1 - data upload

		// Test no data added error handling
		WebForm form = bot.getForm(FORM_NAME);

		form = bot.getForm(FORM_NAME);
		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\mydata.dat", new ByteArrayInputStream(out
						.toByteArray()), "application/vnd.ms-excel") });
		form.setParameter("excelSpreadsheet", "true");

		bot.submit(form, ImportDataController.PARAM_TARGET + "1");

		// ------- Page 2 - mapping
		form = bot.getForm(FORM_NAME);
		form.setParameter("mapping[1]", FIRST_FIELD_NAME);

		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		// ------- Page 3 - approval
		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_FINISH);

		// ------- Confirmation Page
		assertContains("Data has been imported", bot.lastResponse().getText());

		List<FormSubmission> formSubmissions = getResponses();
		assertEquals(2, formSubmissions.size());
		Iterator<FormSubmission> iterator = formSubmissions.iterator();

		FormSubmission currentFormsubmission = iterator.next();
		assertEquals("a2", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("b2", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
	}

	public void testUploadExcelAndUseHeaders() throws RobotException,
			IOException {
		HSSFWorkbook workbook = createExcelWorkbook(new String[][] {
				{ FIRST_FIELD_NAME, SECOND_FIELD_NAME, DECLARED_FIELD_NAME },
				{ "a1", "a2", "a3" }, { "b1", "b2", "b3" } });

		ByteArrayOutputStream out = new ByteArrayOutputStream();
		workbook.write(out);

		navigateToFirstImportPage();

		// ------- Page 1 - data upload

		// Test no data added error handling
		WebForm form = bot.getForm(FORM_NAME);

		form = bot.getForm(FORM_NAME);
		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\mydata.dat", new ByteArrayInputStream(out
						.toByteArray()), "application/vnd.ms-excel") });
		form.setParameter("excelSpreadsheet", "true");

		bot.submit(form, ImportDataController.PARAM_TARGET + "1");

		// ------- Page 2 - mapping
		assertContains("It appears that the first row is the field names.", bot
				.getPageText());

		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		// ------- Page 3 - approval
		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_FINISH);

		// ------- Confirmation Page
		assertContains("Data has been imported", bot.lastResponse().getText());

		List<FormSubmission> formSubmissions = getResponses();
		assertEquals(2, formSubmissions.size());
		Iterator<FormSubmission> iterator = formSubmissions.iterator();

		FormSubmission currentFormsubmission = iterator.next();
		assertEquals("a1", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("a2", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("a3", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("b1", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("b2", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("b3", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());
	}

	public void testDuplicateFieldAssignment() throws RobotException,
			IOException {
		HSSFWorkbook workbook = createExcelWorkbook(new String[][] {
				{ FIRST_FIELD_NAME, SECOND_FIELD_NAME, THIRD_FIELD_NAME,
						DECLARED_FIELD_NAME }, { "a1", "a2", "a3", "a4" },
				{ "b1", "b2", "b3", "b4" } });

		ByteArrayOutputStream out = new ByteArrayOutputStream();
		workbook.write(out);

		navigateToFirstImportPage();

		// ------- Page 1 - data upload

		// Test no data added error handling
		WebForm form = bot.getForm(FORM_NAME);

		form = bot.getForm(FORM_NAME);
		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\mydata.dat", new ByteArrayInputStream(out
						.toByteArray()), "application/vnd.ms-excel") });
		form.setParameter("excelSpreadsheet", "true");

		bot.submit(form, ImportDataController.PARAM_TARGET + "1");

		// ------- Page 2 - mapping
		assertContains("It appears that the first row is the field names.", bot
				.getPageText());

		// --- Test single field name is duplicate.
		form = bot.getForm(FORM_NAME);
		form.setParameter("mapping[1]", FIRST_FIELD_NAME);
		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		assertContains("Field \"" + FIRST_FIELD_NAME
				+ "\" is selected more than once.", bot.getPageText());

		// --- Test multiple field names are duplicate.
		form = bot.getForm(FORM_NAME);
		form.setParameter("mapping[2]", SECOND_FIELD_NAME);
		form.setParameter("mapping[3]", SECOND_FIELD_NAME);
		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		assertContains("Fields " + FIRST_FIELD_NAME + ", " + SECOND_FIELD_NAME
				+ " are selected more than once.", bot.getPageText());

		form = bot.getForm(FORM_NAME);
		form.setParameter("mapping[1]", SECOND_FIELD_NAME);
		form.setParameter("mapping[2]", THIRD_FIELD_NAME);
		form.setParameter("mapping[3]", DECLARED_FIELD_NAME);
		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		// ------- Page 3 - approval
		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_FINISH);

		// ------- Confirmation Page
		assertContains("Data has been imported", bot.lastResponse().getText());

		List<FormSubmission> formSubmissions = getResponses();
		assertEquals(2, formSubmissions.size());
		Iterator<FormSubmission> iterator = formSubmissions.iterator();

		FormSubmission currentFormsubmission = iterator.next();
		assertEquals("a1", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("a2", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("a3", currentFormsubmission.getValue(
				new Reference(THIRD_FIELD_NAME)).toString());
		assertEquals("a4", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("b1", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("b2", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("b3", currentFormsubmission.getValue(
				new Reference(THIRD_FIELD_NAME)).toString());
		assertEquals("b4", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());
	}

	public void testUploadExcelWithNonExistingCells() throws RobotException,
			IOException {
		HSSFWorkbook workbook = createExcelWorkbook(new String[][] {
				{ FIRST_FIELD_NAME, SECOND_FIELD_NAME, DECLARED_FIELD_NAME },
				{ "a1", null, "a3" }, { null, "b2", "b3" },
				{ null, null, "c3" } });

		ByteArrayOutputStream out = new ByteArrayOutputStream();
		workbook.write(out);

		navigateToFirstImportPage();

		// ------- Page 1 - data upload

		// Test no data added error handling
		WebForm form = bot.getForm(FORM_NAME);

		form = bot.getForm(FORM_NAME);
		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\mydata.dat", new ByteArrayInputStream(out
						.toByteArray()), "application/vnd.ms-excel") });
		form.setParameter("excelSpreadsheet", "true");

		bot.submit(form, ImportDataController.PARAM_TARGET + "1");

		// ------- Page 2 - mapping
		assertContains("It appears that the first row is the field names.", bot
				.getPageText());

		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		// ------- Page 3 - approval
		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_FINISH);

		// ------- Confirmation Page
		assertContains("Data has been imported", bot.lastResponse().getText());

		List<FormSubmission> formSubmissions = getResponses();
		assertEquals(3, formSubmissions.size());
		Iterator<FormSubmission> iterator = formSubmissions.iterator();

		FormSubmission currentFormsubmission = iterator.next();
		assertEquals("a1", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("a3", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("b2", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("b3", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("c3", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());
	}

	public void testUploadExcelAndSkipEmptyRows() throws RobotException,
			IOException {
		HSSFWorkbook workbook = createExcelWorkbook(new String[][] {
				{ FIRST_FIELD_NAME, SECOND_FIELD_NAME, DECLARED_FIELD_NAME },
				{ "a1", "a2", "a3" }, { null, null, "c3" }, { "", "", "" } });

		ByteArrayOutputStream out = new ByteArrayOutputStream();
		workbook.write(out);

		navigateToFirstImportPage();

		// ------- Page 1 - data upload

		// Test no data added error handling
		WebForm form = bot.getForm(FORM_NAME);

		form = bot.getForm(FORM_NAME);
		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\mydata.dat", new ByteArrayInputStream(out
						.toByteArray()), "application/vnd.ms-excel") });
		form.setParameter("excelSpreadsheet", "true");

		bot.submit(form, ImportDataController.PARAM_TARGET + "1");

		// ------- Page 2 - mapping
		assertContains("It appears that the first row is the field names.", bot
				.getPageText());

		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		// ------- Page 3 - approval
		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_FINISH);

		// ------- Confirmation Page
		assertContains("Data has been imported", bot.lastResponse().getText());

		List<FormSubmission> formSubmissions = getResponses();
		assertEquals(2, formSubmissions.size());
		Iterator<FormSubmission> iterator = formSubmissions.iterator();

		FormSubmission currentFormsubmission = iterator.next();
		assertEquals("a1", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("a2", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("a3", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("c3", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());
	}

	public void testUploadExcelWithDifferentNumberOfColumns()
			throws RobotException, IOException {
		HSSFWorkbook workbook = createExcelWorkbook(new String[][] {
				{ FIRST_FIELD_NAME, SECOND_FIELD_NAME, DECLARED_FIELD_NAME },
				{ "a1", "a2", "a3" }, { "b1", null, null }, { "", "c2", "" } });

		ByteArrayOutputStream out = new ByteArrayOutputStream();
		workbook.write(out);

		navigateToFirstImportPage();

		// ------- Page 1 - data upload

		// Test no data added error handling
		WebForm form = bot.getForm(FORM_NAME);

		form = bot.getForm(FORM_NAME);
		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\mydata.dat", new ByteArrayInputStream(out
						.toByteArray()), "application/vnd.ms-excel") });
		form.setParameter("excelSpreadsheet", "true");

		bot.submit(form, ImportDataController.PARAM_TARGET + "1");

		// ------- Page 2 - mapping
		assertContains("It appears that the first row is the field names.", bot
				.getPageText());

		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_TARGET + "2");

		// ------- Page 3 - approval
		form = bot.getForm(FORM_NAME);
		bot.submit(form, ImportDataController.PARAM_FINISH);

		// ------- Confirmation Page
		assertContains("Data has been imported", bot.lastResponse().getText());

		List<FormSubmission> formSubmissions = getResponses();
		assertEquals(3, formSubmissions.size());
		Iterator<FormSubmission> iterator = formSubmissions.iterator();

		FormSubmission currentFormsubmission = iterator.next();
		assertEquals("a1", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("a2", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("a3", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("b1", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());

		currentFormsubmission = iterator.next();
		assertEquals("", currentFormsubmission.getValue(
				new Reference(FIRST_FIELD_NAME)).toString());
		assertEquals("c2", currentFormsubmission.getValue(
				new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("", currentFormsubmission.getValue(
				new Reference(DECLARED_FIELD_NAME)).toString());
	}

	private static HSSFWorkbook createExcelWorkbook(String[][] data) {
		HSSFWorkbook workbook = new HSSFWorkbook();
		HSSFSheet sheet = workbook.createSheet();

		for (int i = 0; i < data.length; i++) {
			HSSFRow row = sheet.createRow(i);
			String[] rowCells = data[i];
			for (short j = 0; j < rowCells.length; j++) {
				if (rowCells[j] != null) {
					HSSFCell cell = row.createCell(j);
					cell.setCellValue(new HSSFRichTextString(rowCells[j]));
				}
			}
		}

		return workbook;
	}

}
