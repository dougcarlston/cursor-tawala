package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.Domain;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.Form;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.MultipleChoiceTest;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.FakeRequest;
import com.tawala.web.Request;
import com.tawala.web.WorldInitializer;

public class ConditionsTest extends TestCase {

	public ConditionsTest() {
		setUserNamesToDelete(new String[] { "testuser" });
	}

	private Domain domain;
	private User owner;
	private HibernateTestSetup hibernateTestSetup = new HibernateTestSetup();

	public void setUp() throws Exception {
		super.setUp();
		hibernateTestSetup.onSetUp();

		domain = new Domain(new UsersHibernateImpl());
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));

		owner = UserTest.aUser("testuser");
		domain.users().addOrSave(owner);
	}

	public void testEquals() {
		checkLiteral(true, "foo", "equals", "foo");
		checkLiteral(true, "foo", "equals", "Foo");
		checkLiteral(true, "foo", "equals", " Foo ");
		checkLiteral(false, "foo", "equals", "bar");

		checkLiteral(true, "1", "equals", "1");
		checkLiteral(true, "1", "equals", "1.0");
		checkLiteral(true, "1", "equals", " 1.0");
		checkLiteral(true, "1", "equals", "1.0 ");
		checkLiteral(true, "1", "equals", " 1.0 ");

		checkLiteral(false, "1", "equals", "$1");
		checkLiteral(true, "$1", "equals", "$1");
		checkLiteral(false, "1", "equals", "1x");
		checkLiteral(true, "1x", "equals", "1x");
	}

	public void testNotEquals() {
		checkLiteral(!true, "foo", "doesNotEqual", "foo");
		checkLiteral(!false, "foo", "doesNotEqual", "bar");
	}

	public void testBeginsWith() {
		checkLiteral(true, "foo", "beginsWith", "foo");
		checkLiteral(true, "foo", "beginsWith", "Foo");
		checkLiteral(true, "foobar", "beginsWith", "foo");
		checkLiteral(false, "foobar", "beginsWith", "oob");
		checkLiteral(false, "foobar", "beginsWith", "bar");
	}

	public void testEndsWith() {
		checkLiteral(true, "bar", "endsWith", "bar");
		checkLiteral(true, "bar", "endsWith", "Bar");
		checkLiteral(true, "foobar", "endsWith", "bar");
		checkLiteral(false, "foobar", "endsWith", "ba");
		checkLiteral(false, "foobar", "endsWith", "foo");
	}

	public void testContains() {
		checkLiteral(true, "bar", "contains", "bar");
		checkLiteral(true, "bar", "contains", "Bar");
		checkLiteral(true, "foobar", "contains", "bar");
		checkLiteral(true, "barfoo", "contains", "bar");
		checkLiteral(true, "foobar", "contains", "oob");
		checkLiteral(false, "foobar", "contains", "barfoo");
	}

	public void testDoesNotContain() {
		checkLiteral(!true, "bar", "doesNotContain", "bar");
		checkLiteral(!true, "bar", "doesNotContain", "Bar");
		checkLiteral(!true, "foobar", "doesNotContain", "bar");
		checkLiteral(!true, "barfoo", "doesNotContain", "bar");
		checkLiteral(!true, "foobar", "doesNotContain", "oob");
		checkLiteral(!false, "foobar", "doesNotContain", "barfoo");
	}

	public void testIsBlank() {
		checkLiteral(false, "foo", "isBlank");
		checkLiteral(false, "f o o", "isBlank");
		checkLiteral(true, "", "isBlank");
		checkLiteral(true, " ", "isBlank");
		checkLiteral(true, "  ", "isBlank");
	}

	public void testIsNotBlank() {
		checkLiteral(!false, "foo", "isNotBlank");
		checkLiteral(!false, "f o o", "isNotBlank");
		checkLiteral(!true, "", "isNotBlank");
		checkLiteral(!true, " ", "isNotBlank");
		checkLiteral(!true, "  ", "isNotBlank");
	}

	public void testIsLessThan() {
		checkLiteral(true, "1", "isLessThan", "2");
		checkLiteral(false, "2", "isLessThan", "2");
		checkLiteral(false, "2", "isLessThan", "1");
		checkLiteral(true, "1", "isLessThan", "1.00000001");
	}

	public void testIsGreaterThan() {
		checkLiteral(true, "2", "isGreaterThan", "1");
		checkLiteral(false, "2", "isGreaterThan", "2");
		checkLiteral(false, "1", "isGreaterThan", "2");
		checkLiteral(true, "1.00000001", "isGreaterThan", "1");
	}

	public void testIsLessThanOrEqualTo() {
		checkLiteral(true, "1", "isLessThanOrEqualTo", "2");
		checkLiteral(true, "2", "isLessThanOrEqualTo", "2");
		checkLiteral(false, "2", "isLessThanOrEqualTo", "1");
		checkLiteral(true, "1", "isLessThanOrEqualTo", "1.00000001");
	}

	public void testIsGreaterThanOrEqualTo() {
		checkLiteral(true, "2", "isGreaterThanOrEqualTo", "1");
		checkLiteral(true, "2", "isGreaterThanOrEqualTo", "2");
		checkLiteral(false, "1", "isGreaterThanOrEqualTo", "2");
		checkLiteral(true, "1.00000001", "isGreaterThanOrEqualTo", "1");
	}

	public void testMultipleChoiceEquals() {
		checkMc(true, 3, "a", "mcEquals", "a");
		checkMc(true, 3, "b", "mcEquals", "b");
		checkMc(false, 3, "a", "mcEquals", "a", "b");
		checkMc(false, 3, "b", "mcEquals", "a", "b");
		checkMc(false, 3, "a", "mcEquals", "b");
		checkMc(false, 3, "a", "mcEquals", "b", "c");
		checkMc(false, 3, "a", "mcEquals");
	}

	private void checkMultipleChoice(boolean expectedResult, String operation) {
		checkMcToMc(expectedResult, 3, operation, new String[] { "a" },
				new String[] { "a" });
		checkMcToMc(expectedResult, 3, operation, new String[] { "b" },
				new String[] { "b" });
		checkMcToMc(expectedResult, 3, operation, new String[] { "c" },
				new String[] { "c" });
		checkMcToMc(expectedResult, 3, operation, new String[] { "a", "b" },
				new String[] { "a", "b" });
		checkMcToMc(expectedResult, 3, operation, new String[] { "b", "c" },
				new String[] { "b", "c" });
		checkMcToMc(expectedResult, 3, operation, new String[] { "a", "c" },
				new String[] { "a", "c" });
		checkMcToMc(expectedResult, 3, operation,
				new String[] { "a", "b", "c" }, new String[] { "a", "b", "c" });
		checkMcToMc(expectedResult, 3, operation, new String[] {},
				new String[] {});
		checkMcToMc(!expectedResult, 3, operation, new String[] { "a" },
				new String[] { "a", "b", "c" });
		checkMcToMc(!expectedResult, 3, operation, new String[] { "b" },
				new String[] { "a", "b", "c" });
		checkMcToMc(!expectedResult, 3, operation, new String[] { "c" },
				new String[] { "a", "b", "c" });
		checkMcToMc(!expectedResult, 3, operation, new String[] { "a" },
				new String[] {});
		checkMcToMc(!expectedResult, 3, operation,
				new String[] { "a", "b", "c" }, new String[] { "a" });
		checkMcToMc(!expectedResult, 3, operation,
				new String[] { "a", "b", "c" }, new String[] { "b" });
		checkMcToMc(!expectedResult, 3, operation,
				new String[] { "a", "b", "c" }, new String[] { "c" });
		checkMcToMc(!expectedResult, 3, operation,
				new String[] { "a", "b", "c" }, new String[] {});
	}

	public void testMultipleChoiceEqualsMC() {
		checkMultipleChoice(true, "mcEquals");
	}

	public void testMultipleChoiceDoesNotEqualMC() {
		checkMultipleChoice(false, "mcDoesNotEqual");
	}

	public void testMultipleChoiceDoesNotEqual() {
		checkMc(false, 3, "a", "mcDoesNotEqual", "a");
		checkMc(false, 3, "b", "mcDoesNotEqual", "b");
		checkMc(true, 3, "a", "mcDoesNotEqual", "a", "b");
		checkMc(true, 3, "b", "mcDoesNotEqual", "a", "b");
		checkMc(true, 3, "a", "mcDoesNotEqual", "b");
		checkMc(true, 3, "a", "mcDoesNotEqual", "b", "c");
		checkMc(true, 3, "a", "mcDoesNotEqual");
	}

	public void testMultipleChoiceContainsValue() {
		checkMc(true, 3, "a", "mcContains", "a");
		checkMc(true, 3, "b", "mcContains", "b");
		checkMc(true, 3, "a", "mcContains", "a", "b");
		checkMc(true, 3, "b", "mcContains", "a", "b");
		checkMc(false, 3, "a", "mcContains", "b");
		checkMc(false, 3, "a", "mcContains", "b", "c");
		checkMc(false, 3, "a", "mcContains");
	}

	public void testMultipleChoiceDoesNotContainValue() {
		checkMc(false, 3, "a", "mcDoesNotContain", "a");
		checkMc(false, 3, "b", "mcDoesNotContain", "b");
		checkMc(false, 3, "a", "mcDoesNotContain", "a", "b");
		checkMc(false, 3, "b", "mcDoesNotContain", "a", "b");
		checkMc(true, 3, "a", "mcDoesNotContain", "b");
		checkMc(true, 3, "a", "mcDoesNotContain", "b", "c");
		checkMc(true, 3, "a", "mcDoesNotContain");
	}

	public void testMultipleChoiceContainsReference() {
		String operation = "mcContains";

		checkMcToMc(true, 3, operation, new String[] { "a" },
				new String[] { "a" });
		checkMcToMc(true, 3, operation, new String[] { "b" },
				new String[] { "b" });
		checkMcToMc(true, 3, operation, new String[] { "c" },
				new String[] { "c" });
		checkMcToMc(true, 3, operation, new String[] { "a", "b" },
				new String[] { "a", "b" });
		checkMcToMc(true, 3, operation, new String[] { "b", "c" },
				new String[] { "b", "c" });
		checkMcToMc(true, 3, operation, new String[] { "a", "c" },
				new String[] { "a", "c" });
		checkMcToMc(true, 3, operation, new String[] { "a", "b", "c" },
				new String[] { "a", "b", "c" });
		checkMcToMc(false, 3, operation, new String[] {}, new String[] {});
		checkMcToMc(true, 3, operation, new String[] { "a" }, new String[] {
				"a", "b", "c" });
		checkMcToMc(false, 3, operation, new String[] { "b" }, new String[] {
				"a", "b", "c" });
		checkMcToMc(false, 3, operation, new String[] { "c" }, new String[] {
				"a", "b", "c" });
		checkMcToMc(false, 3, operation, new String[] { "a" }, new String[] {});
		checkMcToMc(true, 3, operation, new String[] { "a", "b", "c" },
				new String[] { "a" });
		checkMcToMc(true, 3, operation, new String[] { "a", "b", "c" },
				new String[] { "b" });
		checkMcToMc(true, 3, operation, new String[] { "a", "b", "c" },
				new String[] { "c" });
		checkMcToMc(false, 3, operation, new String[] { "a", "b", "c" },
				new String[] {});
	}

	public void testMultipleChoiceDoesNotContainReference() {
		String operation = "mcDoesNotContain";

		checkMcToMc(false, 3, operation, new String[] { "a" },
				new String[] { "a" });
		checkMcToMc(false, 3, operation, new String[] { "b" },
				new String[] { "b" });
		checkMcToMc(false, 3, operation, new String[] { "c" },
				new String[] { "c" });
		checkMcToMc(false, 3, operation, new String[] { "a", "b" },
				new String[] { "a", "b" });
		checkMcToMc(false, 3, operation, new String[] { "b", "c" },
				new String[] { "b", "c" });
		checkMcToMc(false, 3, operation, new String[] { "a", "c" },
				new String[] { "a", "c" });
		checkMcToMc(false, 3, operation, new String[] { "a", "b", "c" },
				new String[] { "a", "b", "c" });
		checkMcToMc(true, 3, operation, new String[] {}, new String[] {});
		checkMcToMc(false, 3, operation, new String[] { "a" }, new String[] {
				"a", "b", "c" });
		checkMcToMc(true, 3, operation, new String[] { "b" }, new String[] {
				"a", "b", "c" });
		checkMcToMc(true, 3, operation, new String[] { "c" }, new String[] {
				"a", "b", "c" });
		checkMcToMc(true, 3, operation, new String[] { "a" }, new String[] {});
		checkMcToMc(false, 3, operation, new String[] { "a", "b", "c" },
				new String[] { "a" });
		checkMcToMc(false, 3, operation, new String[] { "a", "b", "c" },
				new String[] { "b" });
		checkMcToMc(false, 3, operation, new String[] { "a", "b", "c" },
				new String[] { "c" });
		checkMcToMc(true, 3, operation, new String[] { "a", "b", "c" },
				new String[] {});
	}

	public void testFieldComparisons() {
		checkFieldToField(true, "foo", "equals", " Foo ");
		checkFieldToField(true, "1", "equals", " 1.0 ");
		checkFieldToField(true, "1", "isLessThan", "1.00000001");
		checkFieldToField(true, "2", "isGreaterThanOrEqualTo", "1");
	}

	private void checkLiteral(boolean expected, String fieldValue,
			String operator, String literal) {
		ConfigElement config = parseConfig("<" + operator
				+ " field='aForm:Q1:a'><string value='" + literal + "'/></"
				+ operator + ">");
		checkLiteral(expected, config, operator, fieldValue);
	}

	private void checkLiteral(boolean expected, String fieldValue,
			String operator) {
		ConfigElement config = parseConfig("<" + operator
				+ " field=\"aForm:Q1:a\" />");
		checkLiteral(expected, config, operator, fieldValue);
	}

	private void checkLiteral(boolean expected, ConfigElement config,
			String operator, String fieldValue) {
		BooleanExpression condition = BooleanExpression.load(config);
		assertNotNull("unknown operator " + operator, condition);
		assertEquals(expected, condition.isTrue(contextWithQ1a(fieldValue)));
	}

	private ExecutionContext contextWithQ1a(String fieldValue) {
		Project project = ProjectBuilder.buildMinimalisticProject();
		String formText = "<form name=\"aForm\" process=\"if\">\n"
				+ "<items>\n"
				+ "<fib label='Q1'><blank length='10' label='a'/></fib>\n"
				+ "</items>\n" + "</form>\n";

		project = ProjectBuilder.addForm(project, formText);

		Form form = project.getForm("aForm");
		if (form == null)
			throw new IllegalStateException("Unable to find form.");

		FakeRequest request = new FakeRequest(true, "Q1:a", fieldValue);
		UserProject userProject = new UserProject(project, UserTest.aUser(),
				"test");
		return new ExecutionContext(domain, LinkToUserProject
				.createUnauthenticatedLink(userProject), form, request,
				EntryPointType.REAL_PROJECT);
	}

	private void checkMc(boolean expected, int responseCount, String literal,
			String operator, String... values) {
		String configXML = "<" + operator + " field=\"aForm:Q1\">"
				+ "<string value=\"" + literal + "\"/>" + "</" + operator + ">";
		checkMcForParticularXMLStyle(configXML, expected, responseCount,
				operator, values);

		configXML = "<" + operator + " field=\"aForm:Q1\" value=\"" + literal
				+ "\" />";
		checkMcForParticularXMLStyle(configXML, expected, responseCount,
				operator, values);
	}

	private void checkMcForParticularXMLStyle(String configXML,
			boolean expected, int responseCount, String operator,
			String... values) {
		ConfigElement config = parseConfig(configXML);
		BooleanExpression condition = BooleanExpression.load(config);
		assertNotNull("unknown operator " + operator, condition);
		assertEquals(expected, condition.isTrue(contextWithQ1Mc(responseCount,
				values)));
	}

	private ExecutionContext contextWithQ1Mc(int responseCount, String[] values) {
		Project project = ProjectBuilder.buildMinimalisticProject();
		FormBuilder formBuilder = MultipleChoiceTest.newMultipleChoiceForm(
				"aForm", responseCount);

		project = ProjectBuilder.addForm(project, formBuilder.xmlAsString());

		String[] responses = new String[values.length * 2];
		for (int i = 0; i < values.length; i++) {
			responses[i * 2] = "Q1";
			responses[(i * 2) + 1] = values[i];
		}

		Form form = project.getForm("aForm");
		FakeRequest request = new FakeRequest(true, responses);
		UserProject userProject = new UserProject(project, UserTest.aUser(),
				"test");
		return new ExecutionContext(domain, LinkToUserProject
				.createUnauthenticatedLink(userProject), form, request,
				EntryPointType.REAL_PROJECT);
	}

	private void checkMcToMc(boolean expected, int responseCount,
			String operator, String[] q1Selections, String[] q2Selections) {
		ConfigElement config = parseConfig("<" + operator
				+ " field=\"Form 1:Q1\">" + "<string field=\"Form 1:Q2\"/>"
				+ "</" + operator + ">");

		BooleanExpression condition = BooleanExpression.load(config);
		assertNotNull("unknown operator " + operator, condition);
		assertEquals(expected, condition.isTrue(contextWithQ1Q2Mc(
				responseCount, q1Selections, q2Selections)));
	}

	private ExecutionContext contextWithQ1Q2Mc(int responseCount,
			String[] q1Selections, String[] q2Selections) {

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("Form 1");
		formBuilder.addMc("MC Item 1", "Choice A", "Choice B", "Choice C");
		formBuilder.addMc("MC Item 2", "Choice A", "Choice B", "Choice C");

		Project project = projectBuilder.build();
		UserProject userProject = new UserProject(project, owner,
				"contextWithQ1Q2Mc");
		domain.projects().put(userProject);
		Form form = project.getForm("Form 1");

		Request request = getMcMcSubmission(userProject, form, "Q1",
				q1Selections, "Q2", q2Selections);
		FakeExecutionContext context = new FakeExecutionContext(domain,
				new UserProject(project, owner, "test"), form, request);

		return context;
	}

	private Request getMcMcSubmission(UserProject project, Form form,
			String q1, String[] q1Selections, String q2, String[] q2Selections) {

		LinkedHashMap<String, List<String>> parameters = new LinkedHashMap<String, List<String>>();

		parameters.put(q1, valueList(q1Selections));
		parameters.put(q2, valueList(q2Selections));

		return new FakeRequest(true, parameters);
	}

	private List<String> valueList(String[] valueArray) {

		List<String> valueList = new ArrayList<String>();

		for (int i = 0; i < valueArray.length; i++) {
			valueList.add(valueArray[i]);
		}

		return valueList;
	}

	private void checkFieldToField(boolean expected, String fieldValueA,
			String operator, String fieldValueB) {
		ConfigElement config = parseConfig("<" + operator
				+ " field='aForm:Q1:a' format='1.3'>"
				+ "<string field='aForm:Q1:b' /></" + operator + ">");
		BooleanExpression condition = BooleanExpression.load(config);
		assertNotNull("unknown operator " + operator, condition);
		assertEquals(expected, condition.isTrue(contextWithQ1ab(fieldValueA,
				fieldValueB)));
	}

	private ExecutionContext contextWithQ1ab(String fieldValueA,
			String fieldValueB) {
		Project project = ProjectBuilder.buildMinimalisticProject();
		String formText = "<form name=\"aForm\" process=\"if\">\n"
				+ "<items>\n"
				+ "<fib label='Q1'><blank length='10' label='a'/><blank length='10' label='b'/></fib>"
				+ "</items>\n" + "</form>\n";
		project = ProjectBuilder.addForm(project, formText);

		Form form = project.getForm("aForm");

		FakeRequest request = new FakeRequest(true, "Q1:a", fieldValueA,
				"Q1:b", fieldValueB);
		UserProject userProject = new UserProject(project, owner, "aProject");
		return new ExecutionContext(domain, LinkToUserProject
				.createUnauthenticatedLink(userProject), form, request,
				EntryPointType.REAL_PROJECT);
	}

}
