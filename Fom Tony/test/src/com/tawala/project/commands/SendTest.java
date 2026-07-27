package com.tawala.project.commands;

import java.io.IOException;
import java.util.Arrays;
import java.util.Date;
import java.util.List;

import org.springframework.mail.javamail.JavaMailSenderImpl;
import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.scissor.Log;
import com.scissor.LogMonitor;
import com.tawala.TestCase;
import com.tawala.domain.Domain;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.email.Email;
import com.tawala.email.EmailService;
import com.tawala.email.Emailer;
import com.tawala.email.UserProjectEmail;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.Document;
import com.tawala.project.Form;
import com.tawala.project.Process;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.ForEachBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ImageInstanceBuilder;
import com.tawala.project.builder.PageHeaderBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.FakeRequest;
import com.tawala.web.WorldInitializer;

import fake.smtp.FakeSmtpMessage;
import fake.smtp.FakeSmtpServer;

public class SendTest extends TestCase {
	private FakeSmtpServer server;

	private FakeExecutionContext context;

	private LogMonitor logs;

	private User user = UserTest.aUser("testuser");
	private UserProject userProject = null;

	public SendTest() {
		setUserNamesToDelete(new String[] { user.getId() });
		EmailService.setSendImmediately(true);
	}

	protected void setUp() throws Exception {
		new HibernateTestSetup().onSetUp();
		super.setUp();

		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));

		WorldInitializer.getDefaultWorld().domain().users().addOrSave(user);

		server = new FakeSmtpServer();
		context = newContext();
		logs = new LogMonitor();
		Log.captureLogging(logs);

		JavaMailSenderImpl senderImpl = new JavaMailSenderImpl();
		senderImpl.setPort(server.getPort());
		senderImpl.setHost("127.0.0.1");
		new Emailer().setSender(senderImpl);
	}

	protected void tearDown() throws Exception {
		Log.normalLogging();
		server.shutDown();
		super.tearDown();
	}

	public void testBasicSend() throws IOException, InterruptedException {
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressLiteral=\"toName@example.com\" />\n"
				+ "    <subject>Test email</subject>\n" + "    <body>line 1\n"
				+ "line 2</body>\n" + "</send>\n"));

		FakeSmtpMessage message = execute(command);

		List<String> recipients = message.getRecipients();
		assertEquals(Arrays.asList("toName@example.com"), recipients);
		assertEquals("toName@example.com", message.getHeader("To"));
		assertNull(message.getHeader("Cc"));

//		assertEquals("test@example.com", message.getSender());
		//assertEquals("test@example.com", message.getHeader("From"));

		assertEquals("Test email", message.getHeader("Subject"));

		assertEquals("text/plain; charset=us-ascii", message
				.getHeader("Content-Type"));
		assertEquals("line 1\nline 2\n", message.getBody());

		List<UserProjectEmail> savedEmails = EmailService
				.getAllEmailsForProject(user, userProject.getId());
		assertNotNull(savedEmails);
		assertEquals(1, savedEmails.size());
		UserProjectEmail savedEmail = savedEmails.get(0);
		assertEquals(Email.State.SENT, savedEmail.getState());
		assertEquals("toName@example.com", savedEmail.getTo());
		assertEquals("Test email", savedEmail.getSubject());
		assertTrue(savedEmail.getCreatedDate().before(new Date()));
		assertTrue(savedEmail.getSentDate().before(new Date()));

		// --- Retrieve the whole thing.
		savedEmail = EmailService.getUserProjectEmailById(user, savedEmail
				.getId());
		assertEquals("line 1\nline 2", savedEmail.getMessageText());

	}

	public void testSendWhenUserDoesntHaveEmail() throws IOException,
			InterruptedException {
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressLiteral=\"toName@example.com\" />\n"
				+ "    <subject>Test email</subject>\n" + "    <body>line 1\n"
				+ "line 2</body>\n" + "</send>\n"));

		// --- Make sure the user doesn't have email.
		user.setEmail(null);
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(user);
		user = WorldInitializer.getDefaultWorld().domain().users().get(
				user.getDatabaseId());

		Project project = ProjectBuilder.buildMinimalisticProject();
		UserProject userProject = new UserProject(project, user, "Project One");
		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);
		FakeExecutionContext context = new FakeExecutionContext(userProject,
				project.defaultForm(), new String[0]);

		command.execute(context);
		FakeSmtpMessage message = exactlyOneMessage();

		List<String> recipients = message.getRecipients();
		assertEquals(Arrays.asList("toName@example.com"), recipients);
		assertEquals("toName@example.com", message.getHeader("To"));
		assertNull(message.getHeader("Cc"));

//		assertEquals("message@tawala.com", message.getSender());
//		assertEquals("message@tawala.com", message.getHeader("From"));

		assertEquals("Test email", message.getHeader("Subject"));

		assertEquals("text/plain; charset=us-ascii", message
				.getHeader("Content-Type"));
		assertEquals("line 1\nline 2\n", message.getBody());
	}

	public void testWithCarbonCopy() throws IOException, InterruptedException {
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressLiteral=\"toName@example.com\" />\n"
				+ "    <cc addressLiteral=\"ccName@example.com\" />\n"
				+ "    <subject>subject</subject>\n"
				+ "    <body>body</body>\n" + "</send>\n"));

		FakeSmtpMessage message = execute(command);

		List<String> recipients = message.getRecipients();
		assertEquals(Arrays.asList("toName@example.com", "ccName@example.com"),
				recipients);

		assertEquals("toName@example.com", message.getHeader("To"));
		assertEquals("ccName@example.com", message.getHeader("Cc"));
	}

	public void testOnlyCarbonCopy() throws IOException, InterruptedException {
		Send command = new Send(parseConfig("<send>\n"
				+ "    <cc addressLiteral=\"ccName@example.com\" />\n"
				+ "    <subject>subject</subject>\n"
				+ "    <body>body</body>\n" + "</send>\n"));

		FakeSmtpMessage message = execute(command);

		List<String> recipients = message.getRecipients();
		assertEquals(Arrays.asList("ccName@example.com"), recipients);

		assertNull(message.getHeader("To"));
		assertEquals("ccName@example.com", message.getHeader("Cc"));
	}

	public void testMultipleRecipients() throws IOException,
			InterruptedException {
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressLiteral=\"one@example.com\" />\n"
				+ "    <to addressLiteral=\"two@example.com\" />\n"
				+ "    <cc addressLiteral=\"three@example.com\" />\n"
				+ "    <cc addressLiteral=\"four@example.com\" />\n"
				+ "    <subject>subject</subject>\n"
				+ "    <body>body</body>\n" + "</send>\n"));

		FakeSmtpMessage message = execute(command);

		List<String> recipients = message.getRecipients();
		assertEquals(Arrays.asList("one@example.com", "two@example.com",
				"three@example.com", "four@example.com"), recipients);

		assertEquals("one@example.com, two@example.com", message
				.getHeader("To"));
		assertEquals("three@example.com, four@example.com", message
				.getHeader("Cc"));
	}

	public void testSendToEnteredAddress() throws InterruptedException {
		context = newContext("Q1:a", "joe@example.org");
		context.getProject().add(new Document("doc1", "doc text"));
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressField=\"aForm:Q1:a\" />\n"
				+ "    <subject>subject</subject>\n" + "    <body>foo</body>\n"
				+ "</send>\n"));

		FakeSmtpMessage message = execute(command);

		assertEquals(Arrays.asList("joe@example.org"), message.getRecipients());
		assertEquals("joe@example.org", message.getHeader("To"));

	}

	public void testSendToAndCcEnteredAddress() throws InterruptedException {
		context = newContext("Q1:a", "joe@example.org", "Q1:b",
				"jane@example.org");
		context.getProject().add(new Document("doc1", "doc text"));
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressField=\"aForm:Q1:a\" />\n"
				+ "    <cc addressField=\"aForm:Q1:b\" />\n"
				+ "    <subject>subject</subject>\n" + "    <body>foo</body>\n"
				+ "</send>\n"));

		FakeSmtpMessage message = execute(command);

		assertEquals(Arrays.asList("joe@example.org", "jane@example.org"),
				message.getRecipients());
		assertEquals("joe@example.org", message.getHeader("To"));
		assertEquals("jane@example.org", message.getHeader("Cc"));
	}

	public void testSendToAnIncorrectAddress() throws InterruptedException {
		context = newContext("Q1:a", "joe@example@@@@org", "Q1:b",
				"jane@example@@@@org");
		context.getProject().add(new Document("doc1", "doc text"));
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressField=\"aForm:Q1:a\" />\n"
				+ "    <cc addressField=\"aForm:Q1:b\" />\n"
				+ "    <subject>subject</subject>\n" + "    <body>foo</body>\n"
				+ "</send>\n"));

		command.execute(context);
		assertEquals(0, server.getMessageCount());
	}

	public void testSendDocument() throws InterruptedException {
		context.getProject().add(new Document("doc1", "doc text"));
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressLiteral=\"one@example.com\" />\n"
				+ "    <subject>subject</subject>\n"
				+ "    <body document=\"doc1\" />\n" + "</send>\n"));

		FakeSmtpMessage message = execute(command);
		assertEquals("text/html; charset=us-ascii", message
				.getHeader("Content-Type"));
		assertContains("<div class=\"document\">doc text</div>\n", message
				.getBody());
	}

	public void testSendDocumentWithResets() throws InterruptedException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder doc1 = builder.addDocument("Document 1");
		DocumentBuilder doc2 = builder.addDocument("Document 2");
		DocumentBuilder doc3 = builder.addDocument("Document 3");
		doc1.addText("one");
		doc2.addText("two");
		doc3.addText("three");

		ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
		proc1.addAppend(doc1, doc2);
		proc1.addAppend(doc1, doc3);
		proc1.addSend("Main:Email", "test@example.org",
				"Document with appended text", doc1, true, true);
		proc1.addAppend(doc1, doc3);
		proc1.addSend("Main:Email", "test@example.org",
				"Document after text reset", doc1, true, true);

		builder.addForm("Main", proc1);

		Project project = builder.build();

		UserProject userProject2 = new UserProject(project, user,
				"NormalAppend");
		userProject2 = WorldInitializer.getDefaultWorld().domain().projects()
				.put(userProject2);
		FakeExecutionContext context = new FakeExecutionContext(userProject2,
				project.defaultForm());
		context.setValue("Main:Email", "test@tawala.com");

		Process process = project.getProcess(proc1.getName());
		process.execute(context);

		server.waitForAllConnectionsToClose();

		assertEquals(2, server.getMessageCount());

		FakeSmtpMessage message = server.getMessage(0);
		assertMatches("<div class=\"document\">.*one.*</div>\n"
				+ "<div class=\"document\">.*two.*</div>\n"
				+ "<div class=\"document\">.*three.*</div>\n", message
				.getBody());

		message = server.getMessage(1);
		assertMatches("<div class=\"document\">.*one.*</div>\n"
				+ "<div class=\"document\">.*three.*</div>\n", message
				.getBody());
	}

	private FakeExecutionContext newContext(String... params) {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm("aForm");
		formBuilder.addFib("Name?", 255, 255, 255, 255);
		formBuilder.addFib("Email Alias", "Alias", 50);
		Project project = builder.build();
		userProject = new UserProject(project, user, "Project One");
		userProject = WorldInitializer.getDefaultWorld().domain().projects()
				.put(userProject);
		context = new FakeExecutionContext(userProject, project.defaultForm(),
				params);
		return context;
	}

	private FakeSmtpMessage execute(Send command) throws InterruptedException {
		command.execute(context);
		FakeSmtpMessage message = exactlyOneMessage();
		return message;
	}

	private FakeSmtpMessage exactlyOneMessage() throws InterruptedException {
		server.waitForAllConnectionsToClose();
		assertEquals(1, server.getMessageCount());
		return server.getMessage(0);
	}

	public void testSendWithinForEachLoop() throws InterruptedException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder doc1 = builder.addDocument("Document 1");
		doc1.addText("one");

		ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
		proc1.addGet("records", new String[] { "Form 1" });

		ForEachBuilder forEachBuilder = proc1.addForEach("record", "records");

		forEachBuilder.addSend("record:Form 1:email", "",
				"Document with appended text", doc1, true, true);

		builder.addForm("Form 1", proc1).addFib("Email:").addBlank("email");

		Project project = builder.build();
		Domain domain = WorldInitializer.getDefaultWorld().domain();

		UserProject userProject = new UserProject(project, user, "NormalAppend");
		domain.projects().put(userProject);

		record(domain, userProject, userProject.getProject().getForm("Form 1"),
				"email", "test1@example.com");
		record(domain, userProject, userProject.getProject().getForm("Form 1"),
				"email", "test2@example.com");

		FakeExecutionContext context = new FakeExecutionContext(userProject,
				project.defaultForm());

		Process process = project.getProcess(proc1.getName());
		process.execute(context);

		server.waitForAllConnectionsToClose();

		assertEquals(2, server.getMessageCount());

		FakeSmtpMessage message = server.getMessage(0);
		assertMatches("<div class=\"document\">.*one.*</div>\n", message
				.getBody());

		assertEquals("test1@example.com", message.getRecipients().get(0));

		message = server.getMessage(1);
		assertMatches("<div class=\"document\">.*one.*</div>\n", message
				.getBody());
		assertEquals("test2@example.com", message.getRecipients().get(0));
	}

	private void record(Domain domain, UserProject project, Form form,
			String... data) {
		domain.storedData().record(
				new FakeExecutionContext(project, form, new FakeRequest(true,
						data)).getSubmission());
	}

	public void testSendFromLiteralAddress() throws InterruptedException {
		context = newContext("Q1:a", "joe@example.org");
		context.getProject().add(new Document("doc1", "doc text"));
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressField=\"aForm:Q1:a\" />\n"
				+ "    <from addressLiteral=\"test@example.com\" />\n"
				+ "    <subject>subject</subject>\n" + "    <body>foo</body>\n"
				+ "</send>\n"));

		FakeSmtpMessage message = execute(command);

		assertEquals(Arrays.asList("joe@example.org"), message.getRecipients());
		assertEquals("joe@example.org", message.getHeader("To"));
