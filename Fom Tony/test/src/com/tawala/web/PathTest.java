package com.tawala.web;

import junit.framework.TestCase;
import mock.javax.servlet.http.FakeHttpServletRequest;

public class PathTest extends TestCase {

    public void testBasics() {
        FakeHttpServletRequest req = new FakeHttpServletRequest();
        req.setPathInfo("/about/contact/support");
        Path path = new Path(req);
        assertEquals(4, path.size());
        checkRoot(path.element(0));
        checkDir("about", path.element(1));
        checkDir("contact", path.element(2));
        checkFile("support", path.element(3));
    }

    public void testMappedNotAtRoot() {
        FakeHttpServletRequest req = new FakeHttpServletRequest();
        req.setServletPath("/forum");
        req.setPathInfo("/view/article/1234");
        Path path = new Path(req);
        assertEquals(5, path.size());
        checkRoot(path.element(0));
        checkDir("forum", path.element(1));
        checkDir("view", path.element(2));
        checkDir("article", path.element(3));
        checkFile("1234", path.element(4));
    }

    public void testNull() {
        Path path = new Path(new FakeHttpServletRequest());
        assertEquals(0, path.size());
    }


    public void testStartsWith() {
        Path root = new Path("/");
        Path foo = new Path("/foo");
        Path foobar = new Path("/foo/bar");
        assertTrue(root.startsWith(root));
        assertTrue(foo.startsWith(root));
        assertTrue(foobar.startsWith(root));

        assertFalse(root.startsWith(foo));
        assertTrue(foo.startsWith(foo));
        assertTrue(foobar.startsWith(foo));

        assertFalse(root.startsWith(foobar));
        assertFalse(foo.startsWith(foobar));
        assertTrue(foobar.startsWith(foobar));
    }

    public void testEquals() {
        Path one = new Path("/foo/bar");
        Path two = new Path("/foo/bar");
        Path other = new Path("/foo/bark");
        assertEquals(one, one);
        assertEquals(one, two);
        assertEquals(two, one);
        assertFalse(one.equals(other));
        assertFalse(other.equals(one));
    }

    public void testUrlDecoding() {
        FakeHttpServletRequest req = new FakeHttpServletRequest();
        req.setPathInfo("/one+word/foo%5cbar");
        Path path = new Path(req);
        checkRoot(path.element(0));
        checkDir("one word", path.element(1));
        checkFile("foo\\bar", path.element(2));
        assertEquals(3, path.size());
    }

    private void checkRoot(PathElement element) {
        assertEquals("/", element.getName());
        assertTrue(element.isRoot());
        assertTrue(element.isDir());
    }

    private void checkDir(String expectedName, PathElement element) {
        assertEquals(expectedName, element.getName());
        assertFalse(element.isRoot());
        assertTrue(element.isDir());
    }

    private void checkFile(String expectedName, PathElement element) {
        assertEquals(expectedName, element.getName());
        assertFalse(element.isRoot());
        assertFalse(element.isDir());
    }

}
