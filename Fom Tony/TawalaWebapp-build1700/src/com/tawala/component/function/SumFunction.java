package com.tawala.component.function;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.ComponentRuntimeSupport;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.component.ReturnType;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.SmartNumber;

public class SumFunction extends FunctionMetadataSupport {
	private static final String COMPONENT_ID = "sum";
	public static final String FIELD_PARAMETER = "field";
	public static final String WHERE_CLAUSE_PARAMETER = "conditions";

	public SumFunction() {
		super(COMPONENT_ID, 1);
		addParameters(new Parameter[] {
				new Parameter(FIELD_PARAMETER, COMPONENT_ID + "_"
						+ FIELD_PARAMETER, ParameterType.BLANK_FIELD_NAME, true),
				new Parameter(WHERE_CLAUSE_PARAMETER, COMPONENT_ID + "_"
						+ WHERE_CLAUSE_PARAMETER,
						ParameterType.RECORD_SELECTOR, true) });
	}

	public ReturnType getReturnType() {
		return ReturnType.DOUBLE;
	}

	public Class<? extends Function> getRuntimeClass() {
		return Runtime.class;
	}

	public static class Runtime extends ComponentRuntimeSupport implements
			Function {
		private final RecordSelector recordSelector;
		private final String fieldName;

		public Runtime(ConfigElement configElement) {
			super(configElement);
			this.fieldName = configElement.child(FIELD_PARAMETER).text();
			this.recordSelector = RecordSelector
					.instantiateFrom(configElement
					.child(WHERE_CLAUSE_PARAMETER));
		}

		public Runtime(String fieldName, RecordSelector recordSelector) {
			this.fieldName = fieldName;
			this.recordSelector = recordSelector;
		}

		public Value execute(ExecutionContext executionContext) {
			List<CompositeFormSubmission> records = recordSelector
					.getRecords(executionContext);
			if(records == null || records.size() == 0) {
				return new Value(0);
			}
			
			Reference fieldReference = new Reference(fieldName, true);
			double result = 0;
			for (CompositeFormSubmission record : records) {
				FormSubmission submission = record.getFormSubmission(fieldReference);
				if(submission == null) {
					continue;
				}
				Value value = submission.getValue(fieldReference);
				SmartNumber number = value.asNumber();
				if(number.isNumeric()) {
					result += number.doubleValue();
				}
			}
			return new Value(result);
		}
	}
}
