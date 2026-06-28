package com.tawala.web.projectmanager;

import java.util.ArrayList;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Set;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.Checkbox;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.MultipleChoice;
import com.tawala.project.UserProject;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.web.WorldInitializer;

public class ViewProjectManagerSummaryController implements Controller {

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		ModelAndView result = new ModelAndView("projectmanager.summaryview");

		User user = (User) request.getSession(false).getAttribute("user");
		String projectName = request.getParameter("projectName");
		UserProject project = WorldInitializer.getDefaultWorld().domain()
				.projects().getWithProjectRuntime(user.getId(), projectName);
		Form form = project.getProject().getForm(
				request.getParameter("formName"));
		List<FormSubmission> formSubmissions = WorldInitializer
				.getDefaultWorld().domain().storedData().responsesFor(
						project.getProject(), form);
		List<String> fieldIds = form.getMcItemIds();
		ExecutionContext context = new ExecutionContext(WorldInitializer
				.getDefaultWorld().domain(), project);
		List<String> summaryColumns = summaryColumns(form, context);

		List<List> summaryData = new ArrayList<List>();
		for (String fieldId : fieldIds) {
			List<String> row = new ArrayList<String>();
			row.add(fieldId);
			for (int i = 0; i < summaryColumns.size(); i++) {
				row.add(fieldSummary(fieldId, summaryColumns.get(i),
						formSubmissions));
			}
			summaryData.add(row);
		}

		List<Message> messages = new ArrayList<Message>();

		result.addObject("projectName", projectName);
		result.addObject("formName", form.getName());
		result.addObject("formSubmissions", formSubmissions);
		result.addObject("fieldIds", fieldIds);
		result.addObject("summaryData", summaryData);
		result.addObject("summaryColumns", summaryColumns);
		result.addObject("messages", messages);
		return result;
	}

	private String fieldSummary(String fieldId, String stringValue,
			List<FormSubmission> formSubmissions) {
		Value value = new Value(stringValue);

		Integer total = 0;
		for (FormSubmission submission : formSubmissions) {
			List<Value> values = submission.getValues(new Reference(fieldId));
			if ((!values.equals(null)) && values.contains(value))
				total++;
		}
		return total.toString();
	}

	private List<String> summaryColumns(Form form, ExecutionContext executionContext) {
		Set<String> columns = new LinkedHashSet<String>();
		for (String fieldId : form.getMcItemIds()) {
			MultipleChoice field = (MultipleChoice) form.getFieldById(fieldId);
			for (Checkbox item : field.getItems(executionContext)) {
				columns.add(item.getId());
			}
		}
		return new ArrayList<String>(columns);
	}

}
