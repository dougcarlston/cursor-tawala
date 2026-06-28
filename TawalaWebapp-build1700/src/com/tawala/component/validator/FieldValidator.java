package com.tawala.component.validator;


import org.json.JSONException;

import com.tawala.project.FormSubmission;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;

public interface FieldValidator {
	String getJavaScriptFunctionName(ExecutionContext context);
	String getJavaScriptParameters(ExecutionContext context) throws JSONException;
	boolean isValid(FormSubmission submission, Reference fieldReference, ExecutionContext context);
	String getErrorMessage(ExecutionContext context);
}
