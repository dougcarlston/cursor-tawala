package com.tawala.project;

import java.io.IOException;
import java.io.PrintWriter;
import java.io.StringWriter;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import com.tawala.TestCase;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.project.formatting.Font;
import com.tawala.web.oldhtml.RenderingContext;

public class FillInBlankTest extends TestCase {

	public void testXmlConstruction() {
		FormBuilder form = new FormBuilder();
		FibBuilder fibBuilder = form.addFib();
		fibBuilder.addText("What's your name? ");
		fibBuilder.addBlank("first", 20);
		fibBuilder.addText(" ");
		fibBuilder.addBlank("mi", 1);
		fibBuilder.addText(". ");
		fibBuilder.addBlank("last", 20);
		Form projectForm = form.build();
		FillInBlank fib = (FillInBlank) projectForm.getItems().get(0);

		assertEquals("Q1", fib.getId());

		List contents = fib.getContents();
		assertEquals(1, contents.size());

		FormParagraph paragraph = (FormParagraph) contents.get(0);

		contents = paragraph.getContents();
		Iterator i = contents.iterator();
		checkText("What's your name? ", i.next());
		checkField("a", "first", 20, i.next());
		checkText(" ", i.next());
		checkField("b", "mi", 1, i.next());
		checkText(". ", i.next());
		checkField("c", "last", 20, i.next());
		assertFalse(i.hasNext());

		i = fib.fields().iterator();
		checkField("a", "first", 20, i.next());
		checkField("b", "mi", 1, i.next());
		checkField("c", "last", 20, i.next());
		assertFalse(i.hasNext());
	}

	public void testFormId() {
		FormBuilder form = new FormBuilder("f");
		FibBuilder q1 = form.addFib();
		q1.addBlank();
		q1.addBlank("blankAlt");

		FibBuilder qAlt = form.addFib("qAlt");
		qAlt.addBlank();
		qAlt.addBlank("doubleAlt");

		List<FormItem> items = form.build().getItems();
		List<Field> fields = new ArrayList<Field>();
		for (FormItem item : items) {
			fields.addAll(item.fields());
		}
		Iterator<Field> i = fields.iterator();
		assertEquals("Q1:a", i.next().getHtmlId());
		assertEquals("blankAlt", i.next().getHtmlId());
		assertEquals("qAlt:a", i.next().getHtmlId());
		assertEquals("doubleAlt", i.next().getHtmlId());
		assertFalse(i.hasNext());

	}

	private void checkField(String expectedId, String expectedAltLabel,
			int expectedLength, Object o) {
		Blank blank = (Blank) o;
		assertEquals(expectedId, blank.getId());
		assertEquals(expectedAltLabel, blank.getAlternateLabel());
		assertEquals(expectedLength, blank.getLength());
	}

	private void checkText(String expectedContents, Object o) {
		Font actual = (Font) o;
		Text text = (Text)(actual.getContents().get(0));
		assertEquals(expectedContents, text.getContents());
	}

	public void testRequired() {
		FibBuilder builder = new FibBuilder("Q1");
		builder.addBlank(true);
		builder.addBlank(false);
		FillInBlank fib = new FillInBlank(builder.asConfig());
		List<Blank> blanks = fib.getBlanks();
		assertTrue(blanks.get(0).isRequired());
		assertFalse(blanks.get(1).isRequired());
	}

	public void testHtmlWithFib() throws IOException {
		String xmlString = "<fib label=\"Q1\">" + "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Black Text" + "</font>" + 
				"<blank label=\"a\" length=\"10\"/>" + "</paragraph>" + "</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<div class=\"fib\">" +
			"<div>" +
			"Black Text" + 
			"<input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"10\" />" +
			"</div>" + 
			"</div>\n";

		assertEquals(htmlString, renderAsHtml(fib));
	}

	public void testHtmlWithTabbedFib() throws IOException {
		String xmlString =
			"<fib label=\"Q1\">" +
			"<paragraph>" +
			"<tabPositions>" +
			"<tabStop position=\"2880\"/>" +
			"</tabPositions>" +
			"<font face=\"Arial\" size=\"200\" color=\"000000\">Text</font>" +
			"<tab />" +
			"<blank label=\"a\" length=\"10\"/>" +
			"</paragraph>" +
			"</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<div class=\"fib\">" +
			"<table class=\"TAWALA-tabbed-paragraph\">" +
			"<tr>" +
			"<td style=\"width: 144px\">Text</td>\n" +
			"<td><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"10\" /></td>\n" +
			"</tr>\n" +
			"</table>\n" +
			"</div>\n";

		assertEquals(htmlString, renderAsHtml(fib));
	}

