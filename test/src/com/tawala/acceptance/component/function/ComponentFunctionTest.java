package com.tawala.acceptance.component.function;

import java.util.Collections;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.function.RecordCountFunction;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class ComponentFunctionTest extends AcceptanceTestCase {
	private UserProject userProject;
	public static final String FORM_NAME = "Main";
	private static final String NAME_FIELD = "name";
	private static final String REPORT_FORM = "Report";

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		Project project = buildSimpleProject();

		userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);
	}

	public void testDisplayIfNoData() throws RobotException {
		bot.go(userProject, REPORT_FORM);
		String expectedDocumentOutput = "<div class=\"document\"><div><span .*>0</span>";
		assertMatches(expectedDocumentOutput, bot.getPageText());
	}

	public void testDisplayWithData() throws Exception {
		for (int i = 0; i < 11; i++) {
			addData("Name " + i);
		}

		bot.go(userProject, REPORT_FORM);
		String expectedDocumentOutput = "<div class=\"document\"><div><span .*>11</span>";
		assertMatches(expectedDocumentOutput, bot.getPageText());
	}

	public static Project buildSimpleProject() {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		FormBuilder mainForm = projectBuilder
				.addForm(ComponentFunctionTest.FORM_NAME);
		mainForm.addFib("Your name:", NAME_FIELD, 25);

		DocumentBuilder document = projectBuilder.addDocument("Report");

		ComponentBuilder recordCountBuilder = new ComponentBuilder(
				new RecordCountFunction());
		recordCountBuilder.addConditionsParameter(
				RecordCountFunction.WHERE_CLAUSE_PARAMETER, Collections
						.singletonList(new Object[] {FORM_NAME, false}), null);

		document.addComponent(recordCountBuilder);

		ProcessBlockBuilder process = projectBuilder
				.addProcess("Preprocess for the report");
		process.addShow(document);

		FormBuilder reportForm = projectBuilder.addForm(REPORT_FORM);
		reportForm.setPreProcess(process);

		return projectBuilder.build();
	}

	private void addData(String name) throws Exception {
		bot.go(userProject, FORM_NAME);
		bot.setParameter(NAME_FIELD, name);
		bot.submit();
	}
}
