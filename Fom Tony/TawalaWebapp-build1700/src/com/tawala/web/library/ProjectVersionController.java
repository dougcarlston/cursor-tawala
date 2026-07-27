package com.tawala.web.library;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.LibraryProject;

public abstract class ProjectVersionController implements Controller {
    public static final String VERSION_ID_PARAMETER_NAME = "version_id";
    public static final String PROJECT_ID_PARAMETER_NAME = "project_id";

    abstract public ModelAndView doHandle(HttpServletRequest request,
            HttpServletResponse response, LibraryProject project,
            LibraryProjectVersion version) throws Exception;

    public final ModelAndView handleRequest(HttpServletRequest request,
            HttpServletResponse response) throws Exception {

        long projectId = Long.parseLong(request
                .getParameter(PROJECT_ID_PARAMETER_NAME));
        LibraryProject project = ProjectLibraryService.findProjectById(projectId);

        if (project == null) {
            Log.warn(this, "Unable to find project for id=" + projectId);
            return LibraryNavigation.navigateToSearchPage(response);
        }

        long versionId = Long.parseLong(request
                .getParameter(VERSION_ID_PARAMETER_NAME));
        LibraryProjectVersion version = null;
        for (LibraryProjectVersion nextVersion : project.getVersions()) {
            if (nextVersion.getId() == versionId) {
                version = nextVersion;
                break;
            }
        }

        if (version == null) {
            Log.warn(this, "Unable to find version with id=" + versionId);
            return LibraryNavigation.navigateToProjectDetailPage(response,
                    projectId);
        }

        return doHandle(request, response, project, version);
    }

}
