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

public class SumFunctionTest extends TestCase {
	public static final String NAME_PARAMETER = "name";
	public static final String AMOUNT_PARAMETER = "amount";
	public static final String MAIN_FORM = "Main";
	private World world;
	private User owner;
	private Project project;
	private UserProject userProject;
	private ExecutionContext executionContext;

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		owner = UserTest.aUser();

		world = new World("/tmp", new UsersPersistentBunchImpl());
		world.domain().users().addOrSave(owner);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 25);
		formBuilder.addFib("How much are you willing to contribute?",
				AMOUNT_PARAMETER, 25);

		project = projectBuilder.build();

		userProject = new UserProject(project, owner, "test");
		world.domain().projects().put(userProject);

		Request request = new FakeRequest(true);

		Form form = project.getForm(MAIN_FORM);
		executionContext = new ExecutionContext(world.domain(),
				LinkToUserProject.createUnauthenticatedLink(userProject), form,
				request, EntryPointType.REAL_PROJECT);
	}

	public void testNoData() {
		SumFunction.Runtime function = new SumFunction.Runtime(
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM + ":"
						+ AMOUNT_PARAMETER, allDataRecordSelector());
		Value result = function.execute(executionContext);
		assertEquals(0., result.asNumber().doubleValue());
	}

	public void testNonExistentField() {
		SumFunction.Runtime function = new SumFunction.Runtime(
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM + ":"
						+ "some garbage", allDataRecordSelector());
		Value result = function.execute(executionContext);
		assertEquals(0., result.asNumber().doubleValue());
	}

	public void testSingleRow() throws IOException {
		addData("Joe", "123");

		SumFunction.Runtime function = new SumFunction.Runtime(
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM + ":"
						+ AMOUNT_PARAMETER, allDataRecordSelector());
		Value result = function.execute(executionContext);
		assertEquals(123., result.asNumber().doubleValue());
	}

	public void testMultipleRowsAndDoublePrecisionCounting() throws IOException {
		double sum = 0;
		for (int i = 0; i < 10; i++) {
			double amount = i * Math.PI;
			sum += amount;
			addData("Name " + i, "" + amount);
		}

		SumFunction.Runtime function = new SumFunction.Runtime(
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM + ":"
						+ AMOUNT_PARAMETER, allDataRecordSelector());
		Value result = function.execute(executionContext);
		assertEquals(sum, result.asNumber().doubleValue());
	}

	public void testConditions() throws IOException {
		addData("abc", "12");
		addData("dce", "13");
		addData("fgh", "14");
		addData("dce", "22");
		addData("xyz", "44");
		addData("dce", "77");

		assertCorrectCountForValue("ewrwer", 0);
		assertCorrectCountForValue("abc", 12.);
		assertCorrectCountForValue("dce", 112.);
		assertCorrectCountForValue("fgh", 14.);
		assertCorrectCountForValue("xy", 0.);
	}

	private RecordSelector allDataRecordSelector() {
		return new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								MAIN_FORM)), BooleanExpression.TRUE, "Record");
	}

	private void assertCorrectCountForValue(String value, double expectedResult) {
		BooleanExpression condition = BooleanExpression.load(new ConfigElement(
				"<equals field=\"" + RecordSelector.DEFAULT_RECORD_LIST_NAME
						+ ":" + MAIN_FORM + ":" + NAME_PARAMETER + "\">"
						+ "<string value=\"" + value + "\" />" + "</equals>"));

		SumFunction.Runtime function = new SumFunction.Runtime(
				MAIN_FORM + ":" + AMOUNT_PARAMETER,
				new RecordSelector(
						Collections
								.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
										MAIN_FORM)), condition,
						RecordSelector.DEFAULT_RECORD_LIST_NAME));
		Value result = function.execute(executionContext);
		assertEquals("Expected result for value " + value, expectedResult,
				result.asNumber().doubleValue());
	}

	private void addData(String name, String amount) throws IOException {
		ExecutionContext context = new FakeExecutionContext(userProject,
				project.getForm(PopularChoiceAlgorithmTest.FORM_NAME),
				new FakeRequest(true, NAME_PARAMETER, name, AMOUNT_PARAMETER,
						amount));
		world.domain().storedData().record(context.getSubmission());
	}
}
