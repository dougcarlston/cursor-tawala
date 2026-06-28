package com.tawala.component.function;

import java.util.Collections;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.ComponentRuntimeSupport;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.component.ReturnType;
import com.tawala.project.Value;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.RecordSelector.FormDataProvider;

public class RecordCountFunction extends FunctionMetadataSupport {
	private static final String COMPONENT_ID = "record-count";
	public static final String FORM_NAME_PARAMETER = "form-name";
	public static final String WHERE_CLAUSE_PARAMETER = "conditions";

	public RecordCountFunction() {
		super(COMPONENT_ID, 3);
		addParameters(new Parameter[] {
				new Parameter(FORM_NAME_PARAMETER, COMPONENT_ID + "_"
						+ FORM_NAME_PARAMETER, ParameterType.FORM_NAME, true),
				new Parameter(WHERE_CLAUSE_PARAMETER, COMPONENT_ID + "_"
						+ WHERE_CLAUSE_PARAMETER,
						ParameterType.RECORD_SELECTOR, true) });
	}

	public ReturnType getReturnType() {
		return ReturnType.INT;
	}

	public Class<? extends Function> getRuntimeClass() {
		return Runtime.class;
	}

	public static class Runtime extends ComponentRuntimeSupport implements
			Function {
		private final RecordSelector recordSelector;

		public Runtime(ConfigElement configElement) {
			super(configElement);
			ConfigElement conditionsElement = configElement
					.child(WHERE_CLAUSE_PARAMETER);
			switch (getVersion()) {
			case 1:
				String formName = configElement.child(FORM_NAME_PARAMETER)
						.text();
				BooleanExpression condition = conditionsElement == null ? BooleanExpression.TRUE
						: BooleanExpression.load(conditionsElement.childElement(0));
				recordSelector = new RecordSelector(
						Collections
								.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
										formName)), condition,
						RecordSelector.DEFAULT_RECORD_LIST_NAME);
				break;

			case 2:
			case 3:
				this.recordSelector = RecordSelector
						.instantiateFrom(conditionsElement);
				break;

			default:
				throw new IllegalStateException("Unhandled version #"
						+ getVersion());
			}
		}

		/**
		 * Needed to support version 1.
		 */
		public Runtime(RecordSelector recordSelector) {
			this.recordSelector = recordSelector;
		}

		public Value execute(ExecutionContext executionContext) {
			return new Value(recordSelector.getRecordCount(executionContext));
		}
	}
}
