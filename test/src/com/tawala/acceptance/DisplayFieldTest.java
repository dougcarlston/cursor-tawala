package com.tawala.acceptance;

import com.scissor.webrobot.RobotException;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class DisplayFieldTest extends AcceptanceTestCase {

    public void testDisplayFieldShowsFibResponse() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        ProcessBlockBuilder process = builder.addProcess("Process 1");

        // add form with FIB in one segment and text item in another
        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addFib("FIB Item 1", 10);
        form1.addBreak();
        form1.addTextWithFields("Text Item with FIB blank display field: ",
                "<<Form 1:Q1:a>>");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testFibBlankField");
        world.domain().projects().put(userProject);
        bot.go(userProject);

        // fill in blank on first page
        bot.setParameter("Q1:a", "Response 1");

        bot.submit();

        // verify contents of second page
        assertContains("Text Item with FIB blank display field: Response 1",
                bot.getPageText());
    }

    public void testTextItemTextOnly() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        // add form with FIB in one segment and text item in another
        FormBuilder form1 = builder.addForm("Form 1");
        form1.addText("Text Item");
        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testTextItemTextOnly");
        world.domain().projects().put(userProject);
        bot.go(userProject);

        assertContains("Text Item", bot.getPageText());
    }

    public void testTextItemFieldOnly() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        // add form with FIB in one segment and text item in another
        FormBuilder form1 = builder.addForm("Form 1");
        form1.addFibNoBlankAlt("FIB Item 1", 10, false);
        form1.addBreak();
        form1.addTextWithFields("<<Form 1:Q1:a>>");
        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testTextItemFieldOnly");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.setParameter("Q1:a", "Response 1");
        bot.submit();

        assertContains("Response 1", bot.getPageText());
    }

    public void testTextItemFontFieldOnly() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        // add form with FIB in one segment and text item in another
        FormBuilder form1 = builder.addForm("Form 1");
        form1.addFibNoBlankAlt("FIB Item 1", 10, false);
        form1.addBreak();
        form1.addFontTextWithFields("<<Form 1:Q1:a>>");
        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testTextItemFieldOnly");
        world.domain().projects().put(userProject);
        bot.go(userProject);
        bot.setParameter("Q1:a", "Response 1");
        bot.submit();

        assertContains("Response 1", bot.getPageText());
    }

    public void testMultipleDisplayFieldsShowFibResponses()
            throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        ProcessBlockBuilder process = builder.addProcess("Process 1");

        // add form with FIB in one segment and text item in another
        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addFib("FIB Item 1", 10, 10, 10);
        form1.addBreak();
        form1.addTextWithFields("Text Item with FIB blank display fields: ",
                "<<Form 1:Q1:a>>", " ", "<<Form 1:Q1:b>>", " ",
                "<<Form 1:Q1:c>>");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testFibBlankFields");
        world.domain().projects().put(userProject);
        bot.go(userProject);

        // fill in blanks on first page
        bot.setParameter("Q1:a", "Response 1");
        bot.setParameter("Q1:b", "Response 2");
        bot.setParameter("Q1:c", "Response 3");

        bot.submit();

        // verify contents of second page
        assertContains(
                "Text Item with FIB blank display fields: Response 1 Response 2 Response 3",
                bot.getPageText());
    }

    public void testDisplayFieldShowsMCResponse() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();

        ProcessBlockBuilder process = builder.addProcess("Process 1");

        // add form with MC in one segment and text item in another
        FormBuilder form1 = builder.addForm("Form 1", process);
        form1.addMc("FIB Item 1", "Choice A", "Choice B");
        form1.addBreak();
        form1.addTextWithFields("Text Item with MC display field: ",
                "<<Form 1:Q1>>");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testMCField");
        world.domain().projects().put(userProject);
        bot.go(userProject);

        // make selection on first page
        bot.setParameter("Q1", "b");

        bot.submit();

        // verify contents of second page
        assertContains("Text Item with MC display field: b", bot.getPageText());
    }

}