	public void testHtmlWithLeftAlignedFib() throws IOException {
		String xmlString =
			"<fib label=\"Q1\" style=\"leftAlignLabels\">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<tabPositions>" +
			"<tabStop position=\"2880\"/>" +
			"</tabPositions>" +
			"<font face=\"Arial\" size=\"200\" color=\"000000\">Left Aligned Label</font>" + 
			"<blank label=\"a\" length=\"10\"/>" +
			"</paragraph>" +
			"</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<table class=\"fib\">\n" +
			"<tr>" +
			"<td class=\"label left\">Left Aligned Label</td>\n" +
			"<td class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"10\" /></td>\n" +
			"<td class=\"remainder\"></td>\n" +
			"</tr>\n" +
			"</table>";

		assertEquals(htmlString, renderAsHtml(fib));
	}

	public void testHtmlWithLeftAlignedTwoParagraphFib() throws IOException {
		String xmlString =
			"<fib label=\"Q1\" style=\"leftAlignLabels\">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<tabPositions>" +
			"<tabStop position=\"2880\"/>" +
			"</tabPositions>" +
			"<font face=\"Arial\" size=\"200\" color=\"000000\">Left Aligned Label One</font>" + 
			"<blank label=\"a\" length=\"10\"/>" +
			"</paragraph>" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<tabPositions>" +
			"<tabStop position=\"2880\"/>" +
			"</tabPositions>" +
			"<font face=\"Arial\" size=\"200\" color=\"000000\">Left Aligned Label Two</font>" + 
			"<blank label=\"b\" length=\"10\"/>" +
			"</paragraph>" +
			"</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<table class=\"fib\">\n" +
			"<tr>" +
			"<td class=\"label left\">Left Aligned Label One</td>\n" +
			"<td class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"10\" /></td>\n" +
			"<td class=\"remainder\"></td>\n" +
			"</tr>\n" +
			"<tr>" +
			"<td class=\"label left\">Left Aligned Label Two</td>\n" +
			"<td class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:b\" id=\"tawalaField_Q1:b\" size=\"10\" /></td>\n" +
			"<td class=\"remainder\"></td>\n" +
			"</tr>\n" +
			"</table>";

		assertEquals(htmlString, renderAsHtml(fib));
	}

	public void testHtmlWithLeftAlignedJustifiedFib() throws IOException {
		String xmlString =
			"<fib label=\"Q1\" style=\"leftAlignLabelsJustified\">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<font>Left Aligned Label</font>" + 
			"<blank label=\"a\" length=\"10\"/>" +
			"</paragraph>" +
			"</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<table class=\"fib justified\">\n" +
			"<tr>" +
			"<td class=\"label left\">Left Aligned Label</td>\n" +
			"<td class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"10\" /></td>\n" +
			"</tr>\n" +
			"</table>";

		assertEquals(htmlString, renderAsHtml(fib));
	}

	public void testHtmlWithLeftAlignedJustifiedMultipleBlankFib() throws IOException {
		String xmlString =
			"<fib label=\"Q1\" style=\"leftAlignLabelsJustified\">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<font>First Name</font>" + 
			"<blank label=\"a\" length=\"10\"/>" +
			"<font>Last Name</font>" + 
			"<blank label=\"b\" length=\"10\"/>" +
			"</paragraph>" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<font>Address</font>" + 
			"<blank label=\"c\" length=\"20\"/>" +
			"</paragraph>" +
			"</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<table class=\"fib justified\">\n" +
			"<tr>" +
			"<td class=\"label left\">First Name</td>\n" +
			"<td style=\"padding-bottom: 0px; padding-top: 0px;\"><table class=\"remainder\" width=\"100%\">\n" +
			"<tr><td class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"10\" /></td>\n" +
			"<td class=\"right\">Last Name</td>\n" +
			"<td class=\"right\" style=\"padding-top: 0px;\"><input type=\"text\" class=\"text\" name=\"Q1:b\" id=\"tawalaField_Q1:b\" size=\"10\" /></td>\n" +
			"</tr>\n" +
			"</table></td>\n" +
			"</tr>\n" +
			"<tr>" +
			"<td class=\"label left\">Address</td>\n" +
			"<td class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:c\" id=\"tawalaField_Q1:c\" size=\"20\" /></td>\n" +
			"</tr>\n" +
			"</table>";

		assertEquals(htmlString, renderAsHtml(fib));
	}

