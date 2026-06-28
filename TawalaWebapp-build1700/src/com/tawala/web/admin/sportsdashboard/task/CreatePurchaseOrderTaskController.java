package com.tawala.web.admin.sportsdashboard.task;

import java.math.BigDecimal;
import java.text.DecimalFormat;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;

import org.springframework.beans.propertyeditors.CustomDateEditor;
import org.springframework.beans.propertyeditors.CustomNumberEditor;
import org.springframework.validation.Errors;
import org.springframework.web.bind.ServletRequestDataBinder;

import com.scissor.Log;
import com.tawala.jbpm.sportsdashboards.UserProjectProcessTask;
import com.tawala.project.Form;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;

public class CreatePurchaseOrderTaskController extends
		DefaultViewTaskController {
	private long purchaseOrderProjectId;

	public CreatePurchaseOrderTaskController(long purchaseOrderProjectId) {
		this.purchaseOrderProjectId = purchaseOrderProjectId;
		setFormView("admin.sports-dashboard.task.create.purchase.order");
	}

	public static class CreatePOForm extends
			DefaultViewTaskController.ViewTaskForm {
		private String purchaseOrderNumber;
		private Date registrationStartDate;
		private BigDecimal costPerPlayer;

		protected CreatePOForm(UserProjectProcessTask taskInstance) {
			super(taskInstance);
			registrationStartDate = taskInstance.getUserProject()
					.getRegistrationStartDate();
			if (registrationStartDate == null) {
				registrationStartDate = new Date();
			}
		}

		public String getPurchaseOrderNumber() {
			return purchaseOrderNumber;
		}

		public void setPurchaseOrderNumber(String purchaseOrderNumber) {
			this.purchaseOrderNumber = purchaseOrderNumber;
		}

		public Date getRegistrationStartDate() {
			return registrationStartDate;
		}

		public void setRegistrationStartDate(Date registrationStartDate) {
			this.registrationStartDate = registrationStartDate;
		}

		public BigDecimal getCostPerPlayer() {
			return costPerPlayer;
		}

		public void setCostPerPlayer(BigDecimal costPerPlayer) {
			this.costPerPlayer = costPerPlayer;
		}
	}

	@Override
	protected Object formBackingObject(HttpServletRequest request)
			throws Exception {
		return new CreatePOForm(getRequestedTask(request));
	}

	@Override
	protected void performTaskSpecificWork(ViewTaskForm form) {
		CreatePOForm actualForm = (CreatePOForm) form;
		UserProject project = actualForm.getProcessTask().getUserProject();
		project.setPurchaseOrderNumber(actualForm.getPurchaseOrderNumber());
		project.setRegistrationStartDate(actualForm.getRegistrationStartDate());
		project.setRegistrationFee(actualForm.getCostPerPlayer());

		ProjectsHibernateImpl.updateProjectDetails(project);
	}

	@SuppressWarnings("unchecked")
	@Override
	protected Map referenceData(HttpServletRequest request, Object command,
			Errors errors) throws Exception {
		Map<String, Object> result = super.referenceData(request, command,
				errors);

		UserProject userProject = ProjectsHibernateImpl
				.getUserProjectWithRuntimeById(purchaseOrderProjectId);
		if (userProject == null) {
			Log.error(this, "Unable to find invoicing project #"
					+ purchaseOrderProjectId);
		} else {
			String url = findUrlByFormName(userProject, "Administration");
			if (url == null) {
				Log.error(this,
						"Unable to find CreateInvoice form in project #"
								+ purchaseOrderProjectId);
			} else {
				result.put("createPurchaseOrderURL", url);
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

	@Override
	protected void initBinder(HttpServletRequest request,
			ServletRequestDataBinder binder) throws Exception {
		binder.registerCustomEditor(Date.class, null, new CustomDateEditor(
				new SimpleDateFormat("MM/dd/yyyy"), true));
		binder.registerCustomEditor(BigDecimal.class, null,
				new CustomNumberEditor(BigDecimal.class, new DecimalFormat(
						"##0.00"), true));
	}

}
