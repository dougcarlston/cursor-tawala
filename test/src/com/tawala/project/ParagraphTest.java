package com.tawala.project;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.util.List;

import com.tawala.TestCase;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.web.oldhtml.RenderingContext;

public class ParagraphTest extends TestCase {

	public void testConstructTextFromXml() {

		String xmlString = "<paragraph>" + "Blue Text" + "</paragraph>";

		Paragraph paragraph = new Paragraph(parseConfig(xmlString));

		List<FormRenderable> contents = paragraph.getContents();

		assertEquals(1, contents.size());
		assertTrue(contents.get(0) instanceof Text);
	}

	public void testHtml() {
		String xmlString = "<paragraph>" + "<unsupportedTag>"
				+ "Unsupported Text" + "</unsupportedTag>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Black Text" + "</font>" + "</paragraph>";

		Paragraph paragraph = new Paragraph(parseConfig(xmlString));

		String htmlString = "<div>"
				+ "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">"
				+ "Black Text" + "</span>" + "</div>";

		assertEquals(htmlString, renderAsHtml(paragraph));
	}

	public void testEmptyParagraphHtml() {
		String xmlString = "<paragraph>" + "</paragraph>";

		Paragraph paragraph = new Paragraph(parseConfig(xmlString));

		String htmlString = "<div>" + "&nbsp;" + "</div>";

		assertEquals(htmlString, renderAsHtml(paragraph));
	}

	public void testOldTabButNoTabPositionsCompatibility() {
		String xmlString = "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "First Column" + "</font>" + "<tab />"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Second Column" + "</font>" + "<tab />"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Thrid Column" + "</font>" + "</paragraph>";

		Paragraph paragraph = new Paragraph(parseConfig(xmlString));

		String htmlString = "<div>"
				+ "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">First Column</span>"
				+ " "
				+ "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Second Column</span>"
				+ " "
				+ "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Thrid Column</span>"
				+ "</div>";

		assertEquals(htmlString, renderAsHtml(paragraph));
	}

	public void testFewerTabStopsThanTabs() {
		String xmlString = "<paragraph>" + "<tabPositions>"
				+ "<tabStop position=\"800\"/>" + "</tabPositions>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "First Column" + "</font>" + "<tab />"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Second Column" + "</font>" + "<tab />"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Thrid Column" + "</font>" + "</paragraph>";

		Paragraph paragraph = new Paragraph(parseConfig(xmlString));

		String htmlString = "<table class=\"TAWALA-tabbed-paragraph\">"
				+ "<tr>"
				+ "<td style=\"width: 40px\">"
				+ "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">First Column</span>"
				+ "</td>\n"
				+ "<td style=\"width: 200px\">"
				+ "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Second Column</span>"
				+ "</td>\n"
				+ "<td>"
				+ "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Thrid Column</span>"
				+ "</td>\n" + "</tr>\n" + "</table>\n";

		assertEquals(htmlString, renderAsHtml(paragraph));
	}

	public void testTabs() {
		String xmlString = "<paragraph>" + "<tabPositions>"
				+ "<tabStop position=\"800\"/>"
				+ "<tabStop position=\"1200\"/>"
				+ "<tabStop position=\"1600\"/>"
				+ "<tabStop position=\"2000\"/>" + "</tabPositions>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "First Column" + "</font>" + "<tab />"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Second Column" + "</font>" + "<tab />"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Thrid Column" + "</font>" + "</paragraph>";

		Paragraph paragraph = new Paragraph(parseConfig(xmlString));

		String htmlString = "<table class=\"TAWALA-tabbed-paragraph\">"
				+ "<tr>"
				+ "<td style=\"width: 40px\">"
				+ "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">First Column</span>"
				+ "</td>\n"
				+ "<td style=\"width: 20px\">"
				+ "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Second Column</span>"
				+ "</td>\n"
				+ "<td>"
				+ "<span style=\"font-family: Arial, Helvetica, sans-serif; font-size: 10pt; color: #000000;\">Thrid Column</span>"
				+ "</td>\n" + "</tr>\n" + "</table>\n";

		assertEquals(htmlString, renderAsHtml(paragraph));
	}

	public void testAlignmentAndIndent() {
		String xmlString = "<paragraph indent=\"600\" align=\"center\">Text</paragraph>";
		Paragraph paragraph = new Paragraph(parseConfig(xmlString));
		String htmlString = "<div style=\"margin-left: 30pt; text-align: center\">Text</div>";
		assertEquals(htmlString, renderAsHtml(paragraph));

		xmlString = "<paragraph align=\"right\">Text</paragraph>";
		paragraph = new Paragraph(parseConfig(xmlString));
		htmlString = "<div style=\"text-align: right\">Text</div>";
		assertEquals(htmlString, renderAsHtml(paragraph));

		xmlString = "<paragraph indent=\"800\">Text</paragraph>";
		paragraph = new Paragraph(parseConfig(xmlString));
		htmlString = "<div style=\"margin-left: 40pt\">Text</div>";
		assertEquals(htmlString, renderAsHtml(paragraph));
	}

	private String renderAsHtml(Paragraph paragraph) {
		StringWriter output = new StringWriter();
		paragraph.toHtml(
				new FakeExecutionContext(ProjectBuilder
						.buildMinimalisticProject())).render(
				new PrintWriter(output), new RenderingContext());
		return output.toString();
	}
}
