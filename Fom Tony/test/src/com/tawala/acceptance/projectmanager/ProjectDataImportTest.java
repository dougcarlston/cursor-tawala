package com.tawala.acceptance.projectmanager;

import java.util.List;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;

public class ProjectDataImportTest extends DataImportTestSupport {
	private Project project;
	private UserProject userProject;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		WorldInitializer.getDefaultWorld().domain().users()
				.onUserUpgradeToFullyRegistered(projectOwner);

		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm(PROJECT_FORM_NAME);
		FibBuilder fibBuilder = formBuilder.addFib("My field:");
		fibBuilder.addBlank(FIRST_FIELD_NAME);
		fibBuilder.addBlank(SECOND_FIELD_NAME);
		fibBuilder.addBlank(THIRD_FIELD_NAME);
		formBuilder.addDeclaredFields(DECLARED_FIELD_NAME);

		project = builder.build();
		userProject = new UserProject(project, projectOwner, "test");

		world.domain().projects().put(userProject);
	}

	@Override
	protected void navigateToFirstImportPage() throws RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");
		bot.followLink("import1");
	}

	@Override
	protected List<FormSubmission> getResponses() {
		return WorldInitializer.getDefaultWorld().domain().storedData()
				.responsesFor(project, PROJECT_FORM_NAME);
	}

	@Override
	void createARecord() throws RobotException {
		bot.go(userProject);
		WebForm form = bot.getForm(0);
		form.setParameter(FIRST_FIELD_NAME, "value1");
		bot.submit(form);
	}

	@Override
	protected String getExpectedCancelDestination() {
		return WellKnown.urls.getProjectManagerProjectDetailView()
				+ "?projectName=test";
	}
}
