package com.tawala.acceptance;

import java.util.List;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.SkipBlockBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.SkipBlock;

public class PreProcessTest extends AcceptanceTestCase {
	private UserProject userProject;

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		ProjectBuilder builder = new ProjectBuilder();

		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addText("Line 1");

		ProcessBlockBuilder process = builder.addProcess("Process 1");
		process.addShow(document1);
		process.addSet("MyVar", OperandType.VALUE, "55");

		FormBuilder form = builder.addForm("Form 1");
		form.addText("First page, should have a document prepended.");
		form.addBreak();
		form.addTextWithFields("Second page - displaying myvar: ", "<<MyVar>>");

		form.setPreProcess(process);

		Project project = builder.build();

		userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);
	}

	public void testShowDocumentFromPreProcess() throws RobotException {
		bot.go(userProject);

		assertContains("Line 1", bot.getPageText());
	}

	public void testVariableSurvivesPageSubmissions() throws RobotException {
		// --- First page
		bot.go(userProject);
		// --- Second page
		bot.submit();

		assertContains("Second page - displaying myvar: 55", bot.getPageText());
	}

	public void testPreProcessWithSkipToTheEndOfForm() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();

		FormBuilder firstForm = builder.addForm("Form 1");
		firstForm.addFib("Your name?", "name", 22);

		FormBuilder secondForm = builder.addForm("SecondForm");
		SkipBlockBuilder skipBlockBuilder = secondForm.addSkip();
		skipBlockBuilder.addIfSkip("Form 1:name", "equals", "value", "Joe",
				SkipBlock.SKIP_TO_END, null);
		secondForm.addText("Second form, first page");

		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addText("Document contents");

		ProcessBlockBuilder firstFormPostProcess = builder
				.addProcess("First Form Post Process");
		firstFormPostProcess.addShow(secondForm);
		firstForm.setPostProcess(firstFormPostProcess);

		ProcessBlockBuilder secondFormPreprocess = builder
				.addProcess("Process 1");
		secondFormPreprocess.addShow(document1);
		secondForm.setPreProcess(secondFormPreprocess);

		Project project = builder.build();

		userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		// --- First pass - skip doesn't trigger.
		bot.go(userProject);

		WebForm form = bot.getForm(0);
		form.setParameter("name", "Jack");
		bot.submit(form);

		assertContains("Second form, first page", bot.getPageText());
		assertContains("Document contents", bot.getPageText());

		// --- Second pass - skip does trigger.
		bot.go(userProject);

		form = bot.getForm(0);
		form.setParameter("name", "Joe");
		bot.submit(form);

		assertContains("Document contents", bot.getPageText());
	}

	public void testPreProcessAndPostProcessWithSkippedForm()
			throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();

		FormBuilder form = builder.addForm("Form 1");
		SkipBlockBuilder skipBlockBuilder = form.addSkip();
		skipBlockBuilder.addSkip(SkipBlock.SKIP_TO_END);

		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addText("Document1");

		DocumentBuilder document2 = builder.addDocument("Document 2");
		document2.addText("Document2");

		ProcessBlockBuilder preprocess = builder.addProcess("Process 1");
		preprocess.addShow(document1);
		form.setPreProcess(preprocess);

		ProcessBlockBuilder postprocess = builder
				.addProcess("First Form Post Process");
		postprocess.addShow(document2);
		form.setPostProcess(postprocess);

		Project project = builder.build();

		userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		bot.go(userProject);
		assertMatches("Document1.*Document2", bot.getPageText());
	}

	public void testNoCycleWhenUsingShowFormToTheSameFormInPreProcess() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();

		FormBuilder form = builder.addForm("Form 1");
		form.addText("First line of form");
		
		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addText("Final value: ");
		document1.addField("value");

		ProcessBlockBuilder preprocess = builder.addProcess("Process 1");
		preprocess.addAddTo("value", OperandType.VALUE, "1");
		preprocess.addShow(document1);
		preprocess.addShow(form);
		
		form.setPreProcess(preprocess);

		Project project = builder.build();

		userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		bot.go(userProject);
		assertContains("Final value: 1", bot.getPageText());
		assertDoesntContain("Final value: 2", bot.getPageText());
	}

	public void testShowAnotherFormInPreProcess() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();

		FormBuilder firstForm = builder.addForm("Form 1");

		FormBuilder secondForm = builder.addForm("SecondForm");
		secondForm.addText("Second form, first page");

		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addText("Document contents");

		ProcessBlockBuilder firstFormPreProcess = builder
				.addProcess("First Form Pre Process");
		firstFormPreProcess.addShow(secondForm);
		firstForm.setPreProcess(firstFormPreProcess);

		ProcessBlockBuilder secondFormPreprocess = builder
				.addProcess("Second Form Preprocess");
		secondFormPreprocess.addShow(document1);
		secondForm.setPreProcess(secondFormPreprocess);

		Project project = builder.build();

		userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		bot.go(userProject);

		assertContains("Document contents", bot.getPageText());
		assertContains("Second form, first page", bot.getPageText());
	}

	public void testSavingFormSubmissionWithoutEffectOfPreprocess() throws RobotException {
		ProjectBuilder builder = new ProjectBuilder();

		FormBuilder firstForm = builder.addForm("Form 1");
		firstForm.addText("First form text");
		
		FormBuilder secondForm = builder.addForm("SecondForm");
		secondForm.addText("Second form, first page");

		DocumentBuilder document1 = builder.addDocument("Document 1");
		document1.addText("x=");
		document1.addField("x");

		ProcessBlockBuilder firstFormPostProcess = builder
				.addProcess("First Form Post Process");
		firstFormPostProcess.addSet("x", OperandType.VALUE, "0");
		firstFormPostProcess.addShow(secondForm);
		firstForm.setPostProcess(firstFormPostProcess);

		ProcessBlockBuilder secondFormPreprocess = builder
				.addProcess("Second Form Preprocess");
		secondFormPreprocess.addAddTo("x", OperandType.VALUE, "2");
		secondForm.setPreProcess(secondFormPreprocess);

		ProcessBlockBuilder secondFormPostprocess = builder
		.addProcess("Second Form postprocess");
		secondFormPostprocess.addShow(document1);
		secondFormPostprocess.addShow(secondForm);
		secondForm.setPostProcess(secondFormPostprocess);

		Project project = builder.build();

		userProject = new UserProject(project, projectOwner, "test");
		userProject = world.domain().projects().put(userProject);

		bot.go(userProject, firstForm.getName());
		assertContains("First form text", bot.getPageText());
		
		bot.submit();
		assertContains("Second form, first page", bot.getPageText());

		bot.submit();
		assertContains("Second form, first page", bot.getPageText());
		assertContains("x=2", bot.getPageText());
		
		List<FormSubmission> data = world.domain().storedData().responsesFor(userProject.getProject(), project.getForm(secondForm.getName()));
		assertEquals(1, data.size());
		
		FormSubmission submission = data.get(0);
		assertEquals("2", submission.getValue(new Reference("x")).toString());
	}
}
