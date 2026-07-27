package com.tawala.web.library;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.project.library.Comment;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class DeleteCommentController implements Controller {
    public static final String COMMENT_ID_PARAMETER_NAME = "comment_id";
    public static final String PROJECT_ID_PARAMETER_NAME = "project_id";

    public ModelAndView handleRequest(HttpServletRequest request,
            HttpServletResponse response) throws Exception {

        long projectId = Long.parseLong(request.getParameter(PROJECT_ID_PARAMETER_NAME));
        LibraryProject project = ProjectLibraryService.findProjectById(projectId);

        if (project == null) {
            Log.warn(this, "Unable to find project for id=" + projectId);
            navigateOnError(response);
            return null;
        }
        
        long commentId = Long.parseLong(request.getParameter(COMMENT_ID_PARAMETER_NAME));
        Comment comment = null;
        for (Comment nextComment : project.getComments()) {
            if(nextComment.getId() == commentId) {
                comment = nextComment;
                break;
            }
        }

        if(comment == null) {
            Log.warn(this, "Unable to find comment with id=" + commentId);
            navigateOnError(response);
            return null;
        }
        
        ProjectLibraryService.onCommentDeletion(project, comment,
                UserInfoPreparationInterceptor.getSessionUser(request).getId());
        Log.info(this, "Comment has been deleted");

        return LibraryNavigation.navigateToProjectDetailPage(response, project.getId());
    }

    private void navigateOnError(HttpServletResponse response) throws IOException {
        // TODO: add error message.
        response.sendRedirect(WellKnown.urls.getLibrarySearch());
    }
}