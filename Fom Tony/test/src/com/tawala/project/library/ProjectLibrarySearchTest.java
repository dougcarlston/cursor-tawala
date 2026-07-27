package com.tawala.project.library;

import java.io.IOException;
import java.util.Collection;
import java.util.Date;

import org.apache.lucene.queryParser.ParseException;

import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.search.ProjectIndexer;
import com.tawala.web.WorldInitializer;

public class ProjectLibrarySearchTest extends ProjectLibraryTestCase {
	private static final String CATEGORY_LEVEL_THREE_NAME = "levelthree";
	private static final String CATEGORY_LEVEL_TWO_NAME = "leveltwo";
	private static final String CATEGORY_LEVEL_ONE_NAME = "levelone";
	private static final String PROJECT_NAME = "Menu Sample";
	private static final String VETTED_PROJECT_NAME = "Vetted Menu Sample";

	private User projectOwner = UserTest.aUser("tester");

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(
				projectOwner);
	}

	public ProjectLibrarySearchTest() {
		setProjectNamesToDelete(new String[] { PROJECT_NAME,
				VETTED_PROJECT_NAME });
		setCategoryNamesToDelete(new String[] { CATEGORY_LEVEL_ONE_NAME,
				CATEGORY_LEVEL_TWO_NAME, CATEGORY_LEVEL_THREE_NAME });
		addUserNameToDelete(projectOwner.getId());
	}

	public void testSimpleSearch() throws IOException, ParseException {
		Category level1 = new Category(ProjectLibrary.COMMUNITY_LIBRARY, CATEGORY_LEVEL_ONE_NAME, "test category");
		ProjectLibraryService.createCategory(level1, projectOwner);

		Category level2 = new Category(level1, CATEGORY_LEVEL_TWO_NAME,
				"test category");
		ProjectLibraryService.createCategory(level2, projectOwner);

		Category level3 = new Category(level2, CATEGORY_LEVEL_THREE_NAME,
				"test category");
		ProjectLibraryService.createCategory(level3, projectOwner);

		UserProject deployedProject = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), projectOwner, PROJECT_NAME);
		WorldInitializer.getDefaultWorld().domain().projects().put(
				deployedProject);

		LibraryProject original = new LibraryProject("John Smith",
				deployedProject);
		original
				.setLongDescription("Some very long project documentation with uniquewordinitsdescription.");
		original.setShortDescription("Living will - testing excluded words");
		original.setSubmittedDate(new Date());
		original.setCategory(level3);

		ProjectLibraryService.onProjectSubmission(original, deployedProject);

		assertNotSame(0, original.getId());

		verifySearchSuccessful(original, "uniquewordinitsdescription");

		// "will" is on the default list of excluded words. This list is
		// changed and we are testing it here.
		verifySearchSuccessful(original, "Living will");
		verifySearchSuccessful(original, "will");

		verifySearchSuccessful(original, "excluded words");

		verifySearchSuccessful(original, CATEGORY_LEVEL_ONE_NAME);
		verifySearchSuccessful(original, CATEGORY_LEVEL_TWO_NAME);
		verifySearchSuccessful(original, CATEGORY_LEVEL_THREE_NAME);

		verifySearchSuccessful(original, ProjectIndexer.FIELD_CATEGORY + ":"
				+ CATEGORY_LEVEL_ONE_NAME);
		verifySearchSuccessful(original, ProjectIndexer.FIELD_CATEGORY + ":"
				+ CATEGORY_LEVEL_TWO_NAME);
		verifySearchSuccessful(original, ProjectIndexer.FIELD_CATEGORY + ":"
				+ CATEGORY_LEVEL_THREE_NAME);

		verifySearchSuccessful(original, "menu");
		verifySearchSuccessful(original, ProjectIndexer.FIELD_NAME + ":menu");
		verifySearchUnsuccessful(ProjectIndexer.FIELD_CATEGORY + ":menu");

		verifySearchSuccessful(original, "smith");
		verifySearchUnsuccessful(ProjectIndexer.FIELD_CATEGORY + ":smith");
		verifySearchSuccessful(original, ProjectIndexer.FIELD_AUTHOR + ":smith");
	}

	private void verifySearchSuccessful(LibraryProject original, String query)
			throws ParseException, IOException {
		Collection<LibraryProject> foundProjects = ProjectLibraryService.search(original.getCategory().getLibrary(), query);
		assertTrue("Query: '" + query + "'", foundProjects.contains(original));
	}

	private void verifySearchUnsuccessful(String query) throws ParseException,
			IOException {
		Collection<LibraryProject> foundProjects = ProjectLibraryService.search(ProjectLibrary.COMMUNITY_LIBRARY, query);
		assertEquals("found count for '" + query + "'", 0, foundProjects.size());
	}
}
