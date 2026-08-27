package com.tawala.web.oldhtml;

import com.tawala.project.Value;

public class TextInputTest extends HtmlTestCase {
	public void testTextareaStripsStoredCrEntityBeforeEscape() {
		TextInput input = new TextInput("Pre", 40, 3, new Value(
				"&#x0D;\nTry this test on Japan"));
		String html = renderHtml(input);
		assertDoesntContain("&#x0D;", html);
		assertDoesntContain("&#x0d;", html);
		assertDoesntContain("&#13;", html);
		assertDoesntContain("&amp;#x0D;", html);
		assertContains("Try this test on Japan", html);
		assertContains("<textarea", html);
	}

	public void testTextareaStripsDecimalCrEntity() {
		TextInput input = new TextInput("Post", 40, 3, new Value(
				"&#13;Try this test on Japan"));
		String html = renderHtml(input);
		assertDoesntContain("&#13;", html);
		assertContains("Try this test on Japan", html);
	}

	public void testTextareaStripsRawCarriageReturn() {
		TextInput input = new TextInput("Pre", 40, 3, new Value(
				"\r\nTry this test on Japan"));
		String html = renderHtml(input);
		assertDoesntContain("&#x0D;", html);
		assertContains("Try this test on Japan", html);
	}

	public void testEscapeStillEscapesAmpersandOnce() {
		assertEquals("A &amp; B", TextInput.escapeFieldValue("A & B"));
		assertEquals("&lt;b&gt;", TextInput.escapeFieldValue("<b>"));
	}

	public void testRichTextTextareaKeepsMceClass() {
		TextInput input = new TextInput("Pre", 40, 3, new Value("hello"), true);
		String html = renderHtml(input);
		assertContains("mceRichText", html);
		assertContains("hello", html);
	}
}
