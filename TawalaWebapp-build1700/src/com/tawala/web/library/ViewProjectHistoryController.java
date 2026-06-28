package com.tawala.web.library;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;
import com.tawala.web.controller.WellKnown;

public class ViewProjectHistoryController extends ProjectSupportController implements Controller {
    public ModelAndView handleRequest(HttpServletRequest request, HttpServletResponse response) throws Exception {
        LibraryProject project = ProjectLibraryService
        .findProjectById(extractProjectId(request));
        if (project == null) {
            response.sendRedirect(WellKnown.urls.getLibrarySearch());
            return null;
        }

        ModelAndView result = new ModelAndView("library.project.history");
        result.addObject("project", project);
        result.addObject("events", ProjectLibraryService.getProjectHistory(project.getId()));
        
        return result;
    }
}