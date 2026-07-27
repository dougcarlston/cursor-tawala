package com.tawala.component.validator;


import java.util.List;

import org.json.JSONException;
import org.json.JSONObject;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.domain.EmailAddress;
import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.StringConcatenationExpression;

public class EmailValidatorMetadata extends FIBValidatorMetadataSupport {
	private static final String COMPONENT_ID = "email-validator";
	public static final String ERROR_MESSAGE = "error-message";

	public EmailValidatorMetadata() {
		super(COMPONENT_ID, 1);
		addParameter(new Parameter(ERROR_MESSAGE, COMPONENT_ID + "_"
				+ ERROR_MESSAGE, ParameterType.EXPRESSION, true)
				.useDefaultValue());
	}

	public Class<? extends FieldValidator> getFibValidator() {
		return Validator.class;
	}

	public static class Validator implements FieldValidator {
		private final StringConcatenationExpression errorMessage;

		public Validator(ConfigElement config) {
			this.errorMessage = new StringConcatenationExpression(config
					.child(ERROR_MESSAGE));
		}

		public String getJavaScriptFunctionName(ExecutionContext context) {
			return "Tawala.validation.emailValidation";
		}

		public String getJavaScriptParameters(ExecutionContext context)
				throws JSONException {
			JSONObject parameters = new JSONObject();
			parameters.put("errorMessage", errorMessage.evaluate(context));

			return parameters.toString();
		}

		public boolean isValid(FormSubmission submission, Reference fieldReference, ExecutionContext context) {
			List<Value> values = submission.getValues(fieldReference);

			//--- The assumption is that empty field is a valid response.
			if(values == null || values.size() == 0) {
				return true;
			}
			EmailAddress emailAddress = new EmailAddress(values.get(0).toString());
			if(emailAddress.isEmpty()) {
				return true;
			}
			try {
				emailAddress.asInternetAddress();
				return true;
			} catch(Exception e) {
				return false;
			}
		}

		public String getErrorMessage(ExecutionContext context) {
			return "At least one of the email addresses is invalid.";
		}
	}
}
