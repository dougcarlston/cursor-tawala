package com.tawala.acceptance.component.web.display;

import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collections;
import java.util.List;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.display.ItemizationTable;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.EditRecordBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;
import com.tawala.project.commands.RecordSelector;

public class ItemizationTableTest extends AcceptanceTestCase {
	private static final String NAME_PARAMETER = "name";
	private static final String EMAIL_PARAMETER = "email";
	private static final String COLOR_PARAMETER = "Q3";
	private static final String MAIN_FORM = "Main";
	private static final String REPORT_FORM = "Report";
	private UserProject userProject;
	private static final SimpleDateFormat dateformatter = new SimpleDateFormat("yyyy/MM/dd");

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		Project project = buildProject(null, 1);

		userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);
	}

	private Project buildProject(String condition, int versionNumber) {
		ProjectBuilder projectBuilder = assembleProjectBuilder(condition,
				versionNumber);
		return projectBuilder.build();
	}

	private ProjectBuilder assembleProjectBuilder(String condition, int versionNumber) {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 30);
		formBuilder.addFib("What's your email?", EMAIL_PARAMETER, 30);

		formBuilder.addMc("Favorite color:", "Red", "Green", "Blue");

		DocumentBuilder documentBuilder = projectBuilder.addDocument("Report");
		ComponentBuilder itemizationTableBuilder = new ComponentBuilder(
				new ItemizationTable(), versionNumber);

		itemizationTableBuilder
				.addPreformattedParameter(ItemizationTable.COLUMN_PARAMETER_ID,
				// ---
						createColumnDefinition(versionNumber, "Name",
								NAME_PARAMETER),
						// ---
						createColumnDefinition(versionNumber, "Email",
								EMAIL_PARAMETER),
						// ---
						createColumnDefinition(versionNumber, "Color",
								COLOR_PARAMETER));

		itemizationTableBuilder.addConditionsParameter(
				ItemizationTable.CONDITIONS_PARAMETER_ID, Collections
						.singletonList(new Object[] { MAIN_FORM, false }),
				condition);

		documentBuilder.addComponent(itemizationTableBuilder);

		FormBuilder reportFormBuilder = projectBuilder.addForm(REPORT_FORM);
		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("Pre-Main Process");
		processBlockBuilder.addShow(documentBuilder);

		reportFormBuilder.setPreProcess(processBlockBuilder);
		return projectBuilder;
	}

	private String createColumnDefinition(int versionNumber, String header,
			String fieldName) {
		return createColumnDefinition(versionNumber, header, MAIN_FORM,
				fieldName, null, null, null);
	}

	private String createColumnDefinition(int versionNumber, String header,
			String formName, String fieldName, String headerStyle,
			String cellStyle, String displayConditions) {
		return
		// ---
		"<"
				+ ItemizationTable.HEADER_PARAMETER_ID
				+ ">"
				+ header
				+ "</"
				+ ItemizationTable.HEADER_PARAMETER_ID
				+ ">"
				// ----
				+ "<"
				+ ItemizationTable.CONTENTS_PARAMETER_ID
				+ ">"
				+ "<field name=\"Record:"
				+ formName
				+ ":"
				+ fieldName
				+ "\" />"
				+ "</"
				+ ItemizationTable.CONTENTS_PARAMETER_ID
				+ ">"
				+
				// ---
				(headerStyle == null ? "" : "<"
						+ ItemizationTable.HEADER_STYLE_PARAMETER_ID + ">"
						+ headerStyle + "</"
						+ ItemizationTable.HEADER_STYLE_PARAMETER_ID + ">") +
				// ---
				(cellStyle == null ? "" : "<"
						+ ItemizationTable.CELL_STYLE_PARAMETER_ID + ">"
						+ cellStyle + "</"
						+ ItemizationTable.CELL_STYLE_PARAMETER_ID + ">") +
				// --
				(displayConditions == null ? "" : "<"
						+ ItemizationTable.DISPLAY_CONDITION_PARAMETER_ID + ">"
						+ displayConditions + "</"
						+ ItemizationTable.DISPLAY_CONDITION_PARAMETER_ID + ">");
	}

	public void testDisplayEmptyList() throws RobotException {
		bot.go(userProject, REPORT_FORM);
		assertContains("<td colspan=\"3\">No records were found.</td>", bot
				.getPageText());
		assertMatches("<th>.*Name.*</th>", bot.getPageText());
	}

	public void testDisplayNonEmptyList() throws Exception {
		addData("Joe", "joe@example.com");
		bot.go(userProject, REPORT_FORM);
		assertMatches("<td>.*Joe.*</td>", bot.getPageText());
		assertMatches("<td>.*joe@example.com.*</td>", bot.getPageText());

		addData("Jim", "jim@example.com");
		bot.go(userProject, REPORT_FORM);
		assertMatches("<td>.*Joe.*</td>", bot.getPageText());
		assertMatches("<td>.*Jim.*</td>", bot.getPageText());

		addData("Sarah", "sarah@example.com");
		bot.go(userProject, REPORT_FORM);
		assertMatches("<td>.*Joe.*</td>", bot.getPageText());
		assertMatches("<td>.*Jim.*</td>", bot.getPageText());
		assertMatches("<td>.*Sarah.*</td>", bot.getPageText());
	}

	public void testDataFiltering() throws Exception {
		Project project = buildProject("<equals field=\""
				+ RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM
				+ ":" + NAME_PARAMETER + "\">" + "<string value=\"" + "Jim"
				+ "\" />" + "</equals>", 1);

		userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		addData("Joe", "joe@example.com");
		addData("Jim", "jim@example.com");
		addData("James", "james@example.com");
		addData("Julia", "julia@example.com");

		bot.go(userProject, REPORT_FORM);
		assertDoesntContain("Joe", bot.getPageText());
		assertDoesntContain("James", bot.getPageText());
		assertDoesntContain("Julia", bot.getPageText());

		assertMatches("<td>.*Jim.*</td>", bot.getPageText());
		assertMatches("<td>.*jim@example.com.*</td>", bot.getPageText());
	}

	public void testDisplayChoiceText() throws Exception {
		addData("Joe", "joe@example.com", new String[] { "a", "b", "c" });
		bot.go(userProject, REPORT_FORM);
		assertMatches("<td>.*Red.*</td>", bot.getPageText());
		assertMatches("<td>.*Green.*</td>", bot.getPageText());
		assertMatches("<td>.*Blue.*</td>", bot.getPageText());

	}

	public void testDisplayOfSharedData() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 30);
		formBuilder.setExternalDataSource("SharedDataSource");

		Project project = projectBuilder.build();
		UserProject sharedDataProject = new UserProject(project, projectOwner,
				"SharedData");

		projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder.addDocument("Report");
		ComponentBuilder itemizationTableBuilder = new ComponentBuilder(
				new ItemizationTable(), 1);

		itemizationTableBuilder.addPreformattedParameter(
				ItemizationTable.COLUMN_PARAMETER_ID, createColumnDefinition(1,
						"Name", "SharedDataSource", NAME_PARAMETER, null, null,
						null));

		itemizationTableBuilder.addConditionsParameter(
				ItemizationTable.CONDITIONS_PARAMETER_ID,
				Collections.singletonList(new Object[] { "SharedDataSource",
						true }), null);

		documentBuilder.addComponent(itemizationTableBuilder);

		FormBuilder reportFormBuilder = projectBuilder.addForm(REPORT_FORM);
		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("Main Post Process");
		processBlockBuilder.addShow(documentBuilder);

		reportFormBuilder.setPreProcess(processBlockBuilder);
		UserProject reportProject = new UserProject(projectBuilder.build(),
				projectOwner, "TestDisplayOfSharedData");

		// --- Create the two projects.
		world.domain().projects().put(sharedDataProject);
		world.domain().projects().put(reportProject);

		bot.go(sharedDataProject, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Jim");
		bot.submit();

		bot.go(sharedDataProject, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Joe");
		bot.submit();

		bot.go(reportProject);
		assertMatches("<td>.*Jim.*</td>", bot.getPageText());
		assertMatches("<td>.*Joe.*</td>", bot.getPageText());
	}

	public void testUseOfHeaderAndCellStyles() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 30);

		DocumentBuilder documentBuilder = projectBuilder.addDocument("Report");
		ComponentBuilder itemizationTableBuilder = new ComponentBuilder(
				new ItemizationTable(), 1);

		itemizationTableBuilder.addPreformattedParameter(
				ItemizationTable.COLUMN_PARAMETER_ID, createColumnDefinition(1,
						"Name", MAIN_FORM, NAME_PARAMETER, "color: red",
						"color: \"blue\"" /* test of escaping of " */, null));

		itemizationTableBuilder
				.addConditionsParameter(
						ItemizationTable.CONDITIONS_PARAMETER_ID,
						Collections.singletonList(new Object[] { MAIN_FORM,
								false }), null);

		documentBuilder.addComponent(itemizationTableBuilder);

		FormBuilder reportFormBuilder = projectBuilder.addForm(REPORT_FORM);
		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("Main Post Process");
		processBlockBuilder.addShow(documentBuilder);

		reportFormBuilder.setPreProcess(processBlockBuilder);

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, projectOwner,
				"Styles Test");

		// --- Create the two projects.
		world.domain().projects().put(userProject);

		bot.go(userProject, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Jim");
		bot.submit();

		bot.go(userProject, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Joe");
		bot.submit();

		bot.go(userProject, REPORT_FORM);
		assertMatches("<th style=\"color: red\">.*Name.*</th>", bot
				.getPageText());
		assertMatches("<td style=\"color: 'blue'\">.*Jim.*</td>", bot
				.getPageText());
		assertMatches("<td style=\"color: 'blue'\">.*Joe.*</td>", bot
				.getPageText());
	}

	public void testDisplayOfDeclaredFields() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 30);
		formBuilder.addDeclaredFields("DisplayName");

		ProcessBlockBuilder postProcessBuilder = projectBuilder
				.addProcess("PostProcess");
		postProcessBuilder.addSet(MAIN_FORM + ":" + "DisplayName",
				OperandType.FIELD, MAIN_FORM + ":" + NAME_PARAMETER);
		formBuilder.setPostProcess(postProcessBuilder);

		DocumentBuilder documentBuilder = projectBuilder.addDocument("Report");
		ComponentBuilder itemizationTableBuilder = new ComponentBuilder(
				new ItemizationTable(), 1);

		itemizationTableBuilder.addPreformattedParameter(
				ItemizationTable.COLUMN_PARAMETER_ID, createColumnDefinition(1,
						"Name", MAIN_FORM, "DisplayName", null, null, null));

		itemizationTableBuilder
				.addConditionsParameter(
						ItemizationTable.CONDITIONS_PARAMETER_ID,
						Collections.singletonList(new Object[] { MAIN_FORM,
								false }), null);

		documentBuilder.addComponent(itemizationTableBuilder);

		FormBuilder reportFormBuilder = projectBuilder.addForm(REPORT_FORM);
		ProcessBlockBuilder reportProcess = projectBuilder
				.addProcess("ReportProcessor");
		reportProcess.addShow(documentBuilder);

		reportFormBuilder.setPreProcess(reportProcess);
		UserProject project = new UserProject(projectBuilder.build(),
				projectOwner, "TestDisplayOfDeclaredFields");

		world.domain().projects().put(project);

		bot.go(project, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Jim");
		bot.submit();

		bot.go(project, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Joe");
		bot.submit();

		bot.go(project, REPORT_FORM);
		assertMatches("<td>.*Jim.*</td>", bot.getPageText());
		assertMatches("<td>.*Joe.*</td>", bot.getPageText());
	}

	public void testExpressionsInHeaders() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 30);

		DocumentBuilder documentBuilder = projectBuilder.addDocument("Report");
		ComponentBuilder itemizationTableBuilder = new ComponentBuilder(
				new ItemizationTable());

		itemizationTableBuilder
				.addPreformattedParameter(
						ItemizationTable.COLUMN_PARAMETER_ID,
						createColumnDefinition(
								2,
								"<string value=\"Literal: \" /><string field=\"MyVar\" />",
								MAIN_FORM, NAME_PARAMETER, null, null, null));

		itemizationTableBuilder
				.addConditionsParameter(
						ItemizationTable.CONDITIONS_PARAMETER_ID,
						Collections.singletonList(new Object[] { MAIN_FORM,
								false }), null);

		documentBuilder.addComponent(itemizationTableBuilder);

		FormBuilder reportFormBuilder = projectBuilder.addForm(REPORT_FORM);
		ProcessBlockBuilder reportProcess = projectBuilder
				.addProcess("ReportProcessor");
		reportProcess.addSet("MyVar", OperandType.VALUE, "Variable Header");
		reportProcess.addShow(documentBuilder);

		reportFormBuilder.setPreProcess(reportProcess);
		UserProject project = new UserProject(projectBuilder.build(),
				projectOwner, "TestDisplayOfDeclaredFields");

		world.domain().projects().put(project);

		bot.go(project, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Jim");
		bot.submit();

		bot.go(project, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Joe");
		bot.submit();

		bot.go(project, REPORT_FORM);
		assertContains("<th>Literal: Variable Header</th>", bot.getPageText());
		assertMatches("<td>.*Jim.*</td>", bot.getPageText());
		assertMatches("<td>.*Joe.*</td>", bot.getPageText());
	}

	public void testConditionalColumnDisplay() throws Exception {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 30);
		formBuilder.addFib("Email:", EMAIL_PARAMETER, 30);

		DocumentBuilder documentBuilder = projectBuilder.addDocument("Report");
		ComponentBuilder itemizationTableBuilder = new ComponentBuilder(
				new ItemizationTable());

		itemizationTableBuilder
				.addPreformattedParameter(
						ItemizationTable.COLUMN_PARAMETER_ID,
						createColumnDefinition(2, "<string value=\"Name\" />",
								MAIN_FORM, NAME_PARAMETER, null, null,
								"<equals field=\"ShowName\"><string value=\"Yes\"/></equals>"));

		itemizationTableBuilder
				.addPreformattedParameter(
						ItemizationTable.COLUMN_PARAMETER_ID,
						createColumnDefinition(2, "<string value=\"Email\" />",
								MAIN_FORM, EMAIL_PARAMETER, null, null,
								"<equals field=\"ShowEmail\"><string value=\"Yes\"/></equals>"));

		itemizationTableBuilder
				.addConditionsParameter(
						ItemizationTable.CONDITIONS_PARAMETER_ID,
						Collections.singletonList(new Object[] { MAIN_FORM,
								false }), null);

		documentBuilder.addComponent(itemizationTableBuilder);

		FormBuilder reportFormBuilder = projectBuilder.addForm(REPORT_FORM);
		ProcessBlockBuilder reportProcess = projectBuilder
				.addProcess("ReportProcessor");
		reportProcess.addSet("ShowName", OperandType.VALUE, "Yes");
		reportProcess.addSet("ShowEmail", OperandType.VALUE, "No");

		reportProcess.addShow(documentBuilder);

		reportFormBuilder.setPreProcess(reportProcess);
		UserProject project = new UserProject(projectBuilder.build(),
				projectOwner, "Test of conditional display of columns");

		world.domain().projects().put(project);

		bot.go(project, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Jim");
		bot.setParameter(EMAIL_PARAMETER, "jim@example.org");
		bot.submit();

		bot.go(project, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Mary");
		bot.setParameter(EMAIL_PARAMETER, "mary@example.org");
		bot.submit();

		bot.go(project, REPORT_FORM);
		assertContains("<th>Name</th>", bot.getPageText());
		assertDoesntContain("<th>Email</th>", bot.getPageText());

		assertMatches("<td>.*Jim.*</td>", bot.getPageText());
		assertMatches("<td>.*Mary.*</td>", bot.getPageText());

		assertDoesntContain("jim@example.org", bot.getPageText());
		assertDoesntContain("mary@example.org", bot.getPageText());
	}

	public void testTableIncludesTimestamps() throws Exception {
		addData("Joe", "joe@example.com");
		bot.go(userProject, REPORT_FORM);
		assertContains("<th>__Created__</th>", bot.getPageText());
		assertContains("<th>__Updated__</th>", bot.getPageText());
		assertMatches("<td>.*" + getCurrentDate() + ".*</td", bot.getPageText());
	}
	
	public void testTableWithEmptyListDoesNotIncludeTimestamps() throws RobotException {
		bot.go(userProject, REPORT_FORM);
		assertDoesntContain("<th>__Created__</th>", bot.getPageText());
		assertDoesntContain("<th>__Updated__</th>", bot.getPageText());
	}

	public void testWithMultipleFormsDoesNotIncludeTimestamps() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 30);
		
		FormBuilder formBuilder2 = projectBuilder.addForm("Form 2");
		formBuilder2.addFib("What's your email?", EMAIL_PARAMETER, 30);

		DocumentBuilder documentBuilder = projectBuilder.addDocument("Report");
		
		int versionNumber = 1;
		ComponentBuilder itemizationTableBuilder = new ComponentBuilder(new ItemizationTable(), versionNumber);
		itemizationTableBuilder
				.addPreformattedParameter(ItemizationTable.COLUMN_PARAMETER_ID,
						createColumnDefinition(versionNumber, "Name", MAIN_FORM,
								NAME_PARAMETER, null, null, null),
						// ---
						createColumnDefinition(versionNumber, "Email", "Form 2",
								EMAIL_PARAMETER, null, null, null));
		
		List<Object[]> formList = new ArrayList<Object[]>();
		formList.add(new Object[] { MAIN_FORM, false });
		formList.add(new Object[] { "Form 2", false });
		
		itemizationTableBuilder
				.addConditionsParameter(
						ItemizationTable.CONDITIONS_PARAMETER_ID, formList, null);

		documentBuilder.addComponent(itemizationTableBuilder);

		FormBuilder reportFormBuilder = projectBuilder.addForm(REPORT_FORM);
		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("Pre-Report Process");
		processBlockBuilder.addShow(documentBuilder);

		reportFormBuilder.setPreProcess(processBlockBuilder);
		
		UserProject project = new UserProject(projectBuilder.build(),
				projectOwner, "TestWithMultipleFormsDoesNotIncludeTimestamps");

		world.domain().projects().put(project);

		bot.go(project, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Jim");
		bot.submit();

		bot.go(project, "Form 2");
		bot.setParameter(EMAIL_PARAMETER, "jim@example.org");
		bot.submit();

		bot.go(project, REPORT_FORM);
		assertMatches("<td>.*Jim.*</td>", bot.getPageText());
		assertMatches("<td>.*jim@example.org.*</td>", bot.getPageText());
		assertDoesntContain("<th>__Created__</th>", bot.getPageText());
		assertDoesntContain("<th>__Updated__</th>", bot.getPageText());
	}
	
	public void testTableIncludesTimestampsAfterEdit() throws Exception {
		ProjectBuilder projectBuilder = assembleProjectBuilder(null, 1);

		FormBuilder editFormBuilder = projectBuilder.addForm("Edit Form");
		ProcessBlockBuilder editPreProcess = projectBuilder.addProcess("Pre-Edit Process");

		EditRecordBuilder editRecordBuilder = new EditRecordBuilder(MAIN_FORM, true);
		editPreProcess.add(editRecordBuilder);
		editFormBuilder.setPreProcess(editPreProcess);
		
		UserProject project = new UserProject(projectBuilder.build(),
				projectOwner, "TestWithMultipleFormsDoesNotIncludeTimestamps");

		world.domain().projects().put(project);
		
		bot.go(project, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, "Jim");
		bot.submit();

		bot.go(project, REPORT_FORM);
		assertMatches("<td>.*Jim.*</td>", bot.getPageText());
		assertContains("<th>__Created__</th>", bot.getPageText());
		assertContains("<th>__Updated__</th>", bot.getPageText());
		assertMatches("<td>.*" + getCurrentDate() + ".*</td", bot.getPageText());
		
		bot.go(project, "Edit Form");
		bot.setParameter(NAME_PARAMETER, "Fred");
		bot.submit();
		
		bot.go(project, REPORT_FORM);
		assertMatches("<td>.*Fred.*</td>", bot.getPageText());
		assertContains("<th>__Created__</th>", bot.getPageText());
		assertContains("<th>__Updated__</th>", bot.getPageText());
		assertMatches("<td>.*" + getCurrentDate() + ".*</td", bot.getPageText());
	}

	private String getCurrentDate() {
		return new String(dateformatter.format(Calendar.getInstance().getTime()));
	}

	private void addData(String name, String email) throws Exception {
		bot.go(userProject, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, name);
		bot.setParameter(EMAIL_PARAMETER, email);
		bot.submit();
	}

	private void addData(String name, String email, String[] colors)
			throws Exception {
		bot.go(userProject, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, name);
		bot.setParameter(EMAIL_PARAMETER, email);
		bot.setParameters(COLOR_PARAMETER, colors);
		bot.submit();
	}
}
