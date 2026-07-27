package com.tawala.web.projectmanager;

import java.text.SimpleDateFormat;
import java.util.Date;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.beans.propertyeditors.CustomDateEditor;
import org.springframework.validation.BindException;
import org.springframework.web.bind.ServletRequestDataBinder;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;

public class UpdateProjectDetailsController extends SimpleFormController {
	public static final String PROJECT_ID_PARAMETER = "project-id";
	
	public static class Form {
		private UserProject userProject;
		public Form(UserProject userProject) {
			this.userProject = userProject;
		}
		public UserProject getUserProject() {
			return userProject;
		}
	}
	
	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {
		Form form = (Form) command;
		UserProject userProject = form.getUserProject();
		ProjectsHibernateImpl.updateProjectDetails(userProject);
		
		ViewProjectManagerDetailsController.redirectToProjectDetails(response, userProject.getName());
		
		return null;
	}
	
	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		long projectId = Long.parseLong(request.getParameter(PROJECT_ID_PARAMETER));
		UserProject userProject = ProjectsHibernateImpl.getUserProjectById(projectId);
		if(userProject == null) {
			throw new IllegalStateException("Unable to find user project by id: " + projectId);
		}
		
		return new Form(userProject);
	}
	
	protected void initBinder(HttpServletRequest request, ServletRequestDataBinder binder)
    throws Exception {
        SimpleDateFormat dateFormat = new SimpleDateFormat("MM/dd/yyyy");
        binder.registerCustomEditor(Date.class, null, new CustomDateEditor(dateFormat, true));
    }
}
