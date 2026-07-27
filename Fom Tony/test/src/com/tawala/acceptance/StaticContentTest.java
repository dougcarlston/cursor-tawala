package com.tawala.acceptance;

import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.StringReader;

import org.dom4j.DocumentException;
import org.dom4j.io.SAXReader;
import org.xml.sax.SAXException;

import com.scissor.webrobot.RobotException;
import com.tawala.domain.Status;
import com.tawala.project.Projects;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.controller.WellKnown;

public class StaticContentTest extends AcceptanceTestCase {

	public void testDownload() throws IOException, SAXException, RobotException {
		bot.logOut();
//		checkAccessRejected(WellKnown.urls.getExecutable());

		projectOwner.setStatus(Status.REGISTERED);
		world.domain().users().addOrSave(projectOwner);
		
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getHomePage());
		assertContains("designerLink", bot.getPageText());
		bot.followLink("designerLink");
		checkPageContains("Alpha", "build");
		assertEquals("/Tawala.exe", bot.getLink("downloadLink").getURLString());
		bot.followLink("downloadLink");
		assertEquals("application/octet-stream", bot.getContentType());
		assertTrue(bot.getContentLength() > 500 * 1024);
	}

	public void testManual() throws IOException, SAXException, RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getManual());
		bot.followLink("Manual");
		assertEquals("application/pdf", bot.getContentType());
	}

	public void testBuildInfo() throws RobotException, IOException {
		FileWriter writer = new FileWriter("web/buildinfo.txt");
		writer.write("<buildinfo>For test purposes only.</buildinfo>");
		writer.close();

		bot.go("/buildinfo.txt");
		assertEquals("text/plain", bot.getContentType());
		assertContains("buildinfo", bot.getPageText());
	}

	public void testClientInfo() throws RobotException {
		bot.go("/clientinfo");
		assertEquals("application/xml", bot.getContentType());
		assertContains("<clientInfo>", bot.getPageText());
	}

	public void testProjectThemeInfo() throws RobotException, DocumentException {
		bot.go("/projectThemes");
		assertEquals("application/xml", bot.getContentType());
		assertContains("<projectThemes path=\"css/project\">", bot
				.getPageText());
    	assertEquals("application/xml", bot.getContentType());
    	SAXReader saxReader = new SAXReader();
		saxReader.setMergeAdjacentText(true);
		saxReader.read(new StringReader(bot.getPageText()));

	}

	public void testStaticContent() throws RobotException {
		bot.go("/images/template/tawala-logo.gif");
		assertEquals(bot.getContentType(), "image/gif");
		assertTrue(bot.getContentLength() > 100);

		bot.logInAs(projectOwner.getId(), projectOwner.getPassword());
		bot.go("/images/template/tawala-logo.gif");
		assertEquals(bot.getContentType(), "image/gif");
		assertTrue(bot.getContentLength() > 100);
	}

	public void testExternalContentOverridesDynamicFiles()
			throws RobotException, IOException {
		bot.go("/");
		assertContains("Tawala", bot.getPageText());

		File contentDir = createTestDirectory();
		world.addContentRoot(contentDir.getAbsolutePath());

		bot.go("/");
		assertContains("Tawala", bot.getPageText());

		world.removeContentRoot(contentDir.getAbsolutePath());
	}

	public void testUserInfo() throws RobotException, IOException {
		world.domain().projects().deleteAllProjectsForUser(world, projectOwner);

		bot.logInAs(projectOwner.getId(), projectOwner.getPassword());
		bot.go(WellKnown.urls.getHomePage());
		assertContains("You have no projects deployed", bot.getPageText());

		Projects projects = world.domain().projects();
		projects.put(new UserProject(ProjectBuilder.buildMinimalisticProject(),
				projectOwner, "proj1"));
		bot.go(WellKnown.urls.getHomePage());
		assertContains("You have 1 project deployed", bot.getPageText());

		projects.put(new UserProject(ProjectBuilder.buildMinimalisticProject(),
				projectOwner, "proj2"));
		bot.go(WellKnown.urls.getHomePage());
		assertContains("You have 2 projects deployed", bot.getPageText());

		projects.put(new UserProject(ProjectBuilder.buildMinimalisticProject(),
				projectOwner, "proj3"));
		bot.go(WellKnown.urls.getHomePage());
		assertContains("You have 3 projects deployed", bot.getPageText());
	}
}
