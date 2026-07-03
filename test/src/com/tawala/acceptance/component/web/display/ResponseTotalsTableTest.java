package com.tawala.acceptance.component.web.display;

import java.util.Collections;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.display.ItemizationTable;
import com.tawala.component.web.display.ResponseTotalsTable;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.RecordSelector;

public class ResponseTotalsTableTest extends AcceptanceTestCase {
	private static final String NAME_PARAMETER = "name";
	private static final String LAYOUT_VERTICAL = "vertical";
	private static final String LAYOUT_HORIZONTAL = "horizontal";
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
		ProjectBuilder project = new ProjectBuilder();
		FormBuilder form = project.addForm(MAIN_FORM);
		form.addFib("What's your name?", NAME_PARAMETER, 30);
		form.addMcWithAlternateLabel(CHOICE_PARAMETER,
				"What do you like the most?", false, true, "oranges",
				"bananas", "pears", "cherries");

		DocumentBuilder document = project.addDocument("Report");

		ComponentBuilder responseTotalsVerticalTable = responseTotalsVerticalTableBuilder(condition);
		document.addComponent(responseTotalsVerticalTable);

		ComponentBuilder responseTotalsHorizontalTable = responseTotalsHorizontalTableBuilder(condition);
		document.addComponent(responseTotalsHorizontalTable);

		FormBuilder reportFormBuilder = project.addForm(REPORT_FORM);
		ProcessBlockBuilder processBlockBuilder = project.addProcess("Main Post Process");
		processBlockBuilder.addShow(document);

		reportFormBuilder.setPreProcess(processBlockBuilder);

		return project.build();
	}

	private ComponentBuilder responseTotalsVerticalTableBuilder(String condition) {
		ComponentBuilder responseTotalsTableBuilder = new ComponentBuilder(new ResponseTotalsTable());

		responseTotalsTableBuilder.addTextParameter(ResponseTotalsTable.LAYOUT_TYPE, LAYOUT_VERTICAL);

		responseTotalsTableBuilder.addTextParameter(ResponseTotalsTable.FIELD_ID,
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM + ":"
						+ CHOICE_PARAMETER);
		
		responseTotalsTableBuilder.addConditionsParameter(
				ItemizationTable.CONDITIONS_PARAMETER_ID, Collections
						.singletonList(new Object[] {MAIN_FORM, false}), condition);
		return responseTotalsTableBuilder;
	}

	private ComponentBuilder responseTotalsHorizontalTableBuilder(String condition) {
		ComponentBuilder responseTotalsTableBuilder = new ComponentBuilder(new ResponseTotalsTable());

		responseTotalsTableBuilder.addTextParameter(ResponseTotalsTable.LAYOUT_TYPE, LAYOUT_HORIZONTAL);

		responseTotalsTableBuilder.addTextParameter(ResponseTotalsTable.FIELD_ID,
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM + ":"
						+ CHOICE_PARAMETER);
		
		responseTotalsTableBuilder.addConditionsParameter(
				ItemizationTable.CONDITIONS_PARAMETER_ID, Collections
						.singletonList(new Object[] {MAIN_FORM, false}), condition);
		return responseTotalsTableBuilder;
	}

	public void testVerticalTableLayout() throws Exception {
		addData("Joe", "b");
		bot.go(userProject, REPORT_FORM);
		assertHasRow("oranges", 0);
		assertHasRow("bananas", 1);
		assertHasRow("pears", 0);
		assertHasRow("cherries", 0);
	}

	public void testHorizontalTableLayout() throws Exception {
		addData("Joe", "b");
		bot.go(userProject, REPORT_FORM);
		
		assertHasRow("oranges", "bananas", "pears", "cherries");
		assertHasRow(0, 1, 0, 0);
	}

	private void assertHasRow(String choice, int count) throws RobotException {
		String text = bot.getPageText();

		String[] rows = text.split("\n");

		for (int i = 0; i < rows.length; i++) {
			String row = rows[i];
			if (row.matches("<tr class=\"(odd|even)\"><td>.*" + choice + ".*</td><td>" + count + "</td></tr>")) {
				return;
			}
		}
		fail("Failed to find a row for choice '" + choice + "' with count " + count + ":\n" + text);
	}

	private void assertHasRow(String... contentStrings) throws RobotException {
		String text = bot.getPageText();
		StringBuilder rowString = new StringBuilder();
		
		for (String contentString : contentStrings) {
			rowString.append("<td>");
			rowString.append(contentString);
			rowString.append("</td>");
		}
		
		String[] rows = text.split("\n");

		for (int i = 0; i < rows.length; i++) {
			String row = rows[i];
			if (row.matches("<tr class=\"(odd|even)\">.*" + rowString.toString() + "</tr>")) {
				return;
			}
		}
		fail("Failed to find a row with contents '" + rowString.toString() + "'\n" + text);
	}

	private void assertHasRow(int... contentCounts) throws RobotException {
		String text = bot.getPageText();
		StringBuilder rowString = new StringBuilder();
		
		for (int contentCount : contentCounts) {
			rowString.append("<td>");
			rowString.append(contentCount);
			rowString.append("</td>");
		}
		
		String[] rows = text.split("\n");

		for (int i = 0; i < rows.length; i++) {
			String row = rows[i];
			if (row.matches("<tr class=\"(odd|even)\">.*" + rowString.toString() + "</tr>")) {
				return;
			}
		}
		fail("Failed to find a row with contents '" + rowString.toString() + "'\n" + text);
	}

	private void addData(String name, String... choices) throws Exception {
		bot.go(userProject, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, name);
		bot.setParameters(CHOICE_PARAMETER, choices);
		bot.submit();
	}
}
