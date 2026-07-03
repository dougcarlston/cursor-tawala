package com.tawala.acceptance;

import com.scissor.webrobot.RobotException;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.IfBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class CompareMCTest extends AcceptanceTestCase {

    public void testCompareMCEquals() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder docSame = builder.addDocument("Same");
        docSame.addText("Choices are the same");

        DocumentBuilder docDifferent = builder.addDocument("Different");
        docDifferent.addText("Choices are different");

        ProcessBlockBuilder process = builder.addProcess("Process 1");

        IfBuilder ifBuilder = process.addIf();
        ifBuilder.conditions().addMCToMCComparison("mcEquals", "Form 1:Q1", "Form 1:Q2");
        ifBuilder.trueSet().addShow(docSame);
        ifBuilder.falseSet().addShow(docDifferent);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addMc("MC Item 1", "Choice A", "Choice B");
        form1.addMc("MC Item 2", "Choice A", "Choice B");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testCurrentFormFieldRead");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.setParameter("Q2", "a");
        bot.submit();
        assertContains("Choices are the same", bot.getPageText());

        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.setParameter("Q2", "b");
        bot.submit();
        assertContains("Choices are different", bot.getPageText());
    }

    public void testCompareMCDoesNotEqual() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        DocumentBuilder docNotSame = builder.addDocument("Same");
        docNotSame.addText("Choices are not the same");

        DocumentBuilder docNotDifferent = builder.addDocument("Different");
        docNotDifferent.addText("Choices are not different");

        ProcessBlockBuilder process = builder.addProcess("Process 1");

        IfBuilder ifBuilder = process.addIf();
        ifBuilder.conditions()
                .addMCToMCComparison("mcDoesNotEqual", "Form 1:Q1", "Form 1:Q2");
        ifBuilder.trueSet().addShow(docNotSame);
        ifBuilder.falseSet().addShow(docNotDifferent);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addMc("MC Item 1", "Choice A", "Choice B");
        form1.addMc("MC Item 2", "Choice A", "Choice B");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testCurrentFormFieldRead");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.setParameter("Q2", "a");
        bot.submit();
        assertContains("Choices are not different", bot.getPageText());

        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.setParameter("Q2", "b");
        bot.submit();
        assertContains("Choices are not the same", bot.getPageText());
    }

}
