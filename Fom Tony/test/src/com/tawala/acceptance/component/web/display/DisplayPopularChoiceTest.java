package com.tawala.acceptance.component.web.display;

import java.util.Collections;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.display.DisplayPopularChoice;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.RecordSelector;

public class DisplayPopularChoiceTest extends AcceptanceTestCase {
	private UserProject userProject;
	public static final String FORM_NAME = "Main";
	public static final String PREFER_TO_ATTEND_FIELD = "preferToAttend";
	public static final String ABLE_TO_ATTEND_FIELD = "ableToAttend";
	private static final String NAME_FIELD = "name";
	private static final String REPORT_FORM = "Report";

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		Project project = buildGetTogetherProject();

		userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);
	}

	public void testDisplayNoChoice() throws RobotException {
		bot.go(userProject, REPORT_FORM);
		String expectedDocumentOutput = "<div class=\"document\"><div><span style=\"\">No choices were ranked \"first\".</span></div></div>";
		
		assertMatches(expectedDocumentOutput, bot.getPageText());
	}

	public void testDisplayNonEmptyTable() throws Exception {
		// Just one record, "b" is the choice
		addData("Joe", new String[] { "a", "b" }, "b");

		bot.go(userProject, REPORT_FORM);
		String expectedDocumentOutput = ">Monday<";

		assertMatches(expectedDocumentOutput, bot.getPageText());

		// Another record, now "b" is the choice.
		addData("Jim", new String[] { "b", "c" }, "a");
		bot.go(userProject, REPORT_FORM);

		expectedDocumentOutput = ">Tuesday<";
		assertContains(expectedDocumentOutput, bot.getPageText());
	}

	public static Project buildGetTogetherProject() {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		FormBuilder mainForm = projectBuilder
				.addForm(DisplayPopularChoiceTest.FORM_NAME);
		mainForm.addFib("Your name:", NAME_FIELD, 25);

		mainForm.addMcWithAlternateLabel(
				DisplayPopularChoiceTest.ABLE_TO_ATTEND_FIELD,
				"Pick dates you can attend:", new String[] { "Monday",
						"Tuesday", "Wednesday", "Thursday", "Friday" });
		mainForm.addMcWithAlternateLabel(
				DisplayPopularChoiceTest.PREFER_TO_ATTEND_FIELD,
				"Pick the date you prefer to attend:", new String[] { "Monday",
						"Tuesday", "Wednesday", "Thursday", "Friday" });

		DocumentBuilder document = projectBuilder.addDocument("Report");

		ComponentBuilder displayPopularChoiceBuilder = new ComponentBuilder(
				new DisplayPopularChoice());
		displayPopularChoiceBuilder.addTextParameter(DisplayPopularChoice.RANK,
				"1");
		displayPopularChoiceBuilder.addTextParameter(
				DisplayPopularChoice.CHOICE_AVAILABLE_FIELD_NAME,
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + FORM_NAME + ':'
						+ ABLE_TO_ATTEND_FIELD);

		displayPopularChoiceBuilder.addConditionsParameter(
				DisplayPopularChoice.CONDITIONS_FIELD_NAME, Collections
						.singletonList(new Object[] {FORM_NAME, false}), "");

		document.addComponent(displayPopularChoiceBuilder);

		ProcessBlockBuilder process = projectBuilder.addProcess("Post Main");
		process.addShow(document);

		FormBuilder reportForm = projectBuilder.addForm(REPORT_FORM);
		reportForm.setPreProcess(process);

		return projectBuilder.build();
	}

	private void addData(String name, String[] ableChoices, String preferChoice)
			throws Exception {
		bot.go(userProject, FORM_NAME);

		bot.setParameter(NAME_FIELD, name);
		bot.setParameters(ABLE_TO_ATTEND_FIELD, ableChoices);
		if (preferChoice != null) {
			bot.setParameter(PREFER_TO_ATTEND_FIELD, preferChoice);
		}

		bot.submit();
	}
}
