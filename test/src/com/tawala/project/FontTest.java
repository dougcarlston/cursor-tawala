package com.tawala.project;

import java.io.PrintWriter;
import java.io.StringWriter;

import com.tawala.TestCase;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.project.formatting.Font;
import com.tawala.web.oldhtml.RenderingContext;

public class FontTest extends TestCase {

	private final String xmlBlueText = "<font face=\"Arial\" size=\"200\" color=\"0000FF\">Blue Text</font>\n";

	private final String xmlBlackText = "<font face=\"Arial\" size=\"200\" color=\"000000\">Black Text</font>\n";

	public void testHtml() {
		Font font = new Font(parseConfig(xmlBlackText));

		String htmlString = "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">"
				+ "Black Text" + "</span>";
		assertEquals(htmlString, renderAsHtml(font));

		font = new Font(parseConfig(xmlBlueText));

		htmlString = "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #0000FF;\">"
				+ "Blue Text" + "</span>";
		assertEquals(htmlString, renderAsHtml(font));
	}

	public void xxxtestHtmlEscaping() {
		String fontWithSpecialCharacters = "<font face=\"Arial &amp; &lt;XYZ&gt;\" size=\"200\" color=\"0000FF\">\"Some fancy &lt;text&gt;</font>\n";
		Font font = new Font(parseConfig(fontWithSpecialCharacters));

		String htmlString = "<span style=\"font-family: Arial &amp; &lt;XYZ&gt;; font-size: 10pt; color: #0000FF;\">"
				+ "\"Some fancy &lt;text\"" + "</span>";

		String renderedValue = renderAsHtml(font);
		assertEquals(htmlString, renderedValue);
	}

	public void testBold() {
		String xml = "<font face=\"Arial\" size=\"200\" color=\"000000\"><b>Bold text</b></font>";
		Font font = new Font(parseConfig(xml));

		String htmlString = "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">"
				+ "<strong>Bold text</strong>" + "</span>";
		assertEquals(htmlString, renderAsHtml(font));

		xml = "<font face=\"Arial\" size=\"200\" color=\"0000FF\"><b>Bold text</b></font>";
		font = new Font(parseConfig(xml));

		htmlString = "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #0000FF;\">"
				+ "<strong>Bold text</strong>" + "</span>";
		assertEquals(htmlString, renderAsHtml(font));
	}

	public void testItalics() {
		String xml = "<font face=\"Arial\" size=\"200\" color=\"000000\"><i>text in italics</i></font>";
		Font font = new Font(parseConfig(xml));

		String htmlString = "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">"
				+ "<i>text in italics</i>" + "</span>";
		assertEquals(htmlString, renderAsHtml(font));
	}

	public void testBoldItalicized() {
		String xml = "<font face=\"Arial\" size=\"200\" color=\"000000\"><b><i>bold text in italics</i></b></font>";
		Font font = new Font(parseConfig(xml));

		String htmlString = "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">"
				+ "<strong><i>bold text in italics</i></strong>" + "</span>";
		assertEquals(htmlString, renderAsHtml(font));
	}

	public void testBoldUnderlined() {
		String xml = "<font face=\"Arial\" size=\"200\" color=\"000000\"><b><u>bold underlined text</u></b></font>";
		Font font = new Font(parseConfig(xml));

		String htmlString = "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">"
				+ "<strong><span style=\"text-decoration: underline\">bold underlined text</span></strong>"
				+ "</span>";
		assertEquals(htmlString, renderAsHtml(font));
	}

	public void testBoldItalicsUnderlined() {
		String xml = "<font face=\"Arial\" size=\"200\" color=\"000000\"><b><i><u>bold underlined text in italics</u></i></b></font>";
		Font font = new Font(parseConfig(xml));

		String htmlString = "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">"
				+ "<strong><i><span style=\"text-decoration: underline\">bold underlined text in italics</span></i></strong>"
				+ "</span>";
		assertEquals(htmlString, renderAsHtml(font));
	}

	private String renderAsHtml(Font font) {
		StringWriter output = new StringWriter();
		font.toHtml(
				new FakeExecutionContext(ProjectBuilder
						.buildMinimalisticProject())).render(
				new PrintWriter(output), new RenderingContext());
		return output.toString();
	}
}
