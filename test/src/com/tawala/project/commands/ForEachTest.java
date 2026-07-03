package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.Domain;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.FakeRequest;

public class ForEachTest extends TestCase {

	private static final String USER_NAME = "testuser1";
	private ConfigElement getXml;
	private ConfigElement forEachXml;
	private Domain domain;
	private User owner;

	public ForEachTest() {
		setUserNamesToDelete(new String[] { USER_NAME });
	}

	public void setUp() throws Exception {
		super.setUp();

		getXml = parseConfig("<get recordList=\"RecordList\">\n" + "<forms>\n"
				+ "<form name=\"Form 1\" />\n" + "</forms>\n"
				+ "<conditions>\n" + "</conditions>\n" + "</get>");

		domain = new Domain(new UsersHibernateImpl());

		owner = UserTest.aUser(USER_NAME);
		domain.users().addOrSave(owner);
	}

	public void testCommandCreation() {

		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "</foreach>");

		ForEach forEach = new ForEach(forEachXml);

		assertEquals("Record", forEach.recordName());
		assertEquals("RecordList", forEach.recordListName());
	}

	public void testExecution() {

		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "<addTo field=\"PassCount\">\n"
				+ "<operand value=\"1\"/>\n"
				+ "</addTo>\n" + "</foreach>");

		Get get = new Get(getXml);
		ForEach forEach = new ForEach(forEachXml);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		projectBuilder.addForm("Form 1");

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testExecution");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		record(userProject, form);

		FakeExecutionContext context = makeContext(project, form);
		get.execute(context);
		forEach.execute(context);
		assertEquals(1, forEach.commands().size());
		assertEquals("1", context.getValue("PassCount").toString());
	}

	public void testNonexistentRecordList() {

		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"NonexistentRecordList\">\n"
				+ "</foreach>");

		Get get = new Get(getXml);
		ForEach forEach = new ForEach(forEachXml);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		projectBuilder.addForm("Form 1");

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testExecution");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		record(userProject, form);

		FakeExecutionContext context = makeContext(project, form);
		get.execute(context);
		forEach.execute(context);
		assertEquals(0, forEach.commands().size());
	}

	public void testFieldReadFibAlternateLabel() {

		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "<addTo field=\"TotalScore\">\n"
				+ "<operand field=\"Record:Form 1:Score\"/>\n"
				+ "</addTo>\n"
				+ "</foreach>");

		Get get = new Get(getXml);
		ForEach forEach = new ForEach(forEachXml);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form 1");
		formBuilder.addFib("Fib Item 1", "Score", 10);

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testFieldRead");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		record(userProject, form, "Score", "90");
		record(userProject, form, "Score", "100");

		FakeExecutionContext context = makeContext(project, form);
		get.execute(context);
		forEach.execute(context);
		assertEquals("190", context.getValue("TotalScore").toString());
	}

	public void testFieldReadFibDefaultLabel() {

		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "<addTo field=\"TotalScore\">\n"
				+ "<operand field=\"Record:Form 1:Q1:a\"/>\n"
				+ "</addTo>\n"
				+ "</foreach>");

		Get get = new Get(getXml);
		ForEach forEach = new ForEach(forEachXml);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form 1");
		formBuilder.addFib("Fib Item 1", 10);

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testFieldReadFibDefaultLabel");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		record(userProject, form, "Q1:a", "90");
		record(userProject, form, "Q1:a", "100");

		FakeExecutionContext context = makeContext(project, form);
		get.execute(context);
		forEach.execute(context);
		assertEquals("190", context.getValue("TotalScore").toString());
	}

	public void testIfFibDefaultLabel() {

		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "<if>\n"
				+ "<conditions>\n"
				+ "<equals field=\"Record:Form 1:Q1:a\">\n"
				+ "<string value=\"a\"/>\n"
				+ "</equals>\n"
				+ "</conditions>\n"
				+ "<trueSet>\n"
				+ "<addTo field=\"TotalScore\">\n"
				+ "<operand value=\"10\"/>\n"
				+ "</addTo>\n"
				+ "</trueSet>\n"
				+ "</if>\n" + "</foreach>");

		Get get = new Get(getXml);
		ForEach forEach = new ForEach(forEachXml);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form 1");
		formBuilder.addFib("Fib Item 1", 10);

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testFieldReadMCDefaultLabel");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		record(userProject, form, "Q1:a", "a");
		record(userProject, form, "Q1:a", "a");

		FakeExecutionContext context = makeContext(project, form);
		get.execute(context);
		forEach.execute(context);
		assertEquals("20", context.getValue("TotalScore").toString());
	}

	public void testIfMCDefaultLabel() {

		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "<if>\n"
				+ "<conditions>\n"
				+ "<mcEquals field=\"Record:Form 1:Q1\" value=\"a\"/>\n"
				+ "</conditions>\n"
				+ "<trueSet>\n"
				+ "<addTo field=\"TotalScore\">\n"
				+ "<operand value=\"10\"/>\n"
				+ "</addTo>\n"
				+ "</trueSet>\n"
				+ "</if>\n" + "</foreach>");

		Get get = new Get(getXml);
		ForEach forEach = new ForEach(forEachXml);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form 1");
		formBuilder.addMc("MC Item 1", "Choice A", "Choice B");

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testFieldReadMCDefaultLabel");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		record(userProject, form, "Q1", "a");
		record(userProject, form, "Q1", "a");

		FakeExecutionContext context = makeContext(project, form);
		get.execute(context);
		forEach.execute(context);
		assertEquals("20", context.getValue("TotalScore").toString());
	}

	public void testSettingFieldOfASingleFormGet() {
		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "<set field=\"Record:Form 1:new field\"><string value=\"abc\" /></set>"
				+ "<set field=\"Record:Form 1:Q1:a\"><string value=\"new value\" /></set>"
				+ "</foreach>");

		Get get = new Get(getXml);
		ForEach forEach = new ForEach(forEachXml);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form 1");
		formBuilder.addFib("Fib Item 1", 10);

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testSettingField");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		record(userProject, form, "Q1:a", "a");

		FakeExecutionContext context = makeContext(project, form);
		get.execute(context);
		forEach.execute(context);

		List<FormSubmission> submissions = domain.storedData().responsesFor(
				project, "Form 1");
		assertEquals(1, submissions.size());

		FormSubmission submission = submissions.get(0);
		assertEquals("abc", submission.getValue(new Reference("new field"))
				.toString());
		assertEquals("new value", submission.getValue(new Reference("Q1:a"))
				.toString());
	}

	public void testSettingFieldOfAMultiFormGet() {
		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "<set field=\"Record:Form1:new field\"><string value=\"abc\" /></set>"
				+ "<set field=\"Record:Form1:Q2:a\"><string value=\"changed\" /></set>"
				+ "<set field=\"Record:Form2:Q1:a\"><string value=\"new value\" /></set>"
				+ "</foreach>");

		ConfigElement multiFormGet = parseConfig("<get recordList=\"RecordList\">\n"
				+ "<forms>\n"
				+ "<form name=\"Form1\" />\n"
				+ "<form name=\"Form2\" />\n"
				+ "</forms>\n"
				+ "<conditions>\n"
				+ "<equals field=\"RecordList:Form1:Q1:a\">"
				+ "<string field=\"RecordList:Form2:Q1:a\" />"
				+ "</equals>"
				+ "</conditions>\n" + "</get>");

		Get get = new Get(multiFormGet);
		ForEach forEach = new ForEach(forEachXml);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form1");
		formBuilder.addFib("Form 1 Fib Item 1", 10);
		formBuilder.addFib("Form 1 Fib Item 2", 10);

		formBuilder = projectBuilder.addForm("Form2");
		formBuilder.addFib("Form 2 Fib Item 1", 10);

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testSettingField");
		domain.projects().put(userProject);

		Form form1 = project.getForm("Form1");
		record(userProject, form1, "Q1:a", "a", "Q2:a", "#1");
		record(userProject, form1, "Q1:a", "b", "Q2:a", "#2");
		record(userProject, form1, "Q1:a", "c", "Q2:a", "#3");
		record(userProject, form1, "Q1:a", "d", "Q2:a", "#4");
		record(userProject, form1, "Q1:a", "e", "Q2:a", "#5");

		Form form2 = project.getForm("Form2");
		record(userProject, form2, "Q1:a", "c");
		record(userProject, form2, "Q1:a", "x");

		FakeExecutionContext context = makeContext(project, form1);
		get.execute(context);
		forEach.execute(context);

		List<FormSubmission> submissions = domain.storedData().responsesFor(
				project, "Form1");
		assertEquals(5, submissions.size());

		FormSubmission changedFromForm1 = submissions.get(2);
		Reference newFieldReference = new Reference("new field");
		assertEquals("c", changedFromForm1.getValue(new Reference("Q1:a"))
				.toString());
		assertEquals("abc", changedFromForm1.getValue(newFieldReference)
				.toString());
		assertEquals("changed", changedFromForm1
				.getValue(new Reference("Q2:a")).toString());

		FormSubmission unchangedFromForm1 = submissions.get(1);
		assertEquals("", unchangedFromForm1.getValue(newFieldReference)
				.toString());

		submissions = domain.storedData().responsesFor(project, "Form2");

		FormSubmission changedFromForm2 = submissions.get(0);
		assertEquals("new value", changedFromForm2.getValue(
				new Reference("Q1:a")).toString());
	}

	public void testStopLoopingIfThereIsShowForm() {

		forEachXml = parseConfig("<foreach record=\"Record\" recordList=\"RecordList\">\n"
				+ "<if>\n"
				+ "<conditions>\n"
				+ "<mcEquals field=\"Record:Form 1:Q1\" value=\"a\"/>\n"
				+ "</conditions>\n"
				+ "<trueSet>\n"
				+ "<addTo field=\"Score\">\n"
				+ "<operand value=\"1\"/>\n"
				+ "</addTo>\n"
				+ "<show form=\"Form2\" />\n"
				+ "</trueSet>\n"
				+ "</if>\n" + "</foreach>");

		Get get = new Get(getXml);
		ForEach forEach = new ForEach(forEachXml);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form 1");
		formBuilder.addMc("MC Item 1", "Choice A", "Choice B");
		projectBuilder.addForm("Form2");

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"testFieldReadMCDefaultLabel");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		record(userProject, form, "Q1", "a");
		record(userProject, form, "Q1", "a");

		FakeExecutionContext context = makeContext(project, form);
		get.execute(context);
		forEach.execute(context);
		assertEquals("1", context.getValue("Score").toString());
	}

	private FakeExecutionContext makeContext(Project project, Form form) {
		return new FakeExecutionContext(domain, new UserProject(project, owner,
				"test"), form);
	}

	private void record(UserProject project, Form form, String... data) {
		domain.storedData().record(
				new FakeExecutionContext(project, form, new FakeRequest(true,
						data)).getSubmission());
	}
}
