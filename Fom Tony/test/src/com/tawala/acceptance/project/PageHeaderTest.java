package com.tawala.acceptance.project;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.ImageInstance;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ImageInstanceBuilder;
import com.tawala.project.builder.PageHeaderBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;

public class PageHeaderTest extends AcceptanceTestCase {

	public void testDisplayOfHeaderUnderDifferentFlows() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainFormBuilder = builder.addForm("main");
		FibBuilder fibBuilder = new FibBuilder("ABC");
		fibBuilder.addBlank("a", "email", 20, true);
		mainFormBuilder.add(fibBuilder);

		PageHeaderBuilder pageHeaderBuilder = builder.addPageHeader();
		pageHeaderBuilder.addText("Hello World");

		Project project1 = builder.build();
		Project project = project1;
		UserProject userProject = new UserProject(project, projectOwner, "test");

		userProject = world.domain().projects().put(userProject);

		// --- First access to the project
		bot.go(userProject);
		String expectedHeading = "<h1 class=\"pageHeading\"><div>Hello World</div>\n</h1>";
		assertContains(expectedHeading, bot.getPageText());

		// --- Test of missing parameter
		bot.submit();
		assertContains(expectedHeading, bot.getPageText());

		// --- Test of final display of an empty document
		bot.setParameter("email", "joe@example.org");
		bot.submit();
		assertContains(expectedHeading, bot.getPageText());
	}

	public void testDisplayOfImagesWithText() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainFormBuilder = builder.addForm("main");
		FibBuilder fibBuilder = new FibBuilder("ABC");
		fibBuilder.addBlank("a", "email", 20, true);
		mainFormBuilder.add(fibBuilder);

		PageHeaderBuilder pageHeaderBuilder = builder.addPageHeader();
		pageHeaderBuilder.addText("Hello World");
		ImageInstanceBuilder imageInstanceBuilder = new ImageInstanceBuilder(
				"image1", 40, 60);
		pageHeaderBuilder.setImage(imageInstanceBuilder);

		Project project1 = builder.build();
		Project project = project1;
		UserProject userProject = new UserProject(project, projectOwner, "test");

		userProject = world.domain().projects().put(userProject);

		bot.go(userProject);
		String expectedHeading = "<h1 class=\"pageHeading\" style=\"height: 60px;\">"
				+ "<img src=\""
				+ userProject.getImageUrl(false, userProject
						.getUniqueRandomId(), "image1") + "\" alt=\""
				+ ImageInstance.DEFAULT_IMAGE_ALT_NAME
				+ "\" width=\"40px\" height=\"60px\" />"
				+ "<div>Hello World</div>\n" + "</h1>";
		assertContains(expectedHeading, bot.getPageText());
	}

	public void testDisplayOfImagesWithoutText() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainFormBuilder = builder.addForm("main");
		FibBuilder fibBuilder = new FibBuilder("ABC");
		fibBuilder.addBlank("a", "email", 20, true);
		mainFormBuilder.add(fibBuilder);

		PageHeaderBuilder pageHeaderBuilder = builder.addPageHeader();
		ImageInstanceBuilder imageInstanceBuilder = new ImageInstanceBuilder(
				"image1", 40, 60);
		pageHeaderBuilder.setImage(imageInstanceBuilder);

		Project project1 = builder.build();
		Project project = project1;
		UserProject userProject = new UserProject(project, projectOwner, "test");

		userProject = world.domain().projects().put(userProject);

		bot.go(userProject);
		String expectedHeading = "<h1 class=\"pageHeading\" style=\"height: 60px;\">"
				+ "<img src=\""
				+ userProject.getImageUrl(false, userProject
						.getUniqueRandomId(), "image1") + "\" alt=\""
				+ ImageInstance.DEFAULT_IMAGE_ALT_NAME
				+ "\" width=\"40px\" height=\"60px\" />" + "</h1>";
		assertContains(expectedHeading, bot.getPageText());
	}

	public void testFieldReferences() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainFormBuilder = builder.addForm("main");
		FibBuilder fibBuilder = new FibBuilder("ABC");
		fibBuilder.addBlank("a", "email", 20, true);
		mainFormBuilder.add(fibBuilder);

		ProcessBlockBuilder preprocess = builder.addProcess("preprocess");
		preprocess.addSet("MyVar", OperandType.VALUE, "John");
		mainFormBuilder.setPreProcess(preprocess);

		PageHeaderBuilder pageHeaderBuilder = builder.addPageHeader();
		pageHeaderBuilder.addText("Hello ");
		pageHeaderBuilder.addField("MyVar");
		pageHeaderBuilder.addText("!");

		Project project1 = builder.build();
		Project project = project1;
		UserProject userProject = new UserProject(project, projectOwner, "test");

		userProject = world.domain().projects().put(userProject);

		bot.go(userProject);
		String expectedHeading = "<h1 class=\"pageHeading\"><div>Hello John!</div>\n</h1>";
		assertContains(expectedHeading, bot.getPageText());
	}

}
