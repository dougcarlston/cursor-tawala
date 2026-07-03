package com.tawala.project;

import java.io.IOException;
import java.util.Collection;
import java.util.List;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.TestCase;
import com.tawala.domain.Domain;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.web.FakeRequest;
import com.tawala.web.WorldInitializer;

public class FormSubmissionsTest extends TestCase {
	private static final String[] USER_NAMES = new String[] { "aUser", "secondUser"};
	private User owner;
	private Domain domain;
	HibernateTestSetup hibernateTestSetup = new HibernateTestSetup();

	public FormSubmissionsTest() {
		setUserNamesToDelete(USER_NAMES);
	}

	protected void setUp() throws Exception {
		super.setUp();
		owner = UserTest.aUser(USER_NAMES[0]);

		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
		domain = WorldInitializer.getDefaultWorld().domain();
		domain.users().addOrSave(owner);

		hibernateTestSetup.onSetUp();
	}

	public void testBasicSaveFetchAndErase() {
		FormSubmissions data = domain.storedData();
		Project project = ProjectTest.projectHelloBob();

		UserProject userProject = new UserProject(project, owner, "test");
		domain.projects().put(userProject);

		Form form = project.defaultForm();
		for (int i = 0; i < 10; i++) {
			fetchAndErase(domain, userProject, data, form);
		}
	}

	private void fetchAndErase(Domain domain, UserProject userProject,
			FormSubmissions data, Form form) {
		assertEquals(0, domain.storedData().responsesFor(
				userProject.getProject(),
				userProject.getProject().defaultForm().getName()).size());

		ExecutionContext context = new FakeExecutionContext(userProject, form,
				new FakeRequest(true, "Q1:a", "Joe"));
		data.record(context.getSubmission());
		assertEquals(1, data.responsesFor(userProject.getProject(),
				form.getName()).size());
		assertEquals("Joe", data.responsesFor(userProject.getProject(),
				form.getName()).get(0).getValue(new Reference("Q1:a"))
				.toString());

		context = new FakeExecutionContext(userProject, form, new FakeRequest(
				true, "Q1:a", "Jack"));
		data.record(context.getSubmission());
		assertEquals(2, data.responsesFor(userProject.getProject(),
				form.getName()).size());
		assertEquals("Jack", data.responsesFor(userProject.getProject(),
				form.getName()).get(1).getValue(new Reference("Q1:a"))
				.toString());

		data.eraseResponsesFor(userProject.getProject(), form.getName());
		assertEquals(0, domain.storedData().responsesFor(
				userProject.getProject(),
				userProject.getProject().defaultForm().getName()).size());
	}

	// TODO: test multiple users & multiple projects

	public void testDataPreservedInFiles() throws IOException {
		Project project = whatsYourName();
		UserProject userProject = new UserProject(project, owner, "withData");
		domain.projects().put(userProject);

		ExecutionContext context = new FakeExecutionContext(userProject,
				project.getForm("Form 1"), new FakeRequest(true, "_segment",
						"0", "Q1:a", "Bob", "Q1:b", "Smith"));
		domain.storedData().record(context.getSubmission());

		assertEquals(1, domain.storedData().responsesFor(project, "Form 1")
				.size());

		Domain domain2 = new Domain();
		List<FormSubmission> responses = domain2.storedData().responsesFor(
				domain2.projects().get(owner.getId(), "withData").getProject(),
				"Form 1");
		assertEquals(1, responses.size());
		assertEquals(context.getSubmission(), responses.get(0));
	}

	public void testErasingFileResponses() {
		Project project = whatsYourName();
		UserProject userProject = new UserProject(project, owner, "test");
		domain.projects().put(userProject);

		ExecutionContext context = new FakeExecutionContext(userProject,
				project.getForm("Form 1"), new FakeRequest(true, "_segment",
						"0", "Q1:a", "Bob", "Q1:b", "Smith"));
		domain.storedData().record(context.getSubmission());

		context = new FakeExecutionContext(userProject, project
				.getForm("Form 1"), new FakeRequest(true, "_segment", "0",
				"Q1:a", "Jane", "Q1:b", "Doe"));
		domain.storedData().record(context.getSubmission());

		assertEquals(2, domain.storedData().responsesFor(project, "Form 1")
				.size());
		domain.storedData().eraseResponsesFor(project, "Form 1");
		assertEquals(0, domain.storedData().responsesFor(project, "Form 1")
				.size());
	}

