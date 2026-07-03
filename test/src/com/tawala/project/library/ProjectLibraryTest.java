package com.tawala.project.library;

import java.io.IOException;
import java.util.Calendar;
import java.util.Collection;
import java.util.Date;
import java.util.List;
import java.util.Random;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.library.event.ProjectAdded;
import com.tawala.project.library.event.ProjectCategoryChanged;
import com.tawala.project.library.event.ProjectNameChanged;
import com.tawala.project.library.event.ProjectShortDescriptionChanged;
import com.tawala.web.WorldInitializer;

public class ProjectLibraryTest extends ProjectLibraryTestCase {

	private static final String TO_BE_DELETED_PROJECT_NAME = "To be deleted";
	private static final String TOP_LEVEL_CATEGORY_NAME = "top level";
	private static final String SECOND_TIER_CATEGORY_NAME = "second tier";
	private static final String THIRD_TIER_CATEGORY_NAME = "third tier";
	private static final String TEST_CATEGORY_FOR_ORDERING = "A-test.";
	private static final String NEW_CATEGORY_NAME = "new category";
	private static final String TEST_PROJECT_ANOTHER_ONE = "Test project - Another one";
	private static final String TEST_PROJECT_NAME = "Test project";
	private static final String TEST_PROJECT_NEW_NAME = "Test Project - new name";
	private static final String TEST_CATEGORY_NAME = "test category";

	private User projectOwner = UserTest.aUser("tester");

	public ProjectLibraryTest() {
		setProjectNamesToDelete(new String[] { TEST_PROJECT_NAME,
				TEST_PROJECT_NEW_NAME, TEST_PROJECT_ANOTHER_ONE,
				TO_BE_DELETED_PROJECT_NAME });
		setCategoryNamesToDelete(new String[] { TEST_CATEGORY_NAME,
				NEW_CATEGORY_NAME, TEST_CATEGORY_FOR_ORDERING,
				TOP_LEVEL_CATEGORY_NAME, SECOND_TIER_CATEGORY_NAME,
				THIRD_TIER_CATEGORY_NAME });

		addUserNameToDelete(projectOwner.getId());
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		projectOwner.setStatus(Status.REGISTERED);
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(
				projectOwner);
	}

