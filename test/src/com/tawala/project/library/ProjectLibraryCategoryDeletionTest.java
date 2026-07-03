package com.tawala.project.library;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;

public class ProjectLibraryCategoryDeletionTest extends ProjectLibraryTestCase {
	private static final String[] TEST_PROJECT_NAMES = new String[] {
			"Test project", "Test project 2" };

	private static final String[] TEST_CATEGORY_NAMES = new String[] {
			"A- name is important for sorting", "second tier category" };

	private Category a;
	private Category aa;

	private LibraryProject p1;
	private LibraryProject p2;
	private User projectOwner = UserTest.aUser("tester");

	public ProjectLibraryCategoryDeletionTest() {
		setProjectNamesToDelete(TEST_PROJECT_NAMES);
		setCategoryNamesToDelete(TEST_CATEGORY_NAMES);
		addUserNameToDelete(projectOwner.getId());
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see com.tawala.db4o.DB4OTestCase#setUp()
	 */
	@Override
	protected void setUp() throws Exception {
		super.setUp();

		ProjectLibraryService
				.resetCategoryProjectCount(ProjectLibrary.COMMUNITY_LIBRARY);

		a = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAMES[0], "First top level category");
		aa = new Category(a, TEST_CATEGORY_NAMES[1], "Second level category");

		ProjectLibraryService.createCategory(a, projectOwner);
		ProjectLibraryService.createCategory(aa, projectOwner);

		WorldInitializer.getDefaultWorld().domain().users().addOrSave(
				projectOwner);

		UserProject deployedProject = new UserProject(new ConfigElement(
				"<project name=\"" + TEST_PROJECT_NAMES[0]
						+ "\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		p1 = new LibraryProject("John Smith", deployedProject);
		p1
				.setLongDescription("Some very long description submitted by John Smith.");
		p1.setShortDescription("Simple poll");
		p1.setSubmittedDate(new Date());
		p1.setCategory(ProjectLibraryService.findCategoryById(aa.getId()));

		ProjectLibraryService.onProjectSubmission(p1, deployedProject);

		deployedProject = new UserProject(new ConfigElement("<project name=\""
				+ TEST_PROJECT_NAMES[1]
				+ "\" themePath=\"default\" format=\"1.3\" />"), projectOwner);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		p2 = new LibraryProject("John Smith", deployedProject);
		p2
				.setLongDescription("Some very long description submitted by John Smith.");
		p2.setShortDescription("Simple poll");
		p2.setSubmittedDate(new Date());
		p2.setCategory(ProjectLibraryService.findCategoryById(aa.getId()));

		ProjectLibraryService.onProjectSubmission(p2, deployedProject);
	}

	public void testDeletionAndAssignmentToUpperCategory() throws Exception {
		Collection<LibraryProject> projects = ProjectLibraryService
				.findUndeletedProjectsByCategory(aa);
		assertEquals("retrieved project count", 2, projects.size());
		Collection<LibraryProject> projectsCopy = new ArrayList<LibraryProject>(
				projects);
		assertTrue("contains p1", projectsCopy.contains(p1));
		assertTrue("contains p2", projectsCopy.contains(p2));

		int previousTopLevelCategoryCount = ProjectLibraryService
				.getTopLevelCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY)
				.size();
		Category retreivedAA = ProjectLibraryService.findCategoryById(aa
				.getId());
		ProjectLibraryService
				.deleteCategory(retreivedAA, UserTest.aUser("bob"));

		Collection<Category> topLevelCategories = ProjectLibraryService
				.getTopLevelCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY);
		assertEquals(previousTopLevelCategoryCount, topLevelCategories.size());
		Category retrieved = ProjectLibraryService.findCategoryById(a.getId());
		assertEquals(a, retrieved);

		projects = ProjectLibraryService
				.findUndeletedProjectsByCategory(retrieved);
		assertEquals(2, projects.size());

		projectsCopy = new ArrayList<LibraryProject>(projects);
		assertTrue(projectsCopy.contains(p1));
		assertTrue(projectsCopy.contains(p2));

		validateCorrectProjectCount();
	}

	public void testDeletionAndAssignmentToOtherCategory() throws Exception {
		Category defaultCategory = ProjectLibraryService
				.getOrCreateDefaultCategory(ProjectLibrary.COMMUNITY_LIBRARY);
		int previousTopLevelCategoryCount = ProjectLibraryService
				.getTopLevelCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY)
				.size();
		int previousDefaultCategoryProjectCount = ProjectLibraryService
				.findCategoryById(defaultCategory.getId()).getProjectCount();

		Category retreivedA = ProjectLibraryService.findCategoryById(a.getId());
		ProjectLibraryService.deleteCategory(retreivedA, UserTest.aUser("bob"));

		Collection<Category> topLevelCategories = ProjectLibraryService
				.getTopLevelCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY);
		assertEquals(previousTopLevelCategoryCount - 1, topLevelCategories
				.size());

		Category retrieved = ProjectLibraryService
				.findCategoryById(defaultCategory.getId());

		assertEquals(previousDefaultCategoryProjectCount + 2, retrieved
				.getProjectCount());

		Collection<LibraryProject> projects = ProjectLibraryService
				.findUndeletedProjectsByCategory(retrieved);

		assertTrue(projects.contains(p1));
		assertTrue(projects.contains(p2));

		validateCorrectProjectCount();
	}

	public void testDeletionOfCategoryWithDeletedProjects() throws Exception {
		Category defaultCategory = ProjectLibraryService
				.getOrCreateDefaultCategory(ProjectLibrary.COMMUNITY_LIBRARY);

		int previousTopLevelCategoryCount = ProjectLibraryService
				.getTopLevelCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY)
				.size();
		int previousDefaultCategoryProjectCount = ProjectLibraryService
				.findCategoryById(defaultCategory.getId()).getProjectCount();

		User user = UserTest.aUser("bob");
		user.setStatus(Status.REGISTERED);
		ProjectLibraryService
				.onProjectDelete(p1.getId(), user);

		Category retreivedA = ProjectLibraryService.findCategoryById(a.getId());
		ProjectLibraryService.deleteCategory(retreivedA, user);

		Collection<Category> topLevelCategories = ProjectLibraryService
				.getTopLevelCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY);
		assertEquals(previousTopLevelCategoryCount - 1, topLevelCategories
				.size());

		Category retrieved = ProjectLibraryService
				.findCategoryById(defaultCategory.getId());

		Collection<LibraryProject> projects = ProjectLibraryService
				.findUndeletedProjectsByCategory(retrieved);
		assertEquals(previousDefaultCategoryProjectCount + 1, retrieved
				.getProjectCount());

		assertTrue(projects.contains(p2));

		p1 = ProjectLibraryService.findProjectById(p1.getId());
		assertEquals(defaultCategory, p1.getCategory());
		assertTrue(p1.isDeleted());

		validateCorrectProjectCount();
	}

	private void validateCorrectProjectCount() {
		for (Category category : ProjectLibraryService
				.getAllCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY)) {
			assertEquals(category.getProjectCount(),
					calculateProjectCount(category));
		}
	}

	private int calculateProjectCount(Category category) {
		int totalCount = ProjectLibraryService.findUndeletedProjectsByCategory(
				category).size();

		for (Category child : category.getSubcategories()) {
			totalCount += calculateProjectCount(child);
		}

		return totalCount;

	}
}
