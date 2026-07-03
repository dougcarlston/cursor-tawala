package com.tawala.component.function;

import java.io.IOException;
import java.util.Collections;

import junit.framework.TestCase;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.UsersPersistentBunchImpl;
import com.tawala.World;
import com.tawala.component.runtime.PopularChoiceAlgorithmTest;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.Form;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.Value;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.RecordSelector.FormDataProvider;
import com.tawala.web.FakeRequest;
import com.tawala.web.Request;

public class RecordCountFunctionTest extends TestCase {
	public static final String NAME_PARAMETER = "name";
	public static final String MAIN_FORM = "Main";
	private World world;
	private User owner;
	private Project project;
	UserProject userProject;
	private ExecutionContext executionContext;

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		new HibernateTestSetup().onSetUp();
		owner = UserTest.aUser();

		world = new World("/tmp", new UsersPersistentBunchImpl());
		world.domain().users().addOrSave(owner);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 25);

		project = projectBuilder.build();

		userProject = new UserProject(project, owner, "test");
		world.domain().projects().put(userProject);

		Request request = new FakeRequest(true);

		Form form = project.getForm(MAIN_FORM);
		executionContext = new ExecutionContext(world.domain(),
				LinkToUserProject.createUnauthenticatedLink(userProject,
						userProject.getUniqueRandomId()), form, request,
				EntryPointType.REAL_PROJECT);
	}

	public void testNoData() {
		RecordCountFunction.Runtime function = new RecordCountFunction.Runtime(
				allDataRecordSelector());
		Value result = function.execute(executionContext);
		assertEquals("0", result.toString());
	}

	public void testNonExistentForm() {
		RecordCountFunction.Runtime function = new RecordCountFunction.Runtime(
				allDataRecordSelector());
		Value result = function.execute(executionContext);
		assertEquals("0", result.toString());
	}

	public void testSingleRow() throws IOException {
		addData("Joe");

		RecordCountFunction.Runtime function = new RecordCountFunction.Runtime(
				allDataRecordSelector());
		Value result = function.execute(executionContext);
		assertEquals("1", result.toString());
	}

	public void testMultipleRows() throws IOException {
		for (int i = 0; i < 10; i++) {
			addData("Name " + i);
		}

		RecordCountFunction.Runtime function = new RecordCountFunction.Runtime(
				allDataRecordSelector());
		Value result = function.execute(executionContext);
		assertEquals("10", result.toString());
	}

	public void testConditions() throws IOException {
		addData("abc");
		addData("dce");
		addData("fgh");
		addData("dce");
		addData("xyz");
		addData("dce");

		assertCorrectCountForValue("ewrwer", 0);
		assertCorrectCountForValue("abc", 1);
		assertCorrectCountForValue("dce", 3);
		assertCorrectCountForValue("fgh", 1);
		assertCorrectCountForValue("xy", 0);
	}

	public void testVersion1Parsing() throws IOException {
		for (int i = 0; i < 10; i++) {
			addData("Name " + i);
		}

		RecordCountFunction.Runtime function = new RecordCountFunction.Runtime(
				new ConfigElement("<record-count version=\"1\">" + "<"
						+ RecordCountFunction.FORM_NAME_PARAMETER + ">"
						+ MAIN_FORM + "</"
						+ RecordCountFunction.FORM_NAME_PARAMETER + ">"
						+ "</record-count>"));
		Value result = function.execute(executionContext);
		assertEquals(10, result.asNumber().intValue());

		function = new RecordCountFunction.Runtime(new ConfigElement(
				"<record-count version=\"1\">" + "<"
						+ RecordCountFunction.FORM_NAME_PARAMETER + ">"
						+ MAIN_FORM + "</"
						+ RecordCountFunction.FORM_NAME_PARAMETER + ">" + "<"
						+ RecordCountFunction.WHERE_CLAUSE_PARAMETER + ">"
						+ "<equals field=\""
						+ RecordSelector.DEFAULT_RECORD_LIST_NAME + ":"
						+ MAIN_FORM + ":" + NAME_PARAMETER + "\">"
						+ "<string value=\"" + "Name 3" + "\" />" + "</equals>"
						+ "</" + RecordCountFunction.WHERE_CLAUSE_PARAMETER
						+ ">" + "</record-count>"));

		result = function.execute(executionContext);
		assertEquals(1, result.asNumber().intValue());
	}

	private RecordSelector allDataRecordSelector() {
		return new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								MAIN_FORM)), BooleanExpression.TRUE, "Record");
	}

	private void assertCorrectCountForValue(String value, int expectedResult) {
		BooleanExpression condition = BooleanExpression.load(new ConfigElement(
				"<equals field=\"" + RecordSelector.DEFAULT_RECORD_LIST_NAME
						+ ":" + MAIN_FORM + ":" + NAME_PARAMETER + "\">"
						+ "<string value=\"" + value + "\" />" + "</equals>"));

		RecordCountFunction.Runtime function = new RecordCountFunction.Runtime(
				new RecordSelector(
						Collections
								.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
										MAIN_FORM)), condition,
						RecordSelector.DEFAULT_RECORD_LIST_NAME));
		Value result = function.execute(executionContext);
		assertEquals("Expected result for value " + value, expectedResult,
				result.asNumber().intValue());
	}

	private void addData(String data) throws IOException {
		ExecutionContext context = new FakeExecutionContext(userProject,
				project.getForm(PopularChoiceAlgorithmTest.FORM_NAME),
				new FakeRequest(true, NAME_PARAMETER, data));
		world.domain().storedData().record(context.getSubmission());
	}
}
