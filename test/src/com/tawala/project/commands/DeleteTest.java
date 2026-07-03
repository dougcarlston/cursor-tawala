package com.tawala.project.commands;

import java.util.Arrays;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.ServletException;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.FakeRequest;
import com.tawala.web.WorldInitializer;

/*
 * GetTest verifies complex merges, etc. We just test the basic logic of deletion here.
 */
public class DeleteTest extends TestCase {
	private User projectOwner = UserTest.aUser("tester");
	private HibernateTestSetup hibernateTestSetup = new HibernateTestSetup();

	public DeleteTest() {
		hibernateTestSetup.onSetUp();
		addUserNameToDelete(projectOwner.getId());
	}

	public void setUp() throws ServletException, Exception {
		super.setUp();

		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));

		WorldInitializer.getDefaultWorld().domain().users().addOrSave(
				projectOwner);
	}

	public void testSingleFormMatch() {
		verify(
		// Form definition
				new String[][] { { "Form1", "Q1:a", "Q2:a" } },
				// Condition
				"<equals field=\"Record:Form1:Q1:a\">"
						+ "<string field=\"Record:Form1:Q2:a\" />"
						+ "</equals>",
				// Form submissions
				new Object[][] {
						{ false, "Form1", "Q1:a", "John", "Q2:a", "Steve" },
						{ true, "Form1", "Q1:a", "Tony", "Q2:a", "Tony" },
						{ false, "Form1", "Q1:a", "Steve", "Q2:a", "John" },
						{ false, "Form1", "Q1:a", "John" } });
	}

	public void testNoFormMatch() {
		verify(
		// Form definition
				new String[][] { { "Form1", "Q1:a", "Q2:a" } },
				// Condition
				"<equals field=\"Record:Form1:Q1:a\">"
						+ "<string field=\"Record:Form1:Q2:a\" />"
						+ "</equals>",
				// Form submissions
				new Object[][] {
						{ false, "Form1", "Q1:a", "John", "Q2:a", "Steve" },
						{ false, "Form1", "Q1:a", "Tony", "Q2:a", "Jack" },
						{ false, "Form1", "Q1:a", "Steve", "Q2:a", "John" },
						{ false, "Form1", "Q1:a", "John" } });
	}

	public void testMultipleFormMatch() {
		verify(
		// Form definition
				new String[][] { { "Form1", "Q1:a", "Q2:a" } },
				// Condition
				"<equals field=\"Record:Form1:Q1:a\">"
						+ "<string field=\"Record:Form1:Q2:a\" />"
						+ "</equals>",
				// Form submissions
				new Object[][] {
						{ true, "Form1", "Q1:a", "John", "Q2:a", "John" },
						{ true, "Form1", "Q1:a", "Tony", "Q2:a", "Tony" },
						{ true, "Form1", "Q1:a", "Steve", "Q2:a", "Steve" },
						{ false, "Form1", "Q1:a", "John" } });
	}

	private void verify(String[][] formFields, String conditions,
			Object[][] submissionsData) {
		verify(formFields, conditions, submissionsData, null, null);
	}

	private void verify(String[][] formFields, String conditions,
			Object[][] submissionsData, String[] variables, String[] parameters) {

		ProjectBuilder projectBuilder = new ProjectBuilder();
		for (int i = 0; i < formFields.length; i++) {
			FormBuilder formBuilder = projectBuilder.addForm(formFields[i][0]);
			for (int j = 1; j < formFields[i].length; j++) {
				FibBuilder fibBuilder = formBuilder.addFib("Fib #" + j);
				fibBuilder.addBlank(formFields[i][j]);
			}
		}

		Project project = projectBuilder.build();

		UserProject userProject = new UserProject(project, projectOwner,
				"testExecution");
		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		StringBuffer getXmlDefinition = new StringBuffer("<delete>\n");
		for (int i = 0; i < formFields.length; i++) {
			getXmlDefinition.append("<form name=\"").append(formFields[i][0])
					.append("\" />\n");
		}

		getXmlDefinition.append("<conditions>\n" + conditions
				+ "</conditions>\n" + "</delete>");
		ConfigElement getComplexConditionXml = parseConfig(getXmlDefinition
				.toString());

		Map<Long, FormSubmission> expectedToBeDeleted = new HashMap<Long, FormSubmission>();

		for (int i = 0; i < submissionsData.length; i++) {
			Boolean shouldBeDeleted = (Boolean) submissionsData[i][0];
			String formName = (String) submissionsData[i][1];
			Form form = project.getForm(formName);

			FakeExecutionContext context = new FakeExecutionContext(
					userProject, form, new FakeRequest(true, Arrays.asList(
							submissionsData[i]).subList(2,
							submissionsData[i].length).toArray(new String[0])));
			context.getDomain().storedData().record(context.getSubmission());

			if (shouldBeDeleted) {
				expectedToBeDeleted.put(
						context.getSubmission().getDatabaseId(), context
								.getSubmission());
			}

		}

		FakeExecutionContext context = new FakeExecutionContext(userProject,
				project.defaultForm(), (parameters == null ? new String[0]
						: parameters));

		if (variables != null) {
			for (int i = 0; i < variables.length; i += 2) {
				String variable = variables[i];
				String value = variables[i + 1];
				context.setValue(variable, value);

			}
		}

		Delete deleteCommand = new Delete(getComplexConditionXml);
		deleteCommand.execute(context);

		List<FormSubmission> records = WorldInitializer.getDefaultWorld()
				.domain().storedData().responsesFor(project, "Form1");

		assertEquals("size", expectedToBeDeleted.size(), submissionsData.length
				- records.size());

		for (FormSubmission submission : records) {
			assertTrue("Submission is deleted: " + submission,
					!expectedToBeDeleted
							.containsKey(submission.getDatabaseId()));
		}
	}
}
