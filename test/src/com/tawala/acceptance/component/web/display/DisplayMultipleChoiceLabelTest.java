package com.tawala.acceptance.component.web.display;

import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.display.DisplayMultipleChoiceLabel;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class DisplayMultipleChoiceLabelTest extends AcceptanceTestCase {
	public static final String FORM_NAME = "Main";
	private static final String CHOICE_FIELD = "choice";

	protected UserProject buildProject(boolean labelsOnly, String style)
			throws Exception {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		FormBuilder mainForm = projectBuilder
				.addForm(DisplayMultipleChoiceLabelTest.FORM_NAME);

		mainForm.addMcWithAlternateLabel(CHOICE_FIELD,
				"Pick dates you can attend:", style, false /* only one */,
				false /* required */, new String[] { "Monday", "Tuesday",
						"Wednesday", "Thursday", "Friday" });

		DocumentBuilder documentBuilder = projectBuilder.addDocument("doc");
		ComponentBuilder componentBuilder = new ComponentBuilder(
				new DisplayMultipleChoiceLabel());
		componentBuilder.addTextParameter(
				DisplayMultipleChoiceLabel.FIELD_NAME_PARAMETER, FORM_NAME + ":" + CHOICE_FIELD);
		componentBuilder.addTextParameter(
				DisplayMultipleChoiceLabel.DISPLAY_PARAMETER,
				(labelsOnly ? DisplayMultipleChoiceLabel.DisplayType.label_only
						.toString()
						: DisplayMultipleChoiceLabel.DisplayType.all_choices
								.toString()));
		documentBuilder.addText("Your choice: ");
		documentBuilder.addComponent(componentBuilder);

		ProcessBlockBuilder processBuilder = projectBuilder
				.addProcess("post-process");
		processBuilder.addShow(documentBuilder);

		mainForm.setPostProcess(processBuilder);

		Project project = projectBuilder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		return userProject;
	}

	public void testDisplayValueNotSelected() throws Exception {
		UserProject userProject = buildProject(true, "vertical");
		bot.go(userProject);
		bot.submit();
		assertMatches("Your choice: ", bot.getPageText());
	}

	public void testDisplaySingleValueSelected() throws Exception {
		UserProject userProject = buildProject(true, "vertical");
		bot.go(userProject);
		bot.setParameter(CHOICE_FIELD, "b");
		bot.submit();
		assertMatches("Your choice: Tuesday", bot.getPageText());
	}

	public void testDisplayMultipleValuesSelected() throws Exception {
		UserProject userProject = buildProject(true, "vertical");
		bot.go(userProject);
		bot.setParameters(CHOICE_FIELD, new String[] { "b", "d" });
		bot.submit();
		assertMatches("Your choice: Tuesday, Thursday", bot.getPageText());
	}

	public void testDisplayVerticalLayout() throws Exception {
		UserProject userProject = buildProject(false, "vertical");
		bot.go(userProject);
		bot.setParameters(CHOICE_FIELD, new String[] { "b", "d" });
		bot.submit();
		assertMatches("Your choice: ", bot.getPageText());

		assertMatches(getVerticalOrHorizontalMatchingPattern("vertical"), bot
				.getPageText());
	}

	public void testDisplayHorizontalLayout() throws Exception {
		UserProject userProject = buildProject(false, "horizontal");
		bot.go(userProject);
		bot.setParameters(CHOICE_FIELD, new String[] { "b", "d" });
		bot.submit();
		assertMatches("Your choice: ", bot.getPageText());

		String matchingExpression = getVerticalOrHorizontalMatchingPattern("horizontal");

		assertMatches(matchingExpression, bot.getPageText());
	}

	public void testDisplayMultiColumnLayout() throws Exception {
		UserProject userProject = buildProject(false, "multicolumn");
		bot.go(userProject);
		bot.setParameters(CHOICE_FIELD, new String[] { "b", "d" });
		bot.submit();
		assertMatches("Your choice: ", bot.getPageText());

		String matchingExpression = "<div id=\"choiceContainer\" class=\"mcCheckbox multicolumn\"><table class=\"answer\"><tbody>"
				+ "<tr><td>"
				+ checkboxHtml(false, "Monday", "a")
				+ "</td>"
				+ "<td>"
				+ checkboxHtml(true, "Thursday", "d")
				+ "</td>"
				+ "</tr>"
				+ "<tr><td>"
				+ checkboxHtml(true, "Tuesday", "b")
				+ "</td>"
				+ "<td>"
				+ checkboxHtml(false, "Friday", "e")
				+ "</td>"
				+ "</tr>"
				+ "<tr><td>"
				+ checkboxHtml(false, "Wednesday", "c")
				+ "</td>"
				+ "<td>"
				+ "<span class=\"answer\"> </span>"
				+ "</td>"
				+ "</tr>" + "</tbody>" + "\n" + "</table>";

		assertMatches(matchingExpression, bot.getPageText());
	}

	private static String getVerticalOrHorizontalMatchingPattern(String type) {
		String matchingExpression = "<div id=\"choiceContainer\" class=\"mcCheckbox " + type + "\">"
				+ checkboxHtml(false, "Monday", "a") + "\n"
				+ checkboxHtml(true, "Tuesday", "b") + "\n"
				+ checkboxHtml(false, "Wednesday", "c") + "\n"
				+ checkboxHtml(true, "Thursday", "d") + "\n"
				+ checkboxHtml(false, "Friday", "e") + "\n";
		return matchingExpression;
	}

	private static String checkboxHtml(boolean checked, String label, String value) {
		return "<span class=\"answer\"><img src=\"/images/checkbox_"
				+ (checked ? "on" : "off") + ".gif\" alt=\""
				+ (checked ? "Selected" : "Not selected")
				+ "\" width=\"12px\" height=\"12px\" /> <label for=\"choice-" + value + "\">" + label
				+ "</label></span>";
	}
}
