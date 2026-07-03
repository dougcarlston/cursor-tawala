package com.tawala.project.library;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.TestCaseLifeCycleListener;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.search.InProcessProjectIndexer;
import com.tawala.search.InProcessUserIndexer;
import com.tawala.search.Indexers;
import com.tawala.web.WorldInitializer;

public abstract class ProjectLibraryTestCase extends com.tawala.TestCase {
	private static boolean LUCENE_INITIALIZED = false;
	
	private List<TestCaseLifeCycleListener> listeners = new ArrayList<TestCaseLifeCycleListener>();

	public ProjectLibraryTestCase() {
		listeners.add(new HibernateTestSetup());
	}

	public static String generateLuceneDirectoryName() {
		return generateLuceneDirectoryName("");
	}

	public static String generateLuceneDirectoryName(String prefix) {
		String userHome = System.getProperty("user.home");
		return userHome + File.separator + prefix + "test_lucene";
	}

	/**
	 * TODO: find a better home for this method.
	 * 
	 * @throws IOException
	 */
	public static void setupLuceneForUnitTest() throws IOException {
		if(LUCENE_INITIALIZED) {
			return;
		}
		
		File directory = new File(generateLuceneDirectoryName("unit_"));
		if (!directory.exists()) {
			directory.mkdirs();
		}

		Indexers.initIndexers(new InProcessProjectIndexer(directory
				.getAbsolutePath()
				+ File.separatorChar + "projects"), new InProcessUserIndexer(
				directory.getAbsolutePath() + File.separatorChar + "users"));
		
		LUCENE_INITIALIZED = true;
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		for (Iterator iter = listeners.iterator(); iter.hasNext();) {
			TestCaseLifeCycleListener listener = (TestCaseLifeCycleListener) iter
					.next();
			listener.onSetUp();
		}

		ProjectLibrarySearchTest.setupLuceneForUnitTest();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
	}

	@Override
	protected void tearDown() throws Exception {
		for (int i = listeners.size() - 1; i >= 0; i--) {
			listeners.get(i).onTearDown();
		}

		super.tearDown();
	}

}
