package com.tawala.web.project.theme;

import java.util.HashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.web.bind.ServletRequestDataBinder;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.UsersHibernateImpl;
import com.tawala.domain.User;
import com.tawala.project.theme.CommonTheme;
import com.tawala.project.theme.UserDefinedTheme;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class EditThemeController extends SimpleFormController {
	public static final String THEME_ID_PARAMETER = "id";

	public EditThemeController() {
		setCommandName("theme");
		setCommandClass(UserDefinedTheme.class);
		setFormView("edit.theme");
		setSuccessView("redirect:" + WellKnown.urls.getProjectManagerView());
	}

	@Override
	protected void initBinder(HttpServletRequest request,
			ServletRequestDataBinder binder) throws Exception {
		binder.setAllowedFields(new String[] { "name", "parentThemeId",
				"styleDefinitions", "headerImage" });
	}
	
	@Override
	protected Object formBackingObject(HttpServletRequest request) throws Exception {
		String themeId = request.getParameter(THEME_ID_PARAMETER);
		UserDefinedTheme theme = null;
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		if (themeId == null) {
			theme = new UserDefinedTheme(user);
			theme.setParentThemeId("plain");
		} else {
			theme = UsersHibernateImpl.getUserThemeById(user, Long
					.parseLong(themeId));
		}
		return theme;
	}
	
	@Override
	protected Map referenceData(HttpServletRequest request) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();
		result.put("availableThemes", CommonTheme.ALL_THEMES);
		result.put("allThemeAttributes", CommonTheme.getThemeCSSAttributesAsJSONString(request));
		return result;
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request, HttpServletResponse response, Object command, BindException errors) throws Exception {
		UserDefinedTheme theme = (UserDefinedTheme) command;
		UsersHibernateImpl.saveTheme(theme);
		return new ModelAndView(getSuccessView());
	}
}
