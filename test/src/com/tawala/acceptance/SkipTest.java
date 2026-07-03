package com.tawala.acceptance;

import static com.tawala.project.builder.ProcessBuilder.OperandType.FIELD;
import static com.tawala.project.builder.ProcessBuilder.OperandType.VALUE;

import com.scissor.webrobot.RobotException;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.SkipBlockBuilder;
import com.tawala.project.commands.SkipBlock;

public class SkipTest extends AcceptanceTestCase {
    private ProjectBuilder builder;

    @Override
    protected void setUp() throws Exception {
        super.setUp();
        builder = new ProjectBuilder();
    }

    public void testBasicSkip() throws RobotException {
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addField("Form 1:First");
        doc1.addText(" ");
        doc1.addField("Form 1:Last");
        doc1.addText(" is ");
        doc1.addField("Form 1:Age");

        com.tawala.project.builder.ProcessBlockBuilder proc1 = builder
                .addProcess("Process 1");
        proc1.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", proc1);
        form1.addFib("First name? ", "First", 10);
        SkipBlockBuilder skip = form1.addSkip();
        skip.addIfSkip("Form 1:First", "equals", "value", "Madonna", "Q3", null);
        form1.addFib("Last name? ", "Last", 10);
        form1.addBreak();
        form1.addFib("Age? ", "Age", 3);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                getName());
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("First", "John");
        bot.submit();
        bot.setParameter("Last", "Smith");
        bot.submit();
        bot.setParameter("Age", "21");
        bot.submit();
        assertContains("John Smith is 21", bot.getPageText());

        bot.go(userProject);
        bot.setParameter("First", "Madonna");
        bot.submit();
        assertDoesntContain("Last", bot.getPageText());
        bot.setParameter("Age", "47");
        bot.submit();
        assertContains("Madonna  is 47", bot.getPageText());
    }

    public void testSetInSkip() throws RobotException {
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Score=");
        doc1.addField("Score");

        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addShow(doc1);

        FormBuilder form1 = builder.addForm("Form 1", proc1);
        form1.addFib("Age? ", 3);
        SkipBlockBuilder skip = form1.addSkip();
        skip.addSet("Score", VALUE, "10");
        skip.addAddTo("Score", FIELD, "Form 1:Q1:a");

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("Q1:a", "21");
        bot.submit();
        assertContains("Score=31", bot.getPageText());
    }

    public void testSkipToEndOfForm() throws RobotException {
    	DocumentBuilder documentBuilder = builder.addDocument("doc");
    	documentBuilder.addText("This is the final document");
    	
    	ProcessBlockBuilder processBlockBuilder = builder.addProcess("postprocess");
    	processBlockBuilder.addShow(documentBuilder);
    	
        FormBuilder form1 = builder.addForm("Form 1", processBlockBuilder);
        form1.addFib("First name? ", "First", 10);
        SkipBlockBuilder skip = form1.addSkip();
        skip.addIfSkip("Form 1:First", "equals", "value", "Madonna", "__EndOfForm__",
                null);
        form1.addFib("Last name? ", "Last", 10);

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("First", "John");
        bot.submit();
        bot.setParameter("Last", "Smith");
        bot.submit();
        assertContains("This is the final document", bot.getPageText());

        bot.go(userProject);
        bot.setParameter("First", "Madonna");
        bot.submit();
        assertDoesntContain("Last", bot.getPageText());
        assertContains("This is the final document", bot.getPageText());

    }

    public void testCarryOnWithInvalidId() throws RobotException {
        FormBuilder form1 = builder.addForm("Form 1");
        form1.addFib("First name? ", "First", 10);
        SkipBlockBuilder skip = form1.addSkip();
        skip.addIfSkip("First", "equals", "value", "Madonna", "Incorrect ID!",
                null);
        form1.addFib("Last name? ", "Last", 10);

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("First", "Madonna");
        bot.submit();
        bot.setParameter("Last", "Ciccone");
        bot.submit();
        assertContains("Thank you!", bot.getPageText());
    }

    public void testSkipAtEnd() throws RobotException {
        FormBuilder form1 = builder.addForm("Form 1");
        form1.addMc("Keep looping?", "yes", "no");
        SkipBlockBuilder skip = form1.addSkip();
        skip.addIfSkip("Form 1:Q1", "equals", "value", "a", "Q1", null);

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.submit();
        bot.setParameter("Q1", "b");
        bot.submit();
        assertContains("Thank you!", bot.getPageText());
    }

    public void testTwoSkipBlocksTogether() throws RobotException {
        FormBuilder form1 = builder.addForm("Form 1");
        form1.addMc("Keep looping?", "yes", "maybe", "no");
        SkipBlockBuilder skip1 = form1.addSkip();
        skip1.addIfSkip("Form 1:Q1", "equals", "value", "a", "Q1", null);
        SkipBlockBuilder skip2 = form1.addSkip();
        skip2.addIfSkip("Form 1:Q1", "equals", "value", "b", "Q1", null);

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.submit();
        bot.setParameter("Q1", "b");
        bot.submit();
        bot.setParameter("Q1", "c");
        bot.submit();
        assertContains("Thank you!", bot.getPageText());
    }

    public void testSkipBlockFirstInForm() throws RobotException {
        FormBuilder ageQuery = builder.addForm("AgeQuery");
        ageQuery.addFib("How old are you?", "Age", 3);

        FormBuilder drinkingQuestions = builder.addForm("DrinkingQuestions");
        SkipBlockBuilder skipBlockBuilder = drinkingQuestions.addSkip();
        skipBlockBuilder.addIfSkip("AgeQuery:Age", "isLessThan", "value", "21",
                SkipBlock.SKIP_TO_END, null);
        drinkingQuestions.addFib("How many beers to you drink each week?", 3);

        ProcessBlockBuilder process = builder.addProcess("ShowNextForm");
        process.addShow(drinkingQuestions);
        ageQuery.setPostProcess(process);

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.setParameter("Age", "20");
        bot.submit();
        assertDoesntContain("beers", bot.getPageText());
    }

    // TODO: missing the case where skip first in form has process at end

    public void testInputValuesPresent() throws RobotException {

        ProjectBuilder builder = new ProjectBuilder();

        ProcessBlockBuilder process = builder.addProcess("Process 1");

        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addText("This is Form1");
        form1.addFib("Name:", "Name", 20);
        SkipBlockBuilder skipBlock = form1.addSkip();
        skipBlock.addSkip("T1");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("Name", "Archie");
        bot.submit();

        assertContains("This is Form1", bot.getPageText());
        assertContains("Archie", bot.getPageText());
    }

    public void testSkipWithDeclaredFields() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        FormBuilder form1 = builder.addForm("Form 1");
        form1.addFib("Name:", "Name", 20);
        
        //--- Skip that never triggers.
        SkipBlockBuilder skipBlock = form1.addSkip();
        skipBlock.addIfSkip("xxx", "equals", "value", "yyy", SkipBlock.SKIP_TO_END, null);
        
        form1.addDeclaredFields("declaredField");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("Name", "Archie");
        bot.submit();

        assertContains("Thank you!", bot.getPageText());
    }
}
