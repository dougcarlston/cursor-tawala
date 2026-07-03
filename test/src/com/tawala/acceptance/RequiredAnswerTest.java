package com.tawala.acceptance;

import com.scissor.webrobot.RobotException;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class RequiredAnswerTest extends AcceptanceTestCase {
    public void testFIBRequiredAnswer() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addField("Form 1:First:a");
        doc1.addText(" ");
        doc1.addField("Form 1:Last:a");

        com.tawala.project.builder.ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", proc1);
        form1.addFibNoBlankAlt("First name? ", "First", 10, true);
        form1.addFibNoBlankAlt("Last name? ", "Last", 10, true);
        form1.addBreak();
        form1.addFibNoBlankAlt("Your age? ", "age", 10, true);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner, getName());
        world.domain().projects().put(userProject);

        //--- Navigation to the first page of the form.
        bot.go(userProject);
        assertDoesntContain("Please answer the marked questions before continuing.", bot.getPageText());

        //--- Attempt to post without providing the required answer
        bot.submit();
        assertContains("Please answer the marked questions before continuing.", bot.getPageText());

        //--- Set one of the required parameters.
        bot.setParameter("First:a", "John");
        bot.submit();
        assertContains("Please answer the marked questions before continuing.", bot.getPageText());

        //--- Set the second required parameter.
        bot.setParameter("Last:a", "Smith");
        bot.submit();
        assertDoesntContain("Please answer the marked questions before continuing.", bot.getPageText());
        assertContains("Your age?", bot.getPageText());
        
        //--- Submit post to the second segment without specifying the required parameter
        bot.submit();
        assertContains("Please answer the marked questions before continuing.", bot.getPageText());

        //--- Finally, set the last required parameter and assert that the right doc is produced.
        bot.setParameter("age:a", "22");
        bot.submit();
        assertDoesntContain("Please answer the marked questions before continuing.", bot.getPageText());
        assertContains("John Smith", bot.getPageText());
    }

    public void testMCRequiredAnswer() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addField("Form 1:Q1");
        doc1.addField("Form 1:Q2");

        com.tawala.project.builder.ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", proc1);
        form1.addMc("MC Question 1:", true, true, "no", "yes");
        form1.addMc("MC Question 2:", true, true, "no", "yes");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner, getName());
        world.domain().projects().put(userProject);

        bot.go(userProject);

        bot.submit();
        assertContains("Please answer the marked questions before continuing.", bot.getPageText());

        bot.setParameter("Q1", "a");
        bot.submit();
        assertContains("Please answer the marked questions before continuing.", bot.getPageText());

        bot.setParameter("Q2", "a");
        bot.submit();
        assertContains("aa", bot.getPageText());
    }

    public void testMCMulitipleRequiredAnswer() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addField("Form 1:Q1");
        doc1.addField("Form 1:Q2");

        com.tawala.project.builder.ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", proc1);
        form1.addMc("MC Question 1:", false, true, "yes", "no");
        form1.addMc("MC Question 2:", false, true, "yes", "no");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner, getName());
        world.domain().projects().put(userProject);

        bot.go(userProject);

        bot.submit();
        assertContains("Please answer the marked questions before continuing.", bot.getPageText());

        bot.setParameter("Q1", "a");
        bot.submit();
        assertContains("Please answer the marked questions before continuing.", bot.getPageText());
        assertEquals(new String[]{"a"}, bot.getParameters("Q1"));


        bot.setParameters("Q1", new String[]{"a", "b"});
        bot.submit();
        assertContains("Please answer the marked questions before continuing.", bot.getPageText());

        bot.setParameters("Q2", new String[]{"a", "b"});
        bot.submit();
        assertContains("a, ba, b", bot.getPageText());
    }

    private static void assertEquals(String[] expected, String[] actual) {
        assertEquals(expected.length, actual.length);
        for (int i = 0; i < expected.length; i++) {
            assertEquals("at " + i, expected[i], actual[i]);

        }
    }

}
