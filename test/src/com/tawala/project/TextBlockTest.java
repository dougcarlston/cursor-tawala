package com.tawala.project;

import java.util.List;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.project.formatting.Font;
import com.tawala.web.WorldInitializer;
import com.tawala.web.oldhtml.HtmlTestCase;
import com.tawala.project.formatting.DummyTab;

public class TextBlockTest extends HtmlTestCase {
	public void testConstructTextFromXml() {
		String xmlString = "<text label=\"T1\">Plain Text</text>\n";
		TextBlock textBlock = new TextBlock(parseConfig(xmlString));
		assertEquals("Plain Text", textBlock.getTextContents());
	}

	public void testConstructFieldFromXml() {
		String xmlString = "<text label=\"T1\"><field name=\"Q1:a\"/></text>\n";
		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		List<FormRenderable> contents = textBlock.getContents();

		assertEquals(1, contents.size());
		assertTrue(contents.get(0) instanceof FieldReference);

		FieldReference field = (FieldReference) contents.get(0);
		assertEquals("Q1:a", field.getName());
	}

	public void testConstructTextAndFieldFromXml() {
		String xmlString = "<text label=\"T1\">Plain Text<field name=\"Q1:a\"/></text>\n";
		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		List<FormRenderable> contents = textBlock.getContents();

		assertEquals(2, contents.size());
		assertTrue(contents.get(0) instanceof Text);
		assertTrue(contents.get(1) instanceof FieldReference);

		Text text = (Text) contents.get(0);
		assertEquals("Plain Text", text.getContents());

		FieldReference field = (FieldReference) contents.get(1);
		assertEquals("Q1:a", field.getName());
	}

	public void testConstructFontTextFromXml() {
		String xmlString = "<text label=\"T1\">" + "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Plain Text" + "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));
		List<FormRenderable> textBlockContents = textBlock.getContents();

		assertEquals(1, textBlockContents.size());
		assertTrue(textBlockContents.get(0) instanceof Paragraph);

		Paragraph paragraph = (Paragraph) textBlockContents.get(0);
		List<FormRenderable> paragraphContents = paragraph.getContents();

		assertEquals(1, paragraphContents.size());
		assertTrue(paragraphContents.get(0) instanceof Font);

		Font font = (Font) paragraphContents.get(0);

		assertEquals("Plain Text", ((Text) font.getContents().get(0))
				.getContents());
	}

