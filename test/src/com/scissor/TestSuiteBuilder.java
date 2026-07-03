package com.scissor;

import java.io.File;
import java.io.FileFilter;
import java.io.IOException;
import java.lang.reflect.Modifier;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Comparator;
import java.util.List;
import java.util.StringTokenizer;

import junit.framework.TestSuite;

public class TestSuiteBuilder {

    private static final FileFilter IS_VALID_DIRECTORY = new DirectoryFilter();
    private static final FileFilter IS_TEST = new JavaTestFileFilter();
    private File directory;
    private List<String> skips = new ArrayList<String>();
    private static final NewestFileComparator COMPARATOR = new NewestFileComparator();

    public TestSuiteBuilder(String directory) {
        this.directory = new File(directory);
        if (!this.directory.exists()) {
            try {
                throw new IllegalArgumentException("Couldn't find path " + directory +
                        " starting from directory" + new File(".").getCanonicalPath());
            } catch (IOException e) {
                throw new IllegalStateException("Can't find home path!");
            }
        }
    }

    public TestSuite build(String... packageNames) throws ClassNotFoundException {
        TestSuite suite = new TestSuite(packageNames[0]);
        for (String packageName : packageNames) {
            File myDir = directoryForPackage(packageName);
            addChildDirs(suite, myDir, packageName);
            addTestsInDir(suite, myDir, packageName);
        }
        return suite;
    }

    private void addChildDirs(TestSuite suite, File myDir, String packageName) throws ClassNotFoundException {
        File[] directories = myDir.listFiles(IS_VALID_DIRECTORY);
        Arrays.sort(directories, COMPARATOR);
        for (int i = 0; i < directories.length; i++) {
            File childDir = directories[i];
            String childPackage = packageName + "." + childDir.getName();
            if (!skips.contains(childPackage)) {
                suite.addTest(build(childPackage));
            }
        }
    }

    private void addTestsInDir(TestSuite suite, File myDir, String packageName) throws ClassNotFoundException {
        File[] tests = myDir.listFiles(IS_TEST);
        
        //--- Acceptance tests need to be kept together.
        Arrays.sort(tests, COMPARATOR);
        for (int i = 0; i < tests.length; i++) {
            File test = tests[i];
            Class aClass = loadTest(packageName, test);
            if (testShouldBeIncluded(aClass)) {
                suite.addTest(new TestSuite(aClass));
            }
        }
    }

    @SuppressWarnings("unchecked")
    private boolean testShouldBeIncluded(Class aClass) {
        boolean isAbstract = Modifier.isAbstract(aClass.getModifiers());
        boolean isUnfinished = aClass.isAnnotationPresent(Unfinished.class);
        boolean isExcluded = aClass.isAnnotationPresent(ExcludeFromTests.class);
        return !(isAbstract || isUnfinished || isExcluded);
    }

    private Class loadTest(String packageName, File test) throws ClassNotFoundException {
        String testName = test.getName().substring(0, test.getName().indexOf('.'));
        return (Class) ClassLoader.getSystemClassLoader().loadClass(packageName + "." + testName);
    }

    private File directoryForPackage(String packageName) {
        StringTokenizer st = new StringTokenizer(packageName, ".");
        File current = directory;
        while (st.hasMoreElements()) {
            String packageComponent = (String) st.nextElement();
            current = new File(current, packageComponent);
        }
        return current;
    }

    public void skipPackage(String packageName) {
        this.skips.add(packageName);
    }


    private static class DirectoryFilter implements FileFilter {

        public boolean accept(File file) {
            if (!file.isDirectory()) return false;
            if (file.getName().equals("CVS")) return false;
            if (file.getName().equals(".svn")) return false;
            return true;
        }

    }

    private static class JavaTestFileFilter implements FileFilter {

        public boolean accept(File file) {
            if (file.getName().endsWith("Test.java")) return true;
            return false;
        }

    }

    public static class NewestFileComparator implements Comparator<File> {
        public int compare(File f1, File f2) {
            if (f1.lastModified() < f2.lastModified()) {
                return 1;
            } else if (f1.lastModified() > f2.lastModified()) {
                return -1;
            } else {
                return f1.compareTo(f2);
            }
        }
    }
}
