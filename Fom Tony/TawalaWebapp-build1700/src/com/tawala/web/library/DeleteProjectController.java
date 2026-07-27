package com.tawala.web.library;

import java.util.ArrayList;
import java.util.Collection;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.message.Message;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class DeleteProjectController extends ProjectSupportController implements
        Controller {
    public ModelAndView handleRequest(HttpServletRequest request,
            HttpServletResponse response) throws Exception {
        LibraryProject project = ProjectLibraryService
                .findProjectById(extractProjectId(request));
        if (project == null) {
            response.sendRedirect(WellKnown.urls.getLibrarySearch());
            return null;
        }

        ProjectLibraryService.onProjectDelete(project.getId(),
                UserInfoPreparationInterceptor.getSessionUser(request));

        Collection<Message> messages = new ArrayList<Message>();
        messages.add(new Message("admin.project.deleted", project.getName()));

        Log.info(DeleteProjectController.class, "Project \""
                + project.getName() + "\" has been deleted.");

        ModelAndView result = new ModelAndView(
                "library.project.delete.confirmation");
        result.addObject("project", project);

        return result;
    }
}