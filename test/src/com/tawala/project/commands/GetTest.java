package com.tawala.project.commands;

import java.util.Arrays;
import java.util.List;

import javax.servlet.ServletException;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.Form;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.FakeRequest;
import com.tawala.web.WorldInitializer;

public class GetTest extends TestCase {

	private ConfigElement getXml;

	private ConfigElement getTwoFormXml;

	private User projectOwner = UserTest.aUser("tester");

	private HibernateTestSetup hibernateTestSetup = new HibernateTestSetup();

	public GetTest() {
		hibernateTestSetup.onSetUp();
		addUserNameToDelete(projectOwner.getId());
	}

	public void setUp() throws ServletException, Exception {
		super.setUp();

		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));

		WorldInitializer.getDefaultWorld().domain().users().addOrSave(
				projectOwner);

		getXml = parseConfig("<get recordList=\"RecordList\">\n" + "<forms>\n"
				+ "<form name=\"Form 1\" />\n" + "</forms>\n"
				+ "<conditions>\n" + "</conditions>\n" + "</get>");

		getTwoFormXml = parseConfig("<get recordList=\"RecordList\">\n"
				+ "<forms>\n" + "<form name=\"Form 1\" />\n"
				+ "<form name=\"Form 2\" />\n" + "</forms>\n"
				+ "<conditions>\n" + "</conditions>\n" + "</get>");
	}

	public void testCommandCreation() {
		Get get = new Get(getXml);
		assertEquals("RecordList", get.recordListName());
		assertEquals(1, get.getFormDataProviders().size());
		assertEquals("Form 1", get.getFormDataProviders().get(0).getFormName());
	}

	public void testMultipleFormCommandCreation() {
		Get get = new Get(getTwoFormXml);
		assertEquals("RecordList", get.recordListName());
		assertEquals(2, get.getFormDataProviders().size());
		assertEquals("Form 1", get.getFormDataProviders().get(0).getFormName());
		assertEquals("Form 2", get.getFormDataProviders().get(1).getFormName());
	}

	public void testSingleFormMatch() {
		verify(
		// Form definition
				new String[][] { { "Form1", "Q1:a", "Q2:a" } },
				// Condition
				"<equals field=\"RecordList:Form1:Q1:a\">"
						+ "<string field=\"RecordList:Form1:Q2:a\" />"
						+ "</equals>",
				// Form submissions
				new String[][] { { "Form1", "Q1:a", "John", "Q2:a", "Steve" },
						{ "Form1", "Q1:a", "Tony", "Q2:a", "Tony" },
						{ "Form1", "Q1:a", "Steve", "Q2:a", "John" },
						{ "Form1", "Q1:a", "John" } },
				// Expected results
				new String[][] { { "Form1:Q1:a", "Tony", "Form1:Q2:a", "Tony" } });
	}

	public void testSingleFormMatchWithoutRecordQualifier() {
		verify(
		// Form definition
				new String[][] { { "Form1", "Q1:a", "Q2:a" } },
				// Condition
				"<equals field=\"Form1:Q1:a\">"
						+ "<string field=\"Form1:Q2:a\" />" + "</equals>",
				// Form submissions
				new String[][] { { "Form1", "Q1:a", "John", "Q2:a", "Steve" },
						{ "Form1", "Q1:a", "Tony", "Q2:a", "Tony" },
						{ "Form1", "Q1:a", "Steve", "Q2:a", "John" },
						{ "Form1", "Q1:a", "John" } },
				// Expected results
				new String[][] { /* No records are expected */});
	}

	public void testTwoFormJoinedWithSingleRecordMatch() {
		verify(
		// Form definition
				new String[][] { { "F1", "1", "2" }, { "F2", "3", "4" } },
				// Condition
				"<equals field=\"RecordList:F1:1\">"
						+ "<string field=\"RecordList:F2:3\" />" + "</equals>",
				// Form submissions
				new String[][] { { "F1", "1", "abc", "2", "xxx" },
						{ "F1", "1", "bcd", "2", "yyy" },
						{ "F1", "1", "def", "2", "zzz" },
						{ "F2", "3", "bcd", "4", "ttt" },
						{ "F2", "3", "qwe", "4", "sss" }, },
				// Expected results
				new String[][] { { "F1:1", "bcd", "F1:2", "yyy", "F2:3", "bcd",
						"F2:4", "ttt" } });
	}

	public void testTwoFormJoinedWithMultipleRecordMatch() {
		verify(
		// Form definition
				new String[][] { { "F1", "1", "2" }, { "F2", "3", "4" } },
				// Condition
				"<equals field=\"RecordList:F1:1\">"
						+ "<string field=\"RecordList:F2:3\" />" + "</equals>",
				// Form submissions
				new String[][] { { "F1", "1", "abc", "2", "xxx" },
						{ "F1", "1", "bcd", "2", "yyy" },
						{ "F1", "1", "def", "2", "zzz" },
						{ "F2", "3", "bcd", "4", "ttt" },
						{ "F2", "3", "bcd", "4", "fff" },
						{ "F2", "3", "qwe", "4", "sss" }, },
				// Expected results
				new String[][] {
						{ "F1:1", "bcd", "F1:2", "yyy", "F2:3", "bcd", "F2:4",
								"ttt" },
						{ "F1:1", "bcd", "F1:2", "yyy", "F2:3", "bcd", "F2:4",
								"fff" }, });
	}

	public void testTwoFormJoinedWithMultipleMatchesInBothForms() {
		verify(
		// Form definition
				new String[][] { { "F1", "1", "2" }, { "F2", "3", "4" } },
				// Condition
				"<equals field=\"RecordList:F1:1\">"
						+ "<string field=\"RecordList:F2:3\" />" + "</equals>",
				// Form submissions
				new String[][] { { "F1", "1", "abc", "2", "xxx" },
						{ "F1", "1", "bcd", "2", "yyy" },
						{ "F1", "1", "def", "2", "zzz" },
						{ "F1", "1", "bcd", "2", "iii" },
						{ "F2", "3", "bcd", "4", "ttt" },
						{ "F2", "3", "bcd", "4", "fff" },
						{ "F2", "3", "qwe", "4", "sss" }, },
				// Expected results
				new String[][] {
						{ "F1:1", "bcd", "F1:2", "yyy", "F2:3", "bcd", "F2:4",
								"ttt" },
						{ "F1:1", "bcd", "F1:2", "yyy", "F2:3", "bcd", "F2:4",
								"fff" },
						{ "F1:1", "bcd", "F1:2", "iii", "F2:3", "bcd", "F2:4",
								"ttt" },
						{ "F1:1", "bcd", "F1:2", "iii", "F2:3", "bcd", "F2:4",
								"fff" }, });
	}

	public void testEmptyDataIsNotMerged() {
		verify(
		// Form definition
				new String[][] { { "F1", "1", "2" }, { "F2", "3", "4" } },
				// Condition
				"<equals field=\"RecordList:F1:field1\">"
						+ "<string field=\"RecordList:F2:field2\" />"
						+ "</equals>",
				// Form submissions
				new String[][] { { "F1", "1", "abc", "2", "xxx" },
						{ "F1", "1", "bcd", "2", "yyy" },
						{ "F1", "1", "def", "2", "zzz" },
						{ "F1", "1", "bcd", "2", "iii" },
						{ "F2", "3", "bcd", "4", "ttt" },
						{ "F2", "3", "bcd", "4", "fff" },
						{ "F2", "3", "qwe", "4", "sss" }, },
				// Expected results
				new String[][] {});
	}

	public void testMatchingWhenFieldsAreNotPrefixedWithRecordListName() {
		verify(
		// Form definition
				new String[][] { { "F1", "1", "2" }, { "F2", "3", "4" } },
				// Condition
				"<equals field=\"F1:1\">" + "<string field=\"F2:3\" />"
						+ "</equals>",
				// Form submissions
				new String[][] { { "F1", "1", "abc", "2", "xxx" },
						{ "F1", "1", "bcd", "2", "yyy" },
						{ "F1", "1", "def", "2", "zzz" },
						{ "F2", "3", "bcd", "4", "ttt" },
						{ "F2", "3", "qwe", "4", "sss" }, },
				// Expected results
				new String[][] { /* no records expected */});
	}

	public void testMatchingWithAVariable() {
		verify(
		// Form definition
				new String[][] { { "F1", "1", "2" } },
				// Condition
				"<equals field=\"RecordList:F1:1\">"
						+ "<string field=\"myvariable\" />" + "</equals>",
				// Form submissions
				new String[][] { { "F1", "1", "abc", "2", "xxx" },
						{ "F1", "1", "bcd", "2", "yyy" },
						{ "F1", "1", "def", "2", "zzz" },
						{ "F1", "1", "xyz", "2", "xxx" } },
				// Variables
				new String[] { "myvariable", "def" },
				// Parameters
				null,
				// Expected results
				new String[][] { { "F1:1", "def", "F1:2", "zzz" } });
	}

	public void testMatchingWithAFieldInCurrentSubmission() {
		verify(
		// Form definition
				new String[][] { { "F1", "1", "2" } },
				// Condition
				"<equals field=\"RecordList:F1:1\">"
						+ "<string field=\"F1:1\" />" + "</equals>",
				// Form submissions
				new String[][] { { "F1", "1", "abc", "2", "xxx" },
						{ "F1", "1", "bcd", "2", "yyy" },
						{ "F1", "1", "def", "2", "zzz" },
						{ "F1", "1", "xyz", "2", "xxx" } },
				// Variables
				new String[] {},
				// Parameters
				new String[] { "1", "def" },
				// Expected results
				new String[][] { { "F1:1", "def", "F1:2", "zzz" } });
	}

	private void verify(String[][] formFields, String conditions,
			String[][] submissionsData, String[][] expectedData) {
		verify(formFields, conditions, submissionsData, null, null,
				expectedData);
	}

	private void verify(String[][] formFields, String conditions,
			String[][] submissionsData, String[] variables,
			String[] parameters, String[][] expectedData) {

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

		StringBuffer getXmlDefinition = new StringBuffer(
				"<get recordList=\"RecordList\">\n" + "<forms>\n");
		for (int i = 0; i < formFields.length; i++) {
			getXmlDefinition.append("<form name=\"").append(formFields[i][0])
					.append("\" />\n");
		}

		getXmlDefinition.append("</forms>\n" + "<conditions>\n" + conditions
				+ "</conditions>\n" + "</get>");
		ConfigElement getComplexConditionXml = parseConfig(getXmlDefinition
				.toString());

		for (int i = 0; i < submissionsData.length; i++) {
			String formName = submissionsData[i][0];
			Form form = project.getForm(formName);

			FakeExecutionContext context = new FakeExecutionContext(
					userProject, form);
			context.getDomain().storedData().record(
					new FakeExecutionContext(userProject, form,
							new FakeRequest(true, Arrays.asList(
									submissionsData[i]).subList(1,
									submissionsData[i].length).toArray(
									new String[0]))).getSubmission());

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

		Get get = new Get(getComplexConditionXml);
		get.execute(context);

		List<CompositeFormSubmission> recordList = context
				.getRecordList("RecordList");

		assertEquals("size", expectedData.length, recordList.size());
		for (int i = 0; i < expectedData.length; i++) {
			for (int j = 0; j < expectedData[i].length; j += 2) {
				String field = expectedData[i][j];
				String expectedValue = expectedData[i][j + 1];
				Reference reference = new Reference(field, context);
				assertEquals("checking " + field, expectedValue, recordList
						.get(i).getFormSubmission(reference)
						.getValue(reference).toString());
			}
		}
	}

}
