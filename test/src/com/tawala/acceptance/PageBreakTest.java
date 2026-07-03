package com.tawala.acceptance;

import java.util.List;

import com.scissor.webrobot.RobotException;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.Reference;
import com.tawala.web.SiteRobot;

public class PageBreakTest extends AcceptanceTestCase {

    protected void setUp() throws Exception {
        super.setUp();
        world.domain().users().addOrSave(projectOwner);
    }

    public void testSimpleBreak() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        FormBuilder form = builder.addForm("form");
        form.addText("one");
        form.addBreak();
        form.addText("two");

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner,
                "break");
        world.domain().projects().put(userProject);

        bot.go(userProject);
        assertContains("one", bot.getPageText());
        assertDoesntContain("two", bot.getPageText());

        bot.submit();
        assertDoesntContain("one", bot.getPageText());
        assertDoesntContain("Thank you!", bot.getPageText());
        assertContains("two", bot.getPageText());

        bot.submit();
        assertContains("Thank you!", bot.getPageText());
    }

    public void testBreakWithData() throws RobotException {
        UserProject userProject = twoPageNameGetter();
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("Q1:a", "Bob");
        bot.submit();
        bot.setParameter("Q2:a", "Smith");
        bot.submit();
        assertContains("Bob Smith", bot.getPageText());

    }

    // TODO: jumping into the middle of a multi-page form sends you back to the
    // beginning

    public void testNoDataCrossover() throws RobotException {
        UserProject project1 = twoPageNameGetter();
        world.domain().projects().put(project1);
        UserProject project2 = onePageNameGetter();
        world.domain().projects().put(project2);

        bot.go(project1);
        bot.setParameter("Q1:a", "Bob");
        bot.submit();
        bot.setParameter("Q2:a", "Smith");
        bot.submit();

        bot.go(project2);
        bot.setParameter("Q1:a", "Jane");
        bot.setParameter("Q1:b", "Doe");
        bot.submit();

        List<FormSubmission> formSubmissions = world.domain().storedData()
                .responsesFor(project2.getProject(),
                        project2.getProject().defaultForm());
        assertEquals(1, formSubmissions.size());
        FormSubmission data = formSubmissions.get(0);
        assertEquals("[Jane]", data.getValues(new Reference("Q1:a")).toString());
        assertEquals("[Doe]", data.getValues(new Reference("Q1:b")).toString());
        assertEquals("[]", data.getValues(new Reference("Q2:a")).toString());

    }

    /* This is an unrealistic scenario - the same user tries to do 
     * a couple of sessions at the same time. SL.
     */
    public void COMMENTED_OUT_testTwoVisitsFromOneBrowser() throws RobotException {
        UserProject project = twoPageNameGetter();
        world.domain().projects().put(project);

        bot.go(project);
        SiteRobot bot2 = newBot();
        bot2.copyCookies(bot);
        bot2.go(project);

        bot.setParameter("Q1:a", "Bob");
        bot.submit();
        bot2.setParameter("Q1:a", "Pat");
        bot2.submit();

        bot.setParameter("Q2:a", "Smith");
        bot.submit();
        bot2.setParameter("Q2:a", "Jones");
        bot2.submit();

        assertContains("Bob Smith", bot.getPageText());
        assertContains("Pat Jones", bot2.getPageText());
    }

    public void testUselessBreaksRemoved() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        FormBuilder form = builder.addForm("Form1");
        form.addBreak();
        form.addBreak();
        form.addFib("Name?", 20);
        form.addBreak();
        form.addBreak();
        form.addFib("Age?", 20);
        form.addBreak();
        form.addBreak();

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                getName());
        world.domain().projects().put(userProject);

        bot.go(userProject);
        bot.setParameter("Q1:a", "Jane");
        bot.submit();
        bot.setParameter("Q2:a", "65");
        bot.submit();

        assertContains("Thank you!", bot.getPageText());

    }

    private UserProject onePageNameGetter() {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc = builder.addDocument("doc");
        doc.addField("Q1:a");
        doc.addText(" ");
        doc.addField("Q1:b");

        ProcessBlockBuilder show = builder.addProcess("show");
        show.addShow(doc);

        FormBuilder form = builder.addForm("form", show);
        form.addFib("Name?", 10, 10);

        return new UserProject(builder.build(), projectOwner, "onepage");
    }

    private UserProject twoPageNameGetter() {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc = builder.addDocument("doc");
        doc.addField("form:Q1:a");
        doc.addText(" ");
        doc.addField("form:Q2:a");

        ProcessBlockBuilder show = builder.addProcess("show");
        show.addShow(doc);

        FormBuilder form = builder.addForm("form", show);
        form.addFib("First name?", 10);
        form.addBreak();
        form.addFib("Last name?", 10);

        return new UserProject(builder.build(), projectOwner, "twopages");
    }

}
