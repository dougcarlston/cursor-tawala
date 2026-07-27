package com.tawala.acceptance.customization;

import java.io.IOException;

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
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;

public class DisplayOfCustomizableProjectsInLibraryTest extends AcceptanceTestCase {
	private static final String CUSTOMIZATION_USER_NAME = "customizationuser";
	private String CUSTOMIZABLE_PROJECT_NAME = "Customizable Test";
	private String CATEGORY_NAME = "Customization Test";
	private String CUSTOMIZATION_FORM_NAME = Project.SETUP_WIZARD_FORM_NAMES
			.iterator().next();

	public DisplayOfCustomizableProjectsInLibraryTest() {
		setCategoryNamesToDelete(CATEGORY_NAME);
		setProjectNamesToDelete(CUSTOMIZABLE_PROJECT_NAME);
		addUserNameToDelete(CUSTOMIZATION_USER_NAME);
	}

	private LibraryProject buildCustomizableProject() throws IOException {
		LibraryProject result = buildProject(CUSTOMIZATION_FORM_NAME);
		assertTrue(result.getLatestVersion().getProject().isCustomizable());
		return result;
	}
	
	private LibraryProject buildNonCustomizableProject() throws IOException {
		LibraryProject result = buildProject("Form1");
		assertFalse(result.getLatestVersion().getProject().isCustomizable());
		return result;
	}

	private LibraryProject buildProject(String formName) throws IOException {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder customizationForm = projectBuilder
				.addForm(formName);
		customizationForm.addText("some text on the customization form");
		customizationForm.addFib("Project name:", "projectName", 50);
		
		FormBuilder userForm = projectBuilder.addForm("UserForm");
		userForm.addTextWithFields("Enter something for ", "<<ProjectName>>");
		userForm.addFib("Enter something:", "userentry", 40);
		
		Project project = projectBuilder.build();

		Category category = new Category(ProjectLibrary.SYSTEM_LIBRARY,
				CATEGORY_NAME, "Customization test");
		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject userProject = new UserProject(project, projectOwner,
				CUSTOMIZABLE_PROJECT_NAME);
		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		LibraryProject libraryProject = new LibraryProject(projectOwner.getId(), userProject);
		libraryProject.setFeatured(true);
		libraryProject.setFeaturedOrder(Integer.valueOf(-1));
		libraryProject
				.setShortDescription("Short description of the customization test project");
		libraryProject
				.setLongDescription("Long description of the customization test project");
		libraryProject.setCategory(category);

		ProjectLibraryService.onProjectSubmission(libraryProject, userProject);
		
		assertTrue(libraryProject.isVetted());
		
		return libraryProject;
	}

	public void testNavigationOfCustomizableProjects() throws RobotException, IOException {
		LibraryProject libraryProject = buildCustomizableProject();

		bot.go(WellKnown.urls.getHomePage());
		bot.followLink("projectDescription" + libraryProject.getId());

		assertContains("setPageTitle(\"Project Details\")", bot.getPageText());
		assertContains(libraryProject.getShortDescription(), bot.getPageText());
		assertContains(libraryProject.getLongDescription(), bot.getPageText());

		bot.followLink("startCustomization");

		assertContains("setPageTitle(\"Customize ", bot.getPageText());
	}

	public void testNavigationOfNonCustomizableProjects() throws RobotException, IOException {
		LibraryProject libraryProject = buildNonCustomizableProject();

		bot.go(WellKnown.urls.getHomePage());
		bot.followLink("projectDescription" + libraryProject.getId());

		assertContains("setPageTitle(\"Project Details\")", bot.getPageText());
		assertContains(libraryProject.getShortDescription(), bot.getPageText());
		assertContains(libraryProject.getLongDescription(), bot.getPageText());

		assertFalse(bot.hasLink("startCustomization"));
	}
}
