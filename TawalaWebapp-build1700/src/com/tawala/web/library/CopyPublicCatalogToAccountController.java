package com.tawala.web.library;

import java.net.URLEncoder;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.domain.User;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.ViewProjectManagerDetailsController;

/**
 * Puts a public Library project on the signed-in account.
 *
 * Catalog listings (Potluck, Get Together, and the other five) are live
 * projects, so Deploy to My Tawala cannot see them. Only those seven ids
 * are accepted. A User Project Submission is copied from its library row
 * ({@code app_id}). Either way the new project is a definition only:
 * responses on the original are left where they are.
 */
public class CopyPublicCatalogToAccountController implements Controller {
	public static final String PARAMETER_UNIQUE_ID = "unique_id";
	public static final String PARAMETER_LIBRARY_ID = "app_id";
	public static final String PARAMETER_PROJECT_NAME = "project_name";

	/**
	 * Public Library catalog unique ids. Same set as
	 * website-mock/js/demo-urls.js TAWALA_LIBRARY.
	 */
	private static boolean isPublicCatalogId(String uniqueId) {
		return "gy1zssbrwm4fgfm".equals(uniqueId)
				|| "52ozm3kqd58zlss".equals(uniqueId)
				|| "b6do4s50iq64vl8".equals(uniqueId)
				|| "wg77ytn0bgq1x70".equals(uniqueId)
				|| "grniytf6dvmobqe".equals(uniqueId)
				|| "u3hkqgwtrepjlur".equals(uniqueId)
				|| "bx44wpdnspbsi3k".equals(uniqueId);
	}

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		if (user == null) {
			response.sendRedirect("/login?r="
					+ URLEncoder.encode("/app/library.html", "UTF-8"));
			return null;
		}

		Project definition = null;
		String fallbackName = null;

		String uniqueId = request.getParameter(PARAMETER_UNIQUE_ID);
		if (uniqueId != null) {
			uniqueId = uniqueId.trim();
		}
		if (uniqueId != null && uniqueId.length() > 0) {
			if (!isPublicCatalogId(uniqueId)) {
				response.sendError(HttpServletResponse.SC_NOT_FOUND);
				return null;
			}
			LinkToUserProject link = WorldInitializer.getDefaultWorld()
					.domain().projects().getWithProjectRuntime(uniqueId);
			if (link == null || link.getProject() == null
					|| link.getProject().getProject() == null) {
				response.sendError(HttpServletResponse.SC_NOT_FOUND);
				return null;
			}
			definition = link.getProject().getProject().makeCopy();
			fallbackName = link.getProject().getName();
		} else {
			String libraryId = request.getParameter(PARAMETER_LIBRARY_ID);
			long appId;
			try {
				appId = Long.parseLong(libraryId == null ? "" : libraryId.trim());
			} catch (NumberFormatException e) {
				response.sendError(HttpServletResponse.SC_NOT_FOUND);
				return null;
			}
			LibraryProject libraryProject = ProjectLibraryService
					.findProjectById(appId);
			if (libraryProject == null
					|| libraryProject.getLatestVersion() == null
					|| libraryProject.getLatestVersion().getProject() == null) {
				response.sendError(HttpServletResponse.SC_NOT_FOUND);
				return null;
			}
			definition = libraryProject.getLatestVersion().getProject()
					.makeCopy();
			fallbackName = libraryProject.getName();
		}

		String requested = request.getParameter(PARAMETER_PROJECT_NAME);
		String base = requested == null ? "" : requested.trim();
		if (base.length() == 0) {
			base = fallbackName == null ? "" : fallbackName.trim();
		}
		if (base.length() == 0) {
			base = "Project";
		}
		if (base.length() > 100) {
			base = base.substring(0, 100);
		}

		String chosen = unusedName(user, base);
		if (chosen == null) {
			response.sendError(HttpServletResponse.SC_CONFLICT);
			return null;
		}

		UserProject copy = new UserProject(definition, user, chosen);
		copy.getVersions().get(0).setDescription("Copied from the Library");
		WorldInitializer.getDefaultWorld().domain().projects().put(copy);

		response.sendRedirect(WellKnown.urls
				.getProjectManagerProjectDetailView()
				+ '?'
				+ ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME
				+ '='
				+ URLEncoder.encode(chosen, "UTF-8"));
		return null;
	}

	/**
	 * put() adds a version onto an existing project of the same name.
	 * Pick a free name first so a copy cannot overwrite one the account
	 * already has.
	 */
	private static String unusedName(User user, String base) {
		for (int n = 0; n < 20; n++) {
			String suffix = n == 0 ? "" : " " + (n + 1);
			String candidate = base;
			if (candidate.length() + suffix.length() > 100) {
				candidate = candidate.substring(0, 100 - suffix.length());
			}
			candidate = candidate + suffix;
			if (WorldInitializer.getDefaultWorld().domain().projects()
					.get(user.getId(), candidate) == null) {
				return candidate;
			}
		}
		return null;
	}
}
