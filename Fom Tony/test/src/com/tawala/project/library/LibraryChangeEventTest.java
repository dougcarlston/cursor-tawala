package com.tawala.project.library;

import java.util.Calendar;
import java.util.Collection;
import java.util.Date;
import java.util.Iterator;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.UserProject;
import com.tawala.project.library.event.CategoryAddedEvent;
import com.tawala.project.library.event.CategoryDeletedEvent;
import com.tawala.project.library.event.CategoryNameChangedEvent;
import com.tawala.project.library.event.CategoryParentChangedEvent;
import com.tawala.project.library.event.CommentAdded;
import com.tawala.project.library.event.CommentDeleted;
import com.tawala.project.library.event.CommentRestored;
import com.tawala.project.library.event.ProjectCategoryChanged;
import com.tawala.project.library.event.ProjectDeleted;
import com.tawala.project.library.event.ProjectLongDescriptionChanged;
import com.tawala.project.library.event.ProjectNameChanged;
import com.tawala.project.library.event.ProjectShortDescriptionChanged;
import com.tawala.web.WorldInitializer;

public class LibraryChangeEventTest extends ProjectLibraryTestCase {
	private static final String ANOTHER_CATEGORY_NAME = "another category";

	private static final String TEST_CATEGORY_NAME = "test category";

	private static final String[] PROJECT_NAMES = new String[] {
			"Test project", "Test Project - original name",
			"Test Project - New name" };

	private static final String[] CATEGORY_NAMES = new String[] {
			TEST_CATEGORY_NAME, ANOTHER_CATEGORY_NAME };

	private User projectOwner = UserTest.aUser("tester");

	public LibraryChangeEventTest() {
		setProjectNamesToDelete(PROJECT_NAMES);
		setCategoryNamesToDelete(CATEGORY_NAMES);
		addUserNameToDelete(projectOwner.getId());
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		projectOwner.setStatus(Status.REGISTERED);
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(
				projectOwner);
	}

