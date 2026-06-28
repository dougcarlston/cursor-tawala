package com.tawala.web.controller;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.handler.AbstractUrlHandlerMapping;

import com.tawala.web.admin.AssignRolesController;
import com.tawala.web.admin.ProjectSearchController;
import com.tawala.web.admin.SwitchUserController;
import com.tawala.web.admin.UpgradeProjectsWithNewerVersionsController;
import com.tawala.web.admin.UrgentMessageController;
import com.tawala.web.admin.ViewHibernateStatisticsController;
import com.tawala.web.admin.ViewUserDetailController;
import com.tawala.web.admin.sportsdashboard.AssignTaskController;
import com.tawala.web.admin.sportsdashboard.CreateNewDashboardController;
import com.tawala.web.admin.sportsdashboard.ListProjectWorkflowsInAParticularStateController;
import com.tawala.web.admin.sportsdashboard.SportsDashboardOverviewController;
import com.tawala.web.admin.sportsdashboard.SportsDashboardTaskManagementController;
import com.tawala.web.admin.sportsdashboard.ViewProjectWorkflowDetailsController;
import com.tawala.web.admin.sportsdashboard.task.CreateInvoiceTaskController;
import com.tawala.web.admin.sportsdashboard.task.CreatePurchaseOrderTaskController;
import com.tawala.web.admin.sportsdashboard.task.DefaultViewTaskController;
import com.tawala.web.admin.sportsdashboard.task.PrepareProjectForReleaseTaskController;
import com.tawala.web.email.DeleteAllProjectEmailsController;
import com.tawala.web.library.ManageLibraryController;
import com.tawala.web.projectmanager.UpdateProjectDetailsController;
import com.tawala.web.projectmanager.projectgroup.AddProjectToGroupController;
import com.tawala.web.projectmanager.projectgroup.DeleteProjectFromGroupController;
import com.tawala.web.projectmanager.projectgroup.ManageProjectGroupController;
import com.tawala.web.report.AllEventReportController;
import com.tawala.web.report.AllUserProjectReportController;
import com.tawala.web.report.CountOfClonedProjectsReportController;
import com.tawala.web.user.ManageUsersController;
import com.tawala.web.userdomain.DeleteUserDomainController;
import com.tawala.web.userdomain.EditUserDomainController;
import com.tawala.web.userdomain.ListUserDomainsController;

public class OnlyAdministratorControllerMap extends AbstractUrlHandlerMapping
		implements InitializingBean {

	private long invoicingProjectId;
	private long purchaseOrderProjectId;

	public OnlyAdministratorControllerMap() {
		setAlwaysUseFullPath(true);

		setInterceptors(new HandlerInterceptor[] {
				new UserAccessTicketInterceptor(), new NDCSetupInterceptor(),
				new VisitorTrackerInterceptor(),
				new OnlyAdministratorAccessInterceptor(),
				new AdminStatisticsPreparationInterceptor(),
				new UserInfoPreparationInterceptor() });
	}

	public void afterPropertiesSet() throws Exception {
		registerHandler(WellKnown.urls.getAdminManageUsers(),
				new ManageUsersController());
		registerHandler(WellKnown.urls.getAdminManageLibrary(),
				new ManageLibraryController());
		registerHandler(WellKnown.urls.getAdminViewHibernateStatistics(),
				new ViewHibernateStatisticsController());
		registerHandler(WellKnown.urls.getAdminViewUserInfo(),
				new ViewUserDetailController());
		registerHandler(WellKnown.urls.getListUserDomains(),
				new ListUserDomainsController());
		registerHandler(WellKnown.urls.getEditUserDomain(),
				new EditUserDomainController());
		registerHandler(WellKnown.urls.getDeleteUserDomain(),
				new DeleteUserDomainController());
		registerHandler(WellKnown.urls.getReportCountOfClonedProjects(),
				new CountOfClonedProjectsReportController());
		registerHandler(WellKnown.urls.getSwitchUser(),
				new SwitchUserController());
		registerHandler(WellKnown.urls.getReportAllUserProjects(),
				new AllUserProjectReportController());
		registerHandler(WellKnown.urls.getAdminSearchProject(),
				new ProjectSearchController());
		registerHandler(WellKnown.urls.getReportAllEvents(),
				new AllEventReportController());
		registerHandler(WellKnown.urls
				.getAdminUpgradeProjectsWithNewerVersion(),
				new UpgradeProjectsWithNewerVersionsController());
		registerHandler(WellKnown.urls.getAdminCreateNewDashboard(),
				new CreateNewDashboardController());
		registerHandler(WellKnown.urls.getAdminCreateNewClient(),
				new URLToViewNameDisplayController());
		registerHandler(WellKnown.urls
				.getAdminSportsDashboardProjectTaskManagement(),
				new SportsDashboardTaskManagementController());
		registerHandler(WellKnown.urls
				.getAdminSportsDashboardManagementConsole(),
				new SportsDashboardOverviewController());
		registerHandler(WellKnown.urls
				.getAdminSportsDashboardRegistrationDetails(),
				new URLToViewNameDisplayController());
		registerHandler(WellKnown.urls.getAdminSportsDashboardEmailDetails(),
				new URLToViewNameDisplayController());
		registerHandler(WellKnown.urls.getAdminAssignTask(),
				new AssignTaskController());
		registerHandler(WellKnown.urls.getUserProjectReport(),
				new URLToViewNameDisplayController());
		registerHandler(WellKnown.urls.getAdminViewDefaultTask(),
				new DefaultViewTaskController());
		registerHandler(WellKnown.urls.getAdminViewProjectWorkflowDetails(),
				new ViewProjectWorkflowDetailsController());
		registerHandler(WellKnown.urls
				.getAdminListProjectWorkflowsInAParticularState(),
				new ListProjectWorkflowsInAParticularStateController());
		registerHandler(WellKnown.urls.getAdminAssignUsersToRoles(),
				new AssignRolesController());
		registerHandler(WellKnown.urls.getAdminTaskPrepareForRelease(),
				new PrepareProjectForReleaseTaskController());
		registerHandler(WellKnown.urls.getProjectManagerUpdateProjectDetails(),
				new UpdateProjectDetailsController());
		registerHandler(WellKnown.urls.getAdminTaskCreateInvoice(),
				new CreateInvoiceTaskController(invoicingProjectId));
		registerHandler(WellKnown.urls.getAdminTaskCreatePurchaseOrder(),
				new CreatePurchaseOrderTaskController(purchaseOrderProjectId));
		registerHandler(WellKnown.urls.getAdminManageUrgentMessage(),
				new UrgentMessageController(getServletContext()));
		registerHandler(WellKnown.urls.getManageProjectGroup(),
				new ManageProjectGroupController());
		registerHandler(WellKnown.urls.getAddProjectToGroup(),
				new AddProjectToGroupController());
		registerHandler(WellKnown.urls.getDeleteProjectFromGroup(),
				new DeleteProjectFromGroupController());
		registerHandler(WellKnown.urls.getDeleteAllUserProjectEmails(),
				new DeleteAllProjectEmailsController());
	}

	public long getInvoicingProjectId() {
		return invoicingProjectId;
	}

	public void setInvoicingProjectId(long invoicingProjectId) {
		this.invoicingProjectId = invoicingProjectId;
	}

	public long getPurchaseOrderProjectId() {
		return purchaseOrderProjectId;
	}

	public void setPurchaseOrderProjectId(long purchaseOrderProjectId) {
		this.purchaseOrderProjectId = purchaseOrderProjectId;
	}
}
