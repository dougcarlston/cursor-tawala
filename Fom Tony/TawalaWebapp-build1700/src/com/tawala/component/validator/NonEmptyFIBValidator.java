package com.tawala.component.validator;


import java.util.List;

import org.json.JSONException;

import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;

public class NonEmptyFIBValidator implements FieldValidator {

	public String getJavaScriptFunctionName(ExecutionContext context) {
		return "Tawala.validation.nonEmptyFieldValidation";
	}

	public String getJavaScriptParameters(ExecutionContext context)
			throws JSONException {
		return "null";
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
