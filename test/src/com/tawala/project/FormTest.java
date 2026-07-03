package com.tawala.project;

import java.util.Collections;
import java.util.Iterator;
import java.util.List;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.TestCase;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.SkipBlockBuilder;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.project.commands.SkipBlock;
import com.tawala.project.theme.CommonTheme;
import com.tawala.web.FakeRequest;
import com.tawala.web.WorldInitializer;

public class FormTest extends TestCase {
	@Override
	protected void setUp() throws Exception {
		new HibernateTestSetup().onSetUp();
		super.setUp();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
		if(CommonTheme.ALL_THEMES == null) {
			new CommonTheme.Initializer().setThemes(Collections.singletonList(new CommonTheme("default", "Default Theme")));
		}
	}

	public void testConstructFromXml() {
		String xml = "<form name=\"Hello\" process=\"ShowDoc\">"
				+ "  <items>"
				+ "    <text label=\"T1\">Press submit to show document.</text>"
				+ "  </items>" + "</form>";
		Form form = newForm(xml);
		assertEquals("Hello", form.getName());
		assertEquals("ShowDoc", form.getPostProcessName());
		List<FormItem> items = form.getItems();
		assertEquals(1, items.size());
		TextBlock item = (TextBlock) items.get(0);
		assertEquals("Press submit to show document.", item.getTextContents());
	}

	private Form newForm(String xml) {
		return new Form(parseConfig(xml));
	}

	public void testMultipleChoice() {
		Form form = newForm("<form name=\"mc\" >" + "  <items>"
				+ "    <mc label=\"Q1\" onlyone=\"True\">\n"
				+ "        <question>Does this work?</question>\n"
				+ "        <choice label=\"a\">Yes</choice>\n"
				+ "        <choice label=\"b\">No</choice>\n" + "    </mc>"
				+ "  </items>" + "</form>" + "");
		List<FormItem> content = form.getItems();
		assertEquals(1, content.size());
		assertTrue(content.get(0) instanceof MultipleChoice);
	}

	// TODO: text multiple text blocks

	public void testBreakSegmentation() {
		String xml = "<form name=\"Hello\">" + "  <items>"
				+ "    <text label=\"T1\">p1</text>" + "    <break />"
				+ "    <text label=\"T2\">p2</text>" + "  </items>" + "</form>";
		Form form = newForm(xml);
		assertEquals(2, form.segmentCount());
	}

	public void testEmptySegmentIgnored1() {
		String xml = "<form name=\"Hello\">" + "  <items>"
				+ "    <text label=\"T1\">p1</text>" + "    <break />"
				+ "  </items>" + "</form>";
		Form form = newForm(xml);
		assertEquals(1, form.segmentCount());
	}

	public void testEmptySegmentIgnored2() {
		String xml = "<form name=\"Hello\">" + "  <items>" + "    <break />"
				+ "    <break />" + "    <text label=\"T1\">p1</text>"
				+ "  </items>" + "</form>";
		Form form = newForm(xml);
		assertEquals(1, form.segmentCount());
	}

	public void testDynamicSegmentCreation() {
		String xml = "<form name=\"Hello\">" + "  <items>"
				+ "    <text label=\"T1\">p1</text>" + "    <break />"
				+ "    <text label=\"T2\">p2</text>"
				+ "    <text label=\"T3\">p3</text>" + "  </items>" + "</form>";
		Form form = newForm(xml);
		assertEquals(2, form.segmentCount());
		assertSame(form.getSegment(0), form.getSegmentForSkip("T1"));
		assertSame(form.getSegment(1), form.getSegmentForSkip("T2"));
		FormSegment dymamic = form.getSegmentForSkip("T3");
		assertNotSame(form.getSegment(1), dymamic);
		TextBlock t3 = (TextBlock) dymamic.get(0);
		assertEquals("p3", t3.getTextContents());
	}

	public void testTextBlockFibBlankField() {

		FormBuilder builder = new FormBuilder("Form1");
		builder.addFib("FIB Item 1", 10);
		builder.addBreak();
		builder.addTextWithFields("Text Item with FIB blank display field: ",
				"<<Form1:Q1:a>>");

		Form form = builder.build();

		TextBlock textItem = (TextBlock) form.getSegment(1).get(0);
		assertEquals("Text Item with FIB blank display field: ", textItem
				.getTextContents());

		FakeRequest request = new FakeRequest(true, "Q1:a", "Response 1");
		Project project = ProjectBuilder.buildMinimalisticProject();

		FakeExecutionContext context = new FakeExecutionContext(
				WorldInitializer.getDefaultWorld().domain(), new UserProject(
						project, UserTest.aUser(), "test"), form, request);
		assertContains("Text Item with FIB blank display field: Response 1",
				textItem.toHtml(context).toString());
	}

	public void testTextBlockMultipleFibBlankFields() {

		FormBuilder builder = new FormBuilder("Form1");
		builder.addFib("FIB Item 1", 10, 10, 10);
		builder.addBreak();
		builder.addTextWithFields("Text Item with FIB blank display fields: ",
				"<<Form1:Q1:a>>", " ", "<<Form1:Q1:b>>", " ", "<<Form1:Q1:c>>");

		Form form = builder.build();

		TextBlock textItem = (TextBlock) form.getSegment(1).get(0);
		assertEquals("Text Item with FIB blank display fields:   ", textItem
				.getTextContents());

		FakeRequest request = new FakeRequest(true, "Q1:a", "Response 1",
				"Q1:b", "Response 2", "Q1:c", "Response 3");
		Project project = ProjectBuilder.buildMinimalisticProject();

		FakeExecutionContext context = new FakeExecutionContext(
				WorldInitializer.getDefaultWorld().domain(), new UserProject(
						project, null, "test"), form, request);

		assertContains(
				"Text Item with FIB blank display fields: Response 1 Response 2 Response 3",
				textItem.toHtml(context).toString());
	}

