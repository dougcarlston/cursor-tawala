package com.tawala.acceptance;

import com.scissor.webrobot.RobotException;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class ThemeTest extends AcceptanceTestCase {

    public void testDefaultStyle() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        FormBuilder formBuilder = builder.addForm("Form 1");
        formBuilder.addText("Text Item 1");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testPreviewForm");
        world.domain().projects().put(userProject);
        bot.go(userProject);

        assertContains("/css/project/default.css", bot.getPageText());
        assertContains("/css/project/default/project.css", bot.getPageText());
        assertContains("/css/project/form-layout-core.css", bot.getPageText());
        assertTrue("form-layout-core must load after theme project.css",
                bot.getPageText().indexOf("/css/project/default/project.css")
                        < bot.getPageText().indexOf("/css/project/form-layout-core.css"));
    }

    public void testAlternateStyle() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder("style2");
        FormBuilder formBuilder = builder.addForm("Form 1");
        formBuilder.addText("Text Item 1");

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "testPreviewForm");
        world.domain().projects().put(userProject);
        bot.go(userProject);

        assertContains("/css/project/default.css", bot.getPageText());
        assertContains("/css/project/style2/project.css", bot.getPageText());
        assertContains("/css/project/form-layout-core.css", bot.getPageText());
        assertTrue("form-layout-core must load after theme project.css",
                bot.getPageText().indexOf("/css/project/style2/project.css")
                        < bot.getPageText().indexOf("/css/project/form-layout-core.css"));
    }

}
