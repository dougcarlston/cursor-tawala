package com.tawala.web.library;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.LibraryProject;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class DeleteVersionController implements Controller {
    public static final String VERSION_ID_PARAMETER_NAME = "version_id";
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
        
        long versionId = Long.parseLong(request.getParameter(VERSION_ID_PARAMETER_NAME));
        LibraryProjectVersion version = null;
        for (LibraryProjectVersion nextVersion : project.getVersions()) {
            if(nextVersion.getId() == versionId) {
                version = nextVersion;
                break;
            }
        }

        if(version == null) {
            Log.warn(this, "Unable to find version with id=" + versionId);
            navigateOnError(response);
            return null;
        }
        
        ProjectLibraryService.onProjectVersionDeletion(project, version,
                UserInfoPreparationInterceptor.getSessionUser(request));
        
        Log.info(this, "Version " + version.getVersionNumber() + " has been deleted");

        return LibraryNavigation.navigateToProjectDetailPage(response, project.getId());
    }

    private void navigateOnError(HttpServletResponse response) throws IOException {
        // TODO: add error message.
        response.sendRedirect(WellKnown.urls.getLibrarySearch());
    }
}