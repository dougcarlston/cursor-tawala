package com.tawala.web.library;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;

import com.scissor.Log;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class DownloadLibraryProjectVersionController extends
        ProjectVersionController {

    @Override
    public ModelAndView doHandle(HttpServletRequest request,
            HttpServletResponse response, LibraryProject project,
            LibraryProjectVersion version) throws Exception {
        sendProject(response, project, version);

        ProjectLibraryService.onProjectDownloadByUser(project, version,
                UserInfoPreparationInterceptor.getSessionUser(request));

        Log.info(this, "Project " + project.getName() + ", v."
                + version.getVersionNumber() + " has been downloaded");

		EventService.createEvent(new Event("LibraryProjectDownload", request, project.getName()));

        return null;
    }

    private static void sendProject(HttpServletResponse response,
            LibraryProject project, LibraryProjectVersion version) throws IOException {
        response.setContentType("application/octet-stream");
        response.setHeader("Content-Disposition", "attachment; filename=\""
                + project.getName() + " v." + version.getVersionNumber()
                + ".tawala\";");

        response.getOutputStream().print(
                version.getProject().getProjectXmlDefinition());
    }
}
