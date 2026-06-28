package com.tawala.web.projectmanager;

import java.util.ArrayList;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.validation.BindException;
import org.springframework.validation.Errors;
import org.springframework.validation.ValidationUtils;
import org.springframework.validation.Validator;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.SimpleFormController;

import com.tawala.project.Project;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.data.DataSource;
import com.tawala.project.data.StringField;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;
import com.tawala.web.controller.WellKnown;

public class AddNewDataSourceController extends SimpleFormController {
	public AddNewDataSourceController() {
		setFormView("projectmanager.add.datasource");
		setCommandName("formBean");
		setValidator(new FormBeanValidator());
	}

	@Override
	protected ModelAndView onSubmit(HttpServletRequest request,
			HttpServletResponse response, Object command, BindException errors)
			throws Exception {

		FormBean formBean = (FormBean) command;

		DataSource dataSource = new DataSource(formBean.getDataSourceName());
		for (String fieldName : formBean.getFieldNames()) {
			dataSource.addField(new StringField(fieldName));
		}

		ProjectsHibernateImpl.addSharedDataSources(
				UserInfoPreparationInterceptor.getSessionUser(request),
				Collections.singletonList(dataSource));

		response.sendRedirect(WellKnown.urls.getViewSharedDatasources());

		return null;
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		Project project = WorldInitializer.getDefaultWorld().domain().users()
				.getSharedStorageForUser(
						UserInfoPreparationInterceptor.getSessionUser(request));
		FormBean result = new FormBean(project);

		String[] fields = request.getParameterValues("field");
		if (fields != null) {
			for (String field : fields) {
				field = field.trim();
				if (field.length() > 0) {
					result.getFieldNames().add(field);
				}
			}
		}
		return result;
	}

	public static class FormBean {
		private String dataSourceName;
		private List<String> fieldNames = new ArrayList<String>();
		private Project sharedData;

		public FormBean(Project sharedData) {
			this.sharedData = sharedData;
		}

		public String getDataSourceName() {
			return dataSourceName;
		}

		public void setDataSourceName(String dataSourceName) {
			this.dataSourceName = dataSourceName;
		}

		public List<String> getFieldNames() {
			return fieldNames;
		}

		public void setFieldNames(List<String> fieldNames) {
			this.fieldNames = fieldNames;
		}

		public Project getSharedData() {
			return sharedData;
		}
	}

	public static class FormBeanValidator implements Validator {

		public boolean supports(Class clazz) {
			return clazz.equals(FormBean.class);
		}

		public void validate(Object command, Errors errors) {
			FormBean formBean = (FormBean) command;
			ValidationUtils.rejectIfEmptyOrWhitespace(errors, "dataSourceName",
					"datasource.name.is.empty");

			if (formBean.getSharedData() != null) {
				if (formBean.getSharedData().getDataSourceNamed(
						formBean.getDataSourceName()) != null) {
					errors.rejectValue("dataSourceName",
							"datasource.name.is.duplicate");
				}
			}

			if (formBean.getFieldNames().size() == 0) {
				errors.rejectValue("fieldNames", "datasource.no.fields");
			} else {
				Set<String> uniqueFieldNames = new HashSet<String>();
				int i = 0;
				for (String fieldName : formBean.getFieldNames()) {
					String propertyName = "fieldNames[" + i + "]";
					if (!StringField.isValidFieldName(fieldName)) {
						errors.rejectValue(propertyName,
								"datasource.field.name.illegal");
					} else if(! uniqueFieldNames.add(fieldName)) {
						errors.rejectValue(propertyName,
						"datasource.field.name.is.duplicate");
					}
					i++;
				}
			}
		}
	}
}
