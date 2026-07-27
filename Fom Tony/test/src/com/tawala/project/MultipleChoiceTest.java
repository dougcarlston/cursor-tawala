package com.tawala.project;

import java.util.List;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.builder.FormBuilder;
import com.tawala.web.WorldInitializer;
import com.tawala.web.oldhtml.HtmlTestCase;

public class MultipleChoiceTest extends HtmlTestCase {
	@Override
	protected void setUp() throws Exception {
		new HibernateTestSetup().onSetUp();
		super.setUp();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
	}

	public void testMultipleChoiceBasics() {
		FormBuilder builder = new FormBuilder();
		builder.addMc("What is your favorite color?", "Cyan");

		MultipleChoice mc = (MultipleChoice) builder.build().getItems().get(0);
		assertEquals("Q1", mc.getId());
		assertTrue(mc.hasFields());
		List<Field> fields = mc.fields();
		assertEquals(1, fields.size());
		Field field = fields.get(0);
		assertEquals("Q1", field.getHtmlId());
	}

	public void testAlternateLabel() {
		FormBuilder builder = new FormBuilder();
		builder.addMcWithAlternateLabel("Color",
				"What is your favorite color?", "Cyan");
		Form form = builder.build();
		MultipleChoice mc = (MultipleChoice) form.getItems().get(0);
		assertEquals("Q1", mc.getId());
		assertEquals("Color", mc.fields().get(0).getHtmlId());
	}

	public void testAllowManyHtml() {
		FormBuilder builder = new FormBuilder();
		builder.addMc("What colors do you like?", false, false, "Cyan",
				"Magenta", "Black");
		MultipleChoice mc = (MultipleChoice) builder.build().getItems().get(0);
		assertFalse(mc.onlyOne());
		assertEquals(
				"<div id=\"Q1Container\" class=\"mcCheckbox\">"
						+ "<label class=\"question\">What colors do you like?</label>\n"
						+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-a\" type=\"checkbox\" value=\"a\" /> <label for=\"Q1-a\">Cyan</label></span>\n"
						+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-b\" type=\"checkbox\" value=\"b\" /> <label for=\"Q1-b\">Magenta</label></span>\n"
						+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-c\" type=\"checkbox\" value=\"c\" /> <label for=\"Q1-c\">Black</label></span>\n"
						+ "</div>\n", render(mc));
	}

	public void testOnlyOneHtml() {
		FormBuilder builder = new FormBuilder();
		builder.addMc("What is your favorite color?", true, false, "Cyan",
				"Magenta", "Black");
		MultipleChoice mc = (MultipleChoice) builder.build().getItems().get(0);
		assertTrue(mc.onlyOne());
		assertEquals(
				"<div id=\"Q1Container\" class=\"mcRadio\">"
						+ "<label class=\"question\">What is your favorite color?</label>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-a\" type=\"radio\" value=\"a\" /> <label for=\"Q1-a\">Cyan</label></span>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-b\" type=\"radio\" value=\"b\" /> <label for=\"Q1-b\">Magenta</label></span>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-c\" type=\"radio\" value=\"c\" /> <label for=\"Q1-c\">Black</label></span>\n"
						+ "</div>\n", render(mc));
	}

	public void testVerticalHtml() {
		FormBuilder builder = new FormBuilder();
		builder.addMc("What is your favorite color?", "vertical", true, false,
				"Cyan", "Magenta", "Black");
		FormRenderable mc = builder.build().getItems().get(0);
		assertEquals(
				"<div id=\"Q1Container\" class=\"mcRadio vertical\">"
						+ "<label class=\"question\">What is your favorite color?</label>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-a\" type=\"radio\" value=\"a\" /> <label for=\"Q1-a\">Cyan</label></span>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-b\" type=\"radio\" value=\"b\" /> <label for=\"Q1-b\">Magenta</label></span>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-c\" type=\"radio\" value=\"c\" /> <label for=\"Q1-c\">Black</label></span>\n"
						+ "</div>\n", render(mc));

		// --- Assert that multiple invocations don't cause problems.
		assertEquals(
				"<div id=\"Q1Container\" class=\"mcRadio vertical\">"
						+ "<label class=\"question\">What is your favorite color?</label>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-a\" type=\"radio\" value=\"a\" /> <label for=\"Q1-a\">Cyan</label></span>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-b\" type=\"radio\" value=\"b\" /> <label for=\"Q1-b\">Magenta</label></span>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-c\" type=\"radio\" value=\"c\" /> <label for=\"Q1-c\">Black</label></span>\n"
						+ "</div>\n", render(mc));
	}

