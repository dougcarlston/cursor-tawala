package com.tawala.domain;

import java.util.List;

import com.tawala.Users;
import com.tawala.UsersHibernateImpl;
import com.tawala.persistence.InMemoryPersistenceStrategy;
import com.tawala.persistence.PersistenceStrategy;
import com.tawala.project.FormPreviews;
import com.tawala.project.FormSubmission;
import com.tawala.project.FormSubmissions;
import com.tawala.project.FormSubmissionsHibernateImpl;
import com.tawala.project.FormSubmissionsPersistentBunchImpl;
import com.tawala.project.Projects;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.ProjectsPersistentBunchImpl;

public class Domain {
    private final Users users;
    private final Projects projects;
    private FormSubmissions formSubmissions;
    private FormPreviews formPreviews;
    
    public Domain() {
        users = new UsersHibernateImpl();
        projects = new ProjectsHibernateImpl();
        formSubmissions = new FormSubmissionsHibernateImpl();
        formPreviews = new FormPreviews();
    }

    public Domain(Users usersImplementation) {
        PersistenceStrategy persistenceStrategy = new InMemoryPersistenceStrategy();
        users = usersImplementation;
        projects = new ProjectsPersistentBunchImpl(persistenceStrategy);
        formSubmissions = new FormSubmissionsPersistentBunchImpl(persistenceStrategy);
        formPreviews = new FormPreviews();
    }

    public Users users() {
        return users;
    }

    public Projects projects() {
        return projects;
    }

    public FormPreviews formPreviews() {
        return formPreviews;
    }

    // Replaces existing submissions with those specified
    public void updateSubmissions(List<FormSubmission> submissions) {
    	for (FormSubmission submission : submissions) {
    		formSubmissions.update(submission);
    	}
    }

    public FormSubmissions storedData() {
        return formSubmissions;
    }
}
