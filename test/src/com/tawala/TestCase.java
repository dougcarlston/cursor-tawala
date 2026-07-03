package com.tawala;

import java.io.File;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.io.StringReader;
import java.util.Collection;
import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.util.regex.Pattern;

import org.dom4j.Document;
import org.dom4j.DocumentException;
import org.dom4j.Element;
import org.dom4j.io.SAXReader;
import org.hibernate.SessionFactory;
import org.hibernate.metadata.ClassMetadata;
import org.hibernate.metadata.CollectionMetadata;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.domain.User;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.ProjectLibraryTestCase;

public abstract class TestCase extends junit.framework.TestCase {
	private String[] projectNamesToDelete = null;
	private String[] categoryNamesToDelete = null;
	private Set<String> userNamesToDelete = null;

	protected static final String NEWLINE = "\n";

	public void setProjectNamesToDelete(String... projectNamesToDelete) {
		this.projectNamesToDelete = projectNamesToDelete;
	}

	public void setCategoryNamesToDelete(String... categoryNamesToDelete) {
		this.categoryNamesToDelete = categoryNamesToDelete;
	}

	public void setUserNamesToDelete(String... userNamesToDelete) {
		for (int i = 0; i < userNamesToDelete.length; i++) {
			addUserNameToDelete(userNamesToDelete[i]);
		}
	}

	protected void addUserNameToDelete(String userName) {
		if (this.userNamesToDelete == null)
			this.userNamesToDelete = new HashSet<String>();
		this.userNamesToDelete.add(userName);
	}

	private void cleanDatabase() throws Exception {
		HibernateTestSetup hibernateTestSetup = new HibernateTestSetup();
		if (projectNamesToDelete != null) {
			for (String projectName : projectNamesToDelete) {
				hibernateTestSetup.onSetUp();
				deleteLibraryProjectNamed(projectName);
			}
		}

		if (categoryNamesToDelete != null) {
			for (String categoryName : categoryNamesToDelete) {
				hibernateTestSetup.onSetUp();
				try {
					ProjectLibraryService.deleteCategoryLike(categoryName);
				} catch (Exception e) {
					Log.error(this, "Failed to delete category \""
							+ categoryName + "\".");
					throw e;
				}
			}
		}

		if (userNamesToDelete != null) {
			Users users = new UsersHibernateImpl();
			for (String userName : userNamesToDelete) {
				hibernateTestSetup.onSetUp();
				User user = users.get(userName);
				if (user != null)
					users.delete(user);
			}
		}
	}

	protected void setUp() throws Exception {
		super.setUp();
		ProjectLibraryTestCase.setupLuceneForUnitTest();
		cleanDatabase();
	}

	@Override
	protected void tearDown() throws Exception {
		cleanDatabase();
		super.tearDown();
	}

	protected void assertMatches(String expected, String actual) {
		Pattern pattern = Pattern.compile(expected, Pattern.MULTILINE
				+ Pattern.DOTALL);
		if (!pattern.matcher(actual).find()) {
			fail("couldn't find <" + expected + "> in <" + actual + ">");
		}
	}

	public static ConfigElement parseConfig(String xml) {
		return new ConfigElement(parseXml(xml));
	}

	public static Element parseXml(String xml) {
		return parseXmlDocument(xml).getRootElement();
	}

	private static Document parseXmlDocument(String xml) {
		try {
			SAXReader saxReader = new SAXReader();
			saxReader.setMergeAdjacentText(true);
			Document document = saxReader.read(new StringReader(xml));
			return document;
		} catch (DocumentException e) {
			throw new RuntimeException("unexpected failure parsing XML", e);
		}
	}

	public static void assertEquals(String expected, String actual) {
		assertNotNull("expected <" + expected + "> but was null", actual);
		junit.framework.TestCase.assertEquals(expected, actual);
	}

	public static void assertContains(String expected, String actual) {
		assertTrue("couldn't find <" + expected + "> in <" + actual + ">",
				actual.indexOf(expected) > -1);
	}

	public static void assertDoesntContain(String expected, String actual) {
		assertTrue("found <" + expected + "> in <" + actual + "> at position "
				+ actual.indexOf(expected), actual.indexOf(expected) < 0);
	}

