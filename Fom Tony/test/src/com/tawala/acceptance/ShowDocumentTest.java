package com.tawala.acceptance;

import static com.tawala.project.builder.ProcessBuilder.OperandType.FIELD;

import com.scissor.webrobot.RobotException;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.ForEachBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.WorldInitializer;

public class ShowDocumentTest extends AcceptanceTestCase {

	public void testStyleSheet() throws RobotException {

		ProjectBuilder builder = new ProjectBuilder();

		DocumentBuilder document1 = builder.addDocument("Document 1");

		ProcessBlockBuilder process = builder.addProcess("Process 1");
		process.addShow(document1);

		builder.addForm("Form 1", process);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		assertContains("<link rel=\"stylesheet\" href=\"/css/project/"
				+ "default/project" + ".css?x="
				+ WorldInitializer.getDefaultWorld().getBuildNumber()
				+ "\" type=\"text/css\" media=\"screen\" />", bot.getPageText());
	}

	public void testShowMultipleDocuments() throws RobotException {

		ProjectBuilder builder = new ProjectBuilder();

		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addText("Line 1");

		ProcessBlockBuilder process = builder.addProcess("Process 1");
		process.addShow(document1);
		process.addShow(document1);

		builder.addForm("Form 1", process);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		/*
		 * String expectedDocumentOutput = "<div class=\"document\">" + "<div><span
		 * style=\"font-family: null; font-size: 10pt; color: #000000;\">" +
		 * "Line 1</span>" + "</div></div>";
		 */
		String expectedDocumentOutput = "<div class=\"document\">"
				+ "<div><span style=\"\">" + "Line 1</span>" + "</div></div>";

		assertContains(expectedDocumentOutput + NEWLINE
				+ expectedDocumentOutput, bot.getPageText());
	}

	public void testForEach() throws RobotException {

		ProjectBuilder builder = new ProjectBuilder();

		FormBuilder form1 = builder.addForm("Form 1");
		form1.addFib("Name:", "Name", 20);

		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addField("Name");

		ProcessBlockBuilder process = builder.addProcess("Process 1");

		process.addGet("Records", "Form 1");
		ForEachBuilder forEach = process.addForEach("record", "Records");
		forEach.addSet("Name", FIELD, "record:Form 1:Name");
		forEach.addShow(document1);

		FormBuilder form2 = builder.addForm("Form 2", process);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter("Name", "Archie");
		bot.submit();

		bot.go(userProject);
		bot.setParameter("Name", "Betty");
		bot.submit();

		bot.go(userProject);
		bot.setParameter("Name", "Jughead");
		bot.submit();

		bot.go(userProject, form2.getName());
		bot.submit();

		assertContains("Archie", bot.getPageText());
		assertContains("Betty", bot.getPageText());
		assertContains("Jughead", bot.getPageText());
	}

	public void testQualifiedField() throws RobotException {

		ProjectBuilder builder = new ProjectBuilder();
		ProcessBlockBuilder process = builder.addProcess("Process 1");

		FormBuilder form1 = builder.addForm("Form 1", process);
		form1.addFibNoBlankAlt("Name:", 10, false);

		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addField("Form 1:Q1:a");

		process.addShow(document1);

		Project project = builder.build();
		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter("Q1:a", "Archie");
		bot.submit();

		assertContains("Archie", bot.getPageText());
	}

	public void COMMENTED_OUT_BY_SERGEI_testSharedDocument()
			throws RobotException {

		ProjectBuilder builder = new ProjectBuilder();

		ProcessBlockBuilder process1 = builder.addProcess("Process 1");
		ProcessBlockBuilder process2 = builder.addProcess("Process 2");

		FormBuilder form1 = builder.addForm("Form 1", process1);
		form1.addFib("Name:", "Name", 20);

		FormBuilder form2 = builder.addForm("Form 2", process2);
		form2.addFib("Name:", "Name", 20);

		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addField("Form 1:Name");

		process1.addShow(document1);
		process2.addShow(document1);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject, form1.getName());
		bot.setParameter("Name", "Archie");
		bot.submit();

		assertContains("Archie", bot.getPageText());

		bot.go(userProject, form2.getName());
		bot.setParameter("Name", "Betty");
		bot.submit();

		assertContains("Betty", bot.getPageText());
	}
}
