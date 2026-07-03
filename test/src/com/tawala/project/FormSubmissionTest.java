package com.tawala.project;

import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.Map;

import com.tawala.domain.UserTest;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.web.FakeRequest;
import com.tawala.web.Request;
import com.tawala.web.oldhtml.HtmlTestCase;

public class FormSubmissionTest extends HtmlTestCase {
	private Project project;
	private UserProject userProject;

	/*
	 * @see com.tawala.TestCase#setUp()
	 */
	protected void setUp() throws Exception {
		super.setUp();
		project = ProjectBuilder.buildMinimalisticProject();
		userProject = new UserProject(project, UserTest.aUser(), "test");
	}

	public void testFieldValues() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder builder = projectBuilder.addForm("aForm");

		builder.addFib("Name?", 8, 8);
		Form form = builder.build();
		FakeRequest request = new FakeRequest(true, "Q1:b", "bar", "Q1:a",
				"foo", "Q1:c", "fake");
		ExecutionContext context = new FakeExecutionContext(userProject, form,
				request);
		FormSubmission submission = context.getSubmission();

		assertEquals(2, submission.getFieldIds().size());

		FakeExecutionContext fakeExecutionContext = new FakeExecutionContext(
				projectBuilder.build());
		Iterator<String> fieldIdIterator = submission.getFieldIds().iterator();

		assertEquals("foo", submission.getValues(
				new Reference(fieldIdIterator.next())).get(0).toString());
		assertEquals("bar", submission.getValues(
				new Reference(fieldIdIterator.next())).get(0).toString());