//		assertEquals("test@example.com", message.getSender());

	}

	public void testSendFromFieldReferenceAddress() throws InterruptedException {
		context = newContext("Q1:a", "joe@example.org");
		context.getProject().add(new Document("doc1", "doc text"));
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressField=\"aForm:Q1:a\" />\n"
				+ "    <from addressField=\"aForm:Q1:a\" />\n"
				+ "    <subject>subject</subject>\n" + "    <body>foo</body>\n"
				+ "</send>\n"));

		FakeSmtpMessage message = execute(command);

		assertEquals(Arrays.asList("joe@example.org"), message.getRecipients());
//		assertEquals("joe@example.org", message.getHeader("To"));
//		assertEquals("joe@example.org", message.getSender());
	}

	public void testSendFromProjectOwnerAddress() throws InterruptedException {
		context = newContext("Q1:a", "joe@example.org");
		context.getProject().add(new Document("doc1", "doc text"));
		Send command = new Send(parseConfig("<send>\n"
				+ "    <to addressField=\"aForm:Q1:a\" />\n"
				+ "    <from addressProjectOwner=\"\" />\n"
				+ "    <subject>subject</subject>\n" + "    <body>foo</body>\n"
				+ "</send>\n"));

		FakeSmtpMessage message = execute(command);

		assertEquals(Arrays.asList("joe@example.org"), message.getRecipients());
		assertEquals("joe@example.org", message.getHeader("To"));
//		assertEquals(user.getEmail().toString(), message.getSender());
	}

	public void testSendFromLiteralAddressWithLiteralAlias()
			throws InterruptedException {
		context = newContext("Q1:a", "joe@example.org");
		context.getProject().add(new Document("doc1", "doc text"));
		Send command = new Send(
				parseConfig("<send>\n"
						+ "    <to addressField=\"aForm:Q1:a\" />\n"
						+ "    <from addressLiteral=\"test@example.com\" aliasLiteral=\"Customer Service\"/>\n"
						+ "    <subject>subject</subject>\n"
						+ "    <body>foo</body>\n" + "</send>\n"));

		FakeSmtpMessage message = execute(command);

		assertEquals(Arrays.asList("joe@example.org"), message.getRecipients());
		assertEquals("joe@example.org", message.getHeader("To"));
//		assertEquals("Customer Service <test@example.com>", message
//				.getHeader("From"));
	}

	public void testSendFromLiteralAddressWithFieldReferenceAlias()
			throws InterruptedException {
		context = newContext("Q1:a", "joe@example.org", "Alias",
				"Customer Service");
		context.getProject().add(new Document("doc1", "doc text"));
		Send command = new Send(
				parseConfig("<send>\n"
						+ "    <to addressField=\"aForm:Q1:a\" />\n"
						+ "    <from addressLiteral=\"test@example.com\" aliasField=\"aForm:Alias\"/>\n"
						+ "    <subject>subject</subject>\n"
						+ "    <body>foo</body>\n" + "</send>\n"));

		FakeSmtpMessage message = execute(command);

		assertEquals(Arrays.asList("joe@example.org"), message.getRecipients());
		assertEquals("joe@example.org", message.getHeader("To"));
//		assertEquals("Customer Service <test@example.com>", message
//				.getHeader("From"));
	}

	public void testSubjectConstructedFromReferences()
			throws InterruptedException {
		context = newContext("Q1:a", "joe@example.org", "Alias",
				"Customer Service");
		context.getProject().add(new Document("doc1", "doc text"));
		Send command = new Send(
				parseConfig("<send>\n"
						+ "    <to addressField=\"aForm:Q1:a\" />\n"
						+ "    <from addressLiteral=\"test@example.com\" aliasField=\"aForm:Alias\"/>\n"
						+ "    <subject>Message from <field name=\"aForm:Alias\" /> to <field name=\"aForm:Q1:a\"/>.</subject>\n"
						+ "    <body>foo</body>\n" + "</send>\n"));

		FakeSmtpMessage message = execute(command);

		assertEquals("Message from Customer Service to joe@example.org.",
				message.getHeader("Subject"));
	}

	public void testSendWithHeaderContainingCustomImage()
			throws InterruptedException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder doc1 = builder.addDocument("Document 1");
		doc1.addText("Body of email");

		ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
		proc1.addSend("Main:Email", "test@example.org",
				"Document with appended text", doc1);

		builder.addForm("Main", proc1);

		PageHeaderBuilder pageHeaderBuilder = builder.addPageHeader();
		pageHeaderBuilder.addText("Header Text");
		ImageInstanceBuilder imageInstanceBuilder = new ImageInstanceBuilder(
				"image1", 40, 60);
		pageHeaderBuilder.setImage(imageInstanceBuilder);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, user,
				"Project With a Header");
		userProject.setUniqueRandomId("12345");
		userProject = WorldInitializer.getDefaultWorld().domain().projects()
				.put(userProject);
		FakeExecutionContext context = new FakeExecutionContext(userProject,
				project.defaultForm());
		context.setValue("Main:Email", "test@tawala.com");

		Process process = project.getProcess(proc1.getName());
		process.execute(context);

		server.waitForAllConnectionsToClose();

		assertEquals(1, server.getMessageCount());

		FakeSmtpMessage message = server.getMessage(0);
		assertMatches(
				"<h1 class=\"pageHeading\" style=\"height: 60px;\"><img src=\""
						+ userProject.getImageUrl(true, userProject
								.getUniqueRandomId(), "image1") + "\".* />"
						+ "<div>Header Text</div>\n</h1>\n"
						+ "<div class=\"document\">.*Body of email.*</div>\n",
				message.getBody());
	}

	public void testSendWithHeaderContainingDefaultThemeImage()
			throws InterruptedException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder doc1 = builder.addDocument("Document 1");
		doc1.addText("Body of email");

		ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
		proc1.addSend("Main:Email", "test@example.org",
				"Document with appended text", doc1);

		builder.addForm("Main", proc1);

		PageHeaderBuilder pageHeaderBuilder = builder.addPageHeader();
		pageHeaderBuilder.addText("Header Text");

		Project project = builder.build();
		project.setThemePath("chocolate");

		UserProject userProject = new UserProject(project, user,
				"Project With a Header");
		userProject.setUniqueRandomId("12345");
		userProject = WorldInitializer.getDefaultWorld().domain().projects()
				.put(userProject);

		FakeExecutionContext context = new FakeExecutionContext(userProject,
				project.defaultForm());
		context.setValue("Main:Email", "test@tawala.com");

		Process process = project.getProcess(proc1.getName());
		process.execute(context);

		server.waitForAllConnectionsToClose();

		assertEquals(1, server.getMessageCount());

		FakeSmtpMessage message = server.getMessage(0);
		assertMatches("<h1 class=\"pageHeading\"><img src=\"" + "http://"
				+ UserProject.getWebsiteHostName() + "/css.* />"
				+ "<div>Header Text</div>\n</h1>\n"
				+ "<div class=\"document\">.*Body of email.*</div>\n", message
				.getBody());
	}

	public void testSendWithHeaderSuppressed() throws InterruptedException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder doc1 = builder.addDocument("Document 1");
		doc1.addText("Body of email");

		ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
		proc1.addSend("Main:Email", "test@example.org",
				"Document with appended text", doc1, false, false);

		builder.addForm("Main", proc1);

		PageHeaderBuilder pageHeaderBuilder = builder.addPageHeader();
		pageHeaderBuilder.addText("Header Text");

		Project project = builder.build();

		UserProject userProject = new UserProject(project, user,
				"Project With a Header Suppressed");
		userProject.setUniqueRandomId("12345");
		userProject.getProject().setThemePath("chocolate");
		userProject = WorldInitializer.getDefaultWorld().domain().projects()
				.put(userProject);

		FakeExecutionContext context = new FakeExecutionContext(userProject,
				project.defaultForm());
		context.setValue("Main:Email", "test@tawala.com");

		Process process = project.getProcess(proc1.getName());
		process.execute(context);

		server.waitForAllConnectionsToClose();

		assertEquals(1, server.getMessageCount());

		FakeSmtpMessage message = server.getMessage(0);
		assertDoesntContain("<h1 class=\"pageHeading\">", message.getBody());
	}
}
