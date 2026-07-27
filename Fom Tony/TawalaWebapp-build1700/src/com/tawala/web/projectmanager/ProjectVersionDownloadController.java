package com.tawala.web.projectmanager;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.project.ProjectVersion;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class ProjectVersionDownloadController implements Controller {
	public static final String PROJECT_VERSION_PARAMETER = "project_version";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		long projectId = Long.parseLong(request
				.getParameter(PROJECT_VERSION_PARAMETER));

		ProjectVersion projectVersion = WorldInitializer.getDefaultWorld()
				.domain().projects().findProjectVersion(projectId);
		if (projectVersion == null) {
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return null;
		}

		if (!projectVersion.getParent().getUser().equals(
				UserInfoPreparationInterceptor.getSessionUser(request))) {
			Log
					.warn(
							this,
							"Attempt to download a version of a project that belongs to another user (project version #"
									+ projectVersion.getId()
									+ " belonging to "
									+ projectVersion.getParent().getUser()
											.getId());
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
			return null;
		}

		sendProjectVersion(response, projectVersion);

		Log.info(this, "Downloaded version "
				+ projectVersion.getVersionNumber() + " of "
				+ projectVersion.getProject());

		return null;
	}

	private static void sendProjectVersion(HttpServletResponse response,
			ProjectVersion version) throws IOException {
		response.setContentType("application/octet-stream");
		response.setHeader("Content-Disposition", "attachment; filename=\""
				+ version.getParent().getName() + ".tawala\";");

		response.getOutputStream().print(
				version.getProject().getProjectXmlDefinition());
	}

}