		assertEquals("foo", submission.getValue(
				new Reference("Q1:a", fakeExecutionContext)).toString());
		assertEquals("foo", submission.getValue(
				new Reference("aForm:Q1:a", fakeExecutionContext)).toString());
		assertEquals("bar", submission.getValue(
				new Reference("Q1:b", fakeExecutionContext)).toString());
		assertEquals("bar", submission.getValue(
				new Reference("aForm:Q1:b", fakeExecutionContext)).toString());
	}

	public void testEmpty() {
		FormBuilder builder = new FormBuilder();
		builder.addMc("Food?", "burger", "fries");
		Form form = builder.build();
		FakeRequest request = new FakeRequest(true);
		ExecutionContext context = new FakeExecutionContext(userProject, form,
				request);
		FormSubmission submission = context.getSubmission();
		assertTrue(submission.getValue(new Reference("Q1")).isEmpty());
		List<Value> values = submission.getValues(new Reference("Q1"));
		assertEquals(0, values.size());
		assertEquals("", render(submission.getValue(new Reference("Q1"))));
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

	public void testAlternateLabelsForFib() {
		FormBuilder builder = new FormBuilder();
		FibBuilder q1 = builder.addFib();
		q1.addText("What's your age?");
		q1.addBlank();
		FibBuilder q2 = builder.addFib();
		q2.addText("What's your dog's name?");
		q2.addBlank("Dog");
		FibBuilder q3 = builder.addFib("Name");
		q3.addText("What's your  name?");
		q3.addBlank();
		q3.addBlank("Last");

		String[] goodData = new String[] { "Q1:a", "21", "Dog", "Spot",
				"Name:a", "Jane", "Last", "Smith" };

		String[] badData = new String[] { "Q2:a", "evil", "Q3:a", "evil",
				"Q3:b", "evil", "Q3:Last", "evil" };

		Form form = builder.build();
		checkFillAndFetch(form, goodData, badData);
	}

	public void testRequiredField() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder builder = projectBuilder.addForm("Main");
		FibBuilder fibBuilder = builder.addFib();
		fibBuilder.addBlank(true);
		fibBuilder.addBlank(true);
		fibBuilder.addBlank(true);
		fibBuilder.addBlank(true);

		Project project = projectBuilder.build();
		Form form = project.defaultForm();
		Request request = makeRequest(form, "x", "", " ");
		ExecutionContext context = new FakeExecutionContext(new UserProject(
				project, UserTest.aUser(), "test"), form, request);
		FormSubmission submission = context.getSubmission();
		Value fieldValue = submission.getValue(new Reference("Q1:a"));
		assertFalse(fieldValue.isEmpty());
		assertTrue(submission.getValue(new Reference("Q1:b")).isEmpty());
		assertTrue(submission.getValue(new Reference("Q1:c")).isEmpty());
		assertTrue(submission.getValue(new Reference("Q1:d")).isEmpty());

	}

	public void testCopyFieldChanges() {
		Map<String, String[]> fields1 = new HashMap<String, String[]>();
		fields1.put("a", new String[] { "xx" });
		fields1.put("c", new String[] { "zz" });
		fields1.put("d", new String[] { "ww" });
		FormSubmission original = new FormSubmission(null, fields1);

		Map<String, String[]> fields2 = new HashMap<String, String[]>();
		fields2.put("a", new String[] { "x" });
		fields2.put("b", new String[] { "y" });
		fields2.put("c", new String[] { "z" });
		FormSubmission copy = new FormSubmission(null, fields2);

		copy.copyFieldsFrom(original);
		assertEquals("xx", copy.getValue(new Reference("a")).toString());
		assertEquals("", copy.getValue(new Reference("b")).toString());
		assertEquals("zz", copy.getValue(new Reference("c")).toString());
		assertEquals("ww", copy.getValue(new Reference("d")).toString());
	}

	public void testRequiredFields() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder builder = projectBuilder.addForm("Main");

		FibBuilder fibBuilder = builder.addFib();
		fibBuilder.addBlank(false);
		fibBuilder.addBlank(true);

		Project project = projectBuilder.build();
		Form form = project.defaultForm();

		FormSegment segment = form.getFirstSegment();

		UserProject userProject = new UserProject(project, UserTest.aUser(),
				"test");

		ExecutionContext context = new FakeExecutionContext(userProject
				.getProject());
		assertTrue(segment.validate(makeSubmission(userProject, form, "", ""),
				context).size() > 0);
		assertTrue(segment.validate(makeSubmission(userProject, form, "x", ""),
				context).size() > 0);
		assertTrue(segment.validate(makeSubmission(userProject, form, "", "x"),
				context).size() == 0);
		assertTrue(segment.validate(
				makeSubmission(userProject, form, "x", "x"), context).size() == 0);
	}

	public void testClearInputItems() {

		FormBuilder builder = new FormBuilder("Form1");
		builder.addText("Text Item 1");
		builder.addFibNoBlankAlt("Fib Item 1", 10, false);
		builder.addMc("MC Item 1", "Choice A", "Choice B");

		Form form = builder.build();
		FakeRequest request = new FakeRequest(true, "Q1:a", "Entry 1", "Q2",
				"Choice A");
		ExecutionContext context = new FakeExecutionContext(userProject, form,
				request);
		FormSubmission submission = context.getSubmission();
		assertEquals("Entry 1", submission.getValue(new Reference("Q1:a"))
				.toString());
		assertEquals("Choice A", submission.getValue(new Reference("Q2"))
				.toString());

		submission.clearItems(form.getInputItemIds());
		assertEquals("", submission.getValue(new Reference("Q1:a")).toString());
		assertEquals("", submission.getValue(new Reference("Q2")).toString());
	}

	public void testClearAlternateLabeledInputItems() {

		FormBuilder builder = new FormBuilder("Form1");
		builder.addText("Text Item 1");
		builder.addFib("Fib Item 1", "Name", 10);
		builder.addMcWithAlternateLabel("Choices", "MC Item 1", "Choice A",
				"Choice B");

		Form form = builder.build();
		FakeRequest request = new FakeRequest(true, "Name", "Archie",
				"Choices", "Choice A");
		ExecutionContext context = new FakeExecutionContext(userProject, form,
				request);
		FormSubmission submission = context.getSubmission();

		assertEquals("Archie", submission.getValue(new Reference("Name"))
				.toString());
		assertEquals("Choice A", submission.getValue(new Reference("Choices"))
				.toString());

		submission.clearItems(form.getInputItemIds());
		assertEquals("", submission.getValue(new Reference("Name")).toString());
		assertEquals("", submission.getValue(new Reference("Choices"))
				.toString());
	}

	private Request makeRequest(Form form, String... answers) {
		Iterator<Field> fields = form.getSegment(0).fields().iterator();
		String[] paramMap = new String[answers.length * 2];
		for (int i = 0; i < paramMap.length; i += 2) {
			paramMap[i] = fields.next().getHtmlId();
			paramMap[i + 1] = answers[i / 2];
		}

		return new FakeRequest(true, paramMap);
	}

	private FormSubmission makeSubmission(UserProject userProject, Form form,
			String... answers) {
		return new FakeExecutionContext(userProject, form, makeRequest(form,
				answers)).getSubmission();
	}

	private void checkFillAndFetch(Form form, String[] goodData,
			String[] badData) {
		FakeRequest request = new FakeRequest(true, join(badData, goodData,
				badData));

		ExecutionContext context = new FakeExecutionContext(userProject, form,
				request);

		FormSubmission submission = context.getSubmission();
		Iterator<String> fieldIdIterator = submission.getFieldIds().iterator();
		for (int j = 0; j < goodData.length; j += 2) {
			assertTrue(fieldIdIterator.hasNext());
			String submissionFieldId = fieldIdIterator.next();
			String fieldId = goodData[j];
			assertEquals(fieldId, submissionFieldId);

			String value = goodData[j + 1];
			Value fieldValue = submission.getValue(new Reference(
					submissionFieldId));
			assertEquals(value, fieldValue.toString());
		}
		assertFalse(fieldIdIterator.hasNext());
	}

	private String[] join(String[]... arrays) {
		int length = 0;
		for (int i = 0; i < arrays.length; i++) {
			length += arrays[i].length;
		}
		String[] result = new String[length];
		int pos = 0;
		for (int i = 0; i < arrays.length; i++) {
			String[] array = arrays[i];
			for (int j = 0; j < array.length; j++) {
				result[pos++] = array[j];
			}
		}

		return result;
	}

}
