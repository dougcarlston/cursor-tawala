package com.tawala.acceptance;

import static com.tawala.project.builder.ProcessBuilder.OperandType.FIELD;

import org.springframework.mail.javamail.JavaMailSenderImpl;

import com.scissor.webrobot.RobotException;
import com.tawala.email.Emailer;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ConditionsBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.IfBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.SkipBlockBuilder;

import fake.smtp.FakeSmtpServer;

/**
 * Makes sure alternate labels work. This should probably be merged with some more general tests on form use.
 */
public class AlternateLabelTest extends AcceptanceTestCase {
    private FakeSmtpServer server;

    @Override
    protected void setUp() throws Exception {
        // TODO Auto-generated method stub
        super.setUp();
        server = new FakeSmtpServer();

        JavaMailSenderImpl senderImpl = new JavaMailSenderImpl();
        senderImpl.setPort(server.getPort());
        senderImpl.setHost("127.0.0.1");
        new Emailer().setSender(senderImpl);
    }

    @Override
    protected void tearDown() throws Exception {
        if (server != null)
            server.shutDown();
        super.tearDown();
    }

    public void testDocumentRendering() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Name 1 = ");
        doc1.addField("form1:NameOne");
        doc1.addText(" Name 2 = ");
        doc1.addField("form1:Q2:a");

        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        FormBuilder form1 = builder.addForm("form1", proc1);
        form1.addFib("Name 1:", "NameOne", 10);
        form1.addFib("Name 2:", 10);

        proc1.addShow(doc1);

