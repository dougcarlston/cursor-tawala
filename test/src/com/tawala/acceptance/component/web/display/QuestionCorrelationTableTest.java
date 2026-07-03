package com.tawala.acceptance.component.web.display;

import java.security.InvalidParameterException;
import java.util.Collections;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.display.QuestionCorrelationTable;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.RecordSelector;

public class QuestionCorrelationTableTest extends AcceptanceTestCase {
	private static final String NAME_PARAMETER = "name";
	private static final String CHOICE_PARAMETER = "choice";
	private static final String PREFERRED_CHOICE_PARAMETER = "preferred";
	private static final String MAIN_FORM = "Main";
	private static final String REPORT_FORM = "Report";

	public void testDisplayEmptyList() throws RobotException {
		Project project = buildProject(null, true);

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject, REPORT_FORM);
		assertContains("Nobody has responded yet.", bot.getPageText());
	}

	public void testDisplayNonEmptyList() throws Exception {
		Project project = buildProject(null, true);

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		addData(userProject, "Joe", new String[] { "b" }, "b");
		bot.go(userProject, REPORT_FORM);
		assertHasRow("Joe", 0, 2, 0, 0);

		assertTotals(new int[] { 0, 0, 1, 1, 0, 0, 0, 0 }, 2);

		addData(userProject, "Jim", new String[] { "c" }, "c");
		addData(userProject, "Sarah", new String[] { "c", "d" }, "d");
		bot.go(userProject, REPORT_FORM);
		assertHasRow("Jim", 0, 0, 2, 0);
		assertHasRow("Sarah", 0, 0, 1, 2);

		assertTotals(new int[] { 0, 0, 1, 1, 2, 1, 1, 1 }, 3);
	}

	public void testDataFiltering() throws Exception {
		String userJames = "James";

		String userJim = "Jim";
		Project project = buildProject("<equals field=\""
				+ RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM
				+ ":" + NAME_PARAMETER + "\">" + "<string value=\"" + userJim
				+ "\" />" + "</equals>", true);

		UserProject userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		addData(userProject, "Joe", new String[] { "a", "c" }, "a");
		addData(userProject, userJim, new String[] { "b" }, "b");
		addData(userProject, userJames, new String[] { "a", "c", "d" }, "d");
		addData(userProject, "Julia", new String[] { "b" }, "b");

		bot.go(userProject, REPORT_FORM);
		assertDoesntContain("Joe", bot.getPageText());
		assertDoesntContain("Julia", bot.getPageText());
		assertDoesntContain(userJames, bot.getPageText());
		assertHasRow(userJim, 0, 2, 0, 0);
		assertTotals(new int[] { 0, 0, 1, 1, 0, 0, 0, 0 }, 2);

		project = buildProject("<equals field=\""
				+ RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM
				+ ":" + NAME_PARAMETER + "\">" + "<string value=\"" + userJames
				+ "\" />" + "</equals>", true);

		userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		bot.go(userProject, REPORT_FORM);
		assertDoesntContain("Joe", bot.getPageText());
		assertDoesntContain("Julia", bot.getPageText());
		assertDoesntContain(userJim, bot.getPageText());
		assertHasRow(userJames, 1, 0, 1, 2);
		assertTotals(new int[] { 1, 0, 0, 0, 1, 0, 1, 1 }, 4);
	}

	public void testUsageWithoutPreferredChoices() throws Exception {
		Project project = buildProject(null, false);

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		addData(userProject, "Joe", new String[] { "b" }, "b");
		bot.go(userProject, REPORT_FORM);
		assertHasRow("Joe", 0, 1, 0, 0);

		assertTotalsWithoutPreferredChoice(new int[] { 0, 1, 0, 0 }, 2);

		addData(userProject, "Jim", new String[] { "c" }, "c");
		addData(userProject, "Sarah", new String[] { "c", "d" }, "d");
		bot.go(userProject, REPORT_FORM);
		assertHasRow("Jim", 0, 0, 1, 0);
		assertHasRow("Sarah", 0, 0, 1, 1);

		assertTotalsWithoutPreferredChoice(new int[] { 0, 1, 2, 1 }, 3);
	}

	private void assertTotals(int[] counts, int bestOption)
			throws RobotException {
		String text = bot.getPageText();

		String[] rows = text.split("\n");

		for (int i = 0; i < rows.length; i++) {
			String row = rows[i];
			if (row.matches(".*Totals\\:.*")) {
				String regularExpression = // --- First column
				".*<td.*>"
						+ getTotalsREBasedOnIndicator(counts[0], counts[1],
								bestOption == 1)
						+ "</td>"
						+
						// --- Second column
						"<td.*>"
						+ getTotalsREBasedOnIndicator(counts[2], counts[3],
								bestOption == 2)
						+ "</td>"
						+
						// --- Third column
						"<td.*>"
						+ getTotalsREBasedOnIndicator(counts[4], counts[5],
								bestOption == 3) + "</td>"
						+
						// --- Forth column
						"<td.*>"
						+ getTotalsREBasedOnIndicator(counts[6], counts[7],
								bestOption == 4) + "</td>" + "</tr>";
				if (row.matches(regularExpression)) {
					return;
				} else {
					fail("Totals don't match:\n " + regularExpression + "\n"
							+ row);
				}
			}
		}
		fail("Failed to find the totals row in:\n" + bot.getPageText());

	}

	private void assertTotalsWithoutPreferredChoice(int[] counts, int bestOption)
			throws RobotException {
		String text = bot.getPageText();

		String[] rows = text.split("\n");

		for (int i = 0; i < rows.length; i++) {
			String row = rows[i];
			if (row.matches(".*Totals\\:.*")) {
				String regularExpression = // --- First column
				".*<td.*>"
						+ getTotalsREWithoutPreferredChoice(counts[0])
						+ "</td>"
						+
						// --- Second column
						"<td.*>"
						+ getTotalsREWithoutPreferredChoice(counts[1])
						+ "</td>"
						+
						// --- Third column
						"<td.*>"
						+ getTotalsREWithoutPreferredChoice(counts[2]) + "</td>"
						+
						// --- Forth column
						"<td.*>"
						+ getTotalsREWithoutPreferredChoice(counts[3]) + "</td>" + "</tr>";
				
				if (row.matches(regularExpression)) {
					return;
				} else {
					fail("Totals don't match:\n " + regularExpression + "\n" + row);
				}
			}
		}
		fail("Failed to find the totals row in:\n" + bot.getPageText());

	}

	private String getTotalsREBasedOnIndicator(int i, int j, boolean bestOption) {
		if (bestOption) {
			return ".*<b>" + i + "&nbsp;\\(" + j + "\\) </b>.*star.*";
		} else {
			return ".*" + i + "&nbsp;\\(" + j + "\\)";
		}
	}

	private String getTotalsREWithoutPreferredChoice(int i) {
		return ".*" + i;
	}


	private Project buildProject(String condition,
			boolean includePreferredChoice) {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 30);
		formBuilder.addMcWithAlternateLabel(CHOICE_PARAMETER,
				"Which ones do you like?", false, true, "oranges", "bananas",
				"pears", "cherries");
		formBuilder.addMcWithAlternateLabel(PREFERRED_CHOICE_PARAMETER,
				"Which do you like the most?", true, true, "oranges",
				"bananas", "pears", "cherries");

		DocumentBuilder documentBuilder = projectBuilder.addDocument("Report");
		ComponentBuilder correlationTableBuilder = new ComponentBuilder(
				new QuestionCorrelationTable());

		correlationTableBuilder.addTextParameter(
				QuestionCorrelationTable.DISPLAY_FIELD_NAME,
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM + ":"
						+ NAME_PARAMETER);
		correlationTableBuilder.addTextParameter(
				QuestionCorrelationTable.QUESTION_FIELD_NAME,
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM + ":"
						+ CHOICE_PARAMETER);
		if (includePreferredChoice) {
			correlationTableBuilder.addTextParameter(
					QuestionCorrelationTable.PREFERRED_CHOICE_FIELD_NAME,
					RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM
							+ ":" + PREFERRED_CHOICE_PARAMETER);
		}
		correlationTableBuilder.addConditionsParameter(
				QuestionCorrelationTable.CONDITIONS_FIELD_NAME, Collections
						.singletonList(new Object[] { MAIN_FORM, false }),
				condition);

		documentBuilder.addComponent(correlationTableBuilder);

		FormBuilder reportFormBuilder = projectBuilder.addForm(REPORT_FORM);
		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("Main Post Process");
		processBlockBuilder.addShow(documentBuilder);

		reportFormBuilder.setPreProcess(processBlockBuilder);

		return projectBuilder.build();
	}

	private void assertHasRow(String name, int... checkMarkIndicator)
			throws RobotException {
		String text = bot.getPageText();

		String[] rows = text.split("\n");

		for (int i = 0; i < rows.length; i++) {
			String row = rows[i];
			if (row.matches("<tr class=\"(odd|even)\">"
					+
					// --- Name
					"<th>.*"
					+ name
					+ ".*</th>"
					+
					// --- First column
					"<td.*>"
					+ getREBasedOnIndicator(checkMarkIndicator[0])
					+ "</td>"
					+
					// --- Second column
					"<td.*>"
					+ getREBasedOnIndicator(checkMarkIndicator[1])
					+ "</td>"
					+
					// --- Third column
					"<td.*>" + getREBasedOnIndicator(checkMarkIndicator[2])
					+ "</td>"
					+
					// --- Forth column
					"<td.*>" + getREBasedOnIndicator(checkMarkIndicator[3])
					+ "</td>" + "</tr>")) {
				return;
			}
		}
		fail("Failed to find a row for name '" + name + "' in:\n"
				+ bot.getPageText());
	}

	private String getREBasedOnIndicator(int indicator) {
		switch (indicator) {
		case 0:
			return "&nbsp;";

		case 1:
			return "*.tick.gif.*";

		case 2:
			return "*.star.png.*";

		default:
			throw new InvalidParameterException("Expected 0, 1 or 2, got "
					+ indicator);
		}
	}

	private void addData(UserProject userProject, String name,
			String[] choices, String preferred) throws Exception {
		bot.go(userProject, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, name);
		bot.setParameters(CHOICE_PARAMETER, choices);
		bot.setParameter(PREFERRED_CHOICE_PARAMETER, preferred);
		bot.submit();
	}
}
