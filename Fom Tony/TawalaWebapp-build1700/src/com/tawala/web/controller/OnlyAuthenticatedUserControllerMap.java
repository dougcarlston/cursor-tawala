package com.tawala.web.controller;

import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.handler.AbstractUrlHandlerMapping;

import com.tawala.web.admin.RestoreOriginalUserController;
import com.tawala.web.email.ExportAllProjectEmailController;
import com.tawala.web.email.RetrieveNextProjectEmailPageController;
import com.tawala.web.email.ViewAllUserProjectEmailsController;
import com.tawala.web.email.ViewUserProjectEmail;
import com.tawala.web.email.ViewUserProjectEmailBody;
import com.tawala.web.library.AddVersionToRelatedProjectController;
import com.tawala.web.library.AddVersionToUnrelatedProjectController;
import com.tawala.web.library.DeleteCommentController;
import com.tawala.web.library.DeleteProjectController;
import com.tawala.web.library.DeleteVersionController;
import com.tawala.web.library.DeployToMyTawalaController;
import com.tawala.web.library.DownloadLibraryProjectVersionController;
import com.tawala.web.library.EditCategoryController;
import com.tawala.web.library.EditCommentController;
import com.tawala.web.library.ModifyProjectController;
import com.tawala.web.library.PublishProjectToLibraryController;
import com.tawala.web.library.RateProjectController;
import com.tawala.web.library.RevertEventController;
import com.tawala.web.library.ViewCategoriesController;
import com.tawala.web.library.ViewProjectHistoryController;
import com.tawala.web.library.ViewProjectVersionSampleData;
import com.tawala.web.library.ViewRecentLibraryChangesController;
import com.tawala.web.project.theme.AddUserUploadedImageController;
import com.tawala.web.project.theme.DeleteThemeController;
import com.tawala.web.project.theme.EditThemeController;
import com.tawala.web.project.theme.ViewSampleProjectController;
import com.tawala.web.projectmanager.AddNewDataSourceController;
import com.tawala.web.projectmanager.CSVFormDataExportController;
import com.tawala.web.projectmanager.ChangeOnlineStatusController;
import com.tawala.web.projectmanager.ChangeSubmissionFieldValueController;
import com.tawala.web.projectmanager.DeleteMultipleVersionsController;
import com.tawala.web.projectmanager.DisplayInvitationController;
import com.tawala.web.projectmanager.ExcelFormDataExportController;
import com.tawala.web.projectmanager.ExistingProjectCustomizationController;
import com.tawala.web.projectmanager.HowToIncludeInAnotherWebSiteController;
import com.tawala.web.projectmanager.ProjectVersionDownloadController;
import com.tawala.web.projectmanager.SaveDuringCustomizationController;
import com.tawala.web.projectmanager.SaveFormSelectionController;
import com.tawala.web.projectmanager.SaveHideSelectedFormsStateController;
import com.tawala.web.projectmanager.UpgradeWithNewerVersionController;
import com.tawala.web.projectmanager.ViewProjectManagerController;
import com.tawala.web.projectmanager.ViewProjectManagerDataController;
import com.tawala.web.projectmanager.ViewProjectManagerDetailsController;
import com.tawala.web.projectmanager.ViewProjectManagerSummaryController;
import com.tawala.web.projectmanager.ViewSharedDataSourcesController;
import com.tawala.web.projectmanager.alldataimport.AllDataImportController;
import com.tawala.web.projectmanager.alldataimport.AllProjectDataExportController;
import com.tawala.web.projectmanager.alldataimport.DisplayExportAllDataDescriptionController;
import com.tawala.web.projectmanager.backup.BackupController;
import com.tawala.web.projectmanager.backup.BackupRestoreController;
import com.tawala.web.projectmanager.backup.DeleteOnlineBackupController;
import com.tawala.web.projectmanager.backup.DisplayBackupDescriptionController;
import com.tawala.web.projectmanager.backup.OnlineBackupRestoreController;
import com.tawala.web.projectmanager.backup.SetupDailyBackupController;
import com.tawala.web.projectmanager.dataimport.ProjectDataImportController;
import com.tawala.web.projectmanager.dataimport.SharedDataImportController;
import com.tawala.web.projectmanager.integration.UpdateYourSportsLeagueDataController;
import com.tawala.web.projectmanager.projectgroup.GroupCoachReportController;
import com.tawala.web.projectmanager.projectgroup.GroupRosterReportController;
import com.tawala.web.projectmanager.projectgroup.LoadProjectCoachesController;
import com.tawala.web.projectmanager.projectgroup.SelectCoachController;
import com.tawala.web.projectmanager.projectgroup.UpdateCoachStatusController;
import com.tawala.web.projectmanager.projectgroup.UpdateCoachStatusMemoController;
import com.tawala.web.projectmanager.projectgroup.ViewGroupMenuController;
import com.tawala.web.user.AccountUpdateController;
import com.tawala.web.user.ChangePasswordController;
import com.tawala.web.user.DisplayNextLevelDetailsController;
import com.tawala.web.user.UpgradeToNextLevelController;

