package com.tawala.project;

import java.util.List;

import com.tawala.domain.User;

// TODO: needs better name; not all data comes from forms, and later on in lifecycle isn't really a submission

public interface FormSubmissions {
    public List<FormSubmission> responsesFor(Project project, String formName);

    public long sizeOfResponses(Project project,
            String formName);

    public void record(FormSubmission submission);

    public void update(FormSubmission submission);

    public void eraseResponsesFor(Project project, String name);

    public void purgeProjectResponses(Project project);

    public List<FormSubmission> responsesFor(Project project,
            Form form);
    
    public List<FormSubmission> fullyInitializedResponsesFor(Project project, Form form);

	public long responseCount(Project project);

	public long responseCount(Project project, String formName);

	public void delete(FormSubmission formSubmission);

	public void verifyThatBelongsToUserAndDelete(long recordId, User user);

	public void deleteSharedDataSourceSubmission(long recordId, User user);
}
