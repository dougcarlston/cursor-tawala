package com.tawala.acceptance.component.web.form;

import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;

import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.form.DynamicMultiChoiceDataProvider;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.RecordSelector;

public class DynamicMultipleChoiceTest extends AcceptanceTestCase {
	private static final String OPTION_PARAMETER = "option";
	private static final String OPTION_FORM = "Option";
	private static final String MAIN_FORM = "Main";
	private UserProject userProject;

	private void createProject(boolean sortedMCQ) {
		Project project = buildProject(sortedMCQ);

		userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);
	}

	private Project buildProject(boolean sortedMCQ) {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder optionFormBuilder = projectBuilder.addForm(OPTION_FORM);
		optionFormBuilder.addFib("Add a new date:", OPTION_PARAMETER, 30);

		FormBuilder mainFormBuilder = projectBuilder.addForm(MAIN_FORM);
		ComponentBuilder dynamicMCQBuilder = new ComponentBuilder(
				new DynamicMultiChoiceDataProvider());
		dynamicMCQBuilder.addPreformattedParameter(
				DynamicMultiChoiceDataProvider.DISPLAY_EXPRESSION_PARAMETER,
				"<field name=\"" + RecordSelector.DEFAULT_RECORD_LIST_NAME
						+ ":" + OPTION_FORM + ":" + OPTION_PARAMETER + "\" />");
		dynamicMCQBuilder.addPreformattedParameter(
				DynamicMultiChoiceDataProvider.VALUE_EXPRESSION_PARAMETER,
				"<field name=\"" + RecordSelector.DEFAULT_RECORD_LIST_NAME
						+ ":" + OPTION_FORM + ":" + OPTION_PARAMETER + "\" />");
		if (sortedMCQ) {
			dynamicMCQBuilder.addPreformattedParameter(
					DynamicMultiChoiceDataProvider.SORT_EXPRESSION_PARAMETER,
					"<field name=\"" + RecordSelector.DEFAULT_RECORD_LIST_NAME
							+ ":" + OPTION_FORM + ":" + OPTION_PARAMETER
							+ "\" />");
		}
		dynamicMCQBuilder.addConditionsParameter(
				DynamicMultiChoiceDataProvider.CONDITIONS_PARAMETER,
				Collections.singletonList(new Object[] { OPTION_FORM, false }),
				null);

		mainFormBuilder.addMcWithCustomDataProvider("choice",
				"Please select one:", true, true, dynamicMCQBuilder);

		return projectBuilder.build();
	}

	public void testDisplayNonEmptyList() throws Exception {
		createProject(false);

		addData("12/01/2007");
		bot.go(userProject, MAIN_FORM);
		assertContains(generateExpectedValueFor("12/01/2007"), bot
				.getPageText());

		addData("12/02/2007");
		bot.go(userProject, MAIN_FORM);
		assertContains(generateExpectedValueFor("12/01/2007"), bot
				.getPageText());
		assertContains(generateExpectedValueFor("12/02/2007"), bot
				.getPageText());

		addData("12/03/2007");
		bot.go(userProject, MAIN_FORM);
		assertContains(generateExpectedValueFor("12/01/2007"), bot
				.getPageText());
		assertContains(generateExpectedValueFor("12/02/2007"), bot
				.getPageText());
		assertContains(generateExpectedValueFor("12/03/2007"), bot
				.getPageText());
	}

	public void testSorting() throws Exception {
		createProject(true);
		String[] choices = { "xxxxxx", "yyyyyy", "aaaaaa", "BBBBBB", "Abc",
				"ABCD", "AAA" };
		for (String choice : choices) {
			addData(choice);
		}

		Arrays.sort(choices, new Comparator<String>() {
			public int compare(String o1, String o2) {
				return o1.compareToIgnoreCase(o2);
			}
		});
		bot.go(userProject, MAIN_FORM);
		String pageText = bot.getPageText();

		int lastIndex = -1;
		for (String choice : choices) {
			String choiceHtml = generateExpectedValueFor(choice);
			int index = pageText.indexOf(choiceHtml);
			assertTrue(index > 0);
			assertTrue("Index for " + choice, index > lastIndex);
			lastIndex = index;
		}
	}

	private String generateExpectedValueFor(String value) {
		return "<input class=\"radio\" name=\"choice\" id=\"choice-" + value + "\" type=\"radio\" value=\""
				+ value + "\" /> " + "<label for=\"choice-" + value + "\">" + value + "</label>" + "</span>";
	}

	private void addData(String name) throws Exception {
		bot.go(userProject, OPTION_FORM);
		bot.setParameter(OPTION_PARAMETER, name);
		bot.submit();
	}
}
