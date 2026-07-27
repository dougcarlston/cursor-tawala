package com.tawala.acceptance;

import static com.tawala.project.builder.ProcessBuilder.OperandType.VALUE;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ConditionsBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.ForEachBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.IfBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class ShowFormTest extends AcceptanceTestCase {

	public void testNoItems() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();

		ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
		ProcessBlockBuilder proc2 = builder.addProcess("Process 2");

		builder.addForm("Form 1", proc1);
		FormBuilder form2 = builder.addForm("Form 2", proc2);
		FormBuilder form3 = builder.addForm("Form 3");

		proc1.addShow(form2);
		proc2.addShow(form3);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner,
				"aProject");
		world.domain().projects().put(userProject);
		bot.go(userProject);
		bot.submit();
		bot.submit();
		bot.submit();
		assertContains("Thank you!", bot.getPageText());
	}

	public void testWithSimpleDataFlow() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder form1 = builder.addForm("form1");
		form1.addFib("First name?", "first", 20);
		FormBuilder form2 = builder.addForm("form2");
		form2.addFib("Last name?", "last", 20);

		DocumentBuilder results = builder.addDocument("results");
		results.addField("form1:first");
		results.addText(" ");
		results.addField("form2:last");

		ProcessBlockBuilder showForm = builder.addProcess("ShowForm");
		showForm.addShow(form2);
		form1.setPostProcess(showForm);
		ProcessBlockBuilder showDoc = builder.addProcess("ShowDoc");
		showDoc.addShow(results);
		form2.setPostProcess(showDoc);

		Project project = builder.build();
		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);
		bot.go(userProject);

		bot.setParameter("first", "Jane");
		bot.submit();
		bot.setParameter("last", "Doe");
		bot.submit();
		assertMatches("Jane.*Doe", bot.getPageText());
	}

	// TODO: test multiple Q1:a in following forms
	// TODO: test stored data structure
	// TODO: test show form stops execution immediately

	public void testVirtualDocPreserved()
			throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder doc1 = builder.addDocument("Document 1");
		DocumentBuilder doc2 = builder.addDocument("Document 2");
		doc1.addField("Form 1:Q1:a");
		doc2.addField("Form 2:Q1:a");

		ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
		ProcessBlockBuilder proc2 = builder.addProcess("Process 2");

		FormBuilder form1 = builder.addForm("Form 1", proc1);
		FormBuilder form2 = builder.addForm("Form 2", proc2);
		form1.addFib("F1-Q1", 10);
		form2.addFib("F2-Q1", 10);

		proc1.addAppend("vDoc1", doc1);
		proc1.addShow(form2);
		proc2.addAppend("vDoc1", doc2);
		proc2.addShow("vDoc1");

		Project project = builder.build();
		UserProject userProject = new UserProject(project, projectOwner,
				"ShowForm");
		world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter("Q1:a", "one");
		bot.submit();
		bot.setParameter("Q1:a", "two");
		bot.submit();
		assertMatches("<div class=\"document\">.*one.*</div>"
				+ NEWLINE + "<div class=\"document\">.*two.*</div>" + NEWLINE
				, bot.getPageText());
	}

	public void testProcessVariablePreserved() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder result = builder.addDocument("result");
		result.addField("vTest1");
		result.addField("vTest2");

		ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
		ProcessBlockBuilder proc2 = builder.addProcess("Process 2");

		builder.addForm("Form 1", proc1);
		FormBuilder form2 = builder.addForm("Form 2", proc2);

		proc1.addSet("vTest1", VALUE, "One");
		proc1.addShow(form2);
		proc2.addSet("vTest2", VALUE, "Two");
		proc2.addShow(result);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);
		bot.go(userProject);
		bot.submit();
		bot.submit();
		assertMatches(
				"<div class=\"document\">.*OneTwo.*</div>" + NEWLINE
				, bot.getPageText());
	}

	public void testShowFormWithIf() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();
		DocumentBuilder doc1 = builder.addDocument("Document 1");
		DocumentBuilder doc2 = builder.addDocument("Document 2");
		DocumentBuilder doc3 = builder.addDocument("Document 3");
		DocumentBuilder doc4 = builder.addDocument("Document 4");
		doc1.addText("One");
		doc2.addText("Two");
		doc3.addText("Three");
		doc4.addText("Four");

		ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
		ProcessBlockBuilder proc2 = builder.addProcess("Process 2");
		ProcessBlockBuilder proc3 = builder.addProcess("Process 3");
		ProcessBlockBuilder proc4 = builder.addProcess("Process 4");

		FormBuilder form1 = builder.addForm("Form 1", proc1);
		FormBuilder form2 = builder.addForm("Form 2", proc2);
		FormBuilder form3 = builder.addForm("Form 3", proc3);
		FormBuilder form4 = builder.addForm("Form 4", proc4);
		form1.addFib("F1-Q1", 10);
		form2.addFib("F2-Q1", 10);

		IfBuilder ifBuilder = proc1.addIf();
		ConditionsBuilder conditions = ifBuilder.conditions();
		conditions.addComparison("contains", "Form 1:Q1:a", "string", "two");
		ProcessBlockBuilder trueSet = (ProcessBlockBuilder) ifBuilder.trueSet();
		trueSet.addShow(form2);

		IfBuilder ifBuilder2 = proc1.addIf();
		ConditionsBuilder conditions2 = ifBuilder2.conditions();
		conditions2.addComparison("contains", "Form 1:Q1:a", "string", "three");
		ProcessBlockBuilder trueSet2 = (ProcessBlockBuilder) ifBuilder2
				.trueSet();
		trueSet2.addShow(form3);

		proc1.addShow(doc1);

		IfBuilder ifBuilder3 = proc2.addIf();
		ConditionsBuilder conditions3 = ifBuilder3.conditions();
		conditions3.addComparison("contains", "Form 2:Q1:a", "string", "four");
		ProcessBlockBuilder trueSet3 = (ProcessBlockBuilder) ifBuilder3
				.trueSet();
		trueSet3.addShow(form4);

		proc2.addShow(doc2);

		proc3.addShow(doc3);
		proc4.addShow(doc4);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner,
				"ShowForm");
		world.domain().projects().put(userProject);
		bot.go(userProject);
		bot.setParameter("Q1:a", "one");
		bot.submit();
		assertMatches("<div class=\"document\">.*One.*</div>"
				+ NEWLINE, bot.getPageText());

		bot.go(userProject);
		bot.setParameter("Q1:a", "two");
		bot.submit();
		bot.submit();
		assertMatches("<div class=\"document\">.*Two.*</div>"
				+ NEWLINE, bot.getPageText());

		bot.go(userProject);
		bot.setParameter("Q1:a", "three");
		bot.submit();
		bot.submit();
		assertMatches(
				"<div class=\"document\">.*Three.*</div>" + NEWLINE
				, bot.getPageText());

		bot.go(userProject);
		bot.setParameter("Q1:a", "two");
		bot.submit();
		bot.setParameter("Q1:a", "four");
		bot.submit();
		bot.submit();
		assertMatches(
				"<div class=\"document\">.*Four.*</div>" + NEWLINE
				, bot.getPageText());
	}

	public void testForEach() throws RobotException {

		ProjectBuilder builder = new ProjectBuilder();

		FormBuilder form1 = builder.addForm("Form 1");
		form1.addText("This is Form1");
		form1.addFib("Name:", "Name", 20);

		ProcessBlockBuilder process = builder.addProcess("Process 1");

		process.addGet("Records", "Form 1");
		ForEachBuilder forEach = process.addForEach("record", "Records");
		forEach.addShow(form1);

		FormBuilder form2 = builder.addForm("Form 2", process);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter("Name", "Archie");
		bot.submit();

		bot.go(userProject, form2.getName());
		bot.submit();

		assertContains("This is Form1", bot.getPageText());
	}

	public void testInputFieldsEmptyNavigatingToTheSameForm() throws RobotException {

		ProjectBuilder builder = new ProjectBuilder();

		ProcessBlockBuilder process = builder.addProcess("Process 1");

		FormBuilder form1 = builder.addForm("Form 1", process);
		form1.addText("This is Form 1");
		form1.addFib("Name:", "Name", 20);

		process.addShow(form1);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter("Name", "Archie");
		bot.submit();

		assertContains("This is Form 1", bot.getPageText());
		assertDoesntContain("Archie", bot.getPageText());
	}

	public void testInputFieldsEmptyNavigatingToTheAnotherForm() throws RobotException {

		ProjectBuilder builder = new ProjectBuilder();

		ProcessBlockBuilder process = builder.addProcess("Process 1");

		FormBuilder form1 = builder.addForm("Form 1", process);
		form1.addText("This is Form 1");
		form1.addFib("Name:", "Name", 20);

		FormBuilder form2 = builder.addForm("Form 2", process);
		form2.addText("This is Form 2");
		form2.addFib("Name:", "Name", 20);

		process.addShow(form2);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter("Name", "Archie");
		bot.submit();

		assertContains("This is Form 2", bot.getPageText());
		
		WebForm form = bot.getForm(0);
		assertEquals("", form.getParameterValue("Name"));
	}

	public void testAllInputFieldsEmpty() throws RobotException {

		ProjectBuilder builder = new ProjectBuilder();

		ProcessBlockBuilder process = builder.addProcess("Process 1");

		FormBuilder form1 = builder.addForm("Form 1", process);
		form1.addText("This is Page 1");
		form1.addFib("Name:", "Name", 20);
		form1.addBreak();
		form1.addText("This is Page 2");
		form1.addFib("Your secret number:", "Number", 20);

		process.addShow(form1);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.setParameter("Name", "Archie");
		bot.submit();
		bot.setParameter("Number", "1234567890");
		bot.submit();

		assertContains("This is Page 1", bot.getPageText());
		assertDoesntContain("Archie", bot.getPageText());

		bot.submit();
		assertContains("This is Page 2", bot.getPageText());
		assertDoesntContain("1234567890", bot.getPageText());
	}

	public void testPrependDocument() throws RobotException {

		ProjectBuilder builder = new ProjectBuilder();

		ProcessBlockBuilder process = builder.addProcess("Process 1");

		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addText("This is Document 1");

		FormBuilder form1 = builder.addForm("Form 1", process);
		form1.addText("This is Form 1");

		process.addShow(document1);
		process.addShow(form1);

		Project project = builder.build();

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject);
		bot.submit();

		assertContains("This is Document 1", bot.getPageText());
		assertContains("This is Form 1", bot.getPageText());
	}

}
