package com.tawala.acceptance.project;

import org.springframework.mail.javamail.JavaMailSenderImpl;

import com.meterware.httpunit.WebForm;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.email.Emailer;
import com.tawala.project.Image;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ImageBuilder;
import com.tawala.project.builder.ImageInstanceBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.WorldInitializer;
import com.tawala.web.project.ProjectController;
import com.thoughtworks.xstream.core.util.Base64Encoder;

import fake.smtp.FakeSmtpMessage;
import fake.smtp.FakeSmtpServer;

public class SendDocumentTest extends AcceptanceTestCase {
	private FakeSmtpServer server;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		server = new FakeSmtpServer();

		JavaMailSenderImpl senderImpl = new JavaMailSenderImpl();
		senderImpl.setPort(server.getPort());
		senderImpl.setHost("127.0.0.1");
		new Emailer().setSender(senderImpl);
	}

	@Override
	protected void tearDown() throws Exception {
		if (server != null)
			server.shutDown();
		super.tearDown();
	}

	public void testIncludedStyleSheet() throws Exception {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm("main", true);
		FibBuilder fibBuilder = formBuilder.addFib("Hello");
		fibBuilder.addBlank("emailAddress");

		ProcessBlockBuilder processBuilder = builder.addProcess("process");
		formBuilder.setPostProcess(processBuilder);

		DocumentBuilder documentBuilder = builder
				.addDocument("doc", "Welcome!");
		processBuilder.addSend("main:emailAddress", "", "This is the subject",
				documentBuilder);

		Project project = builder.build();
		UserProject userProject = new UserProject(project, projectOwner,
				"SendTest");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		bot.go(userProject);
		WebForm form = bot.getForm(0);
		form.setParameter("emailAddress", "test@example.com");
		bot.submit(form);

		server.waitForAllConnectionsToClose();

		assertEquals(1, server.getMessageCount());

		FakeSmtpMessage message = server.getMessage(0);
		assertContains("<style type=\"text/css\"><!--\n", message.getBody());
	}

	public void testImageURLIsFullyQualified() throws Exception {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm("main", true);
		FibBuilder fibBuilder = formBuilder.addFib("Hello");
		fibBuilder.addBlank("emailAddress");

		//-- Add two images
		byte[] imageData = new byte[] { 01, 34, 90, 23, 00, 00, 12, 45 };

		ImageBuilder imageBuilder = new ImageBuilder();
		imageBuilder.addImage("image1", new ImageBuilder.ImageData(
				Image.Data.Format.PNG, new Base64Encoder().encode(imageData)));
		builder.add(imageBuilder);

		imageBuilder = new ImageBuilder();
		imageBuilder.addImage("image2", new ImageBuilder.ImageData(
				Image.Data.Format.PNG, new Base64Encoder().encode(imageData)));
		builder.add(imageBuilder);

		
		// -- first document
		DocumentBuilder mainDocument = builder
				.addDocument("doc", "Welcome!");
		mainDocument.addImage(new ImageInstanceBuilder("image1", 40, 60));

		
		//--- appended document
		DocumentBuilder appendedDocument = builder.addDocument("appended document", "This is the appendage:");
		appendedDocument.addImage(new ImageInstanceBuilder("image2", 20, 40));

		ProcessBlockBuilder processBuilder = builder.addProcess("process");
		formBuilder.setPostProcess(processBuilder);


		processBuilder.addAppend(mainDocument, appendedDocument);
		processBuilder.addSend("main:emailAddress", "", "This is the subject",
				mainDocument);

		Project project = builder.build();
		UserProject userProject = new UserProject(project, projectOwner,
				"SendTest");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		bot.go(userProject);
		WebForm form = bot.getForm(0);
		form.setParameter("emailAddress", "test@example.com");
		bot.submit(form);

		server.waitForAllConnectionsToClose();

		assertEquals(1, server.getMessageCount());

		FakeSmtpMessage message = server.getMessage(0);

		assertContains(
				"<img src=\"http://"
						+ UserProject.getWebsiteHostName()
						+ "/p/" + userProject.getUniqueRandomId() + "/"
						+ ProjectController.IMAGE_PATH
						+ "image1" + Project.IMAGE_SUFFIX +
								"\" alt=\"Project Image\" width=\"40px\" height=\"60px\" />",
				message.getBody());

		assertContains(
				"<img src=\"http://"
						+ UserProject.getWebsiteHostName()
						+ "/p/" + userProject.getUniqueRandomId() + "/"
						+ ProjectController.IMAGE_PATH
						+ "image2" + Project.IMAGE_SUFFIX +
								"\" alt=\"Project Image\" width=\"20px\" height=\"40px\" />",
				message.getBody());
	}
}
