package com.tawala.acceptance;

import static com.tawala.project.builder.ProcessBuilder.OperandType.FIELD;
import static com.tawala.project.builder.ProcessBuilder.OperandType.VALUE;

import java.util.List;

import com.scissor.webrobot.RobotException;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.ForEachBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.IfBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.Reference;

public class SimpleGetForEachTest extends AcceptanceTestCase {

    public void testCurrentFormSubmission() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Total Submissions = ");
        doc1.addField("TotalSubmissions");

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addGet("RecordList", "Form 1");
        process.addSet("TotalSubmissions", VALUE, "0");
        ForEachBuilder foreachbuilder = process.addForEach("Record",
                "RecordList");
        foreachbuilder.addAddTo("TotalSubmissions", VALUE, "1");
        process.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addFib("FIB Item 1", 10);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testCurrentFormSubmission");
        world.domain().projects().put(userProject);

        makeFibSubmissions(3, userProject);

        bot.go(userProject);
        bot.submit();

        assertContains("Total Submissions = 3", bot.getPageText());
    }

    public void testCurrentFormFieldRead() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Total Score = ");
        doc1.addField("TotalScore");

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addSet("TotalScore", VALUE, "0");
        process.addSet("Form 1:Score", FIELD, "Form 1:Q1:a");
        process.addMultiplyBy("Form 1:Score", VALUE, "10");
        process.addGet("RecordList", "Form 1");
        ForEachBuilder foreachbuilder = process.addForEach("Record",
                "RecordList");
        foreachbuilder.addAddTo("TotalScore", FIELD, "Record:Form 1:Score");
        process.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addFib("FIB Item 1", 10);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testCurrentFormFieldRead");
        world.domain().projects().put(userProject);

        makeFibSubmissions(5, userProject);

        bot.go(userProject);
        bot.setParameter("Q1:a", "Submission 6");
        bot.submit();

        assertContains("Total Score = 150", bot.getPageText());
    }

    public void testCurrentFormFieldReadDefaultLabel() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Total Score = ");
        doc1.addField("TotalScore");

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addSet("TotalScore", VALUE, "0");
        process.addGet("RecordList", "Form 1");
        ForEachBuilder foreachbuilder = process.addForEach("Record",
                "RecordList");
        foreachbuilder.addAddTo("TotalScore", FIELD, "Record:Form 1:Q1:a");
        process.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addFib("FIB Item 1", 10);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testCurrentFormFieldRead");
        world.domain().projects().put(userProject);
        world.domain().storedData().eraseResponsesFor(project, "Form 1");

        makeFibSubmissions(5, userProject);

        bot.go(userProject);
        bot.setParameter("Q1:a", "Submission 6");
        bot.submit();

        assertContains("Total Score = 15", bot.getPageText());
    }

    public void testCurrentFormMultipleGets() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Total Score 1 = ");
        doc1.addField("TotalScore1");
        doc1.addText("\nTotal Score 2 = ");
        doc1.addField("TotalScore2");

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addSet("TotalScore1", VALUE, "0");
        process.addSet("TotalScore2", VALUE, "0");
        process.addSet("Form 1:Score1", FIELD, "Form 1:Q1:a");
        process.addSet("Form 1:Score2", FIELD, "Form 1:Q1:a");
        process.addMultiplyBy("Form 1:Score1", VALUE, "10");
        process.addMultiplyBy("Form 1:Score2", VALUE, "20");

        process.addGet("RecordList1", "Form 1");
        process.addGet("RecordList2", "Form 1");

        ForEachBuilder forEachBuilder1 = process.addForEach("Record1",
                "RecordList1");
        forEachBuilder1.addAddTo("TotalScore1", FIELD, "Record1:Form 1:Score1");

        ForEachBuilder forEachBuilder2 = process.addForEach("Record2",
                "RecordList2");
        forEachBuilder2.addAddTo("TotalScore2", FIELD, "Record2:Form 1:Score2");

        process.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addFib("FIB Item 1", 10);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testCurrentFormMultipleGets");
        world.domain().projects().put(userProject);

        makeFibSubmissions(5, userProject);

        bot.go(userProject);
        bot.setParameter("Q1:a", "Submission 6");
        bot.submit();

        assertContains("Total Score 1 = 150", bot.getPageText());
        assertContains("Total Score 2 = 300", bot.getPageText());
    }

    public void testCurrentFormIfMC() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Total Score = ");
        doc1.addField("TotalScore");

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addSet("TotalScore", VALUE, "0");
        process.addGet("RecordList", "Form 1");

        ForEachBuilder foreachbuilder = process.addForEach("Record",
                "RecordList");
        IfBuilder ifBuilder = foreachbuilder.addIf();
        ifBuilder.conditions().addMCComparison("Record:Form 1:Q1", "mcEquals", "a");
        ifBuilder.trueSet().addAddTo("TotalScore", VALUE, "10");

        process.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addMc("MC Item 1", "Choice A", "Choice B");

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner,
                "testCurrentFormFieldRead");
        world.domain().projects().put(userProject);

        world.domain().storedData().eraseResponsesFor(project, "Form 1");

        makeMCSubmissions(5, userProject);

        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.submit();

        assertContains("Total Score = 50", bot.getPageText());
    }

    public void testCurrentFormIfMCEqualsMC() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Total Score = ");
        doc1.addField("TotalScore");

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addSet("TotalScore", VALUE, "0");
        process.addGet("RecordList", "Form 1");

        ForEachBuilder foreachbuilder = process.addForEach("Record",
                "RecordList");

        IfBuilder ifBuilder1 = foreachbuilder.addIf();
        ifBuilder1.conditions().addMCToMCComparison("mcEquals", "Record:Form 1:Q1",
                "Form 1:Q1");
        ifBuilder1.trueSet().addAddTo("TotalScore", VALUE, "10");

        IfBuilder ifBuilder2 = foreachbuilder.addIf();
        ifBuilder2.conditions().addMCToMCComparison("mcEquals", "Form 1:Q1",
                "Record:Form 1:Q1");
        ifBuilder2.trueSet().addAddTo("TotalScore", VALUE, "10");

        process.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addMc("MC Item 1", "Choice A", "Choice B");

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner,
                "testCurrentFormFieldRead");
        world.domain().projects().put(userProject);

        world.domain().storedData().eraseResponsesFor(project, "Form 1");

        makeMCSubmissions(5, userProject);

        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.submit();

        assertContains("Total Score = 100", bot.getPageText());
    }

    public void testCurrentFormFieldWriteExisting() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder doc = builder.addDocument("Document 1");
        doc.addText("UserID = ");
        doc.addField("tempID");
        doc.addText("; TestValue = ");
        doc.addField("tempValue");

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addSet("TestValue", VALUE, "0");
        process.addGet("RecordList1", "Form 1");
        ForEachBuilder foreachbuilder = process.addForEach("Record1",
                "RecordList1");
        foreachbuilder.addSet("Record1:Form 1:TestValue", VALUE, "1");

        process.addGet("RecordList2", "Form 1");
        ForEachBuilder foreachbuilder2 = process.addForEach("Record2",
                "RecordList2");
        foreachbuilder2.addSet("tempID", FIELD, "Record2:Form 1:Q1:a");
        foreachbuilder2.addSet("tempValue", FIELD, "Record2:Form 1:TestValue");
        foreachbuilder2.addAppend("Final Document", doc);
        process.addShow("Final Document");

        FormBuilder form = builder.addForm("Form 1", process);
        form.addFib("UserID", 10);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testCurrentFormFieldWriteExisting");
        world.domain().projects().put(userProject);

        makeFibSubmissions(2, userProject);

        bot.go(userProject);
        bot.setParameter("Q1:a", "Submission 3");
        bot.submit();

        assertContains("UserID = 1; TestValue = 1", bot.getPageText());
        assertContains("UserID = 2; TestValue = 1", bot.getPageText());
    }

    public void testQualifiedFieldPersistence() throws RobotException {

        ProjectBuilder builder = new ProjectBuilder();

        ProcessBlockBuilder process1 = setBlankField(builder, "Process1");
        FormBuilder form1 = builder.addForm("Form 1", process1);
        form1.addFib("Fib Item 1", 10);

        ProcessBlockBuilder process2 = setQualifiedFields(builder, "Process2");
        FormBuilder form2 = builder.addForm("Form 2", process2);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testQualifiedFieldPersistence");
        world.domain().projects().put(userProject);

        makeFibSubmissions(3, userProject, form1.getName());

        bot.go(userProject, form2.getName());
        bot.submit();

        List<FormSubmission> submissions = world.domain().storedData()
                .responsesFor(project, "Form 1");
        assertEquals(3, submissions.size());
        assertEquals("10", submissions.get(0).getValue(new Reference("TestValue")).toString());
        assertEquals("20", submissions.get(1).getValue(new Reference("TestValue")).toString());
        assertEquals("30", submissions.get(2).getValue(new Reference("TestValue")).toString());
    }

    public void testCreateNewQualifiedField() throws RobotException {

        ProjectBuilder builder = new ProjectBuilder();

        FormBuilder form1 = builder.addForm("Form 1");
        form1.addFib("Fib Item 1", 10);

        ProcessBlockBuilder process2 = setQualifiedFields(builder, "Process2");
        FormBuilder form2 = builder.addForm("Form 2", process2);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testCreateNewQualifiedField");
        world.domain().projects().put(userProject);

        makeFibSubmissions(3, userProject, form1.getName());

        bot.go(userProject, form2.getName());
        bot.submit();

        List<FormSubmission> submissions = world.domain().storedData()
                .responsesFor(project, "Form 1");
        assertEquals(3, submissions.size());
        assertEquals("10", submissions.get(0).getValue(new Reference("TestValue")).toString());
        assertEquals("20", submissions.get(1).getValue(new Reference("TestValue")).toString());
        assertEquals("30", submissions.get(2).getValue(new Reference("TestValue")).toString());
    }

    private ProcessBlockBuilder setBlankField(ProjectBuilder builder,
            String processName) {
        ProcessBlockBuilder process = builder.addProcess(processName);
        process.addSet("TestValue", FIELD, "Form 1:Q1:a");

        return process;
    }

    private ProcessBlockBuilder setQualifiedFields(ProjectBuilder builder,
            String processName) {
        ProcessBlockBuilder process = builder.addProcess(processName);
        process.addGet("RecordList1", "Form 1");
        ForEachBuilder foreachbuilder = process.addForEach("Record1",
                "RecordList1");
        foreachbuilder.addSet("Record1:Form 1:TestValue", FIELD, "Record1:Form 1:Q1:a");
        foreachbuilder.addMultiplyBy("Record1:Form 1:TestValue", VALUE, "10");

        return process;
    }

    private void makeFibSubmissions(int numSubmissions, UserProject project)
            throws RobotException {

        for (int i = 0; i < numSubmissions; i++) {
            bot.go(project);
            bot.setParameter("Q1:a", Integer.toString(i + 1));
            bot.submit();
        }
    }

    private void makeFibSubmissions(int numSubmissions, UserProject project,
            String formName) throws RobotException {

        for (int i = 0; i < numSubmissions; i++) {
            bot.go(project, formName);
            bot.setParameter("Q1:a", Integer.toString(i + 1));
            bot.submit();
        }
    }

    private void makeMCSubmissions(int numSubmissions, UserProject project)
            throws RobotException {

        for (int i = 0; i < numSubmissions; i++) {
            bot.go(project);
            bot.setParameter("Q1", "a");
            bot.submit();
        }
    }

}