	public static void assertNull(Object actual) {
		junit.framework.TestCase.assertNull("expected null, found <"
				+ String.valueOf(actual) + ">", actual);

	}

	protected void dump(String result) {
		char[] chars = result.toCharArray();
		for (int i = 0; i < chars.length; i++) {
			char aChar = chars[i];
			System.out.print(displayForm(aChar));
			System.out.print("\t");
			System.out.println((int) aChar);
		}
	}

	private String displayForm(char aChar) {
		switch (aChar) {
		case '\b':
			return "\\b";
		case '\f':
			return "\\f";
		case '\n':
			return "\\n";
		case '\r':
			return "\\r";
		case '\t':
			return "\\t";
		default:
			if (aChar > ' ') {
				return Character.toString(aChar);
			} else {
				return " ";
			}

		}
	}

	protected static void assertNotEquals(Object expected, Object actual) {
		assertFalse(expected.equals(actual));
	}

	@SuppressWarnings("unchecked")
	protected void assertCompareEquals(Comparable expected, Comparable actual) {
		assertEquals(expected.compareTo(actual), 0);
	}

	@SuppressWarnings("unchecked")
	protected void assertCompareLess(Comparable expected, Comparable actual) {
		assertTrue(expected.compareTo(actual) < 0);
	}

	protected File createTestDirectory() {
		File contentDir = new File("build/tests/" + testName());
		cleanAndCreate(contentDir);
		return contentDir;
	}

	private void cleanAndCreate(File directory) {
		if (directory.exists())
			deleteRecursively(directory);
		directory.mkdirs();
	}

	protected void deleteRecursively(File item) {
		makeSureItemIsSafeToDelete(item);
		if (item.isDirectory()) {
			File[] contents = item.listFiles();
			for (File child : contents) {
				deleteRecursively(child);
			}
		}
		item.delete();
	}

	/**
	 * build/tests should be somewhere in the path.
	 */
	private void makeSureItemIsSafeToDelete(File directory) {
		File focus = directory;
		boolean foundTests = false;
		boolean pathOk = false;
		while (focus != null) {
			String fileName = focus.getName();
			assertFalse(fileName.equals(".."));
			if (fileName.equals("tests"))
				foundTests = true;
			if (fileName.equals("build")) {
				if (foundTests)
					pathOk = true;
			}
			focus = focus.getParentFile();
		}
		assertTrue(pathOk);
	}

	private String testName() {
		return this.getClass().getName() + "." + this.getName();
	}

	protected void assertEmpty(Collection collection) {
		assertTrue("not empty: " + collection, collection.isEmpty());
	}

	protected void writeFile(File file, String content) throws IOException {
		FileWriter out = new FileWriter(file);
		out.write(content);
		out.close();
	}

	protected static String readFile(File file) throws IOException {
		StringBuffer result = new StringBuffer();
		FileReader reader = new FileReader(file);
		int read;
		char[] cbuf = new char[4096];
		while ((read = reader.read(cbuf)) > 0) {
			result.append(cbuf, 0, read);
		}
		reader.close();

		return result.toString();
	}

	public static void deleteLibraryProjectNamed(String name) {
		LibraryProject project = ProjectLibraryService.findProjectByName(name);
		if (project != null)
			ProjectLibraryService.permanentlyDeleteProject(project);
	}

	public void clearHibernateSecondLevelCache() {
		clearCache(TawalaSessionFactory.MAIN.getFactory());
		clearCache(TawalaSessionFactory.BACKUP.getFactory());
	}

	@SuppressWarnings("unchecked")
	private void clearCache(SessionFactory factory) {
		Map<String, CollectionMetadata> roleMap = factory
				.getAllCollectionMetadata();
		for (String roleName : roleMap.keySet()) {
			factory.evictCollection(roleName);
		}

		Map<String, ClassMetadata> entityMap = factory.getAllClassMetadata();
		for (String entityName : entityMap.keySet()) {
			factory.evictEntity(entityName);
		}

		factory.evictQueries();
	}
}
