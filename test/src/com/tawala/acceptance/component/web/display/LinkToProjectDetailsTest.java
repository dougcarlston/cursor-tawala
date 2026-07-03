package com.tawala.acceptance.component.web.display;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.springframework.mail.javamail.JavaMailSenderImpl;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.display.LinkToProjectDetails;
import com.tawala.email.Emailer;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;

import fake.smtp.FakeSmtpMessage;
import fake.smtp.FakeSmtpServer;

public class LinkToProjectDetailsTest extends AcceptanceTestCase {
	private static final String PROJECT_NAME = "Project to Test Links to Project Details";
	private static final String MAIN_FORM = "Main Form of the Project";
	private UserProject userProject;
	private FakeSmtpServer smtpServer;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		Project project = buildProject();

		userProject = new UserProject(project, projectOwner, PROJECT_NAME);
		world.domain().projects().put(userProject);
		
		smtpServer = new FakeSmtpServer();
		
		JavaMailSenderImpl senderImpl = new JavaMailSenderImpl();
		senderImpl.setPort(smtpServer.getPort());
		senderImpl.setHost("127.0.0.1");
		new Emailer().setSender(senderImpl);

	}
	
	@Override
	protected void tearDown() throws Exception {
		smtpServer.shutDown();
		super.tearDown();
	}

	private Project buildProject() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);

		ComponentBuilder linkBuilder = new ComponentBuilder(
				new LinkToProjectDetails());
		linkBuilder.addPreformattedParameter(
				LinkToProjectDetails.LINK_DESCRIPTION,
				"<string value=\"Back to My Tawala.\" />");
		linkBuilder.addPreformattedParameter(
				LinkToProjectDetails.OPEN_PREFERENCE,
				LinkToProjectDetails.OPEN_IN_CURRENT_WINDOW_OPTION);
		
		DocumentBuilder documentBuilder = projectBuilder.addDocument("main");
		documentBuilder.addComponent(linkBuilder);

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("Main Pre Process");
		processBlockBuilder.addShow(documentBuilder);
		processBlockBuilder.addSet("email", OperandType.VALUE, "test@example.com");
		processBlockBuilder.addSend("email", "", "Test of the link in an email", documentBuilder);

		formBuilder.setPreProcess(processBlockBuilder);

		return projectBuilder.build();
	}

	public void testDisplay() throws RobotException, InterruptedException {
		bot.go(userProject, MAIN_FORM);

		Pattern pattern = Pattern
				.compile("<a href=\"([^\"]+)\".*>Back to My Tawala.</a>", Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());
		String url = matcher.group(1);

		bot.logInAs(projectOwner);
		bot.go(url);

		assertContains("<h2>" + PROJECT_NAME + "</h2>", bot.getPageText());
		
		smtpServer.waitForAllConnectionsToClose();
		
		FakeSmtpMessage message = smtpServer.getMessage(0);
		matcher = pattern.matcher(message.getBody());
		assertTrue(matcher.find());
		url = matcher.group(1);
		
		assertEquals("http", url.substring(0,4));

	}
}
