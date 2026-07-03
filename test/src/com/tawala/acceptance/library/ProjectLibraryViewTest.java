package com.tawala.acceptance.library;

import java.io.IOException;
import java.util.Date;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.WebLink;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;

public class ProjectLibraryViewTest extends AcceptanceTestCase {
    private Category POLLS;
    private LibraryProject project;

    private static final String POLLS_CATEGORY_NAME = "polls";
    private static final String PROJECT_NAME = "Simple Poll";

    public ProjectLibraryViewTest() {
        setCategoryNamesToDelete(new String[] { POLLS_CATEGORY_NAME });
        setProjectNamesToDelete(new String[] { PROJECT_NAME });
    }

    public void setUp() throws Exception {
        super.setUp();

        POLLS = new Category(ProjectLibrary.SYSTEM_LIBRARY, POLLS_CATEGORY_NAME, "Projects that...");
        ProjectLibraryService.createCategory(POLLS, projectOwner);

        deleteLibraryProjectNamed("Simple poll");

        UserProject deployedProject = new UserProject(ProjectBuilder
                .buildMinimalisticProject(), projectOwner, PROJECT_NAME);
        world.domain().projects().put(deployedProject);

        project = new LibraryProject(projectOwner.getId(), deployedProject);
        project
                .setLongDescription("Some very long description submitted by John Smith.");
        project.setShortDescription("Simple poll");
        project.setSubmittedDate(new Date());
        project.setCategory(POLLS);
        ProjectLibraryService.onProjectSubmission(project, deployedProject);
    }

    @Override
    protected void tearDown() throws Exception {
        deleteLibraryProjectNamed("Simple poll");

        super.tearDown();
    }

    public void testViewLibrary() throws RobotException, IOException {
    	bot.logInAs(projectOwner);
        bot.go(WellKnown.urls.getLibrarySearch());

        assertContains("Simple poll", bot.getPageText());

        bot.followLink("projectDetailsLink" + project.getId());

        assertContains("<span class=\"label\">Original Author:</span><span>" + project.getAuthorId() + "</span>"
                , bot.getPageText());
        assertContains(project.getShortDescription(), bot.getPageText());
        assertContains(project.getLongDescription(), bot.getPageText());

        WebLink linkToEdit = bot.getLink("linkToEditProject");
		// --- Test with insufficient permissions.
        assertNull(linkToEdit);

        bot.logOut();
        
        projectOwner.setAdministrator(true);
        WorldInitializer.getDefaultWorld().domain().users().addOrSave(projectOwner);
        
        bot.logInAs(projectOwner);
        
        bot.go(WellKnown.urls.getLibrarySearch());
        bot.followLink("projectDetailsLink" + project.getId());

        bot.followLink("linkToEditProject");

        WebForm form = bot.getForm("editProjectForm");
        bot.submit(form);
        assertContains(project.getShortDescription(), bot.getPageText());

        // --- Test submission.
        bot.followLink("linkToEditProject");

        String newDescription = "New Short Description";
        form.setParameter("project.shortDescription", newDescription);

        bot.submit(form);

        assertContains(newDescription, bot.getPageText());
        assertContains(WellKnown.urls.getLibraryProjectDetailView(), bot
                .getPath().localPart());
    }

    public void testViewHistory() throws RobotException {
    	WorldInitializer.getDefaultWorld().domain().users().onUserUpgradeToFullyRegistered(projectOwner);
        bot.logInAs(projectOwner.getId(), projectOwner.getPassword());
        bot.go(WellKnown.urls.getLibrarySearch());
        bot.followLink("projectDetailsLink" + project.getId());

        WebForm form = bot.getForm("projectHistoryForm");
        bot.submit(form);

        assertContains("Added project to the library.", bot.getPageText());
    }
}
