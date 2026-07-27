package com.tawala.project;

import com.tawala.web.oldhtml.HtmlTestCase;

public class HeadingItemTest extends HtmlTestCase {

	public void testConstructHeadingItemFromXml() {
		String xmlString = "<heading label=\"H1\" type=\"Main\">Heading Item</heading>\n";
		HeadingItem headingItem = new HeadingItem(parseConfig(xmlString));
		assertEquals("Heading Item", headingItem.getTextContents());
	}

	public void testMainHeadingHtml() {
		String xmlString = "<heading label=\"H1\" type=\"Main\">Heading Item</heading>\n";
		HeadingItem headingItem = new HeadingItem(parseConfig(xmlString));

		String htmlString = "<h1 class=\"heading\">" + "Heading Item" + "</h1>\n";

		assertEquals(1, headingItem.getContents().size());
		assertEquals(htmlString, render(headingItem));
	}

	public void testSubHeadingHtml() {
		String xmlString = "<heading label=\"H1\" type=\"Sub\">Subheading Item</heading>\n";
		SubheadingItem headingItem = new SubheadingItem(parseConfig(xmlString));

		String htmlString = "<h2 class=\"subheading\">" + "Subheading Item" + "</h2>\n";

		assertEquals(1, headingItem.getContents().size());
		assertEquals(htmlString, render(headingItem));
	}
	
	public void testParagraphInHeadingItemXmlIsIgnored() {
		String xmlString = 
			"<heading label=\"H1\" type=\"Main\">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<tabPositions><tabStop position=\"2880\"/></tabPositions>" +
			"<font face=\"Arial\" size=\"360\" color=\"000000\">Heading Item</font></paragraph>" +
			"</heading>\n";

		HeadingItem headingItem = new HeadingItem(parseConfig(xmlString));

		String htmlString = "<h1 class=\"heading\">" + "Heading Item" + "</h1>\n";

		assertEquals(1, headingItem.getContents().size());
		assertEquals(htmlString, render(headingItem));
	}

	public void testParagraphInSubheadingItemXmlIsIgnored() {
		String xmlString = 
			"<heading label=\"H1\" type=\"Sub\">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<tabPositions><tabStop position=\"2880\"/></tabPositions>" +
			"<font face=\"Arial\" size=\"360\" color=\"000000\">Subheading Item</font></paragraph>" +
			"</heading>\n";

		SubheadingItem headingItem = new SubheadingItem(parseConfig(xmlString));

		String htmlString = "<h2 class=\"subheading\">" + "Subheading Item" + "</h2>\n";

		assertEquals(1, headingItem.getContents().size());
		assertEquals(htmlString, render(headingItem));
	}

	public void testParagraphWithoutFontTag() {
		String xmlString = 
			"<heading label=\"H1\" type=\"Main\">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"Heading Item" +
			"</paragraph>" +
			"</heading>\n";

		HeadingItem headingItem = new HeadingItem(parseConfig(xmlString));

		String htmlString = "<h1 class=\"heading\">" + "Heading Item" + "</h1>\n";

		assertEquals(1, headingItem.getContents().size());
		assertEquals(htmlString, render(headingItem));
	}
}
