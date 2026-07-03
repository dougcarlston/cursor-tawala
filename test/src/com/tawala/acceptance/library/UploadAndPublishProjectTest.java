package com.tawala.acceptance.library;

import java.io.IOException;
import java.util.Calendar;
import java.util.Collection;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.xml.sax.SAXException;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.parsing.HTMLParserFactory;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.acceptance.ApiTest;
import com.tawala.project.UserProject;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryChangeEvent;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectChangeEvent;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.ViewProjectDetailsController;

public class UploadAndPublishProjectTest extends AcceptanceTestCase {

    private static final String PROJECT_NAME = "Simple poll";
    private static final String POLLS_CATEGORY_NAME = "polls";

    public UploadAndPublishProjectTest() {
        setCategoryNamesToDelete(new String[] { POLLS_CATEGORY_NAME });
        setProjectNamesToDelete(new String[] { PROJECT_NAME });
    }

    public void testUploadAndPublishProject() throws Exception {
    	WorldInitializer.getDefaultWorld().domain().users().onUserUpgradeToFullyRegistered(projectOwner);
    	
    	HTMLParserFactory.useJTidyParser();

        Category POLLS = new Category(ProjectLibrary.COMMUNITY_LIBRARY, POLLS_CATEGORY_NAME, "Projects that...");
        ProjectLibraryService.createCategory(POLLS, projectOwner);

        // --- Deploy a new version using Client API.
        postProject("<project name=\"" + PROJECT_NAME
                + "\" themePath=\"NEW THEME\" format=\"1.3\" />", projectOwner
                .getId(), projectOwner.getPassword());

        UserProject deployedProject = world.domain().projects().getWithProjectRuntime(projectOwner.getId(), PROJECT_NAME);
        assertNotNull(deployedProject);

        bot.logInAs(projectOwner.getId(), projectOwner.getPassword());
        bot.go(WellKnown.urls.getProjectManagerView());
        bot.followLink("projectDetailsLink1");

		// --- Extract the link from the page. It's stored in a JavaScript
		// variable.
		Pattern pattern = Pattern.compile(
				"var linkToPublishAsNew = '([^']*)'", Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());

		String link = matcher.group(1);
		link = link.replace("\\", ""); //--- this removes Javascript escaping.
		bot.go(link);

        WebForm form = bot.getForm("publishProjectForm");

        assertEquals(PROJECT_NAME, form.getParameterValue("project.name"));

        form.setParameter("project.shortDescription", "some short description");
        form.setParameter("project.longDescription", "some long description");
        form.setParameter("project.category", Long.toString(POLLS.getId()));

        bot.submit(form, "submit");

        assertContains("Your project has been published to the Library!", bot
                .getPageText());

        Calendar calendar = Calendar.getInstance();
        calendar.add(Calendar.MINUTE, -1);
        Collection<LibraryChangeEvent> changes = ProjectLibraryService
                .getChangesSince(calendar.getTime());

        long projectId = ((ProjectChangeEvent) changes.iterator().next())
                .getProjectId();

        LibraryProject project = ProjectLibraryService.findProjectById(projectId);

        bot.go(WellKnown.urls.getLibraryProjectDetailView() + "?"
                + ViewProjectDetailsController.PARAMETER_ID + "="
                + project.getId());

        bot
                .followLink("downloadVersion"
                        + project.getVersions().get(0).getId());

        assertEquals("content type", "application/octet-stream", bot
                .lastResponse().getContentType());

        assertEquals("project", deployedProject.getProject()
                .getProjectXmlDefinition(), bot.lastResponse().getText());
    }

    private void postProject(String contents, String userId, String password)
            throws IOException, SAXException {
        getResponse(ApiTest.postXml("<request type=\"" + "uploadProject"
                + "\" protocol=\"1.0\">\n" + "<credentials user=\"" + userId
                + "\" password=\"" + password + "\" />\n" + contents
                + "</request>"), client);
        assertEquals(200, response.getResponseCode());
    }
}
