package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormSubmission;

public class Delete extends ProcessCommand {
	private RecordSelector recordSelector;

	public Delete(ConfigElement config) {
		this.recordSelector = RecordSelector.instantiateFrom(config);
	}

	public ExecutionResult execute(ExecutionContext context) {
		List<CompositeFormSubmission> result = recordSelector.getRecords(context);
		
		if (result != null) {
			for (CompositeFormSubmission compositeSubmission : result) {
				for (FormSubmission submission : compositeSubmission.getAllSubmissions()) {
					context.getDomain().storedData().delete(submission);	
				}
			}
		}

		return ExecutionResult.NULL;
	}
}
