package com.tawala.project;

import java.util.List;

import com.tawala.domain.User;
import com.tawala.persistence.PersistenceStrategy;
import com.tawala.persistence.PersistentBunch;

// TODO: needs better name; not all data comes from forms, and later on in lifecycle isn't really a submission

public class FormSubmissionsPersistentBunchImpl extends PersistentBunch
        implements FormSubmissions {
    public FormSubmissionsPersistentBunchImpl(
            PersistenceStrategy persistenceStrategy) {
        super(persistenceStrategy);
    }

    public List<FormSubmission> responsesFor(Project project, String formName) {
        List<FormSubmission> result = getPersistenceStrategy()
                .loadResponsesFor(project, formName);
        return result;
    }

    public long sizeOfResponses(Project project, String formName) {
        List<FormSubmission> all = getPersistenceStrategy().loadResponsesFor(
                project, formName);
        long result = 0;
        for (FormSubmission submission : all) {
            if (submission.getFormName().equals(formName)) {
                result += (getPersistenceStrategy().size(submission));
            }
        }
        return result;
    }

    public void record(FormSubmission submission) {
        getPersistenceStrategy().save(submission);
    }

    // Replaces an existing condition with that specified
    public void update(FormSubmission submission) {
        getPersistenceStrategy().save(submission);
    }

    public void eraseResponsesFor(Project project, String name) {
        List<FormSubmission> formSubmissions = responsesFor(project, name);
        for (FormSubmission submission : formSubmissions) {
            getPersistenceStrategy().delete(submission);
        }
    }

    public void purgeProjectResponses(Project project) {
        List<Form> forms = project.getForms();
        for (Form form : forms) {
            List<FormSubmission> formSubmissions = responsesFor(project, form
                    .getName());
            for (FormSubmission submission : formSubmissions) {
                getPersistenceStrategy().delete(submission);
            }
        }
    }

    public List<FormSubmission> responsesFor(Project project, Form form) {
        return responsesFor(project, form.getName());
    }

    public List<FormSubmission> fullyInitializedResponsesFor(Project project, Form form) {
        return responsesFor(project, form);
    }

	public long responseCount(Project project) {
		throw new IllegalStateException("responseCount is not implemented");
	}

	public long responseCount(Project project, String formName) {
		return responsesFor(project, formName).size();
	}

	public void delete(FormSubmission formSubmission) {
        getPersistenceStrategy().delete(formSubmission);
	}

	public void verifyThatBelongsToUserAndDelete(long recordId, User user) {
		throw new IllegalStateException("verifyThatBelongsToUserAndDelete is not implemented");
	}

	public void deleteSharedDataSourceSubmission(long recordId, User user) {
		throw new IllegalStateException("deleteSharedDataSourceSubmission is not implemented");
	}
}