	public void testUpdateSubmissions() throws IOException {
		Project project = whatsYourName();
		UserProject userProject = new UserProject(project, owner, "withData");
		domain.projects().put(userProject);

		ExecutionContext context = new FakeExecutionContext(userProject,
				project.getForm("Form 1"), new FakeRequest(true, "_segment",
						"0", "Q1:a", "Bob", "Q1:b", "Smith"));
		domain.storedData().record(context.getSubmission());

		assertEquals(1, domain.storedData().responsesFor(project, "Form 1")
				.size());

		List<FormSubmission> responses = domain.storedData().responsesFor(
				domain.projects().get(USER_NAMES[0], "withData").getProject(),
				"Form 1");

		responses.get(0).setValue("Q1:b", "Jones");
		domain.updateSubmissions(responses);

		Domain domain2 = new Domain();
		responses = domain2.storedData().responsesFor(
				domain2.projects().get(USER_NAMES[0], "withData").getProject(),
				"Form 1");
		assertEquals(1, responses.size());
		assertEquals("Jones", responses.get(0).getValue(new Reference("Q1:b"))
				.toString());
	}

	private Project whatsYourName() {
		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm("Form 1").addFib("What's your name?", 20, 20);
		return builder.build();
	}

	private void assertEquals(FormSubmission submission1,
			FormSubmission submission2) {
		Collection<String> fieldIds1 = submission1.getFieldIds();
		Collection<String> fieldIds2 = submission2.getFieldIds();

		assertEquals("keys", fieldIds1, fieldIds2);

		for (String fieldId : fieldIds1) {
			assertEquals(submission1.getValues(new Reference(fieldId)),
					submission2.getValues(new Reference(fieldId)));

		}
	}

	public void testDeleteAllProjectData() {
		Project project = whatsYourName();
		UserProject userProject = new UserProject(project, owner, "withData");
		domain.projects().put(userProject);

		ExecutionContext context = new FakeExecutionContext(userProject,
				project.getForm("Form 1"), new FakeRequest(true, "_segment",
						"0", "Q1:a", "Bob", "Q1:b", "Smith"));
		domain.storedData().record(context.getSubmission());

		assertEquals(1, domain.storedData().responsesFor(project, "Form 1")
				.size());

		userProject = domain.projects().get(owner.getId(), "withData");

		domain.storedData().purgeProjectResponses(userProject.getProject());

		assertEquals(0, domain.storedData().responsesFor(project, "Form 1")
				.size());
	}

	public void testResponseCount() {
		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm("Form 1").addFib("What's your name?", 20, 20);
		builder.addForm("Form 2").addFib("What's your age?", 20);

		Project project = builder.build();
		UserProject userProject = new UserProject(project, owner, "withData");
		domain.projects().put(userProject);

		ExecutionContext context = new FakeExecutionContext(userProject,
				project.getForm("Form 1"), new FakeRequest(true, "_segment",
						"0", "Q1:a", "Bob", "Q1:b", "Smith"));
		domain.storedData().record(context.getSubmission());

		assertEquals(1, domain.storedData().responseCount(project));
		assertEquals(1, domain.storedData().responseCount(project, "Form 1"));
		assertEquals(0, domain.storedData().responseCount(project, "Form 2"));

		context = new FakeExecutionContext(userProject, project
				.getForm("Form 1"), new FakeRequest(true, "_segment", "0",
				"Q1:a", "Jane", "Q1:b", "X"));
		domain.storedData().record(context.getSubmission());

		assertEquals(2, domain.storedData().responseCount(project));
		assertEquals(2, domain.storedData().responseCount(project, "Form 1"));
		assertEquals(0, domain.storedData().responseCount(project, "Form 2"));

		context = new FakeExecutionContext(userProject, project
				.getForm("Form 2"), new FakeRequest(true, "_segment", "0",
				"Q1:a", "21"));
		domain.storedData().record(context.getSubmission());

		assertEquals(3, domain.storedData().responseCount(project));
		assertEquals(2, domain.storedData().responseCount(project, "Form 1"));
		assertEquals(1, domain.storedData().responseCount(project, "Form 2"));

	}

	public void testVerifyUserAndDeleteSubmission() {
		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm("Form 1").addFib("What's your name?", 20, 20);

		Project project = builder.build();
		UserProject userProject = new UserProject(project, owner, "project1");
		domain.projects().put(userProject);

		ExecutionContext context = new FakeExecutionContext(userProject,
				project.getForm("Form 1"), new FakeRequest(true, "_segment",
						"0", "Q1:a", "Bob", "Q1:b", "Smith"));
		domain.storedData().record(context.getSubmission());

		List<FormSubmission> submissions = domain.storedData().responsesFor(
				project, "Form 1");
		assertEquals(1, submissions.size());

		User anotherOne = UserTest.aUser(USER_NAMES[1]);
		domain.users().addOrSave(anotherOne);
		
		//--- Attempt to delete by another user
		domain.storedData().verifyThatBelongsToUserAndDelete(submissions.get(0).getDatabaseId(), anotherOne);
		submissions = domain.storedData().responsesFor(
				project, "Form 1");
		assertEquals(1, submissions.size());

		//--- Attempt to delete by project owner
		domain.storedData().verifyThatBelongsToUserAndDelete(submissions.get(0).getDatabaseId(), owner);
		submissions = domain.storedData().responsesFor(
				project, "Form 1");
		assertEquals(0, submissions.size());
	}

}
