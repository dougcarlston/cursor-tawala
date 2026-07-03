package com.tawala.acceptance.component.web.display;

import java.util.Collections;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.display.SimpleList;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.RecordSelector;

public class SimpleListTest extends AcceptanceTestCase {
	private static final String NAME_PARAMETER = "name";
	private static final String MAIN_FORM = "Main";
	private static final String REPORT_FORM = "Report";
	private UserProject userProject;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		Project project = buildProject();

		userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);
	}

	private Project buildProject() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addFib("What's your name?", NAME_PARAMETER, 30);

		DocumentBuilder documentBuilder = projectBuilder.addDocument("Report");
		ComponentBuilder simpleListBuilder = new ComponentBuilder(
				new SimpleList(), SimpleList.FIELD_NAME,
				RecordSelector.DEFAULT_RECORD_LIST_NAME + ":" + MAIN_FORM + ":"
						+ NAME_PARAMETER);
		simpleListBuilder.addConditionsParameter(SimpleList.CONDITIONS_NAME,
				Collections.singletonList(new Object[] { MAIN_FORM, false }),
				"");

		documentBuilder.addComponent(simpleListBuilder);

		FormBuilder reportFormBuilder = projectBuilder.addForm(REPORT_FORM);
		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("Main Post Process");
		processBlockBuilder.addShow(documentBuilder);

		reportFormBuilder.setPreProcess(processBlockBuilder);

		return projectBuilder.build();
	}

	public void testDisplayEmptyList() throws RobotException {
		bot.go(userProject, REPORT_FORM);
		String expectedDocumentOutput = "<td>No records were found.</td>";
		assertContains(expectedDocumentOutput, bot.getPageText());
	}

	public void testDisplayNonEmptyList() throws Exception {
		addData("Joe");
		bot.go(userProject, REPORT_FORM);
		assertContains("<td>Joe</td>", bot.getPageText());

		addData("Jim");
		bot.go(userProject, REPORT_FORM);
		assertContains("<td>Joe</td>", bot.getPageText());
		assertContains("<td>Jim</td>", bot.getPageText());

		addData("Sarah");
		bot.go(userProject, REPORT_FORM);
		assertContains("<td>Joe</td>", bot.getPageText());
		assertContains("<td>Jim</td>", bot.getPageText());
		assertContains("<td>Sarah</td>", bot.getPageText());
	}

	private void addData(String name) throws Exception {
		bot.go(userProject, MAIN_FORM);
		bot.setParameter(NAME_PARAMETER, name);
		bot.submit();
	}
}
