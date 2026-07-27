package com.tawala.acceptance;

import static com.tawala.project.builder.ProcessBuilder.OperandType.VALUE;

import com.scissor.webrobot.RobotException;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.ForEachBuilder;
import com.tawala.project.builder.ForEachMcBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.IfBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class ForEachMcTest extends AcceptanceTestCase {

    public void testIfMCEqualsMCOne() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Total Score = ");
        doc1.addField("TotalScore");

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addSet("TotalScore", VALUE, "0");
        process.addGet("RecordList", "Form 1");

        ForEachBuilder forEachbuilder = process.addForEach("Record",
                "RecordList");
        ForEachMcBuilder forEachMcBuilder = forEachbuilder.addForEachMc();

        IfBuilder ifBuilder1 = forEachMcBuilder.addIf();
        ifBuilder1.conditions().addMCToMCComparison("mcEquals",
                "Record:Form 1:(selection)", "Form 1:(selection)");
        ifBuilder1.trueSet().addAddTo("TotalScore", VALUE, "10");

        IfBuilder ifBuilder2 = forEachMcBuilder.addIf();
        ifBuilder2.conditions().addMCToMCComparison("mcEquals", "Form 1:(selection)",
                "Record:Form 1:(selection)");
        ifBuilder2.trueSet().addAddTo("TotalScore", VALUE, "10");

        process.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addMc("MC Item 1", "Choice A", "Choice B");

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner,
                "testIfMCEqualsMCOne");
        world.domain().projects().put(userProject);

        makeMCOneSubmissions(5, userProject);

        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.submit();
        assertContains("Total Score = 100", bot.getPageText());

        bot.go(userProject);
        bot.setParameter("Q1", "b");
        bot.submit();
        assertContains("Total Score = 0", bot.getPageText());
    }

    private void makeMCOneSubmissions(int numSubmissions, UserProject project)
            throws RobotException {

        for (int i = 0; i < numSubmissions; i++) {
            bot.go(project);
            bot.setParameter("Q1", "a");
            bot.submit();
        }
    }

    public void testIfMCEqualsMCMany() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Total Score = ");
        doc1.addField("TotalScore");

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addSet("TotalScore", VALUE, "0");
        process.addGet("RecordList", "Form 1");

        ForEachBuilder forEachbuilder = process.addForEach("Record",
                "RecordList");
        ForEachMcBuilder forEachMcBuilder = forEachbuilder.addForEachMc();

        IfBuilder ifBuilder2 = forEachMcBuilder.addIf();
        ifBuilder2.conditions().addMCToMCComparison("mcEquals", "Form 1:(selection)",
                "Record:Form 1:(selection)");
        ifBuilder2.trueSet().addAddTo("TotalScore", VALUE, "10");

        process.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addMc("MC Item 1", false, false, "Choice A", "Choice B");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testIfMCEqualsMCMany");
        world.domain().projects().put(userProject);

        makeMCManySubmissions(5, userProject);

        bot.go(userProject);
        bot.setParameters("Q1", new String[] { "a" });
        bot.submit();
        assertContains("Total Score = 50", bot.getPageText());

        bot.go(userProject);
        bot.setParameters("Q1", new String[] { "b" });
        bot.submit();
        assertContains("Total Score = 50", bot.getPageText());

        bot.go(userProject);
        bot.setParameters("Q1", new String[] { "a", "b" });
        bot.submit();
        assertContains("Total Score = 0", bot.getPageText());
    }

    public void testMultipleQuestions() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Total Score = ");
        doc1.addField("TotalScore");

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addSet("TotalScore", VALUE, "0");
        process.addGet("RecordList", "Form 1");

        ForEachBuilder forEachbuilder = process.addForEach("Record",
                "RecordList");
        ForEachMcBuilder forEachMcBuilder = forEachbuilder.addForEachMc();

        IfBuilder ifBuilder2 = forEachMcBuilder.addIf();
        ifBuilder2.conditions().addMCToMCComparison("mcEquals", "Form 1:(selection)",
                "Record:Form 1:(selection)");
        ifBuilder2.trueSet().addAddTo("TotalScore", VALUE, "10");

        process.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addMc("MC Item 1", false, false, "Choice A", "Choice B");
        form1.addMc("MC Item 2", false, false, "Choice A", "Choice B");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testIfMCEqualsMCMany");
        world.domain().projects().put(userProject);

        world.domain().storedData().eraseResponsesFor(project, "Form 1");

        makeMultiMCManySubmissions(5, userProject);

        bot.go(userProject);
        bot.setParameters("Q1", new String[] { "a" });
        bot.setParameters("Q2", new String[] { "a" });
        bot.submit();
        assertContains("Total Score = 100", bot.getPageText());

        bot.go(userProject);
        bot.setParameters("Q1", new String[] { "b" });
        bot.setParameters("Q2", new String[] { "b" });
        bot.submit();
        assertContains("Total Score = 100", bot.getPageText());

        bot.go(userProject);
        bot.setParameters("Q1", new String[] { "a", "b" });
        bot.setParameters("Q2", new String[] { "a", "b" });
        bot.submit();
        assertContains("Total Score = 0", bot.getPageText());
    }

    private void makeMCManySubmissions(int numSubmissions, UserProject project)
            throws RobotException {

        for (int i = 0; i < numSubmissions; i++) {
            bot.go(project);
            bot.setParameters("Q1", new String[] { "a" });
            bot.submit();
        }

        for (int i = 0; i < numSubmissions; i++) {
            bot.go(project);
            bot.setParameters("Q1", new String[] { "b" });
            bot.submit();
        }
    }

    private void makeMultiMCManySubmissions(int numSubmissions,
            UserProject project) throws RobotException {

        for (int i = 0; i < numSubmissions; i++) {
            bot.go(project);
            bot.setParameters("Q1", new String[] { "a" });
            bot.setParameters("Q2", new String[] { "a" });
            bot.submit();
        }

        for (int i = 0; i < numSubmissions; i++) {
            bot.go(project);
            bot.setParameters("Q1", new String[] { "b" });
            bot.setParameters("Q2", new String[] { "b" });
            bot.submit();
        }
    }

}
