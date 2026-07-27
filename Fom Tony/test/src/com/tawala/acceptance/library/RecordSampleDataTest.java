package com.tawala.acceptance.library;

import java.io.IOException;
import java.util.Date;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.Reference;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.ViewProjectDetailsController;

public class RecordSampleDataTest extends AcceptanceTestCase {
    private static final String UGLY_NAME_WITH_VARIOUS_SPECIAL_CHARACTERS = "Ugly name with various special characters <> # xxx \" ' ";
    private static final String TEST_PROJECT_NAME = "TestProject";
    private static final String POLLS_CATEGORY_NAME = "polls";

    public RecordSampleDataTest() {
        setProjectNamesToDelete(new String[] { TEST_PROJECT_NAME, UGLY_NAME_WITH_VARIOUS_SPECIAL_CHARACTERS });
        setCategoryNamesToDelete(new String[] {POLLS_CATEGORY_NAME});
    }

    public void testSampleDataRecording() throws RobotException, IOException {
    	WorldInitializer.getDefaultWorld().domain().users().onUserUpgradeToFullyRegistered(projectOwner);
    	
        Category POLLS = new Category(ProjectLibrary.COMMUNITY_LIBRARY, POLLS_CATEGORY_NAME, "Projects that...");
        ProjectLibraryService.createCategory(POLLS, projectOwner);

        ProjectBuilder builder = new ProjectBuilder();
        FormBuilder formBuilder = builder.addForm("Form1");
        formBuilder.addText("This is the first form");
        FibBuilder fibBuilder = formBuilder.addFib("question1");
        fibBuilder.addBlank("question1");

        Project deployedProject = builder.build();
        
        UserProject userProject = new UserProject(deployedProject, projectOwner, TEST_PROJECT_NAME);
        world.domain().projects().put(userProject);

        LibraryProject project = new LibraryProject("John Smith",
                userProject);
        project
                .setLongDescription("Long description");
        project.setShortDescription("Simple poll");
        project.setSubmittedDate(new Date());
        project.setCategory(POLLS);
        project.setName(UGLY_NAME_WITH_VARIOUS_SPECIAL_CHARACTERS);

        ProjectLibraryService.onProjectSubmission(project, userProject);
        
        bot.logInAs(projectOwner.getId(), projectOwner.getPassword());

        bot.go(WellKnown.urls.getLibraryProjectDetailView() + "?"
                + ViewProjectDetailsController.PARAMETER_ID + "="
                + project.getId());

        assertContains(WellKnown.urls.getLibraryProjectDetailView(), bot
                .getPath().localPart());

        assertContains("Ugly name", bot.getPageText());
        
        String linkId = "sampleData" + project.getVersions().get(0).getId();
        bot.followLink(linkId);
        
        assertContains("Form1", bot.lastResponse().getText());
        assertContains("No data recorded", bot.lastResponse().getText());

        Pattern pattern = Pattern.compile("addSampleData\\('([^']*)'\\);");
        Matcher matcher = pattern.matcher(bot.lastResponse().getText());
        
        matcher.find();
        String urlToAddSampleData = matcher.group(1);
        
        bot.go(urlToAddSampleData);
        assertContains("This is the first form", bot.lastResponse().getText());
        
        WebForm form = bot.getForm(0);
        form.setParameter("question1", "abc");
        bot.submit(form);

        project = ProjectLibraryService.findProjectById(project.getId());
        
        Project versionProject = project.getVersions().get(0).getProject();
        List<FormSubmission> formSubmissions = WorldInitializer.getDefaultWorld().domain().storedData().responsesFor(versionProject, "Form1");
        assertNotNull(formSubmissions);
        assertEquals(1, formSubmissions.size());
        
        FormSubmission formSubmission = formSubmissions.get(0);
        assertEquals("abc", formSubmission.getValue(new Reference("question1")).toString());
    }
}
