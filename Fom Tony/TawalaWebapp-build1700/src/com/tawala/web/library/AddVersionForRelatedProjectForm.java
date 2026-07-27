package com.tawala.web.library;

import com.tawala.project.UserProject;



public class AddVersionForRelatedProjectForm extends AddVersionForm {
    public AddVersionForRelatedProjectForm(String userId, UserProject userProject) {
        super(userId, userProject);
    }

    @Override
    public long getLibraryProjectId() {
        return getUserProject().getLibraryProjectId();
    }
    
}
