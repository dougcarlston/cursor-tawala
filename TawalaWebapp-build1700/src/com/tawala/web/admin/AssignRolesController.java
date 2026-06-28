package com.tawala.web.admin;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.UsersHibernateImpl;
import com.tawala.domain.Role;
import com.tawala.domain.User;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class AssignRolesController extends SimpleFormController {
	public AssignRolesController() {
		setFormView("admin.assign.roles");
		setCommandClass(Form.class);
	}

	@SuppressWarnings("unchecked")
	@Override
	protected Map referenceData(HttpServletRequest request) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();
		result.put("roles", Role.PREDEFINED_ROLES);
		return result;
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		return new Form(UsersHibernateImpl.getAllAdminUsers());
	}

	public static class Form {
		private Map<Long, Set<Role>> rolesByUserIdMap;
		private Map<Long, User> usersByUserIdMap;

		public Form(List<User> users) {
			rolesByUserIdMap = new TreeMap<Long, Set<Role>>();
			usersByUserIdMap = new HashMap<Long, User>();
			for (User user : users) {
				rolesByUserIdMap.put(user.getDatabaseId(), user.getRoles());
				usersByUserIdMap.put(user.getDatabaseId(), user);
			}
		}

		public Map<Long, Set<Role>> getRolesByUserIdMap() {
			return rolesByUserIdMap;
		}

		public Map<Long, User> getUsersByUserIdMap() {
			return usersByUserIdMap;
		}
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		Form form = (Form) command;

		User currentSessionOwner = UserInfoPreparationInterceptor.getSessionUser(request);
		for (Map.Entry<Long, Set<Role>> roleMapEntry : form
				.getRolesByUserIdMap().entrySet()) {
			long userDatabaseId = roleMapEntry.getKey().longValue();
			Set<Role> newRoles = roleMapEntry
					.getValue();
			UsersHibernateImpl.updateUserRoles(userDatabaseId, newRoles);
			if(currentSessionOwner.getDatabaseId() == userDatabaseId) {
				currentSessionOwner.setRoles(newRoles);
			}
		}

		response.sendRedirect(WellKnown.urls.getAdminAssignUsersToRoles());

		return null;
	}
}
