package com.tawala.project.commands;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Process;
import com.tawala.web.WorldInitializer;

public class SetCommandTest extends TestCase {
	private User projectOwner = UserTest.aUser("tester");

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
	}

	public void testBasicContcatenation() {
		String xml = "<string value=\"Your name is \" />\n"
				+ "<string field=\"" + FakeExecutionContext.DEFAULT_FORM_ID
				+ ":Q1:a\" />\n" + "<string value=\" \" />\n"
				+ "<string field=\"" + FakeExecutionContext.DEFAULT_FORM_ID
				+ ":Q1:b\" />\n" + "<string value=\".\" />\n";

		assertEquals("Your name is John Smith.", executeSet(xml, "John",
				"Smith"));
	}

	public void testLiteralAddition() {
		String xml = "<add><operand value='3.50'/><operand value='4'/></add>\n";
		assertEquals("7.50", executeSet(xml));
	}

	public void testFieldAddition() {
		String xml = "<add><operand field='"
				+ FakeExecutionContext.DEFAULT_FORM_ID
				+ ":Q1:a'/><operand field='"
				+ FakeExecutionContext.DEFAULT_FORM_ID + ":Q1:b'/></add>\n";
		assertEquals("7.50", executeSet(xml, "1.50", "6"));
	}

	public void testSubtraction() {
		String xml = "<sub><operand value='3'/><operand value='8'/></sub>\n";
		assertEquals("-5", executeSet(xml));
	}

	public void testMultiplication() {
		String xml = "<mul><operand value='3'/><operand value='8'/></mul>\n";
		assertEquals("24", executeSet(xml));
	}

	public void testDivision() {
		assertEquals(
				"4",
				executeSet("<div><operand value='8'/><operand value='2'/></div>\n"));
		assertEquals(
				"0.5",
				executeSet("<div><operand value='1'/><operand value='2'/></div>\n"));
	}

	public void testNestedOperators() {
		String xml = "<add>" + "  <operand value='2'/>" + "  <add>"
				+ "    <operand value='1'/>" + "    <operand value='1'/>"
				+ "  </add>\n" + "</add>\n";
		assertEquals("4", executeSet(xml));
	}

	public void testCombination() {
		String xml = "        <div>\n" + "          <sub>\n"
				+ "            <mul>\n" + "              <add>\n"
				+ "                <operand value=\"1\" />\n"
				+ "                <operand value=\"1\" />\n"
				+ "              </add>\n"
				+ "              <operand value=\"2\" />\n"
				+ "            </mul>\n"
				+ "            <operand value=\"1\" />\n"
				+ "          </sub>\n" + "          <operand value=\"4\" />\n"
				+ "        </div>\n";
		assertEquals("0.75", executeSet(xml));
	}

	private String executeSet(String xml, String... parameters) {
		ConfigElement config = parseConfig("<process name=\"proc1\">\n"
				+ "<set field=\"foo\">\n" + xml + "</set>\n" + "</process>\n");
		Process process = new Process(config);
		FakeExecutionContext context;
		if (parameters.length > 0) {
			context = FakeExecutionContext.contextWithFibValues(projectOwner,
					parameters);
		} else {
			context = FakeExecutionContext.basicContext(projectOwner);
		}
		process.execute(context);
		return context.getValue("foo").toString();
	}

}
