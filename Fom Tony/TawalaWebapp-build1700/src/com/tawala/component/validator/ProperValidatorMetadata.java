package com.tawala.component.validator;

import java.util.List;

import org.apache.commons.lang.WordUtils;
import org.json.JSONException;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;

public class ProperValidatorMetadata extends FIBValidatorMetadataSupport {
	private static char[] DELIMITERS = new char[] {' ', '.', '-'};
	private static final String COMPONENT_ID = "proper-validator";

	public ProperValidatorMetadata() {
		super(COMPONENT_ID, 1);
	}

	public Class<? extends FieldValidator> getFibValidator() {
		return Validator.class;
	}

	public static class Validator implements FieldValidator {
		public Validator(ConfigElement config) {
		}

		public String getJavaScriptFunctionName(ExecutionContext context) {
			return "Tawala.validation.ProperValidation";
		}

		public String getJavaScriptParameters(ExecutionContext context)
				throws JSONException {
			return "null";
		}

		public boolean isValid(FormSubmission submission, Reference fieldReference, ExecutionContext context) {
			List<Value> values = submission.getValues(fieldReference);

			// --- The assumption is that empty field is a valid response.
			if (values == null || values.size() == 0) {
				return true;
			}

			String value = values.get(0).toString().trim();

			String normalizedValue = WordUtils.capitalizeFully(value, DELIMITERS);
			if(! normalizedValue.equals(value)) {
				submission.setValue(fieldReference.getFieldName(), normalizedValue);
			}

			return true;
		}

		public String getErrorMessage(ExecutionContext context) {
			return null;
		}
	}
}
