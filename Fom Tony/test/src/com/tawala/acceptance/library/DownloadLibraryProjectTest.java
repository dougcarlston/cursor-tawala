package com.tawala.acceptance.library;

import java.util.Date;

import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.DownloadLibraryProjectVersionController;

public class DownloadLibraryProjectTest extends AcceptanceTestCase {

    private static final String PROJECT_NAME = "Simple Poll";
    private static final String CATEGORY_NAME = "polls";
    
    public DownloadLibraryProjectTest() {
        setProjectNamesToDelete(new String[] { PROJECT_NAME });
        setCategoryNamesToDelete(new String[] { CATEGORY_NAME });
    }

    public void testDownloadLibraryProject() throws Exception {
        Category POLLS = new Category(ProjectLibrary.COMMUNITY_LIBRARY, CATEGORY_NAME, "Projects that...");
        ProjectLibraryService.createCategory(POLLS, projectOwner);

        UserProject userProject = new UserProject(ProjectBuilder
                .buildMinimalisticProject(), projectOwner, PROJECT_NAME);

        world.domain().projects().put(userProject);

        LibraryProject project = new LibraryProject(projectOwner.getId(),
                userProject);
        project.setLongDescription("Some long description.");
        project.setShortDescription("Simple poll");
        project.setSubmittedDate(new Date());
        project.setCategory(POLLS);
        ProjectLibraryService.onProjectSubmission(project, userProject);

        bot.go(constractNavigationToDownloadURL(project));

        // --- We got redirected
        assertTrue(bot.getPath().localPart().startsWith(WellKnown.urls.getLogin()));

        bot.logInAs(projectOwner.getId(), projectOwner.getPassword());

        bot.go(constractNavigationToDownloadURL(project));

        assertEquals("application/octet-stream", bot.lastResponse()
                .getContentType());
        assertContains("<project ", bot.lastResponse().getText());

        project = ProjectLibraryService.findProjectById(project.getId());
        assertEquals(1, project.getDownloadCount());
    }

    /**
     * @param project
     * @return
     */
    private String constractNavigationToDownloadURL(LibraryProject project) {
        return WellKnown.urls.getLibraryProjectVersionDownload()
                + "?"
                + DownloadLibraryProjectVersionController.PROJECT_ID_PARAMETER_NAME
                + "="
                + project.getId()
                + "&"
                + DownloadLibraryProjectVersionController.VERSION_ID_PARAMETER_NAME
                + "=" + project.getVersions().get(0).getId();
    }
}
