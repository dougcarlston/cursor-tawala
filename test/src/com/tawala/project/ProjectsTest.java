package com.tawala.project;

import java.util.Date;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.web.FakeRequest;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;

public class ProjectsTest extends TestCase {
	private static final String USER_NAME1 = "testuser1";
	private static final String USER_NAME2 = "testuser2";

	private User user1;

	private User user2;

	private HibernateTestSetup hibernateTestSetup = new HibernateTestSetup();

	public ProjectsTest() {
		addUserNameToDelete(USER_NAME1);
		addUserNameToDelete(USER_NAME2);
	}

	@Override
	protected void setUp() throws Exception {
		hibernateTestSetup.onSetUp();
		super.setUp();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));

		user1 = UserTest.aUser(USER_NAME1);
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(user1);
		user2 = UserTest.aUser(USER_NAME2);
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(user2);
	}

	@Override
	protected void tearDown() throws Exception {
		hibernateTestSetup.onTearDown();
		super.tearDown();
	}

	public void testStoreAndFetch() {
		Projects projects = new ProjectsHibernateImpl();
		assertEmpty(projects.getAllForUserId(USER_NAME1));

		UserProject userProject = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "one");
		projects.put(userProject);

		assertEquals(userProject, projects.get(USER_NAME1, "one"));
		assertNull(projects.get(USER_NAME2, "one"));
		assertNull(projects.get(USER_NAME1, "two"));
		assertNotNull(userProject.getUniqueRandomId());
	}

	public void testFetchByUser() {
		Projects projects = new ProjectsHibernateImpl();

		UserProject one = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "one");
		UserProject three = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "three");
		UserProject five = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "five");

		projects.put(one);
		projects.put(three);
		projects.put(five);

		// listed in alphabetical order
		Iterator<UserProject> i = projects.getAllForUserId(USER_NAME1)
				.iterator();
		assertEquals(five, i.next());
		assertEquals(one, i.next());
		assertEquals(three, i.next());
		assertFalse(i.hasNext());
	}

	public void testLibraryReference() {
		Projects projects = new ProjectsHibernateImpl();

		UserProject oldProject = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "aProject");
		oldProject.setLibraryProjectId(new Long(123));
		oldProject.setLibraryVersionNumber(44);
		projects.put(oldProject);

		UserProject project = projects.get(user1.getId(), "aProject");
		assertEquals(oldProject.getLibraryProjectId(), project
				.getLibraryProjectId());
		assertEquals(oldProject.getLibraryVersionNumber(), project
				.getLibraryVersionNumber());
	}

	public void testVersions() {
		Projects projects = new ProjectsHibernateImpl();

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("First Form");
		formBuilder.addText("First version");

		UserProject version1 = new UserProject(projectBuilder.build(), user1,
				"aProject");
		projects.put(version1);

		UserProject project = projects.getWithVersions(user1.getId(),
				"aProject");
		assertEquals(1, project.getVersions().size());

		UserProject version2 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "aProject");
		projects.put(version2);

		project = projects.getWithVersions(user1.getId(), "aProject");
		assertEquals(2, project.getVersions().size());

		assertProjectVersion(project.getVersions().get(0), version2
				.getProject(), 2);
		assertProjectVersion(project.getVersions().get(1), version1
				.getProject(), 1);
	}

	private void assertProjectVersion(ProjectVersion version, Project project,
			int versionNumber) {
		version = WorldInitializer.getDefaultWorld().domain().projects()
				.findProjectVersion(version.getId());
		assertEquals(project.getProjectXmlDefinition(), version.getProject()
				.getProjectXmlDefinition());
		assertEquals(versionNumber, version.getVersionNumber());

	}

	public void testFindProjectVersion() {
		Projects projects = new ProjectsHibernateImpl();

		UserProject version1 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "aProject");
		projects.put(version1);

		ProjectVersion projectVersion = projects.findProjectVersion(version1
				.getVersions().get(0).getId());
		assertNotNull(projectVersion);

		assertEquals(version1.getProject().getProjectXmlDefinition(),
				projectVersion.getProject().getProjectXmlDefinition());
		assertEquals(version1, projectVersion.getParent());
	}

	public void testDeployProjectVersion() {
		Projects projects = new ProjectsHibernateImpl();

		UserProject version1 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "aProject");
		projects.put(version1);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("First Form");
		formBuilder.addText("Second version");

		UserProject version2 = new UserProject(projectBuilder.build(), user1,
				"aProject");
		projects.put(version2);

		UserProject userProject = projects.getWithVersions(user1.getId(),
				"aProject");
		assertEquals(2, userProject.getVersions().size());
		assertSame(userProject.getDeployedVersion(), userProject.getVersions()
				.get(0));
		assertEquals(2, userProject.getDeployedVersion().getVersionNumber());

		try {
			projects.deployProjectVersion(user2, userProject.getVersions().get(
					1).getId());
			fail("An exception should have been thrown to indicate illegal access");
		} catch (IllegalArgumentException e) {
			// --- Do nothing; this is expected
		}
		projects.deployProjectVersion(user1, userProject.getVersions().get(1)
				.getId());

		userProject = projects.getWithVersions(user1.getId(), "aProject");
		assertEquals(1, userProject.getDeployedVersion().getVersionNumber());

		assertEquals(version1.getProject().getProjectXmlDefinition(),
				userProject.getProject().getProjectXmlDefinition());
	}

	public void testDelete() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form1");
		FibBuilder fibBuilder = formBuilder.addFib();
		fibBuilder.addBlank("Field1");

		UserProject project = new UserProject(projectBuilder.build(), user1,
				"test");
		Projects projects = new ProjectsHibernateImpl();

		projects.put(project);

		Form form = project.getProject().getForm("Form1");
		FakeExecutionContext context = new FakeExecutionContext(project, form);
		context.getDomain().storedData().record(
				new FakeExecutionContext(project, form, new FakeRequest(true,
						new String[] { "Field1", "abc" })).getSubmission());
		context.getDomain().storedData().record(
				new FakeExecutionContext(project, form, new FakeRequest(true,
						new String[] { "Field1", "dce" })).getSubmission());

		project = projects.get(user1.getId(), "test");

		projects.delete(project, WorldInitializer.getDefaultWorld());

		project = projects.get(user1.getId(), "test");
		assertNull(project);
	}

	public void testDeleteProjectVersion() {
		Projects projects = new ProjectsHibernateImpl();

		UserProject version1 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "aProject");
		projects.put(version1);

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("First Form");
		formBuilder.addText("Second version");

		UserProject version2 = new UserProject(projectBuilder.build(), user1,
				"aProject");
		projects.put(version2);

		UserProject userProject = projects.getWithVersions(user1.getId(),
				"aProject");

		projects.deployProjectVersion(user1, userProject.getVersions().get(1)
				.getId());

		try {
			projects.deleteProjectVersion(user1, userProject.getVersions().get(
					1).getId());
			fail("Should have thrown exception on attempt to delete the deployed version.");
		} catch (IllegalArgumentException e) {
			// This is expected
		}

		projects.deleteProjectVersion(user1, userProject.getVersions().get(0)
				.getId());

		userProject = projects.getWithVersions(user1.getId(), "aProject");

		assertEquals(1, userProject.getVersions().size());
		assertEquals(1, userProject.getVersions().get(0).getVersionNumber());
	}

	public void testWithProjectRuntimeByRandomId() {
		Projects projects = new ProjectsHibernateImpl();

		UserProject project = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "aProject");
		projects.put(project);

		LinkToUserProject linkToProject = projects
				.getWithProjectRuntime(project.getUniqueRandomId());
		project = linkToProject.getProject();

		assertNotNull(project);
		assertNotNull(project.getUniqueRandomId());
		assertNotNull(project.getUser().getId());
	}

	public void testChangingProjectOwnership() {
		Projects projects = new ProjectsHibernateImpl();
		UserProject project = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "test");
		projects.put(project);

		projects.changeProjectNameAndOwnership(project.getId(), "newName",
				user2);

		List<UserProject> allProjects = projects.getAllForUserId(user2.getId());
		assertEquals(1, allProjects.size());
		assertEquals(project.getId(), allProjects.get(0).getId());
		assertEquals("newName", allProjects.get(0).getName());

		assertEquals(0, projects.getAllForUserId(user1.getId()).size());
	}

	public void testAddLinkToProject() {
		Projects projects = new ProjectsHibernateImpl();
		UserProject project = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "test");
		projects.put(project);

		LinkToUserProject linkToProject = LinkToUserProject
				.createUnauthenticatedLink(project);
		projects.addLinkToProject(linkToProject);

		assertNotNull(linkToProject.getId());
	}

	public void testAddPrivateLinkToProjectDoesntCreateDuplicates() {
		Projects projects = new ProjectsHibernateImpl();
		UserProject project = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "test");
		projects.put(project);

		LinkToUserProject firstLink = new LinkToUserProject(project, "123");
		projects.addLinkToProject(firstLink);
		assertNotNull(firstLink.getId());

		LinkToUserProject secondLink = new LinkToUserProject(project, "456");
		projects.addLinkToProject(secondLink);
		assertNotEquals(firstLink.getId(), secondLink.getId());

		LinkToUserProject thirdLink = new LinkToUserProject(project, "123");
		projects.addLinkToProject(thirdLink);
		assertEquals(firstLink.getId(), thirdLink.getId());

	}

	public void testGetProjectStatisticsSorting() {
		Projects projects = new ProjectsHibernateImpl();
		UserProject project1 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "test1");
		projects.put(project1);

		UserProject project2 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "test2");
		projects.put(project2);

		for (ProjectSortOrder sortOrder : ProjectSortOrder.values()) {
			List<ProjectStatistics> stats = projects.getProjectStatistics(
					user1, ProjectFilter.all, sortOrder, 0, 10);

			assertNotNull(stats);
			assertEquals(2, stats.size());
		}

		// -- Test name ascending
		List<ProjectStatistics> stats = projects.getProjectStatistics(user1,
				ProjectFilter.all, ProjectSortOrder.nameAscending, 0, 10);
		assertEquals("test1", stats.get(0).getName());
		assertEquals("test2", stats.get(1).getName());

		// -- Test name descending
		stats = projects.getProjectStatistics(user1, ProjectFilter.all,
				ProjectSortOrder.nameDescending, 0, 10);
		assertEquals("test2", stats.get(0).getName());
		assertEquals("test1", stats.get(1).getName());

		// -- Test response count ascending
		stats = projects.getProjectStatistics(user1, ProjectFilter.all,
				ProjectSortOrder.responseCountAscending, 0, 10);
		assertEquals("test1", stats.get(0).getName());
		assertEquals("test2", stats.get(1).getName());

		// -- Test response count descending
		stats = projects.getProjectStatistics(user1, ProjectFilter.all,
				ProjectSortOrder.responseCountAscending, 0, 10);
		assertEquals("test1", stats.get(0).getName());
		assertEquals("test2", stats.get(1).getName());
	}

	public void testGetProjectStatisticsFiltering() {
		Projects projects = new ProjectsHibernateImpl();
		UserProject project1 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "test1");
		project1.setOffline(true);
		projects.put(project1);

		UserProject project2 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "test2");
		project2.setOffline(false);
		projects.put(project2);

		// --- Test that none of the filters break.
		for (ProjectFilter filter : ProjectFilter.values()) {
			List<ProjectStatistics> stats = projects.getProjectStatistics(
					user1, filter, ProjectSortOrder.nameAscending, 0, 10);

			assertNotNull(stats);
		}

		// -- Test all
		List<ProjectStatistics> stats = projects.getProjectStatistics(user1,
				ProjectFilter.all, ProjectSortOrder.nameAscending, 0, 10);
		assertEquals(2, stats.size());
		assertEquals("test1", stats.get(0).getName());
		assertEquals("test2", stats.get(1).getName());

		// -- Test active only
		stats = projects.getProjectStatistics(user1, ProjectFilter.activeOnly,
				ProjectSortOrder.nameAscending, 0, 10);
		assertEquals(1, stats.size());
		assertEquals("test2", stats.get(0).getName());

		// -- Test inactive only
		stats = projects.getProjectStatistics(user1,
				ProjectFilter.inactiveOnly, ProjectSortOrder.nameAscending, 0,
				10);
		assertEquals(1, stats.size());
		assertEquals("test1", stats.get(0).getName());
	}

	public void testProjectCounts() {
		Projects projects = new ProjectsHibernateImpl();
		UserProject project1 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "test1");
		project1.setOffline(true);
		projects.put(project1);

		UserProject project2 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "test2");
		project2.setOffline(false);
		projects.put(project2);

		UserProject project3 = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), user1, "test3");
		project3.setOffline(false);
		projects.put(project3);

		assertEquals(3, projects.projectCountFor(user1));
		assertEquals(1, projects.inactiveProjectCountFor(user1));
	}

	public void testGeneratingRandomFormTokens() {
		Projects projects = new ProjectsHibernateImpl();
		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm("Form1");
		builder.addForm("Form2");
		Project runnableProject = builder.build();
		UserProject project = new UserProject(runnableProject, user1, "test");
		projects.put(project);

		// --- Test initial upload
		project = projects.getWithProjectRuntime(user1.getId(), "test");

		String originalUrl = project.getUrlToForm(EntryPointType.REAL_PROJECT,
				project.getProject().getForm("Form1"));
		assertMatches(getUrlMatchingRE(project, "Form1"), originalUrl);

		// --- Test subsequent updates don't change existing URLs.
		builder.addForm("Form3");
		runnableProject = builder.build();
		project = new UserProject(runnableProject, user1, "test");
		projects.put(project);

		project = projects.getWithProjectRuntime(user1.getId(), "test");
		String newUrl = project.getUrlToForm(EntryPointType.REAL_PROJECT,
				project.getProject().getForm("Form1"));
		assertEquals(originalUrl, newUrl);

		// --- Test that on updates the new forms get tokens
		String urlToForm3 = project.getUrlToForm(EntryPointType.REAL_PROJECT,
				project.getProject().getForm("Form3"));
		assertMatches(getUrlMatchingRE(project, "Form3"), urlToForm3);

	}

	public void testSavingDesignerVersion() {
		Projects projects = new ProjectsHibernateImpl();
		ProjectBuilder projectBuilder = new ProjectBuilder();
		projectBuilder.addForm("aForm");
		projectBuilder.setDesignerVersion("123");

		UserProject project = new UserProject(projectBuilder.build(), user1,
				"test");
		projects.put(project);

		project = projects.getWithProjectRuntime(user1.getId(), "test");
		assertEquals("123", project.getProject().getDesignerVersion());
	}

	public void testGetLastDataRecordedDate() throws InterruptedException {
		Projects projects = new ProjectsHibernateImpl();
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("aForm");
		formBuilder.addFib().addBlank("field1");

		UserProject project = new UserProject(projectBuilder.build(), user1,
				"test");
		projects.put(project);

		assertNull(ProjectsHibernateImpl.getLastDataRecordedDate(project));
		Date beforeFirstSubmission = new Date();
		Thread.sleep(1000);

		FormSubmission firstSubmission = new FormSubmission(project, project
				.getProject().getForm("aForm"));
		firstSubmission.setValue("field1", "123");
		WorldInitializer.getDefaultWorld().domain().storedData().record(
				firstSubmission);

		Date lastUpdated = ProjectsHibernateImpl
				.getLastDataRecordedDate(project);
		assertTrue(lastUpdated.after(beforeFirstSubmission));
		assertTrue(lastUpdated.before(new Date()));

		Thread.sleep(1000);

		FormSubmission secondSubmission = new FormSubmission(project, project
				.getProject().getForm("aForm"));
		secondSubmission.setValue("field1", "451");
		WorldInitializer.getDefaultWorld().domain().storedData().record(
				secondSubmission);

		Date newLastUpdated = ProjectsHibernateImpl
				.getLastDataRecordedDate(project);
		assertTrue(newLastUpdated.after(lastUpdated));
		assertTrue(newLastUpdated.before(new Date()));
	}

	/**
	 * This is not a full test of this functionality. It would required a fairly
	 * complicated setup process. An end-to-end testing of this functionality is
	 * provided in another test.
	 * 
	 * It's main purpose is to quickly test the syntax of the query used to retrieve this data.
	 */
	public void testGetObsoleteProjects() throws InterruptedException {
		Map<UserProject, LibraryProjectVersion> projects = ProjectsHibernateImpl
				.findAllObsoleteProjects(ProjectLibrary.PAID_PROJECT_LIBRARY);
		assertNotNull(projects);
	}

	public static String getUrlMatchingRE(UserProject project, String formName) {
		return "http://[^/]*" + WellKnown.urls.getProjectRunUrlPrefix() + "/"
				+ project.getUniqueRandomId() + "/" + ".{7}\\." + formName;
	}

	public static String getUrlMatchingRE(String formName) {
		return "http://[^/]*" + WellKnown.urls.getProjectRunUrlPrefix()
				+ "/[^/]*/" + ".{7}\\." + formName;
	}
}
