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

public class ViewRecentChangesTest extends AcceptanceTestCase {
    private static final String CATEGORY_NAME = "polls";
    private static final String PROJECT_NAME = "Simple poll";
    
    public ViewRecentChangesTest() {
        setProjectNamesToDelete(new String[] {PROJECT_NAME});
        setCategoryNamesToDelete(new String[] { CATEGORY_NAME });
    }

    public void testViewRecentChangesTest() throws RobotException, IOException {
        Category POLLS = new Category(ProjectLibrary.COMMUNITY_LIBRARY, CATEGORY_NAME, "Projects that...");
        ProjectLibraryService.createCategory(POLLS, projectOwner);

        UserProject deployedProject = new UserProject(
                ProjectBuilder.buildMinimalisticProject(),
                projectOwner, PROJECT_NAME);
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
        bot.go(WellKnown.urls.getLibraryRecentChanges());

        assertContains("<a href=\"" + WellKnown.urls.getLibraryProjectDetailView() + "?"  + ViewProjectDetailsController.PARAMETER_ID + "=" + project.getId() + "\">" + PROJECT_NAME + "</a>", bot.getPageText());
    }
}
