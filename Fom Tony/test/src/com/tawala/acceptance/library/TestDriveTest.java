package com.tawala.acceptance.library;

import java.io.IOException;
import java.util.Date;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.ViewProjectDetailsController;

public class TestDriveTest extends AcceptanceTestCase {
    private static final String FORM_NAME = "TestDriveTestForm";
	private static final String TEST_PROJECT_NAME = "TestProject";
    private static final String POLLS_CATEGORY_NAME = "polls";

    private LibraryProject project;
    
    public TestDriveTest() {
        setProjectNamesToDelete(new String[] { TEST_PROJECT_NAME });
        setCategoryNamesToDelete(new String[] {POLLS_CATEGORY_NAME});
    }

    @Override
    protected void setUp() throws Exception {
    	super.setUp();
        Category POLLS = new Category(ProjectLibrary.COMMUNITY_LIBRARY, POLLS_CATEGORY_NAME, "Projects that...");
        ProjectLibraryService.createCategory(POLLS, projectOwner);

        ProjectBuilder builder = new ProjectBuilder();
        FormBuilder formBuilder = builder.addForm(FORM_NAME);
        formBuilder.addText("This is the first form");

        Project deployedProject = builder.build();
        
        UserProject userProject = new UserProject(deployedProject, projectOwner, TEST_PROJECT_NAME);
        world.domain().projects().put(userProject);

        project = new LibraryProject("John Smith",
                userProject);
        project
                .setLongDescription("Some very long description submitted by John Smith.");
        project.setShortDescription("Simple poll");
        project.setSubmittedDate(new Date());
        project.setCategory(POLLS);

        ProjectLibraryService.onProjectSubmission(project, userProject);

        bot.logInAs(projectOwner);
        bot.go(WellKnown.urls.getLibraryProjectDetailView() + "?"
                + ViewProjectDetailsController.PARAMETER_ID + "="
                + project.getId());
        assertContains(WellKnown.urls.getLibraryProjectDetailView(), bot
                .getPath().localPart());

        assertContains(TEST_PROJECT_NAME, bot.getPageText());
    }

    //TODO: test disabled
    public void testOneClickTestDrive() throws RobotException, IOException {
    	/*
        bot.logInAs(projectOwner.getId(), projectOwner.getPassword());
        String linkId = "oneClickTestDriveLink";
        bot.followLink(linkId);
        assertContains("This is the first form", bot.lastResponse().getText());
        */
    }
    
    //TODO: test disabled
    public void DISABLEDtestVersionTestDrive() throws RobotException, IOException {
        bot.logInAs(projectOwner.getId(), projectOwner.getPassword());
        String linkId = "testDriveVersion" + project.getVersions().get(0).getId();
        bot.followLink(linkId);
        assertContains(FORM_NAME, bot.lastResponse().getText());

        bot.followLink("testDriveLink1");
        assertContains("This is the first form", bot.lastResponse().getText());
    }
}
