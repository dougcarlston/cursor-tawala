package com.tawala.web.userdomain;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.userdomain.UserDomain;
import com.tawala.userdomain.UserDomainStorage;

public class EditUserDomainController extends SimpleFormController {
	public static final String DOMAIN_ID_PARAMETER = "domain_id";

	public EditUserDomainController() {
		setSessionForm(false);
		setValidateOnBinding(false);
		setCommandName("form");
		setFormView("edit.user.domain");
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		Form result = new Form();
		String domainIdParameter = request.getParameter(DOMAIN_ID_PARAMETER);
		if (domainIdParameter == null) {
			result.setUserDomain(new UserDomain());
		} else {
			result.setUserDomain(UserDomainStorage.getDomainById(Long
					.parseLong(domainIdParameter)));
		}

		return result;
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		Form form = (Form) command;
		UserDomain userDomain = form.getUserDomain();

		String[] ids = form.getProjectIds().split(",");
		List<Long> projectIds = new ArrayList<Long>(ids.length);
		for (int i = 0; i < ids.length; i++) {
			projectIds.add(Long.parseLong(ids[i]));
		}
		
		//--- TODO: this is a temporary fix until we decide if we would like to drop this altogether.
		if(userDomain.getSuggestionPrompt() == null) {
			userDomain.setSuggestionPrompt("");
		}

		if (userDomain.getId() > 0) {
			UserDomainStorage.update(userDomain, projectIds);
		} else {
			UserDomainStorage.createDomain(userDomain, projectIds);
		}

		form.setUserDomain(UserDomainStorage.getDomainById(userDomain.getId()));
		form.setUpdateSuccessful(true);

		return showForm(request, response, errors);
	}

	@Override
	protected Map referenceData(HttpServletRequest request, Object command,
			Errors errors) throws Exception {
		Form form = (Form) command;

		Map<String, Object> result = new HashMap<String, Object>();
		Collection<LibraryProject> allProjectsWithoutSelected = ProjectLibraryService
				.getProjectEligibleForInclusionOnLandingPages();
		allProjectsWithoutSelected.removeAll(form.getUserDomain()
				.getFeaturedProjects());
		result.put("allprojects", allProjectsWithoutSelected);

		return result;
	}

	public static class Form {
		private UserDomain userDomain;
		private String projectIds;
		private boolean updateSuccessful;

		public boolean isUpdateSuccessful() {
			return updateSuccessful;
		}

		public void setUpdateSuccessful(boolean updateSuccessful) {
			this.updateSuccessful = updateSuccessful;
		}

		public String getProjectIds() {
			return projectIds;
		}

		public void setProjectIds(String projectIds) {
			this.projectIds = projectIds;
		}

		public UserDomain getUserDomain() {
			return userDomain;
		}

		public void setUserDomain(UserDomain userDomain) {
			this.userDomain = userDomain;
		}
	}
}
