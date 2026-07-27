package com.tawala.web.library;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.UserProject;
import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class PublishProjectToLibraryController extends
        ModifyProjectBaseController {

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

        User user = UserInfoPreparationInterceptor.getSessionUser(request);
        UserProject project = WorldInitializer.getDefaultWorld().domain()
                .projects().getWithProjectRuntime(user.getId(), projectId);
        if (project == null)
            throw new IllegalStateException("Unable to find project for id '"
                    + projectId + "'.");

        EditProjectForm form = new EditProjectForm();
        LibraryProject libraryProject = new LibraryProject(user.getId(), project);
		form.setProject(libraryProject);
        form.setCategory(new Category(ProjectLibrary.COMMUNITY_LIBRARY, "", ""));
        form.setUserProject(project);

        return form;
    }

    @Override
    protected String getFormViewName() {
        return "projectmanager.publish";
    }

    @Override
    protected String getSuccessViewName() {
        return "projectmanager.publishconfirm";
    }

    @Override
    protected void performModification(EditProjectForm form, User user)
            throws IOException {
        ProjectLibraryService.onProjectSubmission(form.getProject(), form
                .getUserProject());
        
        Event event = new Event("PublishProjectToLibrary", form.getProject().getName());
        event.setUserId(user.getDatabaseId());
        EventService.createEvent(event);

        Log.info(this, "Successfully submitted project '"
                + form.getProject().getName() + "' to the library.");
    }

    @Override
    protected ModelAndView postProcessResult(ModelAndView result,
            LibraryProject project, HttpServletRequest request,
            HttpServletResponse response) throws Exception {
        return result;
    }

}