	public void testHorizontalHtml() {
		FormBuilder builder = new FormBuilder();
		builder.addMc("What is your favorite color?", "horizontal", true,
				false, "Cyan", "Magenta", "Black");
		FormRenderable mc = builder.build().getItems().get(0);
		assertEquals(
				"<div id=\"Q1Container\" class=\"mcRadio horizontal\">"
						+ "<label class=\"question\">What is your favorite color?</label>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-a\" type=\"radio\" value=\"a\" /> <label for=\"Q1-a\">Cyan</label></span>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-b\" type=\"radio\" value=\"b\" /> <label for=\"Q1-b\">Magenta</label></span>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-c\" type=\"radio\" value=\"c\" /> <label for=\"Q1-c\">Black</label></span>\n"
						+ "</div>\n", render(mc));

		// --- Assert that multiple invocations don't cause problems.
		assertEquals(
				"<div id=\"Q1Container\" class=\"mcRadio horizontal\">"
						+ "<label class=\"question\">What is your favorite color?</label>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-a\" type=\"radio\" value=\"a\" /> <label for=\"Q1-a\">Cyan</label></span>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-b\" type=\"radio\" value=\"b\" /> <label for=\"Q1-b\">Magenta</label></span>\n"
						+ "<span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-c\" type=\"radio\" value=\"c\" /> <label for=\"Q1-c\">Black</label></span>\n"
						+ "</div>\n", render(mc));
	}

	public void testMultiColumnHtml() {
		FormBuilder builder = new FormBuilder();
		builder.addMc("What is your favorite color?", "multicolumn", true,
				false, "Red", "Orange", "Yellow", "Green", "Blue", "Violet");
		FormRenderable mc = builder.build().getItems().get(0);

		String htmlString = "<div id=\"Q1Container\" class=\"mcRadio multicolumn\">"
				+ "<label class=\"question\">What is your favorite color?</label>\n"
				+ "<table class=\"answer\">"
				+ "<tbody>"
				+ "<tr>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-a\" type=\"radio\" value=\"a\" /> <label for=\"Q1-a\">Red</label></span></td>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-d\" type=\"radio\" value=\"d\" /> <label for=\"Q1-d\">Green</label></span></td>"
				+ "</tr>"
				+ "<tr>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-b\" type=\"radio\" value=\"b\" /> <label for=\"Q1-b\">Orange</label></span></td>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-e\" type=\"radio\" value=\"e\" /> <label for=\"Q1-e\">Blue</label></span></td>"
				+ "</tr>"
				+ "<tr>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-c\" type=\"radio\" value=\"c\" /> <label for=\"Q1-c\">Yellow</label></span></td>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-f\" type=\"radio\" value=\"f\" /> <label for=\"Q1-f\">Violet</label></span></td>"
				+ "</tr>" + "</tbody>\n" + "</table>\n" + "</div>\n";

		assertEquals(htmlString, render(mc));
	}