	public void testConstructTabbedFontTextFromXml() {
		String xmlString = "<text label=\"T1\">" + "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Column 1" + "</font>" + "<tab />"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Column 2" + "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));
		List<FormRenderable> textBlockContents = textBlock.getContents();

		assertEquals(1, textBlockContents.size());
		assertTrue(textBlockContents.get(0) instanceof Paragraph);

		Paragraph paragraph = (Paragraph) textBlockContents.get(0);
		List<FormRenderable> paragraphContents = paragraph.getContents();

		assertEquals(3, paragraphContents.size());
		assertTrue(paragraphContents.get(0) instanceof Font);
		assertTrue(paragraphContents.get(1) instanceof DummyTab);
		assertTrue(paragraphContents.get(2) instanceof Font);

		Font font = (Font) paragraphContents.get(0);
		assertEquals("Column 1", ((Text) font.getContents().get(0))
				.getContents());

		font = (Font) paragraphContents.get(2);
		assertEquals("Column 2", ((Text) font.getContents().get(0))
				.getContents());
	}

	public void testConstructParagraphTextFromXml() {
		String xmlString = "<text label=\"T1\">"
				+ "<paragraph indent=\"0\" align=\"left\">" + "Plain Text"
				+ "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));
		List<FormRenderable> textBlockContents = textBlock.getContents();

		assertEquals(1, textBlockContents.size());
		assertTrue(textBlockContents.get(0) instanceof Paragraph);

		Paragraph paragraph = (Paragraph) textBlockContents.get(0);
		List<FormRenderable> paragraphContents = paragraph.getContents();

		assertEquals(1, paragraphContents.size());
		assertTrue(paragraphContents.get(0) instanceof Text);

		Text text = (Text) paragraphContents.get(0);

		assertEquals("Plain Text", text.getContents());
	}

	public void testTextHtml() {
		String xmlString = "<text label=\"T1\">Plain Text</text>\n";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text\">" + "Plain Text" + "</div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof Text);

		assertEquals(htmlString, render(textBlock));
	}

	public void testCompressedHtml() {
		String xmlString = "<text label=\"T1\" paddingBottom=\"false\">Compressed Text</text>\n";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text condensed\">" + "Compressed Text" + "</div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof Text);

		assertEquals(htmlString, render(textBlock));
	}

	public void testFieldHtml() {
		String xmlString = "<text label=\"T1\"><field name=\"Q1:a\"/></text>\n";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text\">" + "</div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof FieldReference);

		assertEquals(htmlString, render(textBlock));
	}

	/*
	 * public void testParagraphHtml() { String xmlString = "<text
	 * label=\"T1\">" + "<paragraph>" + "<font face=\"Arial\" size=\"200\"
	 * color=\"000000\">" + "Black Text" + "</font>" + "</paragraph>" + "</text>";
	 * 
	 * TextBlock textBlock = new TextBlock(parseConfig(xmlString));
	 * 
	 * String htmlString = "<div class=\"text\">" + "<div>" + "<span
	 * style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt;
	 * color: #000000;\">" + "Black Text" + "</span>" + "</div>" + "</div>";
	 * 
	 * assertEquals(1, textBlock.getContents().size());
	 * assertTrue(textBlock.getContents().get(0) instanceof Paragraph);
	 * 
	 * assertEquals(htmlString, render(textBlock)); }
	 */

	public void testPlainTextHtml() {
		String xmlString = "<text label=\"T1\">" + "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Plain Text" + "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text\"><div><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Plain Text</span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof Paragraph);

		assertEquals(htmlString, render(textBlock));
	}

	public void testPlainTextHtmlWithNormalStyle() {
		String xmlString = "<text label=\"T1\" style=\"normal\">"
				+ "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Plain Text" + "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text normal\"><div><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Plain Text</span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof Paragraph);
		assertEquals(htmlString, render(textBlock));
	}

	public void testPlainTextHtmlWithHeadingStyle() {
		String xmlString = "<text label=\"T1\" style=\"heading\">"
				+ "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Plain Text" + "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text heading\"><div><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Plain Text</span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof Paragraph);
		assertEquals(htmlString, render(textBlock));
	}

	public void testPlainTextHtmlWithSubheadingStyle() {
		String xmlString = "<text label=\"T1\" style=\"subheading\">"
				+ "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Plain Text" + "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text subheading\"><div><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Plain Text</span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof Paragraph);
		assertEquals(htmlString, render(textBlock));
	}

	public void testPlainTextHtmlWithInstructionalStyle() {
		String xmlString = "<text label=\"T1\" style=\"instructional\">"
				+ "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Plain Text" + "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text instructional\"><div><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Plain Text</span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof Paragraph);
		assertEquals(htmlString, render(textBlock));
	}

	public void testPlainTextHtmlWithErrorStyle() {
		String xmlString = "<text label=\"T1\" style=\"error\">"
				+ "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Plain Text" + "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text error\"><div><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Plain Text</span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof Paragraph);
		assertEquals(htmlString, render(textBlock));
	}

	public void testBoldTextHtml() {
		String xmlString = "<text label=\"T1\">" + "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "<b>Bold Text</b>" + "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text\"><div><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\"><strong>Bold Text</strong></span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof Paragraph);

		assertEquals(htmlString, render(textBlock));
	}

	public void testItalicTextHtml() {
		String xmlString = "<text label=\"T1\">" + "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "<i>Italic Text</i>" + "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text\"><div><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\"><i>Italic Text</i></span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertTrue(textBlock.getContents().get(0) instanceof Paragraph);

		assertEquals(htmlString, render(textBlock));
	}

	public void testFontFaceHtml() {
		String xmlString = "<text label=\"T1\" style=\"error\">"
				+ "<paragraph>" + "<font face=\"Courier New\">" + "Plain Text"
				+ "</font>" + "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text error\"><div><span style=\"font-family: Courier New; \">Plain Text</span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertEquals(htmlString, render(textBlock));
	}

	public void testFontSizeHtml() {
		String xmlString = "<text label=\"T1\">" + "<paragraph>"
				+ "<font size=\"240\">" + "Plain Text" + "</font>"
				+ "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text\"><div><span style=\"font-size: 12pt; \">Plain Text</span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertEquals(htmlString, render(textBlock));
	}

	public void testFontColorHtml() {
		String xmlString = "<text label=\"T1\">" + "<paragraph>"
				+ "<font color=\"FFAA88\">" + "Plain Text" + "</font>"
				+ "</paragraph>" + "</text>";

		TextBlock textBlock = new TextBlock(parseConfig(xmlString));

		String htmlString = "<div class=\"text\"><div><span style=\"color: #FFAA88;\">Plain Text</span></div></div>";

		assertEquals(1, textBlock.getContents().size());
		assertEquals(htmlString, render(textBlock));
	}

	public void testTableHtml() {
		String xmlString = "<text label=\"T1\">"
				+ "<table indent=\"700\">\n"
				+ "<row>\n"
				+ "<cell width=\"7200\">\n"
				+ "<division indent=\"200\" align=\"center\">\n"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">cell 1</font>\n"
				+ "</division>\n"
				+ "</cell>\n"
				+ "<cell width=\"5400\">\n"
				+ "<division indent=\"0\" align=\"left\">\n"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\"><b><u>cell 2 with underline and bold</u></b></font>\n"
				+ "</division>\n"
				+ "</cell>\n"
				+ "</row>\n"
				+ "<row>\n"
				+ "<cell width=\"7200\">\n"
				+ "<division indent=\"0\" align=\"left\">\n"
				+ "<font face=\"Times New Roman\" size=\"280\" color=\"000000\">cell 3 with Times New Roman 14</font>\n"
				+ "</division>\n"
				+ "</cell>\n"
				+ "<cell width=\"5400\">\n"
				+ "<division indent=\"0\" align=\"left\">\n"
				+ "<font face=\"Arial\" size=\"200\" color=\"FF0000\">cell 4 with 3 lines</font>\n"
				+ "</division>\n"
				+ "<division indent=\"0\" align=\"left\">\n"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">line 2</font>\n"
				+ "</division>\n"
				+ "<division indent=\"0\" align=\"left\">\n"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">line 3</font>\n"
				+ "</division>\n" + "</cell>\n" + "</row>\n" + "</table>"
				+ "</text>";

		String htmlString = "<div class=\"text\"><table style=\"margin-left: 35pt\">\n"
				+ "<tr><td style=\"width: 360pt\"><div style=\"margin-left: 10pt; text-align: center\"><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">cell 1</span></div>\n"
				+ "</td>\n"
				+ "<td style=\"width: 270pt\"><div style=\"text-align: left\"><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\"><strong><span style=\"text-decoration: underline\">cell 2 with underline and bold</span></strong></span></div>\n"
				+ "</td>\n"
				+ "</tr>\n"
				+ "<tr><td style=\"width: 360pt\"><div style=\"text-align: left\"><span style=\"font-family: Times New Roman; font-size: 14pt; color: #000000;\">cell 3 with Times New Roman 14</span></div>\n"
				+ "</td>\n"
				+ "<td style=\"width: 270pt\"><div style=\"text-align: left\"><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #FF0000;\">cell 4 with 3 lines</span></div>\n"
				+ "<div style=\"text-align: left\"><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">line 2</span></div>\n"
				+ "<div style=\"text-align: left\"><span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">line 3</span></div>\n"
				+ "</td>\n" + "</tr>\n" + "</table>" + "</div>";

		assertEquals(htmlString, render(new TextBlock(parseConfig(xmlString))));
	}
}
