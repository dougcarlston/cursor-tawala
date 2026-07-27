package com.tawala.acceptance.projectmanager;

import java.util.List;

import org.xml.sax.SAXException;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.FormSubmission;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.Reference;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.ChangeSubmissionFieldValueController;

public class ChangeSubmissionFieldValueTest extends AcceptanceTestCase {
	private static final String MCQ_NAME = "mcq";
	private static final String DECLARED_FIELD_NAME = "field1";
	private static final String FIB_NAME = "name";
	public static final String FIRST_FORM_NAME = "form1";
	public static final String SECOND_FORM_NAME = "form2";

	protected UserProject userProject;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm(FIRST_FORM_NAME);
		formBuilder.addDeclaredFields(DECLARED_FIELD_NAME);
		formBuilder.addFib("Name:", "NameFib", FIB_NAME, 50);
		formBuilder.addMcWithAlternateLabel(MCQ_NAME, "Your favorites", false,
				false, new String[] { "oranges", "bananas", "peaches" });
		builder.addForm(SECOND_FORM_NAME);

		userProject = new UserProject(builder.build(), projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		// --- Record data
		bot.go(userProject, FIRST_FORM_NAME);
		bot.setParameter(FIB_NAME, "Joe");
		bot.setParameters(MCQ_NAME, new String[] {"a", "c"} );
		bot.submit();

		bot.go(userProject, FIRST_FORM_NAME);
		bot.setParameter(FIB_NAME, "Jim");
		bot.setParameters(MCQ_NAME, new String[] {"c"} );
		bot.submit();

		bot.logInAs(projectOwner);

		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");
	}

	public void testChangingFib() throws RobotException, SAXException,
			InterruptedException {
		List<FormSubmission> submissions = world.domain().storedData()
				.responsesFor(userProject.getProject(), FIRST_FORM_NAME);

		changeFieldValues(submissions.get(0), FIB_NAME, "Jackson");
		verifyData(new String[][] { { FIB_NAME, "Jackson" },
				{ FIB_NAME, "Jim" } });

		changeFieldValues(submissions.get(1), FIB_NAME, "Sarah");
		verifyData(new String[][] { { FIB_NAME, "Jackson" },
				{ FIB_NAME, "Sarah" } });

	}

	public void testChangingDeclaredField() throws RobotException,
			SAXException, InterruptedException {
		List<FormSubmission> submissions = world.domain().storedData()
				.responsesFor(userProject.getProject(), FIRST_FORM_NAME);

		changeFieldValues(submissions.get(0), DECLARED_FIELD_NAME, "123");
		verifyData(new String[][] { { DECLARED_FIELD_NAME, "123" },
				{ DECLARED_FIELD_NAME, "" } });

		changeFieldValues(submissions.get(1), DECLARED_FIELD_NAME, "555");
		verifyData(new String[][] { { DECLARED_FIELD_NAME, "123" },
				{ DECLARED_FIELD_NAME, "555" } });

	}

	public void testChangingMCQField() throws RobotException, SAXException,
			InterruptedException {
		List<FormSubmission> submissions = world.domain().storedData()
				.responsesFor(userProject.getProject(), FIRST_FORM_NAME);

		changeFieldValues(submissions.get(0), MCQ_NAME, "a");
		verifyData(new String[][] { { MCQ_NAME, "a" }, { MCQ_NAME, "c" } });

		changeFieldValues(submissions.get(1), MCQ_NAME, "b");
		verifyData(new String[][] { { MCQ_NAME, "a" }, { MCQ_NAME, "b" } });

	}

	private void changeFieldValues(FormSubmission formSubmission,
			String fieldName, String newValue) throws RobotException {
		String linkToChangeField = WellKnown.urls
				.getProjectManagerChangeFieldValue()
				+ "?"
				+ ChangeSubmissionFieldValueController.USER_PROJECT_ID_PARAMETER
				+ '='
				+ userProject.getId()
				+ '&'
				+ ChangeSubmissionFieldValueController.FIELD_NAME_PARAMETER
				+ '='
				+ fieldName
				+ '&'
				+ ChangeSubmissionFieldValueController.SUBMISSION_ID_PARAMETER
				+ '='
				+ formSubmission.getDatabaseId()
				+ '&'
				+ ChangeSubmissionFieldValueController.VALUES_PARAMETER
				+ '='
				+ newValue;
		bot.go(linkToChangeField);
	}

	public void verifyData(String[][] expectedData) {
		List<FormSubmission> recordList = world.domain().storedData()
				.responsesFor(userProject.getProject(), FIRST_FORM_NAME);

		assertEquals("size", expectedData.length, recordList.size());
		for (int i = 0; i < expectedData.length; i++) {
			for (int j = 0; j < expectedData[i].length; j += 2) {
				String field = expectedData[i][j];
				String expectedValue = expectedData[i][j + 1];
				Reference reference = new Reference(FIRST_FORM_NAME, field);
				assertEquals("checking " + field, expectedValue, recordList
						.get(i).getValue(reference).toString());
			}
		}
	}
}
