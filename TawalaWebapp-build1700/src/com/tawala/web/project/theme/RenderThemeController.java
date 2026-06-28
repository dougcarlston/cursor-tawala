package com.tawala.web.project.theme;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.project.theme.UserDefinedTheme;
import com.tawala.web.CachingContentGenerator;
import com.tawala.web.Response;

public class RenderThemeController implements Controller {
	public static final String THEME_ID = "id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		response.setContentType("text/css");

		new CachingContentGenerator().cacheResponse(response, 20000);
		Response.addP3PPolicyHeader(response);
		String themeId = request.getParameter(THEME_ID);
		UserDefinedTheme theme = null;
		if (themeId == null) {
			Log.error(this, "Unable to find theme id in the request.");
		} else {
			theme = UsersHibernateImpl
					.getUserThemeById(Long.parseLong(themeId));
		}

		response.getOutputStream().print(
				theme == null ? "" : theme.getStyleDefinitions());

		return null;
	}
}
