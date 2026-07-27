package com.tawala.web.library;

import java.util.HashMap;
import java.util.Map;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.web.bind.ServletRequestDataBinder;
import org.springframework.web.multipart.support.ByteArrayMultipartFileEditor;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.scissor.Log;
import com.tawala.project.library.Comment;
import com.tawala.project.library.CommentValidator;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class EditCommentController extends SimpleFormController {
    public static final String PARAMETER_PROJECT_ID = "project_id";

    public EditCommentController() {
        setSessionForm(false);
        setValidateOnBinding(true);
        setCommandName("comment");
        setFormView("library.modify.comment");
        setSuccessView("library.modify.comment");
        setValidator(new CommentValidator());
    }

    /*
     * (non-Javadoc)
     * 
     * @see org.springframework.web.servlet.mvc.SimpleFormController#onSubmit(javax.servlet.http.HttpServletRequest,
     *      javax.servlet.http.HttpServletResponse, java.lang.Object,
     *      org.springframework.validation.BindException)
     */
    @Override
    protected ModelAndView onSubmit(HttpServletRequest request,
            HttpServletResponse response, Object command, BindException errors)
            throws Exception {
        Comment comment = (Comment) command;
        LibraryProject project = instantiateProject(request);

        ProjectLibraryService.onAddingProjectComments(project, comment);

        Log.info(this, "Added a comment to project '" + comment + "'.");

        return LibraryNavigation.navigateToProjectDetailPage(response, project.getId());
    }

    /*
     * (non-Javadoc)
     * 
     * @see org.springframework.web.servlet.mvc.AbstractFormController#formBackingObject(javax.servlet.http.HttpServletRequest)
     */
    @Override
    protected Object formBackingObject(HttpServletRequest request)
            throws Exception {
        Comment comment = new Comment(UserInfoPreparationInterceptor
                .getSessionUser(request).getId());
        return comment;
    }

    /*
     * (non-Javadoc)
     * 
     * @see org.springframework.web.servlet.mvc.SimpleFormController#referenceData(javax.servlet.http.HttpServletRequest)
     */
    @Override
    protected Map referenceData(HttpServletRequest request) throws Exception {
        Map<String, Object> result = new HashMap<String, Object>();
        result.put("project", instantiateProject(request));
        return result;
    }

    /**
     * @param request
     * @throws IllegalStateException
     * @throws NumberFormatException
     */
    private LibraryProject instantiateProject(HttpServletRequest request)
            throws IllegalStateException, NumberFormatException {
        String projectId = request.getParameter(PARAMETER_PROJECT_ID);
        if (projectId == null)
            throw new IllegalStateException("Unable to find parameter '"
                    + PARAMETER_PROJECT_ID + "'.");

        return ProjectLibraryService.findProjectById(Long.parseLong(projectId));
    }

    @Override
    protected void initBinder(HttpServletRequest request,
            ServletRequestDataBinder binder) throws ServletException {
        binder.registerCustomEditor(byte[].class,
                new ByteArrayMultipartFileEditor());
    }
}
