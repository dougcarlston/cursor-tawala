package com.tawala.component.web.form;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterRestriction;
import com.tawala.component.ParameterType;
import com.tawala.component.parameter.WorksWithinRecordIteration;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.project.Checkbox;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.StringConcatenationExpression;

public class DynamicMultiChoiceDataProvider extends WebComponentMetadataSupport {
	public static String COMPONENT_ID = "dynamic-mcq";
	public static String DISPLAY_EXPRESSION_PARAMETER = "display-expression";
	public static String VALUE_EXPRESSION_PARAMETER = "value-expression";
	public static String SORT_EXPRESSION_PARAMETER = "sort-expression";
	public static String CONDITIONS_PARAMETER = "record-selector";

	public DynamicMultiChoiceDataProvider() {
		super(COMPONENT_ID, 1);
		addParameters(
		// ---
				new Parameter(
						DISPLAY_EXPRESSION_PARAMETER,
						COMPONENT_ID + "_" + DISPLAY_EXPRESSION_PARAMETER,
						ParameterType.EXPRESSION,
						true,
						// --- Restriction
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.always,
										RecordSelector.DEFAULT_RECORD_LIST_NAME))),
				// ---
				new Parameter(
						VALUE_EXPRESSION_PARAMETER,
						COMPONENT_ID + "_" + VALUE_EXPRESSION_PARAMETER,
						ParameterType.EXPRESSION,
						true,
						// --- Restriction
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.always,
										RecordSelector.DEFAULT_RECORD_LIST_NAME))),
				// ---
				new Parameter(
						SORT_EXPRESSION_PARAMETER,
						COMPONENT_ID + "_" + SORT_EXPRESSION_PARAMETER,
						ParameterType.EXPRESSION,
						false,
						// --- Restriction
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.always,
										RecordSelector.DEFAULT_RECORD_LIST_NAME))),
				// ---
				new Parameter(CONDITIONS_PARAMETER, COMPONENT_ID + "_"
						+ CONDITIONS_PARAMETER,
						ParameterType.RECORD_SELECTOR, true));
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return Runtime.class;
	}

	public static class Runtime extends MultiChoiceDataProviderRuntimeSupport {
		private RecordSelector recordSelector;
		private final StringConcatenationExpression displayExpression;
		private final StringConcatenationExpression valueExpression;
		private final StringConcatenationExpression sortExpression;

		public Runtime(ConfigElement configElement) {
			this.displayExpression = new StringConcatenationExpression(
					configElement.child(DISPLAY_EXPRESSION_PARAMETER));
			this.valueExpression = new StringConcatenationExpression(
					configElement.child(VALUE_EXPRESSION_PARAMETER));
			ConfigElement sortExpressionElement = configElement
					.child(SORT_EXPRESSION_PARAMETER);
			this.sortExpression = sortExpressionElement == null ? null
					: new StringConcatenationExpression(sortExpressionElement);

			recordSelector = RecordSelector.instantiateFrom(configElement
					.child(CONDITIONS_PARAMETER));
		}

		@SuppressWarnings("unchecked")
		public List<Checkbox> getChoices(ExecutionContext context) {
			CompositeFormSubmission previouslyMappedSubmission = context
					.getMappedSubmission(RecordSelector.DEFAULT_RECORD_LIST_NAME);
			try {
				List<CompositeFormSubmission> records = recordSelector
						.getRecords(context);
				if (records == null) {
					return Collections.EMPTY_LIST;
				}

				if (sortExpression != null) {
					sortRecords(records, context);
				}

				List<Checkbox> result = new ArrayList<Checkbox>(records.size());

				for (CompositeFormSubmission submission : records) {
					context.mapRecord(RecordSelector.DEFAULT_RECORD_LIST_NAME,
							submission);

					String value = valueExpression.evaluate(context);
					String description = displayExpression.evaluate(context);

					if (value.length() == 0 || description.length() == 0) {
						continue;
					}

					result.add(new Checkbox(getQuestion(), value, description));
				}
				context
						.removeRecordMapping(RecordSelector.DEFAULT_RECORD_LIST_NAME);

				return result;
			} finally {
				if (previouslyMappedSubmission == null) {
					context
							.removeRecordMapping(RecordSelector.DEFAULT_RECORD_LIST_NAME);
				} else {
					context.mapRecord(RecordSelector.DEFAULT_RECORD_LIST_NAME,
							previouslyMappedSubmission);
				}
			}
		}

		private void sortRecords(List<CompositeFormSubmission> records,
				ExecutionContext context) {
			final Map<CompositeFormSubmission, String> submissionToSortByValueMap = new HashMap<CompositeFormSubmission, String>(
					records.size());
			for (CompositeFormSubmission submission : records) {
				context.mapRecord(RecordSelector.DEFAULT_RECORD_LIST_NAME,
						submission);
				submissionToSortByValueMap.put(submission, sortExpression
						.evaluate(context));
			}
			context
					.removeRecordMapping(RecordSelector.DEFAULT_RECORD_LIST_NAME);

			Collections.sort(records,
					new Comparator<CompositeFormSubmission>() {
						public int compare(CompositeFormSubmission o1,
								CompositeFormSubmission o2) {
							return submissionToSortByValueMap.get(o1)
									.compareToIgnoreCase(
											submissionToSortByValueMap.get(o2));
						}
					});
		}

		public boolean isSafeToGetChoicesWithoutRealContext() {
			return ! recordSelector.hasConditionsDependentOnExecutionContext();
		}
	}
}