	public void testHtmlWithRightAlignedJustifiedMultipleBlankFib() throws IOException {
		String xmlString =
			"<fib label=\"Q1\" style=\"rightAlignLabelsJustified\">" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<font>First Name</font>" + 
			"<blank label=\"a\" length=\"10\"/>" +
			"<font>Last Name</font>" + 
			"<blank label=\"b\" length=\"10\"/>" +
			"</paragraph>" +
			"<paragraph indent=\"0\" align=\"left\">" +
			"<font>Address</font>" + 
			"<blank label=\"c\" length=\"20\"/>" +
			"</paragraph>" +
			"</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<table class=\"fib justified\">\n" +
			"<tr>" +
			"<td class=\"label right\">First Name</td>\n" +
			"<td style=\"padding-bottom: 0px; padding-top: 0px;\"><table class=\"remainder\" width=\"100%\">\n" +
			"<tr><td class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"10\" /></td>\n" +
			"<td class=\"right\">Last Name</td>\n" +
			"<td class=\"right\" style=\"padding-top: 0px;\"><input type=\"text\" class=\"text\" name=\"Q1:b\" id=\"tawalaField_Q1:b\" size=\"10\" /></td>\n" +
			"</tr>\n" +
			"</table></td>\n" +
			"</tr>\n" +
			"<tr>" +
			"<td class=\"label right\">Address</td>\n" +
			"<td class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:c\" id=\"tawalaField_Q1:c\" size=\"20\" /></td>\n" +
			"</tr>\n" +
			"</table>";

		assertEquals(htmlString, renderAsHtml(fib));
	}

	public void testHtmlWithLeftAlignedFibAndExtraText() throws IOException {
		String xmlString =
			"<fib label=\"Q1\" style=\"leftAlignLabels\">" +
			"<paragraph>" +
			"<font face=\"Arial\" size=\"200\" color=\"000000\">Left Aligned Label</font>" + 
			"<blank label=\"a\" length=\"10\"/>" +
			"<font face=\"Arial\" size=\"200\" color=\"000000\">and some extra text</font>" + 
			"</paragraph>" +
			"</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<table class=\"fib\">\n" +
			"<tr>" +
			"<td class=\"label left\">Left Aligned Label</td>\n" +
			"<td class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"10\" /></td>\n" +
			"<td class=\"remainder\">and some extra text</td>\n" +
			"</tr>\n" +
			"</table>";

		assertEquals(htmlString, renderAsHtml(fib));
	}

	public void testHtmlWithRightAlignedFib() {
		String xmlString =
			"<fib label=\"Q1\" style=\"rightAlignLabels\">" +
			"<paragraph>" +
			"<font face=\"Arial\" size=\"200\" color=\"000000\">Right Aligned Label</font>" + 
			"<blank label=\"a\" length=\"10\"/>" +
			"</paragraph>" +
			"</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<table class=\"fib\">\n" +
			"<tr>" +
			"<td class=\"label right\">Right Aligned Label</td>\n" +
			"<td class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"10\" /></td>\n" +
			"<td class=\"remainder\"></td>\n" +
			"</tr>\n" +
			"</table>";

		assertEquals(htmlString, renderAsHtml(fib));
	}

	public void testHtmlWithTopAlignedFib() {
		String xmlString =
			"<fib label=\"Q1\" style=\"topLabels\">" +
			"<paragraph>" +
			"<font face=\"Arial\" size=\"200\" color=\"000000\">Top Label</font>" + 
			"<blank label=\"a\" length=\"10\"/>" +
			"</paragraph>" +
			"</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<div class=\"fib vertical\">" +
			"<div>Top Label" +
			"<div class=\"blank\"><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"10\" /></div>\n" +
			"</div>" +
			"</div>\n";

		assertEquals(htmlString, renderAsHtml(fib));
	}

	public void testHtmlTextAreaWithFib() {
		String xmlString = "<fib label=\"Q1\">" + "<paragraph>"
				+ "<font face=\"Arial\" size=\"200\" color=\"000000\">"
				+ "Black Text" + "</font>" + 
				"<blank label=\"a\" length=\"10\" height=\"3\"/>" + "</paragraph>" + "</fib>";

		FillInBlank fib = new FillInBlank(parseConfig(xmlString));

		String htmlString =
			"<div class=\"fib\">" +
			"<div>"	+
			"Black Text" +  
			"<textarea class=\"textArea\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" cols=\"10\" rows=\"3\"></textarea>" +
			"</div>" +
			"</div>\n";

		assertEquals(htmlString, renderAsHtml(fib));

	}

	private String renderAsHtml(Question fib) {
		StringWriter output = new StringWriter();
		fib.toHtml(
				new FakeExecutionContext(ProjectBuilder
						.buildMinimalisticProject())).render(
				new PrintWriter(output), new RenderingContext());
		return output.toString();
	}

}
