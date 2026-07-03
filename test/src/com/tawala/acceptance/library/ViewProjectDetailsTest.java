package com.tawala.acceptance.library;

import java.io.IOException;
import java.util.Date;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.ViewProjectDetailsController;

public class ViewProjectDetailsTest extends AcceptanceTestCase {
    private static final String CATEGORY_NAME = "polls";
    private static final String PROJECT_NAME = "Simple poll";

    public ViewProjectDetailsTest() {
        setProjectNamesToDelete(new String[] { PROJECT_NAME });
        setCategoryNamesToDelete(new String[] { CATEGORY_NAME });
    }

    public void testViewProjectDetails() throws RobotException, IOException {
        Category POLLS = new Category(ProjectLibrary.COMMUNITY_LIBRARY, CATEGORY_NAME, "Projects that...");
        ProjectLibraryService.createCategory(POLLS, projectOwner);

        UserProject deployedProject = new UserProject(ProjectBuilder
                .buildMinimalisticProject(), projectOwner, "Simple poll");
        world.domain().projects().put(deployedProject);

        LibraryProject project = new LibraryProject(projectOwner.getId(),
                deployedProject);
        project
                .setLongDescription("Some very long description submitted by John Smith.");
        project.setShortDescription(PROJECT_NAME);
        project.setSubmittedDate(new Date());
        project.setCategory(POLLS);
        ProjectLibraryService.onProjectSubmission(project, deployedProject);

        bot.logInAs(projectOwner);
        bot.go(WellKnown.urls.getLibraryProjectDetailView());
        // --- We got redirected
        assertEquals(WellKnown.urls.getLibrarySearch(), bot.getPath().localPart());

        bot.go(WellKnown.urls.getLibraryProjectDetailView() + "?"
                + ViewProjectDetailsController.PARAMETER_ID + "="
                + "some+garbage");
        // --- We got redirected
        assertEquals(WellKnown.urls.getLibrarySearch(), bot.getPath().localPart());

        bot.go(WellKnown.urls.getLibraryProjectDetailView() + "?"
                + ViewProjectDetailsController.PARAMETER_ID + "="
                + project.getId());
        assertContains(WellKnown.urls.getLibraryProjectDetailView(), bot
                .getPath().localPart());

        assertContains("Comments", bot.getPageText());
    }
}
