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

public class PhoneNumberValidatorMetadata extends FIBValidatorMetadataSupport {
	private static final String COMPONENT_ID = "phone-number-validator";
	public static final String ERROR_MESSAGE = "error-message";

	public PhoneNumberValidatorMetadata() {
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
			return "Tawala.validation.phoneNumberValidation";
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

			String normalizedValue = value.replaceAll("\\s", "");
			normalizedValue = normalizedValue.replaceAll("[)(\\-+\\.,<>/]", "");
			
			
			Pattern pattern = Pattern.compile("1?([2-9]\\d{9})(x\\d+)?");
			Matcher matcher = pattern.matcher(normalizedValue);
			
			if(! matcher.matches() ) {
				return false;
			}
			
			String mainNumber = matcher.group(1);
			String extension = matcher.group(2);
			if(extension == null) {
				extension = "";
			}
			
			normalizedValue = "(" + mainNumber.substring(0, 3) + ") " + mainNumber.substring(3, 6) + "-" + mainNumber.substring(6) + extension; 

			if (!normalizedValue.equals(value)) {
				submission.setValue(fieldReference.getFieldName(), normalizedValue);
			}

			return true;
		}

		public String getErrorMessage(ExecutionContext context) {
			return errorMessage.evaluate(context);
		}
	}
}