        UserProject userProject = new UserProject(builder.build(), projectOwner, "AltNameTest7");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.setParameter("NameOne", "William");
        bot.setParameter("Q2:a", "Steve");
        bot.submit();
        assertContains("William", bot.getPageText());
        assertContains("Steve", bot.getPageText());
    }

    public void testConditions() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Entered Name ");
        doc1.addField("form1:NameOne");
        doc1.addText(" contains 'w'.");
        DocumentBuilder doc2 = builder.addDocument("Document 2");
        doc2.addText("Entered Name ");
        doc2.addField("form1:NameOne");
        doc2.addText(" DOES NOT contain 'w'.");

        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        FormBuilder form1 = builder.addForm("form1", proc1);
        form1.addFib("Name 1:", "NameOne", 10);

        IfBuilder ifBuilder = proc1.addIf();
        ConditionsBuilder conditions = ifBuilder.conditions();
        conditions.addComparison("contains", "form1:NameOne", "string", "w");
        ProcessBlockBuilder trueSet = (ProcessBlockBuilder) ifBuilder.trueSet();
        trueSet.addShow(doc1);
        ProcessBlockBuilder falseSet = (ProcessBlockBuilder) ifBuilder.falseSet();
        falseSet.addShow(doc2);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner, "AltNameTest8");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("NameOne", "William");
        bot.submit();
        assertContains("William", bot.getPageText());
        assertContains("contains", bot.getPageText());

        bot.go(userProject);
        bot.setParameter("NameOne", "Steve");
        bot.submit();
        assertContains("Steve", bot.getPageText());
        assertContains("DOES NOT", bot.getPageText());

        bot.go(userProject);
        bot.setParameter("NameOne", "Shawn");
        bot.submit();
        assertContains("Shawn", bot.getPageText());
        assertContains("contains", bot.getPageText());
    }

    public void testAssignedValues() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addText("Entered Name ");
        doc1.addField("EnteredName");
        doc1.addText(" contains 'w'.");
        DocumentBuilder doc2 = builder.addDocument("Document 2");
        doc2.addText("Entered Name ");
        doc2.addField("EnteredName");
        doc2.addText(" DOES NOT contain 'w'.");

        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addSet("EnteredName", FIELD, "form1:NameOne");
        FormBuilder form1 = builder.addForm("form1", proc1);
        form1.addFib("Name 1:", "NameOne", 10);

        IfBuilder ifBuilder = proc1.addIf();
        ConditionsBuilder conditions = ifBuilder.conditions();
        conditions.addComparison("contains", "EnteredName", "string", "w");
        ProcessBlockBuilder trueSet = (ProcessBlockBuilder) ifBuilder.trueSet();
        trueSet.addShow(doc1);
        ProcessBlockBuilder falseSet = (ProcessBlockBuilder) ifBuilder.falseSet();
        falseSet.addShow(doc2);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner, "AltNameTest9");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.setParameter("NameOne", "William");
        bot.submit();
        assertContains("contains", bot.getPageText());
        bot.go(userProject);
        bot.setParameter("NameOne", "Steve");
        bot.submit();
        assertContains("DOES NOT", bot.getPageText());
    }

    public void testAltLabelRender() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addField("form1:Q1:a");
        doc1.addField("form1:Answer2");
        doc1.addField("form1:AltQ3Label:a");
        doc1.addField("form1:Answer4");
        doc1.addField("form1:Q5");
        doc1.addField("form1:Answer6");

        ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        FormBuilder form1 = builder.addForm("form1", proc1);
        form1.addFib("Question 1:", 10);
        form1.addFib("Question 2:", "Answer2", 10);
        form1.addFibNoBlankAlt("Question 3:", "AltQ3Label", 10);
        form1.addFib("Question 4:", "AltQ4Label", "Answer4", 10);
        form1.addMc("Question 5:", true, true, "five", "six", "seven");
        form1.addMcWithAlternateLabel("Answer6", "Question 6", true, true, "eight", "nine", "ten");
        proc1.addShow(doc1);

        Project project = builder.build();


        UserProject userProject = new UserProject(project, projectOwner, "AltNameTest10");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.setParameter("Q1:a", "one");
        bot.setParameter("Answer2", "two");
        bot.setParameter("AltQ3Label:a", "three");
        bot.setParameter("Answer4", "four");
        bot.setParameter("Q5", "a");
        bot.setParameter("Answer6", "a");

        bot.submit();
        assertMatches(
                "<div class=\"document\">.*onetwothreefouraa.*</div>" + NEWLINE
                , bot.getPageText());
    }

    private ProjectBuilder altBuilder;
    private DocumentBuilder altDoc1;
    private DocumentBuilder altDoc2;
    private DocumentBuilder altDoc3;
    private DocumentBuilder altDoc4;
    private ProcessBlockBuilder altProc1;

    private void initAltLabel() {
        altBuilder = new ProjectBuilder();
        altDoc1 = altBuilder.addDocument("Document 1");
        altDoc2 = altBuilder.addDocument("Document 2");
        altDoc3 = altBuilder.addDocument("Document 3");
        altDoc4 = altBuilder.addDocument("Document 4");

        // 1: FIB with alternate question label only
        // 2: FIB with alternate blank label only
        // 3: FIB with alternate question and blank labels
        // 4: MC with alternate question label

        altProc1 = altBuilder.addProcess("Process 1");
        FormBuilder form1 = altBuilder.addForm("form1", altProc1);

        // create FIB with alternate question label only
        form1.addFibNoBlankAlt("Question 1:", "AltQ1Label", 10);
        // create FIB with alternate blank label only
        form1.addFib("Question 2:", "Answer2", 10);
        // create FIB with alternate question and blank labels
        form1.addFib("Question 3:", "AltQ3Label", "Answer3", 10);
        // create MC with alternate question label (mark not reuqired)
        form1.addMcWithAlternateLabel("AltQ4Label", "Question 4", true, false, "eight", "nine", "ten");
    }

    public void testAltLabelIf() throws RobotException {

        initAltLabel();
        altDoc1.addField("form1:AltQ1Label:a");
        altDoc2.addField("form1:Answer2");
        altDoc3.addField("form1:Answer3");
        altDoc4.addField("form1:AltQ4Label");

        // Create if to test FIB with alternate question label only
        IfBuilder ifBuilder = altProc1.addIf();
        ConditionsBuilder conditions = ifBuilder.conditions();
        conditions.addComparison("equals", "form1:AltQ1Label:a", "string", "one");
        ProcessBlockBuilder trueSet = (ProcessBlockBuilder) ifBuilder.trueSet();
        trueSet.addShow(altDoc1);

        // Create if to test FIB with alternate blank label only
        IfBuilder ifBuilder2 = altProc1.addIf();
        ConditionsBuilder conditions2 = ifBuilder2.conditions();
        conditions2.addComparison("equals", "form1:Answer2", "string", "two");
        ProcessBlockBuilder trueSet2 = (ProcessBlockBuilder) ifBuilder2.trueSet();
        trueSet2.addShow(altDoc2);

        // Create if to test FIB with alternate question and blank labels
        IfBuilder ifBuilder3 = altProc1.addIf();
        ConditionsBuilder conditions3 = ifBuilder3.conditions();
        conditions3.addComparison("equals", "form1:Answer3", "string", "three");
        ProcessBlockBuilder trueSet3 = (ProcessBlockBuilder) ifBuilder3.trueSet();
        trueSet3.addShow(altDoc3);

        // Create if to test MC with alternate question label
        IfBuilder ifBuilder4 = altProc1.addIf();
        ConditionsBuilder conditions4 = ifBuilder4.conditions();
        conditions4.addComparison("equals", "form1:AltQ4Label", "string", "a");
        ProcessBlockBuilder trueSet4 = (ProcessBlockBuilder) ifBuilder4.trueSet();
        trueSet4.addShow(altDoc4);

        Project project = altBuilder.build();

        UserProject userProject = new UserProject(project, projectOwner, "AltNameTest10");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.setParameter("AltQ1Label:a", "one");
        bot.setParameter("Answer2", "two");
        bot.setParameter("Answer3", "three");

        bot.submit();
        assertMatches(
                "<div class=\"document\">.*one.*</div>" + NEWLINE +
                "<div class=\"document\">.*two.*</div>" + NEWLINE +
                "<div class=\"document\">.*three.*</div>" + NEWLINE
                , bot.getPageText());
    }

    public void testAltLabelSet() throws RobotException {
        initAltLabel();
        altDoc1.addField("var1");
        altDoc2.addField("var1");
        altDoc3.addField("var1");
        altDoc4.addField("var1");

        // Create set to test FIB with alternate question label only
        altProc1.addSet("var1", FIELD, "form1:AltQ1Label:a");
        altProc1.addShow(altDoc1);

        // Create set to test FIB with alternate blank label only
        altProc1.addSet("var1", FIELD, "form1:Answer2");
        altProc1.addShow(altDoc2);

        // Create set to test FIB with alternate question and blank labels
        altProc1.addSet("var1", FIELD, "form1:Answer3");
        altProc1.addShow(altDoc3);

        Project project = altBuilder.build();

        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.setParameter("AltQ1Label:a", "one");
        bot.setParameter("Answer2", "two");
        bot.setParameter("Answer3", "three");

        bot.submit();
        assertMatches(
                "<div class=\"document\">.*one.*</div>" + NEWLINE +
                "<div class=\"document\">.*two.*</div>" + NEWLINE +
                "<div class=\"document\">.*three.*</div>"
                , bot.getPageText());
    }

    public void testAltLabelMath() throws RobotException {
        initAltLabel();
        altDoc1.addField("var1");

        altProc1.addSet("var1", FIELD, "form1:AltQ1Label:a");
        altProc1.addAddTo("var1", FIELD, "form1:AltQ1Label:a");
        altProc1.addMultiplyBy("var1", FIELD, "form1:Answer3");
        altProc1.addDivideBy("var1", FIELD, "form1:Answer2");
        altProc1.addSubtractFrom("var1", FIELD, "form1:Answer2");
        altProc1.addShow(altDoc1);
        Project project = altBuilder.build();

        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.setParameter("AltQ1Label:a", "2");
        bot.setParameter("Answer2", "4");
        bot.setParameter("Answer3", "8");

        bot.submit();
        assertMatches(
                "<div class=\"document\">.*4.*</div>" + NEWLINE 
                , bot.getPageText());
    }

    public void testAltLabelSendEmail() throws RobotException {
        initAltLabel();
        altDoc1.addField("form1:AltQ1Label:a");
        altDoc2.addField("form1:Answer2");
        altDoc3.addField("form1:Answer3");

        altProc1.addSend("form1:AltQ1Label:a", "", "Testing", "body of test message goes here.");
        altProc1.addSend("form1:AltQ1Label:a", "", "Testing", altDoc1);
        altProc1.addShow(altDoc1);
        altProc1.addShow(altDoc2);
        altProc1.addShow(altDoc3);
        Project project = altBuilder.build();

        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("AltQ1Label:a", "user@example.com");
        bot.setParameter("Answer2", "Test Message");
        bot.setParameter("Answer3", "This is the body of the test message");

        bot.submit();
        assertMatches(
                "<div class=\"document\">.*user@example.com.*</div>" + NEWLINE +
                "<div class=\"document\">.*Test Message.*</div>" + NEWLINE +
                "<div class=\"document\">.*This is the body of the test message.*</div>" + NEWLINE
                , bot.getPageText());
    }

    public void testAltQuestionLabelSkip() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addField("form1:First");
        doc1.addText(" ");
        doc1.addField("form1:Last");
        doc1.addText(" is ");
        doc1.addField("form1:Age:a");

        com.tawala.project.builder.ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addShow(doc1);

        FormBuilder form1 = builder.addForm("form1", proc1);
        form1.addFib("First name? ", "First", 10);
        SkipBlockBuilder skip = form1.addSkip();
        skip.addIfSkip("form1:First", "equals", "value", "Madonna", "Age", null);
        form1.addFib("Last name? ", "Last", 10);
        form1.addBreak();
        form1.addFibNoBlankAlt("Age? ", "Age", 3);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("First", "John");
        bot.submit();
        bot.setParameter("Last", "Smith");
        bot.submit();
        bot.setParameter("Age:a", "21");
        bot.submit();
        assertContains("John Smith is 21", bot.getPageText());

        bot.go(userProject);
        bot.setParameter("First", "Madonna");
        bot.submit();
        assertDoesntContain("Last", bot.getPageText());
        bot.setParameter("Age:a", "47");
        bot.submit();
        assertContains("Madonna  is 47", bot.getPageText());
    }

    public void testAltTextLabelSkip() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("Document 1");
        doc1.addField("form1:First");
        doc1.addText(" ");
        doc1.addField("form1:Last");
        doc1.addText(" is ");
        doc1.addField("form1:Age:a");

        com.tawala.project.builder.ProcessBlockBuilder proc1 = builder.addProcess("Process 1");
        proc1.addShow(doc1);

        FormBuilder form1 = builder.addForm("form1", proc1);
        form1.addFib("First name? ", "First", 10);
        SkipBlockBuilder skip = form1.addSkip();
        skip.addIfSkip("form1:First", "equals", "value", "Madonna", "altText1", null);
        form1.addFib("Last name? ", "Last", 10);
        form1.addBreak();
        form1.addText("altText1", "Text goes here");
        form1.addFibNoBlankAlt("Age? ", "Age", 3);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("First", "John");
        bot.submit();
        bot.setParameter("Last", "Smith");
        bot.submit();
        bot.setParameter("Age:a", "21");
        bot.submit();
        assertContains("John Smith is 21", bot.getPageText());

        bot.go(userProject);
        bot.setParameter("First", "Madonna");
        bot.submit();
        assertDoesntContain("Last", bot.getPageText());
        bot.setParameter("Age:a", "47");
        bot.submit();
        assertContains("Madonna  is 47", bot.getPageText());
    }


}
