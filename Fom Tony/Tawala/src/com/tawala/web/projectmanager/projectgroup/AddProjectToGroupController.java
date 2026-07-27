package com.tawala.web.projectmanager.projectgroup;

import java.io.IOException;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.lucene.queryParser.ParseException;
import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.ValidationUtils;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.AbstractWizardFormController;

import com.tawala.UsersHibernateImpl;
import com.tawala.domain.ProjectGroup;
import com.tawala.domain.User;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class AddProjectToGroupController extends AbstractWizardFormController {
	public static final String GROUP_ID_PARAMETER = "group_id";
	private static final int PAGE_COUNT = 3;

	public AddProjectToGroupController() {
		setPages(new String[] {
				"projectmanager.add.project.to.group.search.users",
				"projectmanager.add.project.to.group.select.user",
				"projectmanager.add.project.to.group.select.project" });
		setPageAttribute("pageNumber");
	}
	
	@SuppressWarnings("unchecked")
	@Override
	protected Map referenceData(HttpServletRequest request, Object command,
			Errors errors, int page) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();
		result.put("totalPageCount", PAGE_COUNT);
		result.put("sportsdashboardsGroups", UsersHibernateImpl
				.getAllUserSportsDashboardGroups(UserInfoPreparationInterceptor
						.getSessionUser(request)));
		return result;
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		User groupOwner = UserInfoPreparationInterceptor
				.getSessionUser(request);
		ProjectGroup projectGroup = UsersHibernateImpl.getProjectGroup(
				groupOwner, Long.parseLong(request
						.getParameter(GROUP_ID_PARAMETER)));
		return new Form(projectGroup);
	}

	@Override
	protected ModelAndView processFinish(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		Form form = (Form) command;
		ProjectGroup projectGroup = form.getProjectGroup();

		UsersHibernateImpl.addUserProjectToGroup(UserInfoPreparationInterceptor
				.getSessionUser(request), form.getProjectGroup().getId(), form
				.getUserProjectId());

		redirectToTheProjectGroupManagementPage(response, projectGroup);

		return null;
	}

	private void redirectToTheProjectGroupManagementPage(
			HttpServletResponse response, ProjectGroup projectGroup)
			throws IOException {
		response.sendRedirect(WellKnown.urls.getManageProjectGroup() + "?"
				+ ManageProjectGroupController.GROUP_ID_PARAMETER + "="
				+ projectGroup.getId());
	}

	@Override
	protected ModelAndView processCancel(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {

		redirectToTheProjectGroupManagementPage(response, ((Form) command)
				.getProjectGroup());

		return null;
	}

	@Override
	protected void validatePage(Object command, Errors errors, int page) {
		Form form = (Form) command;
		switch (page) {
		case 0:
			ValidationUtils.rejectIfEmptyOrWhitespace(errors, "userQuery",
					"add.project.to.group.empty.query");
			if (!errors.hasErrors()) {
				try {
					List<User> users = WorldInitializer.getDefaultWorld()
							.domain().users().search(form.getUserQuery());
					if (users == null || users.size() == 0) {
						errors.rejectValue("userQuery",
								"add.project.to.group.query.empty.result");
					} else {
						form.setUsers(users);
					}
				} catch (ParseException e) {
					errors.rejectValue("userQuery",
							"add.project.to.group.query.error");
				} catch (IOException e) {
					errors.rejectValue("userQuery",
							"add.project.to.group.search.error");
				}
			}
			break;

		case 1:
			if (form.getUserId() == null) {
				errors.reject("add.project.to.group.no.user.selected");
			} else {
				for (User user : form.getUsers()) {
					if (user.getDatabaseId() == form.getUserId()) {
						List<UserProject> projects = WorldInitializer
								.getDefaultWorld().domain().projects()
								.getAllForUserId(user.getId());
						if (projects == null || projects.size() == 0) {
							errors
									.reject("add.project.to.group.user.has.no.projects");
						} else {
							form.setProjects(projects);
						}
					}
				}

			}
			break;

		case 2:
			if (form.getUserProjectId() == null) {
				errors.reject("add.project.to.group.no.project.selected");
			}

		default:
			break;
		}
	}

	public static class Form {
		private ProjectGroup projectGroup;
		private String userQuery;
		private Long userId;
		private Long userProjectId;
		private List<User> users;
		private List<UserProject> projects;

		Form(ProjectGroup projectGroup) {
			this.projectGroup = projectGroup;
		}

		public ProjectGroup getProjectGroup() {
			return projectGroup;
		}

		public Long getUserId() {
			return userId;
		}

		public void setUserId(Long userId) {
			this.userId = userId;
		}

		public Long getUserProjectId() {
			return userProjectId;
		}

		public void setUserProjectId(Long userProjectId) {
			this.userProjectId = userProjectId;
		}

		public String getUserQuery() {
			return userQuery;
		}

		public void setUserQuery(String userQuery) {
			this.userQuery = userQuery;
		}

		public List<User> getUsers() {
			return users;
		}

		public void setUsers(List<User> users) {
			this.users = users;
		}

		public List<UserProject> getProjects() {
			return projects;
		}

		public void setProjects(List<UserProject> projects) {
			this.projects = projects;
		}
	}
}
