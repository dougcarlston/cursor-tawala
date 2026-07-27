package com.tawala.acceptance.project;

import java.util.List;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ConditionsBuilder;
import com.tawala.project.builder.EditRecordBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.IfBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.SkipBlockBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.SkipBlock;

public class EditRecordTest extends AcceptanceTestCase {

	private static final String EMAIL_FIELD = "email";
	private static final String SEARCH_FORM = "Search";
	private static final String MAIN_FORM = "main";

	public void testEditRegularDataWithNoCondition() throws RobotException {
		Project project = buildSimpleProject(true);
		UserProject userProject = new UserProject(project, projectOwner, "test");

		userProject = world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter(EMAIL_FIELD, "joe@example.org");
		bot.submit();
		validateValuesInSubmission(project, MAIN_FORM, 1, 1, EMAIL_FIELD,
				"joe@example.org");

		assertEquals("joe@example.org", bot.getParameter(EMAIL_FIELD));
		bot.setParameter(EMAIL_FIELD, "jim@abc.com");
		bot.submit();
		validateValuesInSubmission(project, MAIN_FORM, 1, 1, EMAIL_FIELD,
				"jim@abc.com");
	}

	public void testCreateNewSubmissionFlag() throws RobotException {
		Project project = buildSimpleProject(false);

		UserProject userProject = new UserProject(project, projectOwner, "test");

		userProject = world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter(EMAIL_FIELD, "joe@example.org");
		bot.submit();
		validateValuesInSubmission(project, MAIN_FORM, 1, 1, EMAIL_FIELD,
				"joe@example.org");

		assertEquals("joe@example.org", bot.getParameter(EMAIL_FIELD));
		bot.setParameter(EMAIL_FIELD, "jim@abc.com");
		bot.submit();
		validateValuesInSubmission(project, MAIN_FORM, 2, 1, EMAIL_FIELD,
				"joe@example.org");
		validateValuesInSubmission(project, MAIN_FORM, 2, 2, EMAIL_FIELD,
				"jim@abc.com");
	}

	public void testPreservingVariablesInSkips() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainFormBuilder = builder.addForm(MAIN_FORM);
		mainFormBuilder.addFib("Your first name:", "firstName", 20);
		SkipBlockBuilder skipBlockBuilder = mainFormBuilder.addSkip();
		skipBlockBuilder.addIfSkip("main:firstName", "stringEquals", "value",
				"Jim", SkipBlock.SKIP_TO_END, null);
		mainFormBuilder.addFib("Your last name:", "lastName", 20);

		FormBuilder searchFormBuilder = builder.addForm(SEARCH_FORM);

		ProcessBlockBuilder mainPostProcess = builder
				.addProcess("main post process");
		mainPostProcess.addShow(searchFormBuilder);

		ProcessBlockBuilder searchPreProcess = builder
				.addProcess("Search pre process");
		EditRecordBuilder editRecordBuilder = new EditRecordBuilder(MAIN_FORM,
				true);
		searchPreProcess.add(editRecordBuilder);

		mainFormBuilder.setPostProcess(mainPostProcess);
		searchFormBuilder.setPreProcess(searchPreProcess);

		Project project = builder.build();
		UserProject userProject = new UserProject(project, projectOwner, "test");

		userProject = world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter("firstName", "Joe");
		bot.submit();
		bot.setParameter("lastName", "Doe");
		bot.submit();
		validateValuesInSubmission(project, MAIN_FORM, 1, 1, "firstName", "Joe",
				"lastName", "Doe");

