package com.tawala.project;

import com.tawala.project.formatting.Link;

import junit.framework.TestCase;

public class LinkTest extends TestCase {
	public void testURLParsing() {
		doTestLinkParsing("http://www.google.com", false, "");
		doTestLinkParsing("http://www.google.com/", false, "");
		doTestLinkParsing("http://www.google.com/calendar", true, "calendar");
		doTestLinkParsing("http://www.tawala.com/i/2341qwr234/File.pdf", true, "File.pdf");
		doTestLinkParsing("/i/2341qwr234/File.pdf", true, "File.pdf");
		doTestLinkParsing("/i/2341qwr234/File with Spaces.pdf", true, "File with Spaces.pdf");
	}

	private void doTestLinkParsing(String url, boolean isFile,
			String expectedFileName) {
		Link.URLParser parser = new Link.URLParser(url);
		assertEquals("Looks like file: " + url, isFile, parser.looksLikeFile);
		if(isFile) {
			assertEquals("Expected file name: " + url, expectedFileName, parser.fileName);
		}
	}
}
