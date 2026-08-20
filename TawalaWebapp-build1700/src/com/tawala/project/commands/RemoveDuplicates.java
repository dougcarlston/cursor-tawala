package com.tawala.project.commands;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormSubmission;
import com.tawala.project.Value;

/**
 * Delete duplicate form submissions that share a key field value, keeping either
 * the first or the most recently stored row per key.
 *
 * XML:
 * <pre>{@code
 * <remove-duplicates keep="latest|first">
 *   <form name="Form 1"/>
 *   <field>Record:Form 1:PlayerID</field>
 *   <conditions>...</conditions>   <!-- optional extra Where -->
 * </remove-duplicates>
 * }</pre>
 */
public class RemoveDuplicates extends ProcessCommand {
	private final RecordSelector recordSelector;
	private final String fieldName;
	private final boolean keepLatest;

	public RemoveDuplicates(ConfigElement config) {
		this.fieldName = config.child("field").text();
		String keep = config.attribute("keep").stringValue();
		this.keepLatest = keep == null || keep.trim().isEmpty()
				|| !"first".equalsIgnoreCase(keep.trim());
		// Same shape as Delete: <form> (+ optional <conditions>) under the command root.
		this.recordSelector = RecordSelector.instantiateFrom(config);
	}

	public ExecutionResult execute(ExecutionContext context) {
		List<CompositeFormSubmission> result = recordSelector.getRecords(context);
		if (result == null || result.isEmpty()) {
			return ExecutionResult.NULL;
		}

		Reference fieldReference = new Reference(fieldName, true);
		Map<String, CompositeFormSubmission> keepers = new HashMap<String, CompositeFormSubmission>();

		for (CompositeFormSubmission composite : result) {
			FormSubmission submission = composite.getFormSubmission(fieldReference);
			if (submission == null) {
				continue;
			}
			Value value = submission.getValue(fieldReference);
			String key = value == null ? "" : String.valueOf(value).trim();
			if (key.length() == 0) {
				continue;
			}
			if (!keepers.containsKey(key)) {
				keepers.put(key, composite);
			} else if (keepLatest) {
				keepers.put(key, composite);
			}
		}

		for (CompositeFormSubmission composite : result) {
			FormSubmission submission = composite.getFormSubmission(fieldReference);
			if (submission == null) {
				continue;
			}
			Value value = submission.getValue(fieldReference);
			String key = value == null ? "" : String.valueOf(value).trim();
			if (key.length() == 0) {
				continue;
			}
			CompositeFormSubmission keeper = keepers.get(key);
			if (keeper == composite) {
				continue;
			}
			for (FormSubmission doomed : composite.getAllSubmissions()) {
				context.getDomain().storedData().delete(doomed);
			}
		}

		return ExecutionResult.NULL;
	}
}
