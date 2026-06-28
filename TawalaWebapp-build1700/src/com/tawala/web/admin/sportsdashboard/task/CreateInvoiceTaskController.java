package com.tawala.web.admin.sportsdashboard.task;

import java.util.Date;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;

import org.springframework.validation.Errors;

import com.scissor.Log;
import com.tawala.jbpm.sportsdashboards.UserProjectProcessTask;
import com.tawala.project.Form;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;

public class CreateInvoiceTaskController extends DefaultViewTaskController {
	private long invoicingProjectId;
	
	public static class CreateInvoiceForm extends DefaultViewTaskController.ViewTaskForm {
		private String invoiceNumber;
		protected CreateInvoiceForm(UserProjectProcessTask taskInstance) {
			super(taskInstance);
		}
		public String getInvoiceNumber() {
			return invoiceNumber;
		}
		public void setInvoiceNumber(String invoiceNumber) {
			this.invoiceNumber = invoiceNumber;
		}
	}

	public CreateInvoiceTaskController(long invoicingProjectId) {
		this.invoicingProjectId = invoicingProjectId;
		setFormView("admin.sports-dashboard.task.create.invoice");
	}


	@Override
	protected void performTaskSpecificWork(ViewTaskForm form) {
		CreateInvoiceForm createInvoiceForm = (CreateInvoiceForm) form;
		UserProject project = createInvoiceForm.getProcessTask().getUserProject();
		project.setInvoiceNumber(createInvoiceForm.getInvoiceNumber());
		project.setRegistrationInvoiceDate(new Date());
		
		ProjectsHibernateImpl.updateProjectDetails(project);
	}
	
	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		return new CreateInvoiceForm(getRequestedTask(request));
	}
	
	@SuppressWarnings("unchecked")
	@Override
	protected Map referenceData(HttpServletRequest request, Object command,
			Errors errors) throws Exception {
		Map<String, Object> result = super.referenceData(request, command,
				errors);

		UserProjectProcessTask currentTask = getRequestedTask(request);
		
		UserProject invoicingProject = ProjectsHibernateImpl
				.getUserProjectWithRuntimeById(invoicingProjectId);
		if (invoicingProject == null) {
			Log.error(this, "Unable to find invoicing project #"
					+ invoicingProjectId);
		} else {
			String url = findUrlByFormName(invoicingProject, "Start");
			if (url == null) {
				Log.error(this,
						"Unable to find CreateInvoice form in project #"
								+ invoicingProjectId);
			} else {
				if(currentTask.getUserProject().getPurchaseOrderNumber() != null) {
					url = url + "?_segment=4&POSearchCrit=" + currentTask.getUserProject().getPurchaseOrderNumber();
				}

				result.put("createInvoiceURL", url);
			}
		}

		return result;
	}

	private String findUrlByFormName(UserProject userProject, String formName) {
		Map<Form, String> urls = userProject.getEntryPointURLs();
		for (Map.Entry<Form, String> mapEntry : urls.entrySet()) {
			if (mapEntry.getKey().getName().equals(formName)) {
				return mapEntry.getValue();
			}
		}
		return null;
	}

}
