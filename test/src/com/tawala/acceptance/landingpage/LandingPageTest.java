package com.tawala.acceptance.landingpage;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.List;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.domain.User;
import com.tawala.project.UserProject;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.userdomain.UserDomain;
import com.tawala.userdomain.UserDomainStorage;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.general.LandingPageController;

public class LandingPageTest extends AcceptanceTestCase {
	private static final String TESTUSER_NAME = "testuser";
	private static final String DOMAIN_NAME1 = "test";
	private static final String TOP_LEVEL_CATEGORY_NAME = "test category";
	private static final String PROJECT_NAME1 = "project1";
	private static final String PROJECT_NAME2 = "project2";
	private static final String PROJECT_NAME3 = "project3";
	private static final String PROJECT_NAME4 = "project4";
	private static final String PROJECT_NAME5 = "project5";

	private Collection<String> domainNamesToDelete = new ArrayList<String>();

	public LandingPageTest() {
		setCategoryNamesToDelete(TOP_LEVEL_CATEGORY_NAME);
		setProjectNamesToDelete(new String[] { PROJECT_NAME1 });
		addUserNameToDelete(TESTUSER_NAME);
		domainNamesToDelete.add(DOMAIN_NAME1);
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		cleanUpDomains();
	}

	@Override
	protected void tearDown() throws Exception {
		cleanUpDomains();
		super.tearDown();
	}

	private void cleanUpDomains() {
		for (String domainName : domainNamesToDelete) {
			UserDomainStorage.deleteDomainNamed(domainName);
		}
	}

	public void testLandingPage() throws IOException, RobotException {		
		UserDomain userDomain = new UserDomain();
		userDomain.setName(DOMAIN_NAME1);
		userDomain.setDisplayName("domain display name");
		userDomain.setDescription("Domain description");
		userDomain.setDescriptionCaption("domain description caption");
		userDomain.setSubtitle("Subtitle of this domain");
		userDomain.setSuggestionPrompt("Why don't you ...");
		userDomain.setTitle("domain main title");
		userDomain.setFeaturedSolutionsCaption("domain specific featured solutions caption");

		List<LibraryProject> featuredProjects = new ArrayList<LibraryProject>();

		LibraryProject project1 = createDummyLibraryProject(PROJECT_NAME1);
		featuredProjects.add(project1);
		LibraryProject project2 = createDummyLibraryProject(PROJECT_NAME2);
		featuredProjects.add(project2);
		LibraryProject project3 = createDummyLibraryProject(PROJECT_NAME3);
		featuredProjects.add(project3);
		LibraryProject project4 = createDummyLibraryProject(PROJECT_NAME4);
		featuredProjects.add(project4);
		LibraryProject project5 = createDummyLibraryProject(PROJECT_NAME5);
		featuredProjects.add(project5);

		List<Long> projectIds = new ArrayList<Long>();
		projectIds.add(project1.getId());
		projectIds.add(project2.getId());
		projectIds.add(project3.getId());
		projectIds.add(project4.getId());
		projectIds.add(project5.getId());

		UserDomainStorage.createDomain(userDomain, projectIds);

		bot.go(WellKnown.urls.getLandingPagePrefix() + "/" + userDomain.getName());
		
//		assertContains("If you would like to see other Web apps for " + userDomain.getDisplayName(), bot.getPageText());
		assertContains(userDomain.getDescription(), bot.getPageText());
		assertContains(userDomain.getDescriptionCaption(), bot.getPageText());
		assertContains(userDomain.getFeaturedSolutionsCaption(), bot.getPageText());
		assertContains(userDomain.getSubtitle(), bot.getPageText());
		assertContains(userDomain.getTitle(), bot.getPageText());
		// First 4 projects do not show long descriptions
		assertContains(project5.getLongDescription(), bot.getPageText());
		
		String cookieValue = bot.getCookieValue(LandingPageController.ORIGINAL_LANDING_DOMAIN_COOKIE_NAME);
		assertNotNull(cookieValue);
		assertEquals(userDomain.getName(), cookieValue);
		
		bot.followLink("linkToInitialSetup");
		WebForm form = bot.getForm("registrationForm");
		form.setParameter("emailAddress", "test@example.org");
		form.setParameter("user.id", TESTUSER_NAME);
		form.setParameter("password", "abc");
		form.setParameter("repeatedPassword", "abc");
		
		bot.submit(form);
		
		assertContains("You have successfully created a Tawala Account", bot.getPageText());
		
		User testUser = world.domain().users().get(TESTUSER_NAME);
		assertNotNull(testUser);
		assertEquals(userDomain.getName(), testUser.getOriginalDomain());
	}

	private LibraryProject createDummyLibraryProject(String name)
			throws IOException {
		Category top = new Category(ProjectLibrary.SYSTEM_LIBRARY, TOP_LEVEL_CATEGORY_NAME, "Top level");
		ProjectLibraryService.createCategory(top, projectOwner);

		UserProject deployedProject = makeUserProject(name);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject(projectOwner.getId(),
				deployedProject);
		project.setLongDescription("Some very long description submitted by "
				+ projectOwner.getId());
		project.setShortDescription("Simple poll");
		project.setSubmittedDate(new Date());

		project.setCategory(top);
		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		return project;
	}

	private UserProject makeUserProject(String name) {
		UserProject deployedProject = new UserProject(new ConfigElement(
				"<project name=\"" + name
						+ "\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
		return deployedProject;
	}

}
