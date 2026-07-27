package com.tawala.project;

import java.util.Collection;
import java.util.HashMap;
import java.util.Map;

import com.tawala.project.commands.Reference;

public class CompositeFormSubmission {
	private final String recordSetName;
	
	private Map<String, FormSubmission> formSubmissions = new HashMap<String, FormSubmission>();
	
	public CompositeFormSubmission(String recordSetName) {
    	this.recordSetName = recordSetName;
	}

    public void add(FormSubmission formSubmission) {
    	formSubmissions.put(formSubmission.getFormName(), formSubmission);
    }

    public boolean canbeReferedBy(Reference reference) {
    	return recordSetName.equals(reference.getRecordName());
    }

	public FormSubmission getFormSubmission(Reference reference) {
		return formSubmissions.get(reference.getFormName());
	}
	
	public Collection<FormSubmission> getAllSubmissions() {
		return formSubmissions.values();
	}
	
	public Value getValue(Reference reference) {
		FormSubmission submission = getFormSubmission(reference);
		if(submission == null) {
			throw new IllegalStateException("Composite form submission can't find form referenced by " + reference);
		}
		return submission.getValue(reference);
	}
}
