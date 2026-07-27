package com.tawala.component.validator;


import java.util.List;

import org.json.JSONException;
import org.json.JSONObject;

import com.tawala.project.FormSubmission;
import com.tawala.project.MultipleChoice;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;

public class NonEmptyMCQValidator implements FieldValidator {
	private final MultipleChoice question;

	public NonEmptyMCQValidator(MultipleChoice question) {
		this.question = question;
	}
	
	public String getJavaScriptFunctionName(ExecutionContext context) {
		return "Tawala.validation.nonEmptyMCQValidation";
	}

	public String getJavaScriptParameters(ExecutionContext context)
			throws JSONException {
		JSONObject result = new JSONObject();
		result.put("containerId", question.getContainerDivId());
		result.put("fieldName", question.getHtmlId());
		result.put("type", question.onlyOne() ? "radio" : "checkbox");
		return result.toString();
	}

	public boolean isValid(FormSubmission submission, Reference fieldReference, ExecutionContext context) {
		List<Value> values = submission.getValues(fieldReference);

		if (values == null || values.size() == 0)
			return false;

		if (values.get(0).isEmpty()) {
			return false;
		}
		return true;
	}

	public String getErrorMessage(ExecutionContext context) {
		return "Please answer the marked questions before continuing.";
	}

}
