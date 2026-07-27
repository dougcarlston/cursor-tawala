package com.tawala.acceptance;

import com.scissor.webrobot.RobotException;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.IfBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;

public class AdvancedIfTest extends AcceptanceTestCase {

    public void testIfMCEqualsMCOne() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        String result = "result";

        ProcessBlockBuilder process = builder.addProcess("Process 1");

        IfBuilder builder1 = process.addIf();
        builder1.conditions().startAnd();
        builder1.conditions().addMCComparisonAdvancedIfStyle("Form 1:Q1", "mcEquals",
                "a");
        builder1.conditions().addMCComparisonAdvancedIfStyle("Form 1:Q2", "mcEquals",
                "b");
        builder1.conditions().endAnd();
        builder1.trueSet().addSet(result, OperandType.VALUE, "true");
        builder1.falseSet().addSet(result, OperandType.VALUE, "false");

        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Result = ");
        doc1.addField(result);

        process.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addMc("MC Item 1", "Choice A", "Choice B");
        form1.addMc("MC Item 2", "Choice A", "Choice B", "Choice C");

        Project project = builder.build();

        assertContains("<string value=\"a\"/>", process.toString());
        assertContains("<string value=\"b\"/>", process.toString());

        UserProject userProject = new UserProject(project, projectOwner,
                "testAdvancedIf");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.setParameter("Q2", "b");
        bot.submit();
        assertContains("Result = true", bot.getPageText());

        bot.go(userProject);
        bot.setParameter("Q1", "b");
        bot.setParameter("Q2", "b");
        bot.submit();
        assertContains("Result = false", bot.getPageText());
    }
}
