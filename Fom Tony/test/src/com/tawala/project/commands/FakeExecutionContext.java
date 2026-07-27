package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.List;

import com.tawala.domain.Domain;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.FakeRequest;
import com.tawala.web.Request;

public class FakeExecutionContext extends ExecutionContext {
	public static final String DEFAULT_FORM_ID = "form1";

	public FakeExecutionContext(UserProject project, Form form,
			String... params) {
		super(new Domain(), LinkToUserProject.createUnauthenticatedLink(project, project.getUniqueRandomId()), form, new FakeRequest(true, params), EntryPointType.REAL_PROJECT);
	}

	public FakeExecutionContext(UserProject project, Form form, Request request) {
		super(new Domain(), LinkToUserProject.createUnauthenticatedLink(project, project.getUniqueRandomId()), form, request, EntryPointType.REAL_PROJECT);
	}

	public FakeExecutionContext(Domain domain, UserProject project, Form form,
			Request request) {
		super(domain, LinkToUserProject.createUnauthenticatedLink(project, project.getUniqueRandomId()), form, request, EntryPointType.REAL_PROJECT);
	}

	public FakeExecutionContext(Domain domain, UserProject project, Form form,
			String... params) {
		super(domain, LinkToUserProject.createUnauthenticatedLink(project, project.getUniqueRandomId()), form, new FakeRequest(true, params), EntryPointType.REAL_PROJECT);
	}

	public FakeExecutionContext(Project project) {
		this(new UserProject(project, UserTest.aUser(), "test"), project
				.defaultForm());
	}

	public static FakeExecutionContext contextWithFibValues(User projectOwner,
			String... values) {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(DEFAULT_FORM_ID);
		FibBuilder fib = formBuilder.addFib();
		for (int i = 0; i < values.length; i++) {
			fib.addBlank();
		}

		Project project = projectBuilder.build();
		Form form = project.getForm(DEFAULT_FORM_ID);
		Iterator<Field> fields = form.getSegment(0).fields().iterator();
		int pos = 0;
		String[] params = new String[values.length * 2];
		for (String value : values) {
			params[pos++] = fields.next().getHtmlId();
			params[pos++] = value;
		}

		return new FakeExecutionContext(new UserProject(project, projectOwner,
				"proj"), form, params);
	}

	public static FakeExecutionContext basicContext(User projectOwner) {
		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm(DEFAULT_FORM_ID);
		Project project = builder.build();

		Form form = project.getForm(DEFAULT_FORM_ID);
		return new FakeExecutionContext(new UserProject(project, projectOwner,
				"proj"), form);
	}

	public static FakeExecutionContext contextWithMCQValues(User projectOwner,
			String[]... values) {

		LinkedHashMap<String, List<String>> parameters = new LinkedHashMap<String, List<String>>();

		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(DEFAULT_FORM_ID);
		for (int i = 0; i < values.length; i++) {
			String[] choices = new String[10];
			for (int j = 0; j < choices.length; j++) {
				choices[j] = Character.toString((char) ('a' + j));
			}

			String fieldId = values[i][0];
			formBuilder.addMcWithAlternateLabel(fieldId, "MCQ #" + i, false,
					false, choices);

			List<String> parameterValue = new ArrayList<String>();

			for (int j = 1; j < values[i].length; j++) {
				parameterValue.add(values[i][j]);
			}

			parameters.put(fieldId, parameterValue);
		}

		Project project = projectBuilder.build();
		Form form = project.getForm(DEFAULT_FORM_ID);

		return new FakeExecutionContext(new UserProject(project, projectOwner,
				"proj"), form, new FakeRequest(true, parameters));
	}

}