	public void testTextBlockMCField() {

		FormBuilder builder = new FormBuilder("Form1");
		builder.addMc("MC Item 1", "Choice A", "Choice B");
		builder.addBreak();
		builder.addTextWithFields("Text Item with MC display field: ",
				"<<Form1:Q1>>");

		Form form = builder.build();

		TextBlock textItem = (TextBlock) form.getSegment(1).get(0);
		assertEquals("Text Item with MC display field: ", textItem
				.getTextContents());

		FakeRequest request = new FakeRequest(true, "Q1", "b");
		Project project = ProjectBuilder.buildMinimalisticProject();

		FakeExecutionContext context = new FakeExecutionContext(
				WorldInitializer.getDefaultWorld().domain(), new UserProject(
						project, UserTest.aUser(), "test"), form, request);

		assertContains("Text Item with MC display field: b", textItem.toHtml(
				context).toString());
	}

	public void testInitialSkipBlock() {
		FormBuilder drinkingQuestions = new FormBuilder("DrinkingQuestions");
		SkipBlockBuilder skipBlockBuilder = drinkingQuestions.addSkip();
		skipBlockBuilder.addIfSkip("Age", "isLessThan", "value", "21",
				SkipBlock.SKIP_TO_END, null);
		drinkingQuestions.addFib("How many beers to you drink each week?", 3);

		Form form = drinkingQuestions.build();
		assertTrue(form.getSegments().hasInitialSkip());
		assertEquals(1, form.getSegments().getInitialSkip().size());
		assertEquals(1, form.getSegments().getInitialSkip().get(0).size());
	}

	public void testGetItems() {

		FormBuilder builder = new FormBuilder("Form1");
		builder.addText("Text Item 1");
		builder.addFibNoBlankAlt("Fib Item 1", 10, false);
		builder.addMc("MC Item 1", "Choice A", "Choice B");
		builder.addBreak();
		builder.addText("Text Item 2");
		builder.addFibNoBlankAlt("Fib Item 2", 10, false);
		builder.addMc("MC Item 2", "Choice A", "Choice B");

		Form form = builder.build();

		List<FormItem> formItems = form.getItems();

		assertEquals(6, formItems.size());

		Iterator<FormItem> i = formItems.iterator();

		assertEquals("T1", i.next().getId());
		assertEquals("Q1", i.next().getId());
		assertEquals("Q2", i.next().getId());
		assertEquals("T2", i.next().getId());
		assertEquals("Q3", i.next().getId());
		assertEquals("Q4", i.next().getId());
	}

	public void testGetInputItemIds() {

		FormBuilder builder = new FormBuilder("Form1");
		builder.addText("Text Item 1");
		builder.addFibNoBlankAlt("Fib Item 1", 10, false);
		builder.addMc("MC Item 1", "Choice A", "Choice B");
		builder.addBreak();
		builder.addText("Text Item 2");
		builder.addFib("Fib Item 2", "Fib Item 2 Alternate Label", 10);
		builder.addMcWithAlternateLabel("MC Item 2 Alternate Label",
				"MC Item 2", "Choice A", "Choice B");

		Form form = builder.build();

		List<String> itemIds = form.getInputItemIds();
		Iterator<String> i = itemIds.iterator();

		assertEquals(4, itemIds.size());
		assertEquals("Q1:a", i.next());
		assertEquals("Q2", i.next());
		assertEquals("Fib Item 2 Alternate Label", i.next());
		assertEquals("MC Item 2 Alternate Label", i.next());
	}

	public void testGetMcItemIds() {

		FormBuilder builder = new FormBuilder("Form1");
		builder.addText("Text Item 1");
		builder.addFibNoBlankAlt("Fib Item 1", 10, false);
		builder.addMc("MC Item 1", "Choice A", "Choice B");
		builder.addBreak();
		builder.addText("Text Item 2");
		builder.addFib("Fib Item 2", "Fib Item 2 Alternate Label", 10);
		builder.addMcWithAlternateLabel("MC Item 2 Alternate Label",
				"MC Item 2", "Choice A", "Choice B");

		Form form = builder.build();

		List<String> itemIds = form.getMcItemIds();
		Iterator<String> i = itemIds.iterator();

		assertEquals(2, itemIds.size());
		assertEquals("Q2", i.next());
		assertEquals("MC Item 2 Alternate Label", i.next());
	}

	public void testDefaultStartingPointState() {

		FormBuilder builder = new FormBuilder("Form1");
		Form form;

		form = builder.build();
		assertTrue(form.isStartingPoint());

		form = new Form("form 2");
		assertTrue(form.isStartingPoint());
	}

	public void testExplicitStartingPointStates() {

		FormBuilder builder;
		Form form;

		builder = new FormBuilder("Form1", true);
		form = builder.build();
		assertTrue(form.isStartingPoint());

		builder = new FormBuilder("Form1", false);
		form = builder.build();
		assertFalse(form.isStartingPoint());
	}

	public void testThemePath() {

		FormBuilder builder;
		Form form;

		builder = new FormBuilder("Form1", true);
		form = builder.build();
		assertEquals("default", form.getThemePath());

		builder = new FormBuilder("Form1", true, "style2");
		form = builder.build();
		assertEquals("style2", form.getThemePath());
	}

}
