package com.tawala.web.admin.sportsdashboard;

import java.util.Date;
import java.util.HashMap;
import java.util.Map;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.jbpm.JbpmContext;
import org.jbpm.context.exe.ContextInstance;
import org.jbpm.graph.def.ProcessDefinition;
import org.jbpm.graph.exe.ProcessInstance;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;
import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.ValidationUtils;
import org.springframework.validation.Validator;
import org.springframework.web.bind.ServletRequestDataBinder;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.domain.User;
import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.jbpm.JbpmService;
import com.tawala.jbpm.sportsdashboards.Constants;
import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.LibraryProjectEditor;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class CreateNewDashboardController extends SimpleFormController {
	public CreateNewDashboardController() {
		setFormView("admin.sports-dashboard.create.new");
		setCommandClass(Form.class);
		setValidator(new FormValidator());
	}

	@Override
	protected ModelAndView onSubmit(final HttpServletRequest request,
			final HttpServletResponse response, Object command,
			BindException errors) throws Exception {
		final Form form = (Form) command;
		final String versionDescription = "Deployed version "
				+ form.getLibraryProject().getLatestVersionNumber() + " of "
				+ form.getLibraryProject().getName();

		final String themeId = form.getLibraryProject().getLatestVersion()
				.getProject().getTheme().getThemeId();

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		UserProject userProject = (UserProject) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(TransactionStatus arg0) {
						UserProject userProject = ProjectLibraryService
								.cloneProjectToUserAccount(form
										.getLibraryProject(), form.getUser(),
										false, form.getProjectName(), themeId,
										versionDescription);

						userProject.setProjectType(Constants.PROJECT_TYPE);

						JbpmContext jbpmContext = JbpmService.createContext();
						try {
							ProcessDefinition processDefinition = jbpmContext
									.getGraphSession()
									.findLatestProcessDefinition(
											Constants.PROCESS_NAME);
							ProcessInstance processInstance = new ProcessInstance(
									processDefinition);
							processInstance.setKey(String.valueOf(userProject
									.getId()));
							processInstance.setStart(new Date());
							ContextInstance contextInstance = (ContextInstance) processInstance
									.getInstance(ContextInstance.class);

							contextInstance.setVariable(Constants.PROJECT_ID,
									userProject.getId());
							User sessionUser = UserInfoPreparationInterceptor
									.getSessionUser(request);
							contextInstance.setVariable(
									Constants.ORIGINAL_CREATOR, sessionUser
											.getId());

							// These variables are set only be viewed in the
							// console.
							contextInstance.setVariable("user", userProject
									.getUser().getId());
							contextInstance.setVariable("project name",
									userProject.getName());

							processInstance.signal();
							jbpmContext.save(processInstance);
						} finally {
							jbpmContext.close();
						}

						EventService.createEvent(new Event(
								"LibraryProjectCloning", request, form
										.getLibraryProject().getName()));
						return userProject;
					}
				});

		ModelAndView result = new ModelAndView(
				"admin.sports-dashboard.create.new.confirmation");

		result.addObject("userProject", userProject);
		result.addObject("libraryProject", form.libraryProject);
		result.addObject("user", form.getUser());

		return result;
	}

	@Override
	protected void initBinder(HttpServletRequest request,
			ServletRequestDataBinder binder) throws ServletException {
		binder.registerCustomEditor(LibraryProject.class,
				new LibraryProjectEditor());
	}

	@SuppressWarnings("unchecked")
	@Override
	protected Map referenceData(HttpServletRequest request) throws Exception {
		Map<String, Object> result = new HashMap<String, Object>();

		result.put("libraryProjects", ProjectLibraryService
				.getAllProjectsFrom(ProjectLibrary.PAID_PROJECT_LIBRARY));
		return result;
	}

	public static class Form {
		private LibraryProject libraryProject;
		private String userName;
		private String projectName;
		private User user;

		public LibraryProject getLibraryProject() {
			return libraryProject;
		}

		public void setLibraryProject(LibraryProject libraryProject) {
			this.libraryProject = libraryProject;
		}

		public String getUserName() {
			return userName;
		}

		public void setUserName(String userName) {
			this.userName = userName;
		}

		public String getProjectName() {
			return projectName;
		}

		public void setProjectName(String projectName) {
			this.projectName = projectName;
		}

		public User getUser() {
			return user;
		}

		public void setUser(User user) {
			this.user = user;
		}
	}

	public static class FormValidator implements Validator {
		@SuppressWarnings("unchecked")
		public boolean supports(Class clazz) {
			return clazz == Form.class;
		}

		public void validate(Object object, Errors errors) {
			ValidationUtils.rejectIfEmptyOrWhitespace(errors, "userName",
					"create.sports-dashboard.user.name.empty");
			ValidationUtils.rejectIfEmptyOrWhitespace(errors, "projectName",
					"create.sports-dashboard.project.name.empty");

			Form form = (Form) object;
			if (!errors.hasFieldErrors("userName")) {
				User user = WorldInitializer.getDefaultWorld().domain().users()
						.get(form.getUserName());
				if (user == null) {
					errors.rejectValue("userName",
							"create.sports-dashboard.user.not.found");
				} else {
					form.setUser(user);
				}
			}
			if (!errors.hasFieldErrors("userName")
					&& !errors.hasFieldErrors("projectName")) {
				UserProject userProject = WorldInitializer.getDefaultWorld()
						.domain().projects().get(form.getUser().getId(),
								form.getProjectName());
				if (userProject != null) {
					errors.rejectValue("projectName",
							"create.sports-dashboard.project.name.duplicate");
				}
			}
		}

	}
}
