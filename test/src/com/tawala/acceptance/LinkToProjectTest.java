package com.tawala.acceptance;

import com.scissor.webrobot.RobotException;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;
import com.tawala.web.WorldInitializer;
import com.tawala.web.oldhtml.Link;

public class LinkToProjectTest extends AcceptanceTestCase {
	public void testNewStyleInvitations() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addNewStyleInvitation("<string value=\"Link Description - \"/><string field=\"var1\" />", "Form1");

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addSet("var1", OperandType.VALUE, "Variable Text");
		processBlockBuilder.addShow(documentBuilder);

		projectBuilder.addForm("Form1", processBlockBuilder);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		assertMatches("<a href=\""
				+ userProject.getUrlToForm(
						UserProject.EntryPointType.REAL_PROJECT, userProject
								.getProject().getForm("Form1")) + "\""
				+ Link.ON_CLICK_HANDLER + ">Link Description - Variable Text</a>", bot
				.getPageText());
	}

	public void testSuccessfulLinkToExistingProject() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addOldStyleInvitation("Link Description", "Form1");

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addShow(documentBuilder);

		projectBuilder.addForm("Form1", processBlockBuilder);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		assertMatches("<a href=\""
				+ userProject.getUrlToForm(
						UserProject.EntryPointType.REAL_PROJECT, userProject
								.getProject().getForm("Form1")) + "\""
				+ Link.ON_CLICK_HANDLER + ">Link Description</a>", bot
				.getPageText());
	}

	public void testSuccessfulLinkToAnotherProject() throws RobotException {
		UserProject other = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), projectOwner, "another project");
		WorldInitializer.getDefaultWorld().domain().projects().put(other);

		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addOldStyleInvitation("Link Description", other.getName(),
				other.getProject().defaultForm().getName());

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addShow(documentBuilder);

		projectBuilder.addForm("Form1", processBlockBuilder);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		assertMatches("<a href=\""
				+ other.getUrlToForm(UserProject.EntryPointType.REAL_PROJECT,
						other.getProject().defaultForm()) + "\""
				+ Link.ON_CLICK_HANDLER + ">Link Description</a>", bot
				.getPageText());
	}

	public void testUnsuccessfulLinkToExistingProject() throws RobotException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("document");
		documentBuilder.addOldStyleInvitation("Link Description", "Non-existent form");

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("process");
		processBlockBuilder.addShow(documentBuilder);

		projectBuilder.addForm("Form1", processBlockBuilder);

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of links");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		assertMatches("<a href=\"javascript:alert\\('.*'\\)\""
				+ Link.ON_CLICK_HANDLER + ">Link Description</a>", bot
				.getPageText());
	}
}
