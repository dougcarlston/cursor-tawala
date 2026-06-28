package com.tawala.component.validator;

import java.util.Arrays;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

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

public class USStateValidatorMetadata extends FIBValidatorMetadataSupport {
	private static final String COMPONENT_ID = "us-state-validator";
	public static final String ERROR_MESSAGE = "error-message";
	public static final Set<String> VALID_STATE_ABBREVIATIONS = new HashSet<String>(
			Arrays.asList("AL", "AK", "AS", "AZ", "AR", "CA", "CO", "CT", "DE",
					"DC", "FM", "FL", "GA", "GU", "HI", "ID", "IL", "IN", "IA",
					"KS", "KY", "LA", "ME", "MH", "MD", "MA", "MI", "MN", "MS",
					"MO", "MT", "NE", "NV", "NH", "NJ", "NM", "NY", "NC", "ND",
					"MP", "OH", "OK", "OR", "PW", "PA", "PR", "RI", "SC", "SD",
					"TN", "TX", "UT", "VT", "VI", "VA", "WA", "WV", "WI", "WY",
					"AE", "AA", "AP"));

	public USStateValidatorMetadata() {
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
			return "Tawala.validation.USStateValidation";
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

			String state = values.get(0).toString().trim();

			if (state.length() == 0) {
				return true;
			}
			
			
			String normalizedState = state.toUpperCase();
			if(! normalizedState.equals(state)) {
				submission.setValue(fieldReference.getFieldName(), normalizedState);
			}

			return VALID_STATE_ABBREVIATIONS.contains(normalizedState);
		}

		public String getErrorMessage(ExecutionContext context) {
			return "Invalid state abbreviation.";
		}
	}
}