public class OnlyAuthenticatedUserControllerMap extends
		AbstractUrlHandlerMapping {

	public OnlyAuthenticatedUserControllerMap() {
		setAlwaysUseFullPath(true);

		setInterceptors(new HandlerInterceptor[] {
				new UserAccessTicketInterceptor(), new NDCSetupInterceptor(),
				new VisitorTrackerInterceptor(),
				new OnlyAuthenticatedUserAccessInterceptor(),
				new UserInfoPreparationInterceptor() });

		registerHandler(WellKnown.urls.getProjectManagerView(),
				new ViewProjectManagerController());
		registerHandler(WellKnown.urls.getProjectManagerProjectDetailView(),
				new ViewProjectManagerDetailsController());
		registerHandler(WellKnown.urls.getProjectManagerDataView(),
				new ViewProjectManagerDataController());
		registerHandler(WellKnown.urls.getProjectManagerSummaryView(),
				new ViewProjectManagerSummaryController());
		registerHandler(WellKnown.urls.getProjectManagerExportFormCSV(),
				new CSVFormDataExportController());
		registerHandler(WellKnown.urls.getProjectManagerExportFormExcel(),
				new ExcelFormDataExportController());
		registerHandler(WellKnown.urls.getProjectManagerImportData(),
				new ProjectDataImportController());
		registerHandler(WellKnown.urls.getProjectManagerImportSharedData(),
				new SharedDataImportController());
		registerHandler(WellKnown.urls.getProjectManagerDownloadVersion(),
				new ProjectVersionDownloadController());
		registerHandler(WellKnown.urls.getProjectManagerPublish(),
				new PublishProjectToLibraryController());
		registerHandler(WellKnown.urls.getProjectManagerInvite(),
				new DisplayInvitationController());
		registerHandler(WellKnown.urls.getLibraryEditProject(),
				new ModifyProjectController());

		registerHandler(WellKnown.urls.getLibraryProjectHistory(),
				new ViewProjectHistoryController());
		registerHandler(WellKnown.urls.getLibraryRecentChanges(),
				new ViewRecentLibraryChangesController());

		registerHandler(WellKnown.urls.getLibraryDeleteProject(),
				new DeleteProjectController());
		registerHandler(WellKnown.urls.getLibraryEditComment(),
				new EditCommentController());
		registerHandler(WellKnown.urls.getLibraryProjectVersionDownload(),
				new DownloadLibraryProjectVersionController());
		registerHandler(WellKnown.urls.getLibraryDeleteComment(),
				new DeleteCommentController());
		registerHandler(WellKnown.urls.getLibraryUpdateProjectVersion(),
				new AddVersionToRelatedProjectController());
		registerHandler(WellKnown.urls
				.getLibraryAddProjectVersionFromUnreleatedProject(),
				new AddVersionToUnrelatedProjectController());
		registerHandler(WellKnown.urls.getLibraryRevertEvent(),
				new RevertEventController());
		registerHandler(WellKnown.urls.getLibraryRateProject(),
				new RateProjectController());
		registerHandler(WellKnown.urls.getLibraryDeleteProjectVersion(),
				new DeleteVersionController());
		registerHandler(WellKnown.urls.getLibraryViewSampleData(),
				new ViewProjectVersionSampleData());
		registerHandler(WellKnown.urls.getLibraryCategories(),
				new ViewCategoriesController());
		registerHandler(WellKnown.urls.getLibraryEditCategory(),
				new EditCategoryController());

		registerHandler(WellKnown.urls.getUserAccountUpdate(),
				new AccountUpdateController());

		registerHandler(WellKnown.urls.getUserPasswordChange(),
				new ChangePasswordController());

		registerHandler(WellKnown.urls.getDownload(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getWebpageCode(),
				new HowToIncludeInAnotherWebSiteController());

		registerHandler(WellKnown.urls.getUserDisplayNextLevel(),
				new DisplayNextLevelDetailsController());
		registerHandler(WellKnown.urls.getUserUpgradeToNextLevel(),
				new UpgradeToNextLevelController());
		registerHandler(WellKnown.urls
				.getProjectManagerSaveDuringCustomization(),
				new SaveDuringCustomizationController());
		registerHandler(WellKnown.urls.getRestoreOriginalUser(),
				new RestoreOriginalUserController());

		registerHandler(WellKnown.urls.getManualPDF(), new StaticFileHandler(
				"Tawala_Manual.pdf"));

		registerHandler(WellKnown.urls.getViewSharedDatasources(),
				new ViewSharedDataSourcesController());
		registerHandler(WellKnown.urls
				.getProjectManagerDeleteMultipleVersions(),
				new DeleteMultipleVersionsController());
		registerHandler(WellKnown.urls.getCustomizeExistingProject(),
				new ExistingProjectCustomizationController());
		registerHandler(WellKnown.urls.getAddNewDataSource(),
				new AddNewDataSourceController());
		registerHandler(WellKnown.urls.getProjectManagerChangeOnlineStatus(),
				new ChangeOnlineStatusController());
		registerHandler(WellKnown.urls
				.getProjectManagerExportAllDataDescription(),
				new DisplayExportAllDataDescriptionController());
		registerHandler(WellKnown.urls.getProjectManagerExportAllData(),
				new AllProjectDataExportController());
		registerHandler(WellKnown.urls.getProjectManagerRestoreAllData(),
				new AllDataImportController());
		registerHandler(WellKnown.urls.getProjectManagerBackupDescription(),
				new DisplayBackupDescriptionController());
		registerHandler(WellKnown.urls.getProjectManagerCreateBackup(),
				new BackupController());
		registerHandler(WellKnown.urls
				.getProjectManagerRestoreProjectFromBackup(),
				new BackupRestoreController());
		registerHandler(WellKnown.urls.getProjectManagerCreateBackupSchedule(),
				new SetupDailyBackupController());
		registerHandler(WellKnown.urls
				.getProjectManagerRestoreProjectFromOnlineBackup(),
				new OnlineBackupRestoreController());
		registerHandler(WellKnown.urls.getProjectManagerDeleteOnlineBackup(),
				new DeleteOnlineBackupController());
		registerHandler(WellKnown.urls.getProjectManagerHideFormFromView(),
				new SaveFormSelectionController());
		registerHandler(WellKnown.urls.getSaveHideSelectedFormsState(),
				new SaveHideSelectedFormsStateController());
		registerHandler(WellKnown.urls.getProjectManagerChangeFieldValue(),
				new ChangeSubmissionFieldValueController());
		registerHandler(WellKnown.urls.getViewUserProjectEmail(),
				new ViewUserProjectEmail());
		registerHandler(WellKnown.urls.getViewUserProjectEmailBody(),
				new ViewUserProjectEmailBody());
		registerHandler(WellKnown.urls.getRetrieveNextProjectEmailPage(),
				new RetrieveNextProjectEmailPageController());
		registerHandler(WellKnown.urls.getViewAllUserProjectEmails(),
				new ViewAllUserProjectEmailsController());
		registerHandler(WellKnown.urls.getViewSampleProject(),
				new ViewSampleProjectController());
		registerHandler(WellKnown.urls.getEditTheme(),
				new EditThemeController());
		registerHandler(WellKnown.urls.getAddImage(),
				new AddUserUploadedImageController());
		registerHandler(WellKnown.urls.getDeleteUserTheme(),
				new DeleteThemeController());
		registerHandler(WellKnown.urls.getLibraryProjectDeployToMyTawala(),
				new DeployToMyTawalaController());
		registerHandler(WellKnown.urls
				.getProjectManagerUpgradeWithNewerVersionFromLibrary(),
				new UpgradeWithNewerVersionController());
		registerHandler(WellKnown.urls.getExportAllProjectEmail(),
				new ExportAllProjectEmailController());
		registerHandler(WellKnown.urls.getUpdateYourSportsLeagueData(),
				new UpdateYourSportsLeagueDataController());
		registerHandler(WellKnown.urls.getProjectGroupMenu(),
				new ViewGroupMenuController());
		registerHandler(WellKnown.urls.getProjectGroupRosterReport(),
				new GroupRosterReportController());
		registerHandler(WellKnown.urls.getProjectGroupCoachReport(),
				new GroupCoachReportController());
		registerHandler(WellKnown.urls.getProjectGroupManageCoaches(),
				new SelectCoachController());
		registerHandler(WellKnown.urls.getProjectGroupLoadCoachesForProject(),
				new LoadProjectCoachesController());
		registerHandler(WellKnown.urls.getProjectGroupUpdateCoachStatus(),
				new UpdateCoachStatusController());
		registerHandler(WellKnown.urls.getProjectGroupUpdateCoachStatusMemo(),
				new UpdateCoachStatusMemoController());
	}
}
