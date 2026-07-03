package com.tawala.acceptance;

import static com.tawala.project.builder.ProcessBuilder.OperandType.VALUE;

import com.scissor.webrobot.RobotException;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ConditionsBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.IfBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.WorldInitializer;

public class AppendDocumentTest extends AcceptanceTestCase {
    private static final String USER_NAME = "tester";
    private User user;

    public AppendDocumentTest() {
        addUserNameToDelete(USER_NAME);
    }

    public void setUp() throws Exception {
        super.setUp();
        user = UserTest.aUser(USER_NAME);
        WorldInitializer.getDefaultWorld().domain().users().addOrSave(user);
    }

    public void testNormalAppend() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        DocumentBuilder doc2 = builder.addDocument("Document 2");
        doc1.addText("one");
        doc2.addText("two");

        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addAppend(doc1, doc2);
        proc1.addShow(doc1);

        builder.addForm("Form 1", proc1);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, user, "NormalAppend");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.submit();
        assertMatches("<div class=\"document\">.*one.*</div>"
                + NEWLINE + "<div class=\"document\">.*two.*</div>" + NEWLINE
                , bot.getPageText());
    }

    public void testShowAndReset() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        DocumentBuilder doc2 = builder.addDocument("Document 2");
        DocumentBuilder doc3 = builder.addDocument("Document 3");
        doc1.addText("one");
        doc2.addText("two");
        doc3.addText("three");

        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addAppend(doc1, doc2);
        proc1.addAppend(doc1, doc3);
        proc1.addShow(doc1, true);
        proc1.addAppend(doc1, doc3);
        proc1.addShow(doc1);

        builder.addForm("Form 1", proc1);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, user, "NormalAppend");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.submit();
        assertMatches("<div class=\"document\">.*one.*</div>"
                + NEWLINE + "<div class=\"document\">.*two.*</div>" + NEWLINE
                + "<div class=\"document\">.*three.*</div>" + NEWLINE
                + "<div class=\"document\">.*one.*</div>" + NEWLINE
                + "<div class=\"document\">.*three.*</div>" + NEWLINE
                , bot.getPageText());
    }

    public void testAppendWithHtmlData() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        DocumentBuilder doc2 = builder.addDocument("Document 2");
        doc1.addText("one");
        doc2.addText("two");

        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addAppend(doc1, doc2);
        proc1.addShow(doc1);

        builder.addForm("Form 1", proc1);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, user, "NormalAppend");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.submit();

        assertMatches("<div class=\"document\">.*one.*</div>" + NEWLINE
                + "<div class=\"document\">.*two.*</div>", bot.getPageText());
    }

    public void testPathologicalAppend() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addField("x");

        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addSet("x", VALUE, "1");
        proc1.addAppend(doc1, doc1);
        proc1.addSet("x", VALUE, "2");
        proc1.addAppend(doc1, doc1);
        proc1.addSet("x", VALUE, "3");
        proc1.addAppend(doc1, doc1);
        proc1.addSet("x", VALUE, "4");
        proc1.addShow(doc1);

        builder.addForm("Form 1", proc1);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, user,
                "PathologicalAppend");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.submit();
        assertMatches("<div class=\"document\">.*4.*</div>"
                + NEWLINE + "<div class=\"document\">.*1.*</div>" + NEWLINE
                + "<div class=\"document\">.*2.*</div>" + NEWLINE
                + "<div class=\"document\">.*1.*</div>" + NEWLINE
                + "<div class=\"document\">.*3.*</div>" + NEWLINE
                + "<div class=\"document\">.*1.*</div>" + NEWLINE
                + "<div class=\"document\">.*2.*</div>" + NEWLINE
                + "<div class=\"document\">.*1.*</div>" + NEWLINE
                , bot.getPageText());
    }

    public void testVirtualAppend() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        DocumentBuilder doc2 = builder.addDocument("Document 2");
        DocumentBuilder doc3 = builder.addDocument("Document 3");
        doc1.addText("one");
        doc2.addText("two");
        doc3.addText("three");

        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addAppend("Document v4", "Document 2");
        proc1.addAppend("Document v4", "Document 3");
        proc1.addAppend("Document v5", "Document v4");
        proc1.addAppend("Document 1", "Document v5");
        proc1.addShow(doc1);

        builder.addForm("Form 1", proc1);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, user,
                "VirtualAppend");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.submit();
        assertMatches("<div class=\"document\">.*one.*</div>"
                + NEWLINE + "<div class=\"document\">.*two.*</div>" + NEWLINE
                + "<div class=\"document\">.*three.*</div>" + NEWLINE
                , bot.getPageText());
    }

    public void testVirtualAppendMC() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("one");
        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        ProcessBlockBuilder proc2 = builder.addProcess("Process 2");
        FormBuilder form1 = builder.addForm("form1", proc1);
        builder.addForm("form2", proc2);

        IfBuilder ifBuilder = proc1.addIf();
        ConditionsBuilder conditions = ifBuilder.conditions();
        conditions.addMCComparison("form1:Q1", "mcEquals", "a");
        ProcessBlockBuilder trueSet = (ProcessBlockBuilder) ifBuilder.trueSet();
        trueSet.addAppend("doc2", doc1);

        proc1.addShow("doc2");

        form1.addMc("text of question", true, true, "choice1", "choice2");
        Project project = builder.build();

        UserProject userProject = new UserProject(project, user, "MCAppend");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.setParameter("Q1", "a");
        bot.submit();
        assertMatches("<div class=\"document\">.*one.*</div>"
                + NEWLINE, bot.getPageText());
        
        //--- This will force session clearing.
        bot.logOut();
        
        bot.go(userProject);
        bot.setParameter("Q1", "b");
        bot.submit();
        assertContains("Thank you!", bot.getPageText());
    }

    public void testVirtualAppendTwoProcessBlank() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("one");
        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        ProcessBlockBuilder proc2 = builder.addProcess("Process 2");

        builder.addForm("form1", proc1);
        builder.addForm("form2", proc2);
        proc1.addAppend("vdoc2", doc1);
        proc2.addShow("vdoc2");

        Project project = builder.build();
        UserProject userProject = new UserProject(project, user,
                "VirtualAppendTwoProcessBlank");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.submit();
        assertContains("Thank you!", bot.getPageText());
    }

    public void testVirtualAppendTwoProcessText() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("one");
        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        ProcessBlockBuilder proc2 = builder.addProcess("Process 2");
        builder.addForm("form1", proc1);
        FormBuilder form2 = builder.addForm("form2", proc2);

        proc1.addAppend("vdoc2", doc1);
        proc1.addShow(form2);
        proc2.addShow("vdoc2");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, user,
                "VirtualAppendTwoProcessText");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.submit();
        bot.submit();
        assertMatches("<div class=\"document\">.*one.*</div>"
                + NEWLINE, bot.getPageText());
    }

}
