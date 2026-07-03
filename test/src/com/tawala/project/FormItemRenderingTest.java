package com.tawala.project;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.LetterCounter;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.web.FakeRequest;
import com.tawala.web.WorldInitializer;
import com.tawala.web.oldhtml.HtmlTestCase;

public class FormItemRenderingTest extends TestCase {
	private User projectOwner = UserTest.aUser("tester");
	private Project project;

	public FormItemRenderingTest() {
		addUserNameToDelete(projectOwner.getId());
	}

	@Override
	protected void setUp() throws Exception {
		new HibernateTestSetup().onSetUp();
		super.setUp();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
	}

	private FormBuilder formBuilder = new FormBuilder("Test");

	private Form form;

	private boolean built = false;

	public void testBasicFib() {
		FillInBlank fib = buildOneFib(false);
		assertEquals(
				"<div class=\"fib\"><div><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"30\" /></div></div>\n",
				render(fib));
		String value = "foo";
		assertEquals(
				"<div class=\"fib\"><div><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"30\" value=\"foo\" /></div></div>\n",
				render(fib, value));
	}

	public void testRequiredFib() {
		FillInBlank fib = buildOneFib(true);
		assertEquals(
				"<div class=\"fib\"><div><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"30\" onblur=\"Tawala.validation.validate(this);\" /><script>Tawala.validation.register('tawalaField_Q1:a', Tawala.validation.nonEmptyFieldValidation, null);</script><span class=\"qinfo\"> *</span></div></div>\n",
				render(fib));
		assertEquals(
				"<div class=\"fib\"><div><input type=\"text\" class=\"text\" name=\"Q1:a\" id=\"tawalaField_Q1:a\" size=\"30\" value=\"foo\" onblur=\"Tawala.validation.validate(this);\" /><script>Tawala.validation.register('tawalaField_Q1:a', Tawala.validation.nonEmptyFieldValidation, null);</script><span class=\"qinfo\"> *</span></div></div>\n",
				render(fib, "foo"));
	}

	public void testBasicMcOnlyOne() {
		String[] choices = new String[] { "Vanilla", "Chocolate", "Strawberry" };
		formBuilder.addMc("Favorite ice cream?", true, false, choices);
		build();
		MultipleChoice mc = (MultipleChoice) form.getItems().get(0);
		String htmlString = "<div id=\"Q1Container\" class=\"mcRadio\">"
				+ "<label class=\"question\">Favorite ice cream?</label>\n"
				+ mcChoiceHtml("radio", choices) + "</div>\n";
		assertEquals(htmlString, render(mc));
	}

	public void testBasicMcMultiple() {
		String[] choices = new String[] { "Vanilla", "Chocolate", "Strawberry" };
		formBuilder.addMc("Favorite ice cream?", false, false, choices);
		build();
		MultipleChoice mc = (MultipleChoice) form.getItems().get(0);
		String htmlString = "<div id=\"Q1Container\" class=\"mcCheckbox\">"
				+ "<label class=\"question\">Favorite ice cream?</label>\n"
				+ mcChoiceHtml("checkbox", choices) + "</div>\n";
		assertEquals(htmlString, render(mc));
	}

	public void testRequiredMc() {
		String[] choices = new String[] { "Vanilla", "Chocolate", "Strawberry" };
		formBuilder.addMc("Favorite ice cream?", true, true, choices);
		build();
		MultipleChoice mc = (MultipleChoice) form.getItems().get(0);
		assertEquals(
				"<div id=\"Q1Container\" class=\"mcRadio\">"
						+ "<label class=\"question\">Favorite ice cream? *</label>\n"
						+ mcChoiceHtml("radio", choices)
						+ "</div>\n"
						+ "<script>Tawala.validation.register('Q1Container', Tawala.validation.nonEmptyMCQValidation, {\"containerId\":\"Q1Container\",\"fieldName\":\"Q1\",\"type\":\"radio\"});</script>",
				render(mc));
	}

