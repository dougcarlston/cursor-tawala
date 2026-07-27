package com.tawala.acceptance.projectmanager;

import java.util.Collections;
import java.util.List;

import com.scissor.webrobot.RobotException;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.data.DataSource;
import com.tawala.project.data.StringField;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;

public class SharedDataSourceImportTest extends DataImportTestSupport {
	private Project project;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		DataSource dataSource = new DataSource(PROJECT_FORM_NAME);
		dataSource.addField(new StringField(FIRST_FIELD_NAME));
		dataSource.addField(new StringField(SECOND_FIELD_NAME));
		dataSource.addField(new StringField(THIRD_FIELD_NAME));
		dataSource.addField(new StringField(DECLARED_FIELD_NAME));

		ProjectsHibernateImpl.addSharedDataSources(projectOwner, Collections
				.singletonList(dataSource));

		project = world.domain().users().getSharedStorageForUser(projectOwner);
	}

	@Override
	protected void navigateToFirstImportPage() throws RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("linkToViewSharedData1");
		bot.followLink("import1");
	}

	@Override
	protected List<FormSubmission> getResponses() {
		return WorldInitializer.getDefaultWorld().domain().storedData()
				.responsesFor(project, PROJECT_FORM_NAME);
	}

	@Override
	void createARecord() throws RobotException {
		FormSubmission submission = new FormSubmission(world.domain().users()
				.get(projectOwner.getDatabaseId()), project
				.getDataSourceNamed(PROJECT_FORM_NAME));
		submission.setValue(FIRST_FIELD_NAME, "value1");
		world.domain().storedData().record(submission);
	}

	@Override
	protected String getExpectedCancelDestination() {
		return WellKnown.urls.getViewSharedDatasources();
	}
}
