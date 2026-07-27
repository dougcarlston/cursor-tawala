package com.tawala.web.library;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.Validator;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.Rating;
import com.tawala.project.library.RatingValidator;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class RateProjectController implements Controller {
    public static final String PARAMETER_PROJECT_ID = "project_id";
    public static final String PARAMETER_RATING = "rating";
    public static final String PARAMETER_TEXT = "text";
    public static final Validator validator = new RatingValidator();

    public ModelAndView handleRequest(HttpServletRequest request,
            HttpServletResponse response) throws Exception {
        String ratingParameter = request.getParameter(PARAMETER_RATING);
        if (ratingParameter != null) {
            Rating rating = new Rating(UserInfoPreparationInterceptor
                    .getSessionUser(request).getId());
            rating.setValue(Integer.parseInt(ratingParameter));
            rating.setText(request.getParameter(PARAMETER_TEXT));

            Errors errors = new BindException(rating, "rating");
            validator.validate(rating, errors);

            if (!errors.hasErrors()) {
                long projectId = Long.parseLong(request
                        .getParameter(PARAMETER_PROJECT_ID));
                LibraryProject project = ProjectLibraryService
                        .findProjectById(projectId);
                if (project == null) {
                    Log.warn(this, "Not found project for id=" + projectId);
                } else {
                    ProjectLibraryService.onRatingProject(project, rating);
            		EventService.createEvent(new Event("LibraryProjectRated", request, project.getName()));
                }
            }
        }
        
        return LibraryNavigation.navigateToProjectDetailPage(response, Long
                .parseLong(request.getParameter(PARAMETER_PROJECT_ID)));
    }
}