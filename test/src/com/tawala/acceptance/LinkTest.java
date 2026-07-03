package com.tawala.acceptance;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.project.FormSegment;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ConditionsBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.WorldInitializer;
import com.tawala.web.oldhtml.Link;

public class LinkTest extends AcceptanceTestCase {

	public void testLinkToTheCurrentWindow() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addLink("Google", "http://www.google.com", false);

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addShow(documentBuilder);

		projectBuilder.addForm("Form1", processBlockBuilder);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		assertContains("<a href=\"http://www.google.com\"" + Link.ON_CLICK_HANDLER +
				">Google</a>", bot
				.getPageText());
	}

	public void testLinkToTheNewWindow() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addLink("Google", "http://www.google.com", true);

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addShow(documentBuilder);

		projectBuilder.addForm("Form1", processBlockBuilder);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		assertContains(
				"<a href=\"http://www.google.com\" target=\"_blank\">Google</a>",
				bot.getPageText());
	}

	public void testHtmlEscapingOfDescription() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addLink("<Google>", "http://www.google.com", false);

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addShow(documentBuilder);

		projectBuilder.addForm("Form1", processBlockBuilder);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		assertContains("<a href=\"http://www.google.com\"" + Link.ON_CLICK_HANDLER +
				">&lt;Google&gt;</a>",
				bot.getPageText());
	}

	public void testHtmlEscapingOfURL() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addLink("Google", "\"http://www.google.com\"", false);

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addShow(documentBuilder);

		projectBuilder.addForm("Form1", processBlockBuilder);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		assertContains(
				"<a href=\"&quot;http://www.google.com&quot;\"" + Link.ON_CLICK_HANDLER +
				">Google</a>", bot
						.getPageText());
	}

	public void testDisplayingIcons() throws RobotException {
		doTestDisplayingIcon("file.pdf", "page_white_acrobat.png");
		doTestDisplayingIcon("file.doc", "page_white_word.png");
		doTestDisplayingIcon("noextension", "page_white.png");
		doTestDisplayingIcon("file.unknown", "page_white.png");
	}

	public void testDisplayingURLAsDescription() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addLink("", "http://www.google.com", false);

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addShow(documentBuilder);

		projectBuilder.addForm("Form1", processBlockBuilder);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		userProject = WorldInitializer.getDefaultWorld().domain().projects()
				.put(userProject);

		bot.go(userProject);
		bot.submit();

		assertContains(
				"<a href=\"http://www.google.com\"" + Link.ON_CLICK_HANDLER +
				">http://www.google.com</a>", bot
						.getPageText());
	}

	private void doTestDisplayingIcon(String fileName,
			String expectedIconFileName) throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addLink("", "http://www.google.com/" + fileName, false);

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addShow(documentBuilder);

		projectBuilder.addForm("Form1", processBlockBuilder);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		userProject = WorldInitializer.getDefaultWorld().domain().projects()
				.put(userProject);

		bot.go(userProject);
		bot.submit();

		String regex = "<img src=\"(/images/silk/" + expectedIconFileName
				+ ")\" alt=\"[^\"]*\" width=\"16px\" height=\"16px\" />";

		Pattern pattern = Pattern.compile(regex);
		Matcher matcher = pattern.matcher(bot.getPageText());

		assertTrue("Looking for " + expectedIconFileName, matcher.find());

		// --- Retrieve the image
		String imageUrl = matcher.group(1);
		bot.go(imageUrl);
		assertEquals("Checking icon url: " + imageUrl, "image/png", bot
				.getContentType());
	}

	public void testConditionalDisplay() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		ConditionsBuilder displayConditions = new ConditionsBuilder();
		displayConditions.addComparison("equals", "Form1:ShowLink", "string",
				"Yes");

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addLink("Google", "http://www.google.com", false,
				displayConditions);

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addShow(documentBuilder);

		FormBuilder mainForm = projectBuilder.addForm("Form1",
				processBlockBuilder);
		mainForm.addFib("Show the link?", "Show Link FIB", "ShowLink", 20);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		// --- Condition satisfied
		bot.go(userProject);
		WebForm form = bot.getForm(FormSegment.TAWALA_PROJECT_FORM_NAME);
		form.setParameter("ShowLink", "Yes");
		bot.submit();

		String expectedLink = "<a href=\"http://www.google.com\"" + Link.ON_CLICK_HANDLER +
				">Google</a>";
		assertContains(expectedLink, bot.getPageText());

		// --- Condition not satisfied
		bot.go(userProject);
		form = bot.getForm(FormSegment.TAWALA_PROJECT_FORM_NAME);
		form.setParameter("ShowLink", "No");
		bot.submit();

		assertDoesntContain(expectedLink, bot.getPageText());

	}
}
