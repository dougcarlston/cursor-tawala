package com.tawala.util;

import junit.framework.TestCase;

public class LineEndingsTest extends TestCase {
	public void testCrEntityBecomesNewline() {
		assertEquals("\nTry this test on Japan",
				LineEndings.toUnixNewlines("&#x0D;\nTry this test on Japan"));
		assertEquals("\nTry this test on Japan",
				LineEndings.toUnixNewlines("&#x0d;Try this test on Japan"));
		assertEquals("\nTry this test on Japan",
				LineEndings.toUnixNewlines("&#13;Try this test on Japan"));
	}

	public void testRawCrLfBecomesSingleNewline() {
		assertEquals("\nTry this test on Japan",
				LineEndings.toUnixNewlines("\r\nTry this test on Japan"));
		assertEquals("line1\nline2", LineEndings.toUnixNewlines("line1\rline2"));
	}

	public void testNullIsEmpty() {
		assertEquals("", LineEndings.toUnixNewlines(null));
	}

	public void testUnixNewlinesUnchanged() {
		assertEquals("a\nb", LineEndings.toUnixNewlines("a\nb"));
	}
}