	public void testEventRetrieval() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = new UserProject(
				new ConfigElement(
						"<project name=\"Test project\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
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

		LibraryChangeEvent event = ProjectLibraryService.getProjectHistory(
				project.getId()).iterator().next();
		event = ProjectLibraryService.findEventById(event.getId());

		assertNotNull(event);
	}

	public void testCategoryChanged() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		Category anotherCategory = new Category(
				ProjectLibrary.COMMUNITY_LIBRARY, ANOTHER_CATEGORY_NAME,
				ANOTHER_CATEGORY_NAME);
		ProjectLibraryService.createCategory(anotherCategory, projectOwner);

		UserProject deployedProject = new UserProject(
				new ConfigElement(
						"<project name=\"Test project\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
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

		// Change the category
		project = ProjectLibraryService.findProjectById(project.getId());
		anotherCategory = ProjectLibraryService
				.findCategoryById(anotherCategory.getId());

		project.setCategory(anotherCategory);

		User user = UserTest.aUser();
		user.setStatus(Status.REGISTERED);
		ProjectLibraryService.onProjectUpdate(project, user);

		// Verify that the changes took place
		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals(anotherCategory, project.getCategory());

		// Verify that category projects are properly updated, esp. considering
		// caching.
		category = ProjectLibraryService.findCategoryById(category.getId(),
				true);
		anotherCategory = ProjectLibraryService.findCategoryById(
				anotherCategory.getId(), true);

		assertEquals(0, category.getProjects().size());
		assertEquals(1, anotherCategory.getProjects().size());
		assertTrue(anotherCategory.getProjects().contains(project));

		Collection<ProjectChangeEvent> events = ProjectLibraryService
				.getProjectHistory(project.getId());
		assertEquals(2, events.size());

		ProjectChangeEvent event = events.iterator().next();

		assertTrue(event.isCapableOfReverting());
		User user2 = UserTest.aUser("bob");
		user2.setStatus(Status.REGISTERED);
		event.revertChanges(user2);

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals(category, project.getCategory());

		// Test that the change is not reversable if the category has been
		// deleted.
		category = ProjectLibraryService.findCategoryById(category.getId());
		ProjectLibraryService.deleteCategory(category, user2);

		project = ProjectLibraryService.findProjectById(project.getId());

		assertNotSame(category, project.getCategory());

		events = ProjectLibraryService.getProjectHistory(project.getId());
		event = events.iterator().next();

		assertEquals(ProjectCategoryChanged.class, event.getClass());
		assertTrue(!event.isCapableOfReverting());
	}

	public void testProjectDeleted() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = new UserProject(
				new ConfigElement(
						"<project name=\"Test project\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
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

		ProjectLibraryService
				.onProjectDelete(project.getId(), UserTest.aRegisteredUser());

		project = ProjectLibraryService.findProjectById(project.getId());
		assertTrue(project.isDeleted());

		Collection<ProjectChangeEvent> events = ProjectLibraryService
				.getProjectHistory(project.getId());
		assertEquals(2, events.size());

		ProjectChangeEvent event = events.iterator().next();

		assertTrue(event.isCapableOfReverting());
		event.revertChanges(UserTest.aUser("bob"));

		project = ProjectLibraryService.findProjectById(project.getId());
		assertTrue(!project.isDeleted());

		// Test that the change is not reversable if the project has been
		// "undeleted"
		project = ProjectLibraryService.findProjectById(project.getId());

		events = ProjectLibraryService.getProjectHistory(project.getId());
		assertEquals(3, events.size());

		Iterator<ProjectChangeEvent> iterator = events.iterator();
		iterator.next();
		event = iterator.next();

		assertEquals(ProjectDeleted.class, event.getClass());
		assertTrue(!event.isCapableOfReverting());
	}

	public void testLongDescriptionChanged() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = new UserProject(
				new ConfigElement(
						"<project name=\"Test project\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		project.setLongDescription("Original description");
		project.setShortDescription("Simple poll");
		project.setSubmittedDate(new Date());
		project.setCategory(category);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		project = ProjectLibraryService.findProjectById(project.getId());
		project.setLongDescription("New description");

		ProjectLibraryService.onProjectUpdate(project, UserTest.aRegisteredUser());

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals("New description", project.getLongDescription());

		Collection<ProjectChangeEvent> events = ProjectLibraryService
				.getProjectHistory(project.getId());
		assertEquals(2, events.size());

		ProjectChangeEvent event = events.iterator().next();

		assertTrue(event.isCapableOfReverting());
		event.revertChanges(UserTest.aRegisteredUser());

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals("Original description", project.getLongDescription());

		// Test that the change is not reversable if the project's long
		// description matches the previous one
		project = ProjectLibraryService.findProjectById(project.getId());

		events = ProjectLibraryService.getProjectHistory(project.getId());
		assertEquals(3, events.size());

		Iterator<ProjectChangeEvent> iterator = events.iterator();
		iterator.next();
		event = iterator.next();

		assertEquals(ProjectLongDescriptionChanged.class, event.getClass());
		assertTrue(!event.isCapableOfReverting());
	}

	public void testShortDescriptionChanged() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = new UserProject(
				new ConfigElement(
						"<project name=\"Test project\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		project.setShortDescription("Original description");
		project.setLongDescription("Simple poll");
		project.setSubmittedDate(new Date());
		project.setCategory(category);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		project = ProjectLibraryService.findProjectById(project.getId());
		project.setShortDescription("New description");

		ProjectLibraryService.onProjectUpdate(project, UserTest.aRegisteredUser());

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals("New description", project.getShortDescription());

		Collection<ProjectChangeEvent> events = ProjectLibraryService
				.getProjectHistory(project.getId());
		assertEquals(2, events.size());

		ProjectChangeEvent event = events.iterator().next();

		assertTrue(event.isCapableOfReverting());
		event.revertChanges(UserTest.aRegisteredUser());

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals("Original description", project.getShortDescription());

		// Test that the change is not reversable if the project short
		// description matches the previous one
		project = ProjectLibraryService.findProjectById(project.getId());

		events = ProjectLibraryService.getProjectHistory(project.getId());
		assertEquals(3, events.size());

		Iterator<ProjectChangeEvent> iterator = events.iterator();
		iterator.next();
		event = iterator.next();

		assertEquals(ProjectShortDescriptionChanged.class, event.getClass());
		assertTrue(!event.isCapableOfReverting());
	}

	public void testProjectNameChanged() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = new UserProject(
				new ConfigElement(
						"<project name=\"Test Project - original name\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		project.setShortDescription("Description");
		project.setLongDescription("Simple poll");
		project.setSubmittedDate(new Date());
		project.setCategory(category);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		project = ProjectLibraryService.findProjectById(project.getId());
		project.setName("Test Project - New name");

		ProjectLibraryService.onProjectUpdate(project, UserTest.aRegisteredUser());

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals("Test Project - New name", project.getName());

		Collection<ProjectChangeEvent> events = ProjectLibraryService
				.getProjectHistory(project.getId());
		assertEquals(2, events.size());

		ProjectChangeEvent event = events.iterator().next();

		assertTrue(event.isCapableOfReverting());
		event.revertChanges(UserTest.aRegisteredUser());

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals("Test Project - original name", project.getName());

		// Test that the change is not reversable if the project name
		// description matches the previous one.
		project = ProjectLibraryService.findProjectById(project.getId());

		events = ProjectLibraryService.getProjectHistory(project.getId());
		assertEquals(3, events.size());

		Iterator<ProjectChangeEvent> iterator = events.iterator();
		iterator.next();
		event = iterator.next();

		assertEquals(ProjectNameChanged.class, event.getClass());
		assertTrue(!event.isCapableOfReverting());
	}

	public void testCommentAddedAndRestored() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		UserProject deployedProject = new UserProject(
				new ConfigElement(
						"<project name=\"Test Project - original name\" themePath=\"default\" format=\"1.3\" />"),
				projectOwner);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject project = new LibraryProject("John Smith",
				deployedProject);
		project.setShortDescription("Description");
		project.setLongDescription("Simple poll");
		project.setSubmittedDate(new Date());
		project.setCategory(category);

		ProjectLibraryService.onProjectSubmission(project, deployedProject);

		project = ProjectLibraryService.findProjectById(project.getId());

		Comment comment = new Comment("bob");
		comment.setText("some comments");

		ProjectLibraryService.onAddingProjectComments(project, comment);

		project = ProjectLibraryService.findProjectById(project.getId());
		assertEquals(1, project.getComments().size());

		Collection<ProjectChangeEvent> events = ProjectLibraryService
				.getProjectHistory(project.getId());
		assertEquals(2, events.size());

		ProjectChangeEvent event = events.iterator().next();

		assertEquals(CommentAdded.class, event.getClass());
		assertTrue(event.isCapableOfReverting());
		event.revertChanges(UserTest.aUser("joe"));

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals(0, project.getComments().size());

		events = ProjectLibraryService.getProjectHistory(project.getId());
		assertEquals(3, events.size());

		event = events.iterator().next();
		assertEquals(CommentDeleted.class, event.getClass());
		assertTrue(event.isCapableOfReverting());
		event.revertChanges(UserTest.aUser("jim"));

		project = ProjectLibraryService.findProjectById(project.getId());

		assertEquals(1, project.getComments().size());
		assertEquals(comment.getId(), project.getComments().get(0).getId());

		events = ProjectLibraryService.getProjectHistory(project.getId());
		assertEquals(4, events.size());

		event = events.iterator().next();
		assertEquals(CommentRestored.class, event.getClass());
		assertTrue(!event.isCapableOfReverting());
	}

	public void testCategoryAdded() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		Calendar date = Calendar.getInstance();
		date.add(Calendar.MINUTE, -1);
		Collection<LibraryChangeEvent> events = ProjectLibraryService
				.getChangesSince(date.getTime());
		assertTrue(events.size() > 0);

		LibraryChangeEvent event = events.iterator().next();

		assertEquals(CategoryAddedEvent.class, event.getClass());
		assertTrue(event.isCapableOfReverting());

		event.revertChanges(UserTest.aUser("joe"));
		assertNull(ProjectLibraryService.findCategoryById(category.getId()));
	}

	public void testCategoryNameChanged() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		Thread.sleep(1000);

		ProjectLibraryService.updateCategory(projectOwner, category.getId(),
				ANOTHER_CATEGORY_NAME, category.getDescription(), category
						.getParent() == null ? null : category.getParent()
						.getId());

		Calendar date = Calendar.getInstance();
		date.add(Calendar.MINUTE, -1);
		Collection<LibraryChangeEvent> events = ProjectLibraryService
				.getChangesSince(date.getTime());
		assertTrue(events.size() > 0);

		LibraryChangeEvent event = events.iterator().next();

		assertEquals(CategoryNameChangedEvent.class, event.getClass());
		assertTrue(event.isCapableOfReverting());

		event.revertChanges(UserTest.aUser("joe"));
		assertEquals(TEST_CATEGORY_NAME, ProjectLibraryService
				.findCategoryById(category.getId()).getName());
	}

	public void testCategoryParentChanged() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		Category anotherCategory = new Category(
				ProjectLibrary.COMMUNITY_LIBRARY, ANOTHER_CATEGORY_NAME,
				ANOTHER_CATEGORY_NAME);
		ProjectLibraryService.createCategory(anotherCategory, projectOwner);

		Thread.sleep(1000);

		ProjectLibraryService.updateCategory(projectOwner, category.getId(),
				category.getName(), category.getDescription(), anotherCategory
						.getId());

		Calendar date = Calendar.getInstance();
		date.add(Calendar.MINUTE, -1);
		Collection<LibraryChangeEvent> events = ProjectLibraryService
				.getChangesSince(date.getTime());
		assertTrue(events.size() > 0);

		LibraryChangeEvent event = events.iterator().next();

		assertEquals(CategoryParentChangedEvent.class, event.getClass());
		assertTrue(event.isCapableOfReverting());

		event.revertChanges(UserTest.aUser("joe"));
		assertNull(ProjectLibraryService.findCategoryById(category.getId())
				.getParent());
	}

	public void testCategoryDeletion() throws Exception {
		Category category = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				TEST_CATEGORY_NAME, TEST_CATEGORY_NAME);
		ProjectLibraryService.createCategory(category, projectOwner);

		Thread.sleep(1000);
		
		ProjectLibraryService.deleteCategory(category, projectOwner);

		Calendar date = Calendar.getInstance();
		date.add(Calendar.MINUTE, -1);
		Collection<LibraryChangeEvent> events = ProjectLibraryService
				.getChangesSince(date.getTime());
		assertTrue(events.size() > 0);

		LibraryChangeEvent event = events.iterator().next();

		assertEquals(CategoryDeletedEvent.class, event.getClass());
		assertFalse(event.isCapableOfReverting());
	}
}