	public void testMcWithData() {
		String[] choices = new String[] { "Vanilla", "Chocolate", "Strawberry" };
		formBuilder.addMc("Favorite ice cream?", false, false, choices);
		build();
		MultipleChoice mc = (MultipleChoice) form.getItems().get(0);
		String htmlString = "<div id=\"Q1Container\" class=\"mcCheckbox\">"
				+ "<label class=\"question\">Favorite ice cream?</label>\n"
				+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-a\" type=\"checkbox\" value=\"a\" checked=\"checked\" /> <label for=\"Q1-a\">Vanilla</label></span>\n"
				+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-b\" type=\"checkbox\" value=\"b\" /> <label for=\"Q1-b\">Chocolate</label></span>\n"
				+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-c\" type=\"checkbox\" value=\"c\" checked=\"checked\" /> <label for=\"Q1-c\">Strawberry</label></span>\n"
				+ "</div>\n";
		assertEquals(htmlString, render(mc, "a", "c"));
	}

	public void testMcHtmlEscape() {
		String[] choices = new String[] { "Vanilla \"the best\"",
				"Chocolate & Chips", "<Strawberry>" };
		formBuilder.addMc("Favorite <ice> cream?", false, false, choices);
		build();
		MultipleChoice mc = (MultipleChoice) form.getItems().get(0);
		String htmlString = "<div id=\"Q1Container\" class=\"mcCheckbox\">"
				+ "<label class=\"question\">Favorite <ice> cream?</label>\n"
				+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-a\" type=\"checkbox\" value=\"a\" checked=\"checked\" /> <label for=\"Q1-a\">Vanilla \"the best\"</label></span>\n"
				+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-b\" type=\"checkbox\" value=\"b\" /> <label for=\"Q1-b\">Chocolate & Chips</label></span>\n"
				+ "<span class=\"answer\"><input class=\"checkbox\" name=\"Q1\" id=\"Q1-c\" type=\"checkbox\" value=\"c\" checked=\"checked\" /> <label for=\"Q1-c\"><Strawberry></label></span>\n"
				+ "</div>\n";
		assertEquals(htmlString, render(mc, "a", "c"));
	}

	private String mcChoiceHtml(String type, String... choices) {
		LetterCounter value = new LetterCounter();
		StringBuffer result = new StringBuffer();
		for (String choice : choices) {
			result.append("<span class=\"answer\">");
			result.append("<input ");
			if (type == "radio") {
				result.append("class=\"radio\" ");
			} else {
				result.append("class=\"checkbox\" ");
			}
			String currentValue = value.next();
			result.append("name=\"Q1\" ");
			result.append("id=\"Q1-");
			result.append(currentValue);
			result.append("\" ");
			result.append("type=\"");
			result.append(type);
			result.append("\" ");
			result.append("value=\"");
			result.append(currentValue);
			result.append("\"");
			result.append(" /> ");
			result.append("<label for=\"Q1-");
			result.append(currentValue);
			result.append("\">");
			result.append(choice);
			result.append("</label>");
			result.append("</span>\n");
		}
		return result.toString();

	}

	// --- TODO: This needs to be reworked - we separate the project from the
	// form, which doesn't seem right.
	private void build() {
		if (built)
			throw new IllegalStateException("can only build once");
		form = formBuilder.build();
		ProjectBuilder projectBuilder = new ProjectBuilder();
		projectBuilder.addForm(form.getName());
		project = projectBuilder.build();
		built = true;
	}

	private FillInBlank buildOneFib(boolean required) {
		FibBuilder fibBuilder = formBuilder.addFib();
		fibBuilder.addBlank(required);
		build();
		return (FillInBlank) form.getItems().get(0);
	}

	private String render(Question fib) {
		return render(fib, FakeExecutionContext.basicContext(projectOwner));
	}

	private String render(FillInBlank fib, String value) {
		return render(fib, FakeExecutionContext.contextWithFibValues(
				projectOwner, value));
	}

	private String render(MultipleChoice mc, String... values) {
		LinkedHashMap<String, List<String>> fieldContents = new LinkedHashMap<String, List<String>>();

		ArrayList<String> valueList = new ArrayList<String>();
		for (String value : values) {
			valueList.add(value);
		}
		fieldContents.put(mc.getHtmlId(), valueList);

		UserProject userProject = new UserProject(project, projectOwner, "test");
		FakeExecutionContext context = new FakeExecutionContext(
				WorldInitializer.getDefaultWorld().domain(), userProject, form,
				new FakeRequest(true, fieldContents));
		return render(mc, context);
	}

	private String render(FormRenderable formRenderable,
			ExecutionContext context) {
		return HtmlTestCase.renderHtml(formRenderable.toHtml(context));
	}
}
