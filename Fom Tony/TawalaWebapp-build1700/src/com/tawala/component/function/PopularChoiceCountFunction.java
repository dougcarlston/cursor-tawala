package com.tawala.component.function;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.CommonParameter;
import com.tawala.component.Option;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterType;
import com.tawala.component.ReturnType;
import com.tawala.component.runtime.PopularChoiceAlgorithm;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;

public class PopularChoiceCountFunction extends FunctionMetadataSupport {
	private static final String COMPONENT_ID = "popular-choice-count";
	private static final Option[] RANK_OPTIONS = new Option[] {
			new Option("1", "option.first"), new Option("2", "option.second"),
			new Option("3", "option.third") };
	public static final String RANK = "rank";
	public static final String CHOICE_AVAILABLE_FIELD_NAME = CommonParameter.POPULAR_CHOICE_QUESTION;
	public static final String CONDITIONS_FIELD_NAME = "conditions";

	public PopularChoiceCountFunction() {
		super(COMPONENT_ID, 1);
		addParameters(new Parameter[] {
				new Parameter(RANK, RANK, ParameterType.ENUMERATION, true)
						.addOptions(RANK_OPTIONS),
				new Parameter(CHOICE_AVAILABLE_FIELD_NAME,
						CHOICE_AVAILABLE_FIELD_NAME,
						ParameterType.MCQ_FIELD_NAME, true),
				new Parameter(CONDITIONS_FIELD_NAME, COMPONENT_ID + "_"
						+ CONDITIONS_FIELD_NAME,
						ParameterType.RECORD_SELECTOR, false), });
	}

	public ReturnType getReturnType() {
		return ReturnType.INT;
	}

	public Class<? extends Function> getRuntimeClass() {
		return Runtime.class;
	}

	public static class Runtime implements Function {
		private final String availableFieldName;
		private final int rank;
		private final RecordSelector recordSelector;

		public Runtime(ConfigElement configElement) {
			this(configElement.child(CHOICE_AVAILABLE_FIELD_NAME).text(),
					Integer.parseInt(configElement.child(RANK).text()),
					RecordSelector.instantiateFrom(configElement
							.child(CONDITIONS_FIELD_NAME)));
		}

		public Runtime(String availablaFieldName, int rank,
				RecordSelector recordSelector) {
			this.availableFieldName = availablaFieldName;
			this.rank = rank;
			this.recordSelector = recordSelector;
		}

		public Value execute(ExecutionContext executionContext) {
			List<PopularChoiceAlgorithm.RankedChoice> rankedChoices = new PopularChoiceAlgorithm(
					availableFieldName, recordSelector)
					.calculate(executionContext);

			return new Value(rankedChoices == null
					|| rankedChoices.size() < rank ? 0 : rankedChoices
					.get(rank - 1).count);
		}
	}
}
