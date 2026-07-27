package com.tawala.component.validator;


import java.util.List;

import org.json.JSONException;
import org.json.JSONObject;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.StringConcatenationExpression;

public class IntegerRangeValidatorMetadata extends FIBValidatorMetadataSupport {
	public static final String COMPONENT_ID = "integer-range-validator";
	public static final String ERROR_MESSAGE = "error-message";
	public static final String LOWER_LIMIT_PARAMETER = "lower-limit";
	public static final String UPPER_LIMIT_PARAMETER = "upper-limit";

	public IntegerRangeValidatorMetadata() {
		super(COMPONENT_ID, 1);
		addParameters(new Parameter[] {
				new Parameter(ERROR_MESSAGE,
						COMPONENT_ID + "_" + ERROR_MESSAGE,
						ParameterType.EXPRESSION, true).useDefaultValue(),
				new Parameter(LOWER_LIMIT_PARAMETER, COMPONENT_ID + "_"
						+ LOWER_LIMIT_PARAMETER, ParameterType.EXPRESSION,
						false),
				new Parameter(UPPER_LIMIT_PARAMETER, COMPONENT_ID + "_"
						+ UPPER_LIMIT_PARAMETER, ParameterType.EXPRESSION,
						false) });
	}

	public Class<? extends FieldValidator> getFibValidator() {
		return IntegerRangeValidator.class;
	}

	public static class IntegerRangeValidator implements FieldValidator {
		private final StringConcatenationExpression errorMessageExpression;
		private final StringConcatenationExpression lowerLimitExpression;
		private final StringConcatenationExpression upperLimitExpression;

		public IntegerRangeValidator(ConfigElement config) {
			this.errorMessageExpression = new StringConcatenationExpression(
					config.child(ERROR_MESSAGE));
			this.lowerLimitExpression = new StringConcatenationExpression(
					config.child(LOWER_LIMIT_PARAMETER));
			this.upperLimitExpression = new StringConcatenationExpression(
					config.child(UPPER_LIMIT_PARAMETER));

		}

		public String getJavaScriptFunctionName(ExecutionContext context) {
			return "Tawala.validation.integerValidation";
		}

		public String getJavaScriptParameters(ExecutionContext context)
				throws JSONException {
			JSONObject parameters = new JSONObject();
			parameters.put("errorMessage", errorMessageExpression
					.evaluate(context));

			Integer lowerLimit = getLowerLimit(context);
			if (lowerLimit != null) {
				parameters.put("lowerLimit", lowerLimit.intValue());
			}

			Integer upperLimit = getUpperLimit(context);
			if (upperLimit != null) {
				parameters.put("upperLimit", upperLimit.intValue());
			}

			return parameters.toString();
		}

		private Integer getLowerLimit(ExecutionContext context) {
			return getIntegerFromExpression(context, lowerLimitExpression,
					"Lower limit is not a number: ");
		}

		private Integer getUpperLimit(ExecutionContext context) {
			return getIntegerFromExpression(context, upperLimitExpression,
					"Upper limit is not a number: ");
		}

		private static Integer getIntegerFromExpression(
				ExecutionContext context,
				StringConcatenationExpression expression, String errorMessage) {
			String expressionResult = expression.evaluate(context);
			if (expressionResult.length() == 0) {
				return null;
			}
			try {
				return Integer.parseInt(expressionResult);
			} catch (NumberFormatException e) {
				Log.error(IntegerRangeValidator.class, errorMessage, e);
				return null;
			}
		}

		public boolean isValid(FormSubmission submission, Reference fieldReference, ExecutionContext context) {
			List<Value> values = submission.getValues(fieldReference);

			if (values == null || values.size() == 0) {
				// --- Assumption is that we don't validate empty values.
				return true;
			}
			String value = values.get(0).toString();
			if(value.trim().length() == 0) {
				return true;
			}
			try {
				int intValue = Integer.parseInt(value);
				Integer lowerLimit = getLowerLimit(context);
				if(lowerLimit != null && intValue < lowerLimit) {
					return false;
				}
				
				Integer upperLimit = getUpperLimit(context);
				if(upperLimit != null && intValue > upperLimit) {
					return false;
				}

				return true;
			} catch (NumberFormatException e) {
				return false;
			}
		}

		public String getErrorMessage(ExecutionContext context) {
			return "At least one of the numbers is not valid.";
		}
	}
}