		assertEquals("Joe", bot.getParameter("firstName"));
		bot.setParameter("firstName", "Jim");
		bot.submit(); // --- only one submit is needed because of the skip.
		validateValuesInSubmission(project, MAIN_FORM, 1, 1, "firstName", "Jim",
				"lastName", "Doe");
	}

	public void testWhereClause() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainFormBuilder = builder.addForm(MAIN_FORM);
		mainFormBuilder.addFib("Your name:", "name", 20);

		FormBuilder searchFormBuilder = builder.addForm(SEARCH_FORM);

		ProcessBlockBuilder searchPreProcess = builder
				.addProcess("Search pre process");
		EditRecordBuilder editRecordBuilder = new EditRecordBuilder(MAIN_FORM,
				true);
		editRecordBuilder.conditions().addComparison("stringEquals",
				"Record:main:name", "value", "Jim");
		searchPreProcess.add(editRecordBuilder);

		searchFormBuilder.setPreProcess(searchPreProcess);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");

		userProject = world.domain().projects().put(userProject);

		// --- Create 4 records
		bot.go(userProject);
		bot.setParameter("name", "Joe");
		bot.submit();

		bot.go(userProject);
		bot.setParameter("name", "Jack");
		bot.submit();

		bot.go(userProject);
		bot.setParameter("name", "Jim");
		bot.submit();

		bot.go(userProject);
		bot.setParameter("name", "Jennifer");
		bot.submit();

		// --- Going to Search should find the third record and prepopulate the
		// edit field.
		bot.go(userProject, SEARCH_FORM);
		assertEquals("Jim", bot.getParameter("name"));
		bot.setParameter("name", "Judy");
		bot.submit();

		validateValuesInSubmission(project, MAIN_FORM, 4, 3, "name", "Judy");
	}

	public void testEditingSharedData() throws Exception {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainForm = builder.addForm(MAIN_FORM, true);
		mainForm.setExternalDataSource("User");
		mainForm.addFib("Your name:", "name", 25);

		FormBuilder searchFormBuilder = builder.addForm(SEARCH_FORM);
		ProcessBlockBuilder searchPreProcess = builder
				.addProcess("Search pre process");
		EditRecordBuilder editRecordBuilder = new EditRecordBuilder(MAIN_FORM,
				true);
		searchPreProcess.add(editRecordBuilder);

		searchFormBuilder.setPreProcess(searchPreProcess);

		UserProject dataSourceDefiningProject;
		dataSourceDefiningProject = new UserProject(builder.build(),
				projectOwner, "DataSource Defining Project");
		world.domain().projects().put(dataSourceDefiningProject);

		// --- Collect some data
		bot.go(dataSourceDefiningProject);
		bot.setParameter("name", "Jim");
		bot.submit();

		bot.go(dataSourceDefiningProject);
		bot.setParameter("name", "Judy");
		bot.submit();

		bot.go(dataSourceDefiningProject);
		bot.setParameter("name", "Jane");
		bot.submit();

		// --- Go to the search form.
		bot.go(dataSourceDefiningProject, SEARCH_FORM);
		assertEquals("Jim", bot.getParameter("name"));
		bot.setParameter("name", "Jim - corrected name");
		bot.submit();

		validateValuesInSubmission(world.domain().users()
				.getSharedStorageForUser(projectOwner), "User", 3, 1, "name",
				"Jim - corrected name");
	}

	public void testEditingDataWithDeclaredFieldsUsingPostProcess() throws Exception {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainForm = builder.addForm(MAIN_FORM, true);
		mainForm.addFib("Your name:", "name", 25);
		mainForm.addDeclaredFields("declaredField");

		ProcessBlockBuilder processBuilder = builder
				.addProcess("Main Post Process");
		IfBuilder ifBuilder = processBuilder.addIf();
		ConditionsBuilder conditionsBuilder = ifBuilder.conditions();
		conditionsBuilder.addComparison("equals", "EditMode", "value", "true");
		ifBuilder.falseSet().addSet("main:declaredField", OperandType.VALUE,
				"123");
		mainForm.setPostProcess(processBuilder);

		FormBuilder searchFormBuilder = builder.addForm(SEARCH_FORM);
		ProcessBlockBuilder searchPreProcess = builder
				.addProcess("Search pre process");
		searchPreProcess.addSet("EditMode", OperandType.VALUE, "true");
		EditRecordBuilder editRecordBuilder = new EditRecordBuilder(MAIN_FORM,
				true);
		searchPreProcess.add(editRecordBuilder);

		searchFormBuilder.setPreProcess(searchPreProcess);

		UserProject userProject;
		userProject = new UserProject(builder.build(), projectOwner,
				"Project with Declared Fields");
		world.domain().projects().put(userProject);

		// --- Collect some data
		bot.go(userProject, MAIN_FORM);
		bot.setParameter("name", "Jim");
		bot.submit();

		validateValuesInSubmission(userProject.getProject(), MAIN_FORM, 1, 1, "name",
				"Jim", "declaredField", "123");

		
		// --- Go to the search form.
		bot.go(userProject, SEARCH_FORM);
		assertEquals("Jim", bot.getParameter("name"));
		bot.setParameter("name", "Jim - corrected name");
		bot.submit();

		validateValuesInSubmission(userProject.getProject(), MAIN_FORM, 1, 1, "name",
				"Jim - corrected name", "declaredField", "123");
	}

	public void testEditingDataWithDeclaredFieldsUsingPreProcess() throws Exception {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainForm = builder.addForm(MAIN_FORM, true);
		mainForm.addFib("Your name:", "name", 25);
		mainForm.addDeclaredFields("editCount");

		FormBuilder secondForm = builder.addForm("second form");
		ProcessBlockBuilder secondFormPostProcess = builder.addProcess("Second form post-process");
		secondFormPostProcess.addShow(mainForm);
		secondForm.setPostProcess(secondFormPostProcess);

		//--- Post process increments editCount by 1
		ProcessBlockBuilder processBuilder = builder
				.addProcess("Main Post Process");
		processBuilder.addAddTo("main:editCount", OperandType.VALUE,
		"1");
		processBuilder.addShow(secondForm);
		mainForm.setPostProcess(processBuilder);

		//--- Pre process simply issues edit command to the same form.
		ProcessBlockBuilder preProcess = builder
				.addProcess("Pre process");
		EditRecordBuilder editRecordBuilder = new EditRecordBuilder(MAIN_FORM,
				true);
		preProcess.add(editRecordBuilder);
		mainForm.setPreProcess(preProcess);
		
		UserProject userProject;
		userProject = new UserProject(builder.build(), projectOwner,
				"Project with Declared Fields");
		world.domain().projects().put(userProject);

		bot.go(userProject, MAIN_FORM);
		bot.setParameter("name", "Jim");
		bot.submit();

		validateValuesInSubmission(userProject.getProject(), MAIN_FORM, 1, 1, "name",
				"Jim", "editCount", "1");

		bot.logOut();
		bot.go(userProject, secondForm.getName());
		bot.go(userProject, MAIN_FORM);
		bot.setParameter("name", "Joe");
		bot.submit();

		validateValuesInSubmission(userProject.getProject(), MAIN_FORM, 1, 1, "name",
				"Joe", "editCount", "2");

		bot.logOut();
		bot.go(userProject, secondForm.getName());
		bot.go(userProject, MAIN_FORM);
		bot.setParameter("name", "Charles");
		bot.submit();

		validateValuesInSubmission(userProject.getProject(), MAIN_FORM, 1, 1, "name",
				"Charles", "editCount", "3");

	}

	public void testEditAndGet_Bug_550() throws Exception {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainForm = builder.addForm(MAIN_FORM, true);
		mainForm.addFib("Your name:", "name", 25);
		ProcessBlockBuilder processBuilder = builder
				.addProcess("Main Post Process");
		processBuilder.addGet("RecordList", MAIN_FORM);
		mainForm.setPostProcess(processBuilder);

		FormBuilder searchFormBuilder = builder.addForm(SEARCH_FORM);
		ProcessBlockBuilder searchPreProcess = builder
				.addProcess("Search pre process");
		EditRecordBuilder editRecordBuilder = new EditRecordBuilder(MAIN_FORM,
				true);
		searchPreProcess.add(editRecordBuilder);

		searchFormBuilder.setPreProcess(searchPreProcess);

		UserProject userProject;
		userProject = new UserProject(builder.build(), projectOwner,
				"Bug 550 test");
		world.domain().projects().put(userProject);

		// --- Collect some data
		bot.go(userProject, MAIN_FORM);
		bot.setParameter("name", "Jim");
		bot.submit();

		validateValuesInSubmission(userProject.getProject(), MAIN_FORM, 1, 1, "name",
				"Jim");
		
		// --- Go to the search form.
		bot.go(userProject, SEARCH_FORM);
		assertEquals("Jim", bot.getParameter("name"));
		bot.setParameter("name", "Jim - corrected name");
		bot.submit();

		validateValuesInSubmission(userProject.getProject(), MAIN_FORM, 1, 1, "name",
				"Jim - corrected name");
	}

	public void testSessionExpirationDetected() throws RobotException {
		Project project = buildSimpleProject(true);
		UserProject userProject = new UserProject(project, projectOwner, "test");

		userProject = world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter(EMAIL_FIELD, "joe@example.org");
		bot.submit();
		validateValuesInSubmission(project, MAIN_FORM, 1, 1, EMAIL_FIELD,
				"joe@example.org");

		assertEquals("joe@example.org", bot.getParameter(EMAIL_FIELD));

		//--- This will simulate the session expiration.
		bot.getClient().clearContents();
		
		bot.setParameter(EMAIL_FIELD, "jim@abc.com");
		bot.submit();
		
		validateValuesInSubmission(project, MAIN_FORM, 1, 1, EMAIL_FIELD,
				"joe@example.org");

		assertContains("your previous session has expired", bot.getPageText());
	}

	public void testPreviouslyInputValuesTakePrecedenceOverPreviousRecordValue() throws RobotException {
		//--- Project that keeps editing the same record.
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainFormBuilder = builder.addForm(MAIN_FORM);
		mainFormBuilder.addFib("Your email address:", EMAIL_FIELD, 20);
		mainFormBuilder.addMcWithAlternateLabel("mcq", "Do you want to continue?", false, false, "yes");
		
		FormBuilder searchFormBuilder = builder.addForm(SEARCH_FORM);
		
		ProcessBlockBuilder searchPreProcess = builder
				.addProcess("search process");
		EditRecordBuilder editRecordBuilder = new EditRecordBuilder(MAIN_FORM,
				true);
		searchPreProcess.add(editRecordBuilder);
		
		ProcessBlockBuilder mainPreProcess = builder
				.addProcess("Main pre process");
		mainPreProcess.addSet(MAIN_FORM + ":" + EMAIL_FIELD, OperandType.VALUE, "type your email here");
		mainPreProcess.addSet(MAIN_FORM + ":" + "mcq", OperandType.VALUE, " ");
		
		searchFormBuilder.setPreProcess(searchPreProcess);
		mainFormBuilder.setPreProcess(mainPreProcess);
		
		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		//--- First submission
		bot.go(userProject);
		bot.setParameter(EMAIL_FIELD, "joe@example.org");
		bot.setCheckbox("mcq", true);
		bot.submit();
		validateValuesInSubmission(project, MAIN_FORM, 1, 1, EMAIL_FIELD,
				"joe@example.org", "mcq", "a");

		//--- Here we go into edit mode
		bot.go(userProject, SEARCH_FORM);
		assertEquals("type your email here", bot.getParameter(EMAIL_FIELD));
		assertNull(bot.getParameter("mcq"));
	}

	private void validateValuesInSubmission(Project project, String formName,
			int submissionCount, int submissionNumber,
			String... fieldsAndValues) {
		List<FormSubmission> submissions = world.domain().storedData()
				.responsesFor(project, formName);
		assertEquals("submission count", submissionCount, submissions.size());

		FormSubmission submission = submissions.get(submissionNumber - 1);
		for (int i = 0; i < fieldsAndValues.length; i += 2) {
			assertEquals("Value of '" + fieldsAndValues[i] + "'",
					fieldsAndValues[i + 1], submission.getValue(
							new Reference(fieldsAndValues[i])).toString());
		}
	}

	private Project buildSimpleProject(boolean updateRecord) {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainFormBuilder = builder.addForm(MAIN_FORM);
		mainFormBuilder.addFib("Your email address:", EMAIL_FIELD, 20);

		FormBuilder searchFormBuilder = builder.addForm(SEARCH_FORM);

		ProcessBlockBuilder mainPostProcess = builder
				.addProcess("main post process");
		mainPostProcess.addShow(searchFormBuilder);

		ProcessBlockBuilder searchPreProcess = builder
				.addProcess("Search pre process");
		EditRecordBuilder editRecordBuilder = new EditRecordBuilder(MAIN_FORM,
				updateRecord);
		searchPreProcess.add(editRecordBuilder);

		mainFormBuilder.setPostProcess(mainPostProcess);
		searchFormBuilder.setPreProcess(searchPreProcess);

		Project project = builder.build();
		return project;
	}

}
