package com.tawala.acceptance.component.web.display;

import java.util.Collections;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.display.ChoiceTallyTable;
import com.tawala.component.web.display.ItemizationTable;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.RecordSelector;

public class ChoiceTallyTableTest extends AcceptanceTestCase {
	private static final String NAME_PARAMETER = "name";
	private static final String CHOICE_PARAMETER = "choice";
	private static final String MAIN_FORM = "Main";
	private static final String REPORT_FORM = "Report";
	private UserProject userProject;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		Project project = buildProject(null);

		userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);
	}

	private Project buildProject(String condition) {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 30);
		formBuilder.addMcWithAlternateLabel(CHOICE_PARAMETER,
				"What do you like the most?", false, true, "oranges",
				"bananas", "pears", "cherries");

		DocumentBuilder documentBuilder = projectBuilder.addDocument("Report");
		ComponentBuilder choiceTallyTableBuilder = new ComponentBuilder(
				new ChoiceTallyTable());

		choiceTallyTableBuilder.addTextParameter(ChoiceTallyTable.FIELD_ID,
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM + ":"
						+ CHOICE_PARAMETER);
		choiceTallyTableBuilder.addConditionsParameter(
				ItemizationTable.CONDITIONS_PARAMETER_ID, Collections
						.singletonList(new Object[] {MAIN_FORM, false}), condition);

		documentBuilder.addComponent(choiceTallyTableBuilder);

		FormBuilder reportFormBuilder = projectBuilder.addForm(REPORT_FORM);
		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("Main Post Process");
		processBlockBuilder.addShow(documentBuilder);

		reportFormBuilder.setPreProcess(processBlockBuilder);

		return projectBuilder.build();
	}

	public void testDisplayEmptyList() throws RobotException {
		bot.go(userProject, REPORT_FORM);
		assertContains("<td colspan=\"3\">There were no responses to this question.</td>",
				bot.getPageText());
		assertMatches("<th>.*Choice.*</th>", bot.getPageText());
	}

	public void testDisplayNonEmptyList() throws Exception {
		addData("Joe", "b");
		bot.go(userProject, REPORT_FORM);
		assertHasRow("oranges", 0, 0);
		assertHasRow("bananas", 1, 100);
		assertHasRow("pears", 0, 0);
		assertHasRow("cherries", 0, 0);

		addData("Jim", "c");
		addData("Sarah", "b", "c", "d");
		bot.go(userProject, REPORT_FORM);
		assertHasRow("oranges", 0, 0);
		assertHasRow("bananas", 2, 40);
		assertHasRow("pears", 2, 40);
		assertHasRow("cherries", 1, 20);
	}

	public void testDataFiltering() throws Exception {
		String userJames = "James";

		String userJim = "Jim";
		Project project = buildProject("<equals field=\""
				+ RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM
				+ ":" + NAME_PARAMETER + "\">" + "<string value=\"" + userJim
				+ "\" />" + "</equals>");

		userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		addData("Joe", "a", "c");
		addData(userJim, "b");
		addData(userJames, "a", "c", "d");
		addData("Julia", "b");

		bot.go(userProject, REPORT_FORM);
		assertHasRow("oranges", 0, 0);
		assertHasRow("bananas", 1, 100);
		assertHasRow("pears", 0, 0);
		assertHasRow("cherries", 0, 0);

		project = buildProject("<equals field=\""
				+ RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM
				+ ":" + NAME_PARAMETER + "\">" + "<string value=\"" + userJames
				+ "\" />" + "</equals>");

		userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		bot.go(userProject, REPORT_FORM);
		assertHasRow("oranges", 1, 33);
		assertHasRow("bananas", 0, 0);
		assertHasRow("pears", 1, 33);
		assertHasRow("cherries", 1, 33);
	}

	private void assertHasRow(String choice, int count, int percentage)
			throws RobotException {
		String text = bot.getPageText();

		String[] rows = text.split("\n");

		for (int i = 0; i < rows.length; i++) {
			String row = rows[i];
			if (row.matches("<tr class=\"(odd|even)\"><td>.*" + choice + ".*</td><td>" + count
					+ "</td><td>.*>" + percentage + "%</span></strong></div></td></tr>")) {
				return;
			}
		}
		fail("Failed to find a row for choice '" + choice + "' with count "
				+ count + " and percentage " + percentage + ":\n" + text);
	}

	private void addData(String name, String... choices) throws Exception {
		bot.go(userProject, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, name);
		bot.setParameters(CHOICE_PARAMETER, choices);
		bot.submit();
	}
}
