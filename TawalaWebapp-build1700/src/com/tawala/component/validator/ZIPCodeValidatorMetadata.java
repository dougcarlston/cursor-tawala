package com.tawala.component.validator;

import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.json.JSONException;
import org.json.JSONObject;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.StringConcatenationExpression;

public class ZIPCodeValidatorMetadata extends FIBValidatorMetadataSupport {
	private static final String COMPONENT_ID = "zip-code-validator";
	public static final String ERROR_MESSAGE = "error-message";

	public ZIPCodeValidatorMetadata() {
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
			return "Tawala.validation.ZIPCodeValidation";
		}

		public String getJavaScriptParameters(ExecutionContext context)
				throws JSONException {
			JSONObject parameters = new JSONObject();
			parameters.put("errorMessage", errorMessage.evaluate(context));

			return parameters.toString();
		}

		public boolean isValid(FormSubmission submission, Reference fieldReference, ExecutionContext context) {
			List<Value> values = submission.getValues(fieldReference);

			// --- The assumption is that empty field is a valid response.
			if (values == null || values.size() == 0) {
				return true;
			}

			String value = values.get(0).toString().trim();

			if (value.length() == 0) {
				return true;
			}

			String normalizedValue = value.trim();
			if (!normalizedValue.equals(value)) {
				submission.setValue(fieldReference.getFieldName(), normalizedValue);
			}
			
			Pattern pattern = Pattern.compile("\\d{5}(-\\d{4})?");
			Matcher matcher = pattern.matcher(normalizedValue);

			return matcher.matches();
		}

		public String getErrorMessage(ExecutionContext context) {
			return errorMessage.evaluate(context);
		}
	}
}
