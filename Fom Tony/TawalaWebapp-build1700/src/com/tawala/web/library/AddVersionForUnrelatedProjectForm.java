package com.tawala.web.library;

import com.tawala.project.UserProject;

public class AddVersionForUnrelatedProjectForm extends AddVersionForm {
    private long libraryProjectId;
    
    public AddVersionForUnrelatedProjectForm(String userId, UserProject userProject) {
        super(userId, userProject);
    }

    @Override
    public long getLibraryProjectId() {
        return libraryProjectId;
    }

    /**
     * @param libraryProjectId The libraryProjectId to set.
     */
    public void setLibraryProjectId(long libraryProjectId) {
        this.libraryProjectId = libraryProjectId;
    }
}
