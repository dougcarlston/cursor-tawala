package com.tawala.component.runtime;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;

public class PopularChoiceAlgorithm {
	public static class RankedChoice {
		public String value;
		public int count;
	}

	private final String availableChoicesFieldName;
	private final RecordSelector recordSelector;

	public PopularChoiceAlgorithm(String availableChoicesFieldName,
			RecordSelector recordSelector) {
		this.availableChoicesFieldName = availableChoicesFieldName;
		this.recordSelector = recordSelector;
	}

	@Override
	public int hashCode() {
		return availableChoicesFieldName.hashCode() * 31
				+ recordSelector.hashCode();
	}

	@Override
	public boolean equals(Object obj) {
		if (obj.getClass() != PopularChoiceAlgorithm.class) {
			return false;
		}

		PopularChoiceAlgorithm other = (PopularChoiceAlgorithm) obj;

		return availableChoicesFieldName
				.equals(other.availableChoicesFieldName)
				&& recordSelector.equals(other.recordSelector);
	}

	@SuppressWarnings("unchecked")
	public List<RankedChoice> calculate(ExecutionContext executionContext) {
		List<RankedChoice> results = (List<RankedChoice>) executionContext
				.getCachedObject(this);
		if (results != null) {
			return results;
		}

		Reference availableChoicesField = new Reference(
				availableChoicesFieldName, true);

		if (availableChoicesField.getFormName() == null) {
			throw new IllegalArgumentException(
					"Unable to determine form name for availableChoicesField: '"
							+ availableChoicesFieldName + "'");
		}

		final Map<String, Integer> availableChoicesCounts = new LinkedHashMap<String, Integer>();
		List<CompositeFormSubmission> responses = recordSelector
				.getRecords(executionContext);
		if (responses != null) {
			for (CompositeFormSubmission compositeSubmission : responses) {
				FormSubmission submission = compositeSubmission
						.getFormSubmission(availableChoicesField);
				if (submission != null) {
					countSelectedOptions(submission, availableChoicesField,
							availableChoicesCounts);
				}
			}
		}

		List<Map.Entry<String, Integer>> entrySet = new ArrayList<Map.Entry<String, Integer>>(
				availableChoicesCounts.entrySet());
		Collections.sort(entrySet,
				new Comparator<Map.Entry<String, Integer>>() {
					public int compare(Entry<String, Integer> o1,
							Entry<String, Integer> o2) {
						return o2.getValue().compareTo(o1.getValue());
					}
				});

		results = new ArrayList<RankedChoice>(entrySet.size());
		for (Map.Entry<String, Integer> entry : entrySet) {
			RankedChoice rankedChoice = new RankedChoice();
			rankedChoice.value = entry.getKey();
			rankedChoice.count = entry.getValue();
			results.add(rankedChoice);
		}

		executionContext.cacheObject(this, results);

		return results;
	}

	private void countSelectedOptions(FormSubmission submission,
			Reference field, Map<String, Integer> counts) {
		List<Value> selected = submission.getValues(field);
		for (Value value : selected) {
			String selection = value.toString();
			Integer previousCount = counts.get(selection);
			counts.put(selection, previousCount == null ? Integer.valueOf(1)
					: Integer.valueOf(previousCount.intValue() + 1));
		}
	}
}
