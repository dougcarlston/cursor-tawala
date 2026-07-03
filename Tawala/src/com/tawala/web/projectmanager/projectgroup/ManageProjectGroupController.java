package com.tawala.web.projectmanager.projectgroup;

import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.ValidationUtils;
import org.springframework.validation.Validator;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.UsersHibernateImpl;
import com.tawala.domain.ProjectGroup;
import com.tawala.domain.User;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class ManageProjectGroupController extends SimpleFormController {
	public static final String GROUP_ID_PARAMETER = "group_id";

	public ManageProjectGroupController() {
		setFormView("projectmanager.manage.project.group");
		setValidator(new FormValidator());
	}
	
	@SuppressWarnings("unchecked")
	@Override
	protected Map referenceData(HttpServletRequest request) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();
		result.put("sportsdashboardsGroups", UsersHibernateImpl
				.getAllUserSportsDashboardGroups(UserInfoPreparationInterceptor
						.getSessionUser(request)));
		
		return result;
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		String groupIdParameter = request.getParameter(GROUP_ID_PARAMETER);

		User groupOwner = UserInfoPreparationInterceptor
				.getSessionUser(request);
		ProjectGroup projectGroup = groupIdParameter == null ? new ProjectGroup(
				groupOwner, "")
				: UsersHibernateImpl.getProjectGroup(groupOwner, Long
						.parseLong(groupIdParameter));
		return new Form(projectGroup);
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		Form form = (Form) command;
		ProjectGroup projectGroup = form.getProjectGroup();
		if (form.isDelete()) {
			UsersHibernateImpl.deleteProjectGroup(
					UserInfoPreparationInterceptor.getSessionUser(request),
					projectGroup.getId());
			response.sendRedirect(WellKnown.urls.getProjectManagerView());
		} else {
			UsersHibernateImpl.saveProjectGroup(projectGroup);
			response.sendRedirect(WellKnown.urls.getManageProjectGroup() + "?" + GROUP_ID_PARAMETER + "=" + form.getProjectGroup().getId());

		}

		return null;
	}

	public static class Form {
		private ProjectGroup projectGroup;
		private boolean delete;

		Form(ProjectGroup projectGroup) {
			this.projectGroup = projectGroup;
		}

		public ProjectGroup getProjectGroup() {
			return projectGroup;
		}

		public boolean isDelete() {
			return delete;
		}

		public void setDelete(boolean delete) {
			this.delete = delete;
		}

	}

	public static class FormValidator implements Validator {

		@SuppressWarnings("unchecked")
		public boolean supports(Class clazz) {
			return clazz == Form.class;
		}

		public void validate(Object target, Errors errors) {
			Form form = (Form) target;
			ValidationUtils.rejectIfEmptyOrWhitespace(errors,
					"projectGroup.name", "project.group.name.is.empty");

			if (errors.hasErrors()) {
				return;
			}

			ProjectGroup modifiedGroup = form.getProjectGroup();
			List<ProjectGroup> projectGroups = UsersHibernateImpl
					.getAllUserSportsDashboardGroups(modifiedGroup
							.getGroupOwner());
			for (ProjectGroup projectGroup : projectGroups) {
				if (projectGroup.equals(modifiedGroup)) {
					// --- Exclude the group being edited.
					continue;
				}
				if (projectGroup.getName().equals(modifiedGroup.getName())) {
					errors.rejectValue("name", "project.group.name.duplicate",
							new Object[] { modifiedGroup.getName() },
							"Duplicated group name. Please use another one.");
				}
			}
		}
	}
}
