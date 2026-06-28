package com.tawala.web.library;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.project.library.Category;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;

public class ModifyProjectController extends ModifyProjectBaseController {
    public static final String PARAMETER_PROJECT_ID = "project_id";

    /*
     * (non-Javadoc)
     * 
     * @see org.springframework.web.servlet.mvc.AbstractFormController#formBackingObject(javax.servlet.http.HttpServletRequest)
     */
    @Override
    protected Object formBackingObject(HttpServletRequest request)
            throws Exception {
        String projectId = request.getParameter(PARAMETER_PROJECT_ID);
        if (projectId == null)
            throw new IllegalStateException("Unable to find parameter '"
                    + PARAMETER_PROJECT_ID + "'.");

        LibraryProject project = ProjectLibraryService.findProjectById(Long
                .parseLong(projectId));
        if (project == null)
            throw new IllegalStateException("Unable to find project for id '"
                    + projectId + "'.");

        EditProjectForm form = new EditProjectForm();
        form.setProject(project);
        form.setCategory(new Category(ProjectLibrary.COMMUNITY_LIBRARY, "", ""));

        return form;
    }

    @Override
    protected String getFormViewName() {
        return "library.modify.project";
    }

    @Override
    protected String getSuccessViewName() {
        return "library.modify.project";
    }

    @Override
    protected void performModification(EditProjectForm form, User user)
            throws IOException {
        ProjectLibraryService.onProjectUpdate(form.getProject(), user);
        Log.info(this, "Successfully updated project '"
                + form.getProject().getName() + "'.");
    }

    @Override
    protected ModelAndView postProcessResult(ModelAndView result,
            LibraryProject project, HttpServletRequest request,
            HttpServletResponse response) throws Exception {
        return LibraryNavigation.navigateToProjectDetailPage(response, project
                .getId());
    }
}