	public void testRequiredMultiColumnHtml() {
		FormBuilder builder = new FormBuilder();
		builder.addMc("What is your favorite color?", "multicolumn", true,
				true, "Red", "Orange", "Yellow", "Green", "Blue", "Violet");
		FormRenderable mc = builder.build().getItems().get(0);

		String htmlString = "<div id=\"Q1Container\" class=\"mcRadio multicolumn\">"
				+ "<label class=\"question\">What is your favorite color? *</label>\n"
				+ "<table class=\"answer\">"
				+ "<tbody>"
				+ "<tr>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-a\" type=\"radio\" value=\"a\" /> <label for=\"Q1-a\">Red</label></span></td>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-d\" type=\"radio\" value=\"d\" /> <label for=\"Q1-d\">Green</label></span></td>"
				+ "</tr>"
				+ "<tr>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-b\" type=\"radio\" value=\"b\" /> <label for=\"Q1-b\">Orange</label></span></td>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-e\" type=\"radio\" value=\"e\" /> <label for=\"Q1-e\">Blue</label></span></td>"
				+ "</tr>"
				+ "<tr>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-c\" type=\"radio\" value=\"c\" /> <label for=\"Q1-c\">Yellow</label></span></td>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-f\" type=\"radio\" value=\"f\" /> <label for=\"Q1-f\">Violet</label></span></td>"
				+ "</tr>"
				+ "</tbody>\n"
				+ "</table>\n"
				+ "</div>\n"
				+ "<script>Tawala.validation.register('Q1Container', Tawala.validation.nonEmptyMCQValidation, {\"containerId\":\"Q1Container\",\"fieldName\":\"Q1\",\"type\":\"radio\"});</script>";

		assertEquals(htmlString, render(mc));
	}

	public void testColumnCountHtml() {
		FormBuilder builder = new FormBuilder();
		builder.addMultiColumnMc("What is your favorite color?", 3, true,
				false, "Red", "Orange", "Yellow", "Green", "Blue", "Violet");
		FormRenderable mc = builder.build().getItems().get(0);

		String htmlString = "<div id=\"Q1Container\" class=\"mcRadio multicolumn\">"
				+ "<label class=\"question\">What is your favorite color?</label>\n"
				+ "<table class=\"answer\">"
				+ "<tbody>"
				+ "<tr>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-a\" type=\"radio\" value=\"a\" /> <label for=\"Q1-a\">Red</label></span></td>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-c\" type=\"radio\" value=\"c\" /> <label for=\"Q1-c\">Yellow</label></span></td>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-e\" type=\"radio\" value=\"e\" /> <label for=\"Q1-e\">Blue</label></span></td>"
				+ "</tr>"
				+ "<tr>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-b\" type=\"radio\" value=\"b\" /> <label for=\"Q1-b\">Orange</label></span></td>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-d\" type=\"radio\" value=\"d\" /> <label for=\"Q1-d\">Green</label></span></td>"
				+ "<td><span class=\"answer\"><input class=\"radio\" name=\"Q1\" id=\"Q1-f\" type=\"radio\" value=\"f\" /> <label for=\"Q1-f\">Violet</label></span></td>"
				+ "</tr>" + "</tbody>\n" + "</table>\n" + "</div>\n";

		assertEquals(htmlString, render(mc));
	}

	public static FormBuilder newMultipleChoiceForm(String formName,
			int answerCount) {
		String[] answers = new String[answerCount];
		for (int i = 0; i < answers.length; i++) {
			answers[i] = "Choice " + (1 + i);
		}
		FormBuilder builder = new FormBuilder(formName);
		builder.addMc("Which choice do you like?", false, false, answers);
		return builder;
	}

	public static String answerLabel(int position) {
		char letter = (char) ('a' + ((position - 1) % 26));
		int count = 1 + position / 26;
		StringBuffer result = new StringBuffer();
		for (int i = 0; i < count; i++) {
			result.append(letter);
		}
		return result.toString();
	}

	public void testPreviousVersion() {
		String xml = "<mc label=\"Q1\">"
				+ "<question>What colors do you like?</question>"
				+ "<choice label=\"a\">Cyan</choice>"
				+ "<choice label=\"b\">Magenta</choice>"
				+ "<choice label=\"c\">Black</choice>" + "</mc>";
		MultipleChoice mc = new MultipleChoice(new ConfigElement(xml));

		assertFalse(mc.onlyOne());
		assertEquals(
				"<div id=\"Q1Container\" class=\"mcCheckbox\">"
						+ "<label class=\"question\">What colors do you like?</label>\n"
						+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-a\" type=\"checkbox\" value=\"a\" /> <label for=\"Q1-a\">Cyan</label></span>\n"
						+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-b\" type=\"checkbox\" value=\"b\" /> <label for=\"Q1-b\">Magenta</label></span>\n"
						+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-c\" type=\"checkbox\" value=\"c\" /> <label for=\"Q1-c\">Black</label></span>\n"
						+ "</div>\n", render(mc));
	}

}
