package com.tawala.userdomain;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.UserProject;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.ProjectLibraryTestCase;
import com.tawala.web.WorldInitializer;

public class UserDomainStorageTest extends ProjectLibraryTestCase {
	private static final String DOMAIN_NAME1 = "test";
	private static final String TOP_LEVEL_CATEGORY_NAME = "test category";
	private static final String PROJECT_NAME1 = "project1";

	private User projectOwner = UserTest.aUser("tester");
	private Collection<String> domainNamesToDelete = new ArrayList<String>();

	public UserDomainStorageTest() {
		setCategoryNamesToDelete(TOP_LEVEL_CATEGORY_NAME);
		addUserNameToDelete(projectOwner.getId());
		setProjectNamesToDelete(new String[] { PROJECT_NAME1 });
		domainNamesToDelete.add(DOMAIN_NAME1);
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(
				projectOwner);
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

	public void testCreate() throws IOException {
		UserDomain userDomain = new UserDomain();
		userDomain.setName(DOMAIN_NAME1);
		userDomain.setDisplayName("Some display name");
		userDomain.setDescription("Something");
		userDomain.setDescriptionCaption("Caption");
		userDomain.setSubtitle("Subtitle");
		userDomain.setSuggestionPrompt("Why don't you ...");
		userDomain.setTitle("My title");
		userDomain.setFeaturedSolutionsCaption("Featured solutions");

		List<LibraryProject> featuredProjects = new ArrayList<LibraryProject>();

		LibraryProject project1 = createDummyLibraryProject(PROJECT_NAME1);
		featuredProjects.add(project1);

		List<Long> projectIds = new ArrayList<Long>();
		projectIds.add(project1.getId());

		UserDomainStorage.createDomain(userDomain, projectIds);
		

		//--- Test getDomainByName
		userDomain = UserDomainStorage.getDomainNamed(DOMAIN_NAME1);
		assertEquals(featuredProjects, userDomain.getFeaturedProjects());
		
		//--- Tests getDomainById
		userDomain = UserDomainStorage.getDomainById(userDomain.getId());
		assertEquals(featuredProjects, userDomain.getFeaturedProjects());
	}

	private LibraryProject createDummyLibraryProject(String name)
			throws IOException {
		Category top = new Category(ProjectLibrary.COMMUNITY_LIBRARY, TOP_LEVEL_CATEGORY_NAME, "Top level");
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
