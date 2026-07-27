package com.tawala.component.runtime;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import junit.framework.TestCase;

import com.tawala.UsersPersistentBunchImpl;
import com.tawala.World;
import com.tawala.component.web.display.PopularChoiceTable;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Form;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.RecordSelector.FormDataProvider;
import com.tawala.web.FakeRequest;
import com.tawala.web.Request;

public class PopularChoiceAlgorithmTest extends TestCase {
	private World world;
	private User owner;
	private Project project;
	private UserProject userProject;
	private PopularChoiceAlgorithm algorithm = new PopularChoiceAlgorithm(
			PopularChoiceAlgorithmTest.FORM_NAME + ":"
					+ PopularChoiceAlgorithmTest.ABLE_TO_ATTEND_FIELD,
			new RecordSelector(
					Collections
							.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
									FORM_NAME)), BooleanExpression.TRUE,
					RecordSelector.DEFAULT_RECORD_LIST_NAME));
	public static final String NAME_FIELD = "name";
	public static final String REPORT_FORM = "Report";
	public static final String FORM_NAME = "Main";
	public static final String PREFER_TO_ATTEND_FIELD = "preferToAttend";
	public static final String ABLE_TO_ATTEND_FIELD = "ableToAttend";

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		owner = UserTest.aUser();
		world = new World("/tmp", new UsersPersistentBunchImpl());
		world.domain().users().addOrSave(owner);
		project = PopularChoiceAlgorithmTest.buildGetTogetherProject();
		userProject = new UserProject(project, owner, "test");
		world.domain().projects().put(userProject);
	}

	public static ExecutionContext newContext(World world, Project project,
			User owner) {
		UserProject userProject = new UserProject(project, owner, "test");
		world.domain().projects().put(userProject);
		Request request = new FakeRequest(true);
		Form form = project.getForm(PopularChoiceAlgorithmTest.FORM_NAME);
		return new ExecutionContext(world.domain(), LinkToUserProject
				.createUnauthenticatedLink(userProject), form, request,
				EntryPointType.REAL_PROJECT);
	}

	public void testNoData() {
		List<PopularChoiceAlgorithm.RankedChoice> result = algorithm
				.calculate(newContext(world, project, owner));
		assertEquals(0, result.size());
	}

	public void testResultCaching() {
		ExecutionContext executionContext = newContext(world, project, owner);
		List<PopularChoiceAlgorithm.RankedChoice> result = algorithm
				.calculate(executionContext);
		assertNotNull(executionContext.getCachedObject(algorithm));
		// ---Same algorithm
		algorithm = new PopularChoiceAlgorithm(
				PopularChoiceAlgorithmTest.FORM_NAME + ":"
						+ PopularChoiceAlgorithmTest.ABLE_TO_ATTEND_FIELD,
				new RecordSelector(
						Collections
								.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
										FORM_NAME)), BooleanExpression.TRUE,
						RecordSelector.DEFAULT_RECORD_LIST_NAME));

		List<PopularChoiceAlgorithm.RankedChoice> nextResult = algorithm
				.calculate(executionContext);
		assertNotNull(executionContext.getCachedObject(algorithm));
		assertSame(result, nextResult);

		// --- Different algorithm.
		algorithm = new PopularChoiceAlgorithm(
				PopularChoiceAlgorithmTest.FORM_NAME + ":" + "Some Other Field",
				new RecordSelector(
						Collections
								.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
										FORM_NAME)), BooleanExpression.TRUE,
						RecordSelector.DEFAULT_RECORD_LIST_NAME));

		nextResult = algorithm.calculate(executionContext);
		assertNotNull(executionContext.getCachedObject(algorithm));
		assertNotSame(result, nextResult);
	}

	public void testSingleRow() throws IOException {
		addData(world, userProject, new String[] { "b" }, "b");
		List<PopularChoiceAlgorithm.RankedChoice> result = algorithm
				.calculate(newContext(world, project, owner));
		assertNotNull(result);
		assertEquals(1, result.size());
		assertEquals("b", result.get(0).value);
	}

	public void testSimpleMajority() throws IOException {
		submitAndValidateResults(new String[][][] { { { "a", "b", "c" },
				{ "a" } } },
		// --- Results
				new String[] { "a", "b", "c" }, new int[] { 1, 1, 1 });

		submitAndValidateResults(new String[][][] { { { "b" }, { "a" } } },
		// --- Results
				new String[] { "b", "a", "c" }, new int[] { 2, 1, 1 });

		submitAndValidateResults(
				new String[][][] { { { "b", "c" }, { "a" } } },
				// --- Results
				new String[] { "b", "c", "a" }, new int[] { 3, 2, 1 });

		submitAndValidateResults(
				new String[][][] { { { "b", "d" }, { "a" } } },
				// --- Results
				new String[] { "b", "c", "a", "d" }, new int[] { 4, 2, 1, 1 });
		submitAndValidateResults(new String[][][] { { { "c" }, { "a" } } },
		// --- Results
				new String[] { "b", "c", "a", "d" }, new int[] { 4, 3, 1, 1 });
	}

	public void NO_LONGER_VALID_testTieResolutionBasedOnPreferredChoice()
			throws IOException {
		// --- Submissions are cumulative between invokations in the same
		// method.
		submitAndValidateResults(new String[][][] {
		// ---
				{ { "a", "b" }, { "b" } },
				// ---
				{ { "b" }, { "b" } },
				// ---
				{ { "a" }, { "b" } } },
		// --- Results
				new String[] { "b", "a" }, new int[] { 2, 2 });

		// This should bring it to a tie; technically the order is
		// irrelevant, in practice it's the first encountered that wins
		submitAndValidateResults(new String[][][] {
		// ---
				{ { "a", "b" }, { "a" } },
				// ---
				{ { "a", "b" }, { "a" } },
				// ---
				{ { "a", "b" }, { "a" } }, },
		// --- Results
				new String[] { "a", "b" }, new int[] { 5, 5 });

		// --- This should tip the balance toward "a"
		submitAndValidateResults(new String[][][] {
		// ---
				{ { "a", "b" }, { "a" } }, },
				// --- Results
				new String[] { "a", "b" }, new int[] { 6, 6 });
	}

	private void submitAndValidateResults(String[][][] data,
			String[] expectedChoices, int[] expectedCounts) throws IOException {

		assertEquals("choices and values", expectedChoices.length,
				expectedCounts.length);

		for (int i = 0; i < data.length; i++) {
			String[][] submissionData = data[i];
			if (submissionData.length != 2) {
				throw new IllegalArgumentException(
						"Each row must have exactly two arrays - the first for 'able' choices, the second for the 'preferred' choice.");
			}
			if (submissionData[1].length > 1) {
				throw new IllegalArgumentException(
						"There must be one or no preferred choice.");
			}
			addData(world, userProject, submissionData[0],
					submissionData[1].length == 0 ? null : submissionData[1][0]);
		}

		List<PopularChoiceAlgorithm.RankedChoice> result = algorithm
				.calculate(newContext(world, project, owner));
		assertNotNull("result", result);
		assertEquals("result length", expectedChoices.length, result.size());

		for (int i = 0; i < expectedChoices.length; i++) {
			assertEquals("choice #" + (i + 1), expectedChoices[i], result
					.get(i).value);
			assertEquals("choice #" + (i + 1) + " count", expectedCounts[i],
					result.get(i).count);
		}
	}

	public static Project buildGetTogetherProject() {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		FormBuilder mainForm = projectBuilder.addForm(FORM_NAME);
		mainForm.addFib("Your name:", NAME_FIELD, 25);

		mainForm.addMcWithAlternateLabel(ABLE_TO_ATTEND_FIELD,
				"Pick dates you can attend:", new String[] { "Monday",
						"Tuesday", "Wednesday", "Thursday", "Friday" });
		mainForm.addMcWithAlternateLabel(PREFER_TO_ATTEND_FIELD,
				"Pick the date you prefer to attend:", new String[] { "Monday",
						"Tuesday", "Wednesday", "Thursday", "Friday" });

		DocumentBuilder document = projectBuilder.addDocument("Report");

		ComponentBuilder popularChoiceTableBuilder = new ComponentBuilder(
				new PopularChoiceTable());
		popularChoiceTableBuilder
				.addTextParameter(PopularChoiceTable.RANK, "1");
		popularChoiceTableBuilder.addTextParameter(
				PopularChoiceTable.CHOICE_AVAILABLE_FIELD_NAME,
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + FORM_NAME + ':'
						+ ABLE_TO_ATTEND_FIELD);
		popularChoiceTableBuilder.addTextParameter(
				PopularChoiceTable.CHOICE_PREFERRED_FIELD_NAME,
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + FORM_NAME + ':'
						+ PREFER_TO_ATTEND_FIELD);
		popularChoiceTableBuilder.addTextParameter(
				PopularChoiceTable.POPULAR_CHOICE_DISPLAY_FIELD_NAME,
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + FORM_NAME + ':'
						+ NAME_FIELD);
		popularChoiceTableBuilder.addConditionsParameter(
				PopularChoiceTable.CONDITIONS_FIELD_NAME, Collections
						.singletonList(new Object[] { FORM_NAME, false }), "");

		document.addComponent(popularChoiceTableBuilder);

		ProcessBlockBuilder process = projectBuilder.addProcess("Post Main");
		process.addShow(document);

		FormBuilder reportForm = projectBuilder.addForm(REPORT_FORM);
		reportForm.setPreProcess(process);

		return projectBuilder.build();
	}

	public static void addData(World world, UserProject project,
			String[] ableChoices, String preferChoice) throws IOException {
		List<String> parameters = new ArrayList<String>();
		for (String ableChoice : ableChoices) {
			parameters.add(PopularChoiceAlgorithmTest.ABLE_TO_ATTEND_FIELD);
			parameters.add(ableChoice);
		}
		if (preferChoice != null) {
			parameters.add(PopularChoiceAlgorithmTest.PREFER_TO_ATTEND_FIELD);
			parameters.add(preferChoice);
		}

		ExecutionContext context = new FakeExecutionContext(project, project
				.getProject().getForm(PopularChoiceAlgorithmTest.FORM_NAME),
				new FakeRequest(true, parameters.toArray(new String[] {})));
		world.domain().storedData().record(context.getSubmission());
	}
}
