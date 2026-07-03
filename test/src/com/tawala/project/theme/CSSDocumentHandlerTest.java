package com.tawala.project.theme;

import java.util.Map;

import junit.framework.TestCase;

public class CSSDocumentHandlerTest extends TestCase {

	public void testParsingMultipleSelectors() throws Exception {
		String css = "h1 {text-decoration: bold; border: none}\n"
				+ "div.tawalaHeader {display: none; width: 50px; }\n"
				+ "div#tawalaProjectContainer {background-color: #ABCD01}\n"
				+ "div.text.error {background-color: #112233}\n"
				+ "div.text.error.yetanother {background-color: #223344}\n";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(5, result.size());

		assertNotNull(result.get("h1"));
		assertNotNull(result.get("div.tawalaHeader"));
		assertNotNull(result.get("div#tawalaProjectContainer"));
		assertNotNull(result.get("div.text.error"));
		assertNotNull(result.get("div.text.error.yetanother"));
	}

	public void testElementSelector() throws Exception {
		String css = "h1 {text-decoration: bold; border: none}\n";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());
		Map<String, String> h1Attributes = result.get("h1");
		assertEquals(2, h1Attributes.size());
		assertEquals("bold", h1Attributes.get("text-decoration"));
		assertEquals("none", h1Attributes.get("border"));

	}

	public void testIdSelector() throws Exception {
		String css = "div#tawalaProjectContainer {background-color: #ABCD01}\n";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());

		Map<String, String> attributes = result
				.get("div#tawalaProjectContainer");
		assertEquals(1, attributes.size());
		assertEquals("#ABCD01", attributes.get("background-color"));
	}

	public void testClassSelector() throws Exception {
		String css = "div.tawalaHeader {display: none; width: 50px; }\n";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());

		Map<String, String> divAttributes = result.get("div.tawalaHeader");
		assertEquals(2, divAttributes.size());
		assertEquals("none", divAttributes.get("display"));
		assertEquals("50px", divAttributes.get("width"));
	}

	public void testDeepClassSelector() throws Exception {
		String css = "div.level1.level2.level3.level4 {background-color: #223344}\n";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());

		Map<String, String> divAttributes = result
				.get("div.level1.level2.level3.level4");
		assertEquals(1, divAttributes.size());
		assertEquals("#223344", divAttributes.get("background-color"));
	}

	public void testFontAttributes() throws Exception {
		String css = "body {font-family:Georgia, serif; }";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());

		Map<String, String> divAttributes = result.get("body");
		assertEquals(1, divAttributes.size());
		assertEquals("Georgia, serif", divAttributes.get("font-family"));
	}

	public void testDescendentSelector() throws Exception {
		String css = "h1.pageHeading div {visibility: hidden; }";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());

		Map<String, String> divAttributes = result.get("h1.pageHeading div");
		assertEquals(1, divAttributes.size());
		assertEquals("hidden", divAttributes.get("visibility"));
	}

	public void testPseudoClass() throws Exception {
		String css = "a:visited {color: #ff00ff; }";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());

		Map<String, String> divAttributes = result.get("a:visited");
		assertEquals(1, divAttributes.size());
		assertEquals("#FF00FF", divAttributes.get("color"));
	}

	public void testAttbiuteSelector() throws Exception {
		String css = "button[type] {color: #ff00ff; }";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());

		Map<String, String> divAttributes = result.get("button[type]");
		assertEquals(1, divAttributes.size());
		assertEquals("#FF00FF", divAttributes.get("color"));
	}

	public void testDirectAdjacentSelector() throws Exception {
		String css = "button+p {color: #ff00ff; }";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());

		Map<String, String> divAttributes = result.get("button+p");
		assertEquals(1, divAttributes.size());
		assertEquals("#FF00FF", divAttributes.get("color"));
	}

	public void testInheritedStyles() throws Exception {
		String css = "body {color: #ff00ff; }\n" + "body {font-size: 22px; }";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());

		Map<String, String> divAttributes = result.get("body");
		assertEquals(2, divAttributes.size());
		assertEquals("#FF00FF", divAttributes.get("color"));
		assertEquals("22px", divAttributes.get("font-size"));
	}

	public void testQuotedAttributes() throws Exception {
		String css = "h1 {font-family: \'Trebuchet MS', Helvetica, sans-serif }";
		Map<String, Map<String, String>> result = CSSDocumentHandler
				.parseCSS(css);
		assertEquals(1, result.size());

		Map<String, String> divAttributes = result.get("h1");
		assertEquals(1, divAttributes.size());
		assertEquals("'Trebuchet MS', Helvetica, sans-serif", divAttributes.get("font-family"));
	}
}
