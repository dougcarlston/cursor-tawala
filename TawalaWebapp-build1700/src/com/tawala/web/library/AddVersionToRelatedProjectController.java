package com.tawala.web.library;

import com.tawala.domain.User;
import com.tawala.project.UserProject;

public class AddVersionToRelatedProjectController extends
        AddVersionController {

    @Override
    protected AddVersionForRelatedProjectForm instantiateForm(
            User user, UserProject userProject) {
        return new AddVersionForRelatedProjectForm(user.getId(), userProject);
    }

}
