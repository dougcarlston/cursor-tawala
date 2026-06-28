package com.tawala.web.project.theme;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.core.io.Resource;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.notification.BaseNotification;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.theme.UserDefinedTheme;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.oldhtml.OldPage;
import com.tawala.web.oldhtml.RenderingContext;
import com.tawala.web.project.DataCollectingProjectController;

public class ViewSampleProjectController implements Controller {
	public static class SampleProjectHolder {
		private static Project project;

		public static Project getProject() {
			return project;
		}

		public void setProjectXML(Resource projectXML) throws IOException {
			project = new Project(new ConfigElement(BaseNotification
					.getResourceContentsAsString(projectXML)));
		}
	}

	public static final String THEME_ID = "theme_id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		String themeId = request.getParameter(THEME_ID);
		if (themeId != null) {
			boolean isCustomTheme = true;
			long customThemeId = 0;
			try {
				customThemeId = Long.parseLong(themeId);
				isCustomTheme = true;
			} catch (Exception e) {
				isCustomTheme = false;
			}
			if (isCustomTheme) {
				UserDefinedTheme theme = UsersHibernateImpl.getUserThemeById(
						UserInfoPreparationInterceptor.getSessionUser(request),
						customThemeId);
				if (theme == null) {
					// --- Attempt to view somebody else's theme.
					themeId = null;
				} else {
					// --- Display the parent theme. The custom theme will be
					// displayed inline.
					themeId = theme.getParentThemeId();
				}
			}
		}

		if (themeId == null) {
			themeId = "plain";
		}

		Project project = SampleProjectHolder.getProject().makeCopy();
		project.setThemePath(themeId);

		User user = new User(Status.REGISTERED);
		ExecutionContext context = new ExecutionContext(WorldInitializer
				.getDefaultWorld().domain(), new UserProject(project, user,
				"Sample Project"));

		context.setSubmission(new FormSubmission(false));
		OldPage page = project.getForms().get(0).getSegment(0).toPage(context);

		page.setHeaderCSS("");

		DataCollectingProjectController.addHeaderAndStyleSheets(project,
				context, page);

		response.setContentType("text/html");
		page.render(response.getWriter(), new RenderingContext());

		return null;
	}
}