	/*
	 * Test method for
	 * 'com.tawala.project.library.ProjectLibrary.createProject(SubmittedProject)'
	 */
	public void testProjectCreationAndRetrieval() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);

		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = makeUserProject();
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject original = new LibraryProject("John Smith",
				deployedProject);
		original
				.setLongDescription("Some very long description submitted by John Smith.");
		original.setShortDescription("Simple poll");
		original.setSubmittedDate(new Date());
		original.setCategory(category);

		ProjectLibraryService.onProjectSubmission(original, deployedProject);

		assertNotSame(0, original.getId());

		LibraryProject loaded = ProjectLibraryService.findProjectById(original
				.getId());
		assertEquals(category, loaded.getCategory());

		Collection<ProjectChangeEvent> events = ProjectLibraryService
				.getProjectHistory(loaded.getId());
		assertEquals("History size", 1, events.size());
		assertEquals(ProjectAdded.class, events.iterator().next().getClass());

		// --- Updates
		User user = UserTest.aUser("bob", "abc");
		user.setAdministrator(true);

		loaded = ProjectLibraryService.findProjectById(original.getId());

		LibraryProject copy = loaded;
		copy.setShortDescription("new short description");

		ProjectLibraryService.onProjectUpdate(copy, user);

		events = ProjectLibraryService.getProjectHistory(original.getId());
		assertEquals("History size", 2, events.size());
		assertEquals(ProjectShortDescriptionChanged.class, events.iterator()
				.next().getClass());

		ProjectShortDescriptionChanged shortDescriptionChanged = (ProjectShortDescriptionChanged) events
				.iterator().next();
		assertEquals("Simple poll", shortDescriptionChanged.getDescription()
				.getArguments()[0]);
		assertEquals("new short description", shortDescriptionChanged
				.getDescription().getArguments()[1]);
		assertEquals("bob", shortDescriptionChanged.getUserId());

		// --- Name change
		LibraryProject existingProjectWithNewName = ProjectLibraryService
				.findProjectByName(TEST_PROJECT_NEW_NAME);
		if (existingProjectWithNewName != null)
			ProjectLibraryService
					.permanentlyDeleteProject(existingProjectWithNewName);

		loaded = ProjectLibraryService.findProjectById(copy.getId());

		copy = loaded;
		copy.setName(TEST_PROJECT_NEW_NAME);

		ProjectLibraryService.onProjectUpdate(copy, user);

		events = ProjectLibraryService.getProjectHistory(loaded.getId());
		assertEquals("History size", 3, events.size());
		assertEquals(ProjectNameChanged.class, events.iterator().next()
				.getClass());

		ProjectNameChanged nameChanged = (ProjectNameChanged) events.iterator()
				.next();
		assertEquals(TEST_PROJECT_NAME, nameChanged.getDescription()
				.getArguments()[0]);
		assertEquals(TEST_PROJECT_NEW_NAME, nameChanged.getDescription()
				.getArguments()[1]);
		assertEquals("bob", nameChanged.getUserId());

		// --- Category change
		Category newCategory = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				NEW_CATEGORY_NAME, "to test reassignment");

		ProjectLibraryService.createCategory(newCategory, projectOwner);

		loaded = ProjectLibraryService.findProjectById(copy.getId());

		copy = loaded;
		copy.setCategory(newCategory);

		ProjectLibraryService.onProjectUpdate(copy, user);

		loaded = ProjectLibraryService.findProjectById(original.getId());

		events = ProjectLibraryService.getProjectHistory(loaded.getId());
		assertEquals("History size", 4, events.size());
		assertEquals(ProjectCategoryChanged.class, events.iterator().next()
				.getClass());

		ProjectCategoryChanged categoryChanged = (ProjectCategoryChanged) events
				.iterator().next();
		assertEquals(TEST_CATEGORY_NAME, categoryChanged.getDescription()
				.getArguments()[0]);
		assertEquals(NEW_CATEGORY_NAME, categoryChanged.getDescription()
				.getArguments()[1]);
		assertEquals("bob", nameChanged.getUserId());

		// --- Deletion
		User joe = UserTest.aUser("joe");
		joe.setAdministrator(true);
		ProjectLibraryService.onProjectDelete(loaded.getId(), joe);

		loaded = ProjectLibraryService.findProjectById(original.getId());
		assertNotNull(loaded);
		assertTrue(loaded.isDeleted());
	}

	private UserProject makeUserProject() {
		return makeUserProject(TEST_PROJECT_NAME);
	}

	private UserProject makeUserProject(String name) {
		UserProject deployedProject = new UserProject(new ConfigElement(
				"<project name=\"" + name
						+ "\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
		return deployedProject;
	}

	public void testCategoryCreationAndRetrieval() {
		Category a = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_FOR_ORDERING, "First top level category");
		Category b = new Category(a, SECOND_TIER_CATEGORY_NAME,
				"Second level category");

		ProjectLibraryService.createCategory(a, projectOwner);
		ProjectLibraryService.createCategory(b, projectOwner);

		Collection<Category> topLevelCategories = ProjectLibraryService
				.getTopLevelCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY);

		assertTrue(topLevelCategories.contains(a));

		Category retrieved = ProjectLibraryService.findCategoryById(a.getId());

		assertNotNull(a.getSubcategories());
		assertEquals(1, retrieved.getSubcategories().size());
		assertEquals(b, retrieved.getSubcategories().iterator().next());

		Category found = ProjectLibraryService.findCategoryById(a.getId());
		assertNotNull(found);
		assertEquals(a, found);

		Category retrievedB = ProjectLibraryService.findCategoryById(b.getId());
		Category c = new Category(retrievedB, THIRD_TIER_CATEGORY_NAME,
				"Third level category");
		ProjectLibraryService.createCategory(c, projectOwner);

		retrievedB = ProjectLibraryService.findCategoryById(b.getId());
		assertEquals(1, retrievedB.getSubcategories().size());
		assertEquals(c, retrievedB.getSubcategories().iterator().next());
	}

	public void testCommentAddition() throws IOException {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "First top level category");

		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = makeUserProject();
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		project
				.setLongDescription("Some very long description submitted by John Smith.");
		project.setShortDescription("Simple poll");
		project.setSubmittedDate(new Date());
		project.setCategory(category);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		project = ProjectLibraryService.findProjectById(project.getId());

		Comment comment = new Comment("bob");
		comment.setText("So so.");

		ProjectLibraryService.onAddingProjectComments(project, comment);

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals("comment size", 1, project.getComments().size());
		comment = project.getComments().iterator().next();
		assertEquals("bob", comment.getUserId());
		assertEquals("So so.", comment.getText());
		assertTrue(comment.getId() > 0);

		// -- another comment
		comment = new Comment("john");
		comment.setText("Another one");

		ProjectLibraryService.onAddingProjectComments(project, comment);
		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals("comment size", 2, project.getComments().size());

		// -- and yet another comment
		comment = new Comment("jim");
		comment.setText("And yet another one");

		ProjectLibraryService.onAddingProjectComments(project, comment);

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals("comment size", 3, project.getComments().size());

		// -- and another comment to bring the rating to 5
		comment = new Comment("jane");
		comment.setText("And yet another one");

		ProjectLibraryService.onAddingProjectComments(project, comment);

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals("comment size", 4, project.getComments().size());
	}

	public void testVersionAddition() throws IOException {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "First top level category");

		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = makeUserProject();
		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		project
				.setLongDescription("Some very long description submitted by John Smith.");
		project.setShortDescription("Simple poll");
		project.setSubmittedDate(new Date());
		project.setCategory(category);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		project = ProjectLibraryService.findProjectById(project.getId());

		LibraryProjectVersion version = new LibraryProjectVersion(project, "bob",
				ProjectBuilder.buildMinimalisticProject());
		version.setText("Next one.");

		ProjectLibraryService.onAddingProjectVersion(deployedProject, project,
				version);

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals("version size", 2, project.getVersions().size());

		version = project.getVersions().get(0);
		assertEquals("bob", version.getUserId());
		assertEquals("Next one.", version.getText());

		assertTrue(version.getId() > 0);

		// -- another version
		version = new LibraryProjectVersion(project, "john", ProjectBuilder
				.buildMinimalisticProject());
		version.setText("Another one");

		ProjectLibraryService.onAddingProjectVersion(deployedProject, project,
				version);

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals("version size", 3, project.getVersions().size());

		// -- and yet another comment
		version = new LibraryProjectVersion(project, "jim", ProjectBuilder
				.buildMinimalisticProject());
		version.setText("And yet another one");

		ProjectLibraryService.onAddingProjectVersion(deployedProject, project,
				version);

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals("version size", 4, project.getVersions().size());
	}

	public void testRatingLogic() throws IOException {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "First top level category");

		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = makeUserProject();
		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		project
				.setLongDescription("Some very long description submitted by John Smith.");
		project.setShortDescription("Simple poll");
		project.setCategory(category);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		// --- Previous ratings by the same user are replaced.
		addRatingAndVerifyOverallRating(project.getId(), "bob", 2, 2);
		addRatingAndVerifyOverallRating(project.getId(), "bob", 5, 5);
		addRatingAndVerifyOverallRating(project.getId(), "bob", 1, 1);

		// --- Test ratings by other users.
		addRatingAndVerifyOverallRating(project.getId(), "jane", 3, 2);
		addRatingAndVerifyOverallRating(project.getId(), "jane", 5, 3);

		addRatingAndVerifyOverallRating(project.getId(), "jim", 4, 3);

		addRatingAndVerifyOverallRating(project.getId(), "jake", 4, 4);
	}

	private void addRatingAndVerifyOverallRating(long projectId, String userId,
			int ratingValue, int expectedRating) {
		LibraryProject project = ProjectLibraryService
				.findProjectById(projectId);

		Rating rating = new Rating(userId);
		rating.setValue(ratingValue);
		ProjectLibraryService.onRatingProject(project, rating);

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals("expected rating", expectedRating, project.getRating());

	}

	public void testOnProjectDownload() throws IOException {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "First top level category");

		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = makeUserProject();
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);

		project
				.setLongDescription("Some very long description submitted by John Smith.");
		project.setShortDescription("Simple poll");
		project.setSubmittedDate(new Date());
		project.setCategory(category);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		assertEquals("original download count", 0, project.getDownloadCount());

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals("immediately after load", 0, project.getDownloadCount());

		User bob = UserTest.aUser("bob");

		LibraryProjectVersion version1 = new LibraryProjectVersion(project, bob.getId(),
				ProjectBuilder.buildMinimalisticProject());
		version1.setText("Some comments by bob");
		ProjectLibraryService.onAddingProjectVersion(deployedProject, project,
				version1);

		LibraryProjectVersion version2 = new LibraryProjectVersion(project, bob.getId(),
				ProjectBuilder.buildMinimalisticProject());
		version2.setText("Some more comments by bob");
		ProjectLibraryService.onAddingProjectVersion(deployedProject, project,
				version2);

		ProjectLibraryService.onProjectDownloadByUser(project, version1, bob);
		assertEquals("after the first download", 1, project.getDownloadCount());

		ProjectLibraryService.onProjectDownloadByUser(project, version1, bob);
		assertEquals("after the second download by the same user", 1, project
				.getDownloadCount());

		User jane = UserTest.aUser("jane");
		ProjectLibraryService.onProjectDownloadByUser(project, version1, jane);
		assertEquals("after the download by another user", 2, project
				.getDownloadCount());

		ProjectLibraryService.onProjectDownloadByUser(project, version1, jane);
		assertEquals("after the second download by another user", 2, project
				.getDownloadCount());

		ProjectLibraryService.onProjectDownloadByUser(project, version2, jane);
		assertEquals("after of a different version by another user", 3, project
				.getDownloadCount());

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals("after another load", 3, project.getDownloadCount());
	}

	public void testProjectNameExists() throws IOException {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "First top level category");

		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = makeUserProject();
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		project
				.setLongDescription("Some very long description submitted by John Smith.");
		project.setShortDescription("Simple poll");
		project.setSubmittedDate(new Date());
		project.setCategory(category);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		assertTrue("Name found", ProjectLibraryService
				.projectNameExists(TEST_PROJECT_NAME));
		assertFalse("Name not found", ProjectLibraryService
				.projectNameExists("Some other name"));
	}

	public void testCommentDeletion() throws IOException {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "First top level category");

		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = makeUserProject();
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);

		project
				.setLongDescription("Some very long description submitted by John Smith.");
		project.setShortDescription("Simple poll");
		project.setSubmittedDate(new Date());
		project.setCategory(category);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		project = ProjectLibraryService.findProjectById(project.getId());

		Comment comment = new Comment("bob");
		comment.setText("So so.");

		ProjectLibraryService.onAddingProjectComments(project, comment);

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals("comment size", 1, project.getComments().size());
		comment = project.getComments().iterator().next();
		assertEquals("bob", comment.getUserId());
		assertEquals("So so.", comment.getText());
		assertTrue(comment.getId() > 0);

		ProjectLibraryService.onCommentDeletion(project, comment, "john");

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals("comment size", 0, project.getComments().size());
	}

	public void testProjectCounting() throws Exception {
		// --- Assign a new project to the third tier catetory.
		Category top = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "Top level");
		ProjectLibraryService.createCategory(top, projectOwner);

		Category secondTier = new Category(top, SECOND_TIER_CATEGORY_NAME,
				"Second tier");
		ProjectLibraryService.createCategory(secondTier, projectOwner);

		Category thirdTier = new Category(secondTier, THIRD_TIER_CATEGORY_NAME,
				"Third tier");
		ProjectLibraryService.createCategory(thirdTier, projectOwner);

		Category anotherTop = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				NEW_CATEGORY_NAME, "Top level");
		ProjectLibraryService.createCategory(anotherTop, projectOwner);

		UserProject deployedProject = makeUserProject();
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		project
				.setLongDescription("Some very long description submitted by John Smith.");
		project.setShortDescription("Simple poll");
		project.setSubmittedDate(new Date());

		project.setCategory(thirdTier);
		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		confirmCategoryProjectCount(top.getId(), 1);
		confirmCategoryProjectCount(secondTier.getId(), 1);
		confirmCategoryProjectCount(thirdTier.getId(), 1);
		confirmCategoryProjectCount(anotherTop.getId(), 0);

		// --- Reassign it to another top tier
		anotherTop = ProjectLibraryService.findCategoryById(anotherTop.getId());

		project = ProjectLibraryService.findProjectById(project.getId());
		project.setCategory(anotherTop);

		User bob = UserTest.aUser("bob");
		bob.setStatus(Status.REGISTERED);
		ProjectLibraryService.onProjectUpdate(project, bob);

		confirmCategoryProjectCount(top.getId(), 0);
		confirmCategoryProjectCount(secondTier.getId(), 0);
		confirmCategoryProjectCount(thirdTier.getId(), 0);
		confirmCategoryProjectCount(anotherTop.getId(), 1);

		// --- Add another project to the second tier
		secondTier = ProjectLibraryService.findCategoryById(secondTier.getId());

		LibraryProject exitingAnotherProject = ProjectLibraryService
				.findProjectByName(TEST_PROJECT_ANOTHER_ONE);
		if (exitingAnotherProject != null) {
			ProjectLibraryService
					.permanentlyDeleteProject(exitingAnotherProject);
		}

		deployedProject = new UserProject(new ConfigElement("<project name=\""
				+ TEST_PROJECT_ANOTHER_ONE
				+ "\" themePath=\"default\" format=\"1.3\" />"), projectOwner);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		project = new LibraryProject("John Smith", deployedProject);
		project.setLongDescription("Another project");
		project.setShortDescription("Something else");
		project.setSubmittedDate(new Date());
		project.setCategory(secondTier);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		confirmCategoryProjectCount(top.getId(), 1);
		confirmCategoryProjectCount(secondTier.getId(), 1);
		confirmCategoryProjectCount(thirdTier.getId(), 0);
		confirmCategoryProjectCount(anotherTop.getId(), 1);

		// --- Reassign the second project to the third tier.
		thirdTier = ProjectLibraryService.findCategoryById(thirdTier.getId());

		project = ProjectLibraryService.findProjectById(project.getId());
		project.setCategory(thirdTier);

		ProjectLibraryService.onProjectUpdate(project, bob);

		confirmCategoryProjectCount(top.getId(), 1);
		confirmCategoryProjectCount(secondTier.getId(), 1);
		confirmCategoryProjectCount(thirdTier.getId(), 1);
		confirmCategoryProjectCount(anotherTop.getId(), 1);

		// --- Test project deletion.
		project = ProjectLibraryService.findProjectById(project.getId());

		ProjectLibraryService.onProjectDelete(project.getId(), bob);

		confirmCategoryProjectCount(top.getId(), 0);
		confirmCategoryProjectCount(secondTier.getId(), 0);
		confirmCategoryProjectCount(thirdTier.getId(), 0);
		confirmCategoryProjectCount(anotherTop.getId(), 1);

		// --- Test project restore.
		project = ProjectLibraryService.findProjectById(project.getId());

		ProjectLibraryService.onRestoreProject(project.getId(), bob);

		confirmCategoryProjectCount(top.getId(), 1);
		confirmCategoryProjectCount(secondTier.getId(), 1);
		confirmCategoryProjectCount(thirdTier.getId(), 1);
		confirmCategoryProjectCount(anotherTop.getId(), 1);
	}

	private void confirmCategoryProjectCount(long categoryId, int count) {
		Category category = ProjectLibraryService.findCategoryById(categoryId);
		assertNotNull("category", category);

		assertEquals("project count", count, category.getProjectCount());
	}

	public void testGettingRecentChanges() throws Exception {
		Category top = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "Top level");
		ProjectLibraryService.createCategory(top, projectOwner);

		UserProject deployedProject = makeUserProject();
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

		Calendar calendar = Calendar.getInstance();
		calendar.add(Calendar.MINUTE, -2);
		Collection<LibraryChangeEvent> events = ProjectLibraryService
				.getChangesSince(calendar.getTime());
		assertTrue(events.size() > 0);

		events = ProjectLibraryService.getChangesByUserSince(projectOwner,
				calendar.getTime());
	}

	public void testGetCategoryWithProjects() throws IOException {
		Category top = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "Top level");
		ProjectLibraryService.createCategory(top, projectOwner);

		Category secondTier = new Category(ProjectLibraryService
				.findCategoryById(top.getId()), "Second tier",
				"No projects. Tests correct removal");
		ProjectLibraryService.createCategory(secondTier, projectOwner);

		UserProject deployedProject = makeUserProject();

		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		project
				.setLongDescription("Some very long description submitted by John Smith.");
		project.setShortDescription("Simple poll");
		project.setSubmittedDate(new Date());

		top = ProjectLibraryService.findCategoryById(top.getId());
		project.setCategory(top);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		top = ProjectLibraryService.findCategoryById(top.getId());

		Project runnableProject = ProjectBuilder.buildMinimalisticProject();
		UserProject userProject = new UserProject(runnableProject,
				projectOwner, TO_BE_DELETED_PROJECT_NAME);
		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		LibraryProject deletedProject = new LibraryProject("John Smith",
				userProject);
		deletedProject.setLongDescription("Blah");
		deletedProject.setShortDescription("This project will be deleted");
		deletedProject.setCategory(top);

		ProjectLibraryService.onProjectSubmission(deletedProject, userProject);

		// --- Test for lack of duplicates in the result (bug #270)
		Collection<Category> categories = ProjectLibraryService
				.getCategoriesWithProjectsFrom(ProjectLibrary.COMMUNITY_LIBRARY);
		assertNotNull(categories);

		int count = 0;
		for (Category category : categories) {
			if (category.equals(top)) {
				count++;
			}
		}
		assertEquals(1, count);

		// --- Delete the project and validate results
		ProjectLibraryService.onProjectDelete(deletedProject.getId(),
				projectOwner);

		categories = ProjectLibraryService
				.getCategoriesWithProjectsFrom(ProjectLibrary.COMMUNITY_LIBRARY);
		assertNotNull(categories);

		assertTrue(categories.contains(top));

		for (Category category : categories) {
			if (category.equals(top)) {
				assertNotNull(category.getProjects());
				assertEquals(1, category.getProjectCount());
				assertEquals(1, category.getProjects().size());
				assertEquals(project, category.getProjects().iterator().next());
			}
		}
	}

	public void testReverseRatingSorting() throws IOException {
		Category top = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "Top level");
		ProjectLibraryService.createCategory(top, projectOwner);

		UserProject deployedProject = makeUserProject();

		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject(projectOwner.getId(),
				deployedProject);
		project.setLongDescription("Some very long description.");
		project.setShortDescription("Simple poll");
		project.setSubmittedDate(new Date());
		project.setCategory(top);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		Random random = new Random(System.currentTimeMillis());
		for (int i = 0; i < 30; i++) {
			int ratingValue = Math.abs(random.nextInt()) % 5;

			Rating rating = new Rating("user" + i);
			rating.setValue(ratingValue);

			project = ProjectLibraryService.findProjectById(project.getId());
			ProjectLibraryService.onRatingProject(project, rating);
		}

		project = ProjectLibraryService.findProjectById(project.getId());
		int previousRating = 6;

		for (Rating rating : project.getRatingsByUsers().values()) {
			assertTrue("Previous rating is higher or equal",
					previousRating >= rating.getValue());
			previousRating = rating.getValue();
		}
	}

	public void testIncrementProjectTestDrive() throws IOException {
		Category defaultCategory = ProjectLibraryService
				.getOrCreateDefaultCategory(ProjectLibrary.COMMUNITY_LIBRARY);
		UserProject project = makeUserProject();
		WorldInitializer.getDefaultWorld().domain().projects().put(project);

		LibraryProject submittedProject = new LibraryProject(projectOwner
				.getId(), project);
		submittedProject.setCategory(ProjectLibraryService
				.findCategoryById(defaultCategory.getId()));
		submittedProject.setShortDescription("just a test");
		submittedProject.setLongDescription("Long description");

		ProjectLibraryService.onProjectSubmission(submittedProject, project);
		ProjectLibraryService.onProjectTestDrive(submittedProject);
		ProjectLibraryService.onProjectTestDrive(submittedProject);

		submittedProject = ProjectLibraryService
				.findProjectById(submittedProject.getId());

		assertEquals(2, submittedProject.getTestDriveCount());
	}

	public void testGetProjectsWithinCategory() throws IOException {
		Category top = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TOP_LEVEL_CATEGORY_NAME, "Top level");
		ProjectLibraryService.createCategory(top, projectOwner);

		Category secondTier = new Category(ProjectLibraryService
				.findCategoryById(top.getId()), SECOND_TIER_CATEGORY_NAME,
				"Second tier category");
		ProjectLibraryService.createCategory(secondTier, projectOwner);

		Category thirdTier = new Category(ProjectLibraryService
				.findCategoryById(secondTier.getId()),
				THIRD_TIER_CATEGORY_NAME, "Third tier category");
		ProjectLibraryService.createCategory(thirdTier, projectOwner);

		assertEquals(0, ProjectLibraryService.getAllProjectsWithinCategory(
				top.getId()).size());
		assertEquals(0, ProjectLibraryService.getAllProjectsWithinCategory(
				secondTier.getId()).size());
		assertEquals(0, ProjectLibraryService.getAllProjectsWithinCategory(
				thirdTier.getId()).size());

		addProjectToCategory(top, "project1");
		addProjectToCategory(secondTier, "project2");
		addProjectToCategory(thirdTier, "project3");
		addProjectToCategory(thirdTier, "project4");
		addProjectToCategory(thirdTier, "project5");

		assertEquals(5, ProjectLibraryService.getAllProjectsWithinCategory(
				top.getId()).size());
		assertEquals(4, ProjectLibraryService.getAllProjectsWithinCategory(
				secondTier.getId()).size());
		assertEquals(3, ProjectLibraryService.getAllProjectsWithinCategory(
				thirdTier.getId()).size());
	}

	private void addProjectToCategory(Category category, String name)
			throws IOException {
		UserProject project = makeUserProject(name);
		WorldInitializer.getDefaultWorld().domain().projects().put(project);
		project = WorldInitializer.getDefaultWorld().domain().projects()
				.getWithProjectRuntime(projectOwner.getId(), project.getName());

		LibraryProject submittedProject = new LibraryProject(projectOwner
				.getId(), project);
		submittedProject.setCategory(ProjectLibraryService
				.findCategoryById(category.getId()));
		submittedProject.setShortDescription("just a test");
		submittedProject.setLongDescription("Long description");

		ProjectLibraryService.onProjectSubmission(submittedProject, project);
	}

	public void testGetCountsOfProjectsClonedFromCustomizable() {
		List<LibraryProject> result = ProjectLibraryService
				.getClonedProjects();
		assertNotNull(result);
	}
}
