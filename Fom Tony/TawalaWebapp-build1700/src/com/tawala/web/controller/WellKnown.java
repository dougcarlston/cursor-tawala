package com.tawala.web.controller;

public class WellKnown {
    public static WellKnown urls = new WellKnown();

    public String getHome() {
        return "/";
    }
    
    public String getLogin() {
        return "/login";
    }

    public String getLoginDuringCustomization() {
        return "/login-during-customization";
    }

    public String getLogout() {
        return "/logout";
    }
    
    public String getLibrarySearch() {
        return "/library/search";
    }

    public String getLibraryCategories() {
        return "/library/categories";
    }

    public String getLibraryEditCategory() {
        return "/library/editcategory";
    }

    public String getLibraryProjectDetailView() {
        return "/library/projectdetail";
    }

    public String getLibraryProjectVersionDownload() {
        return "/library/download";
    }
    public String getLibraryEditProject() {
        return "/library/editproject";
    }

    public String getLibraryDeleteProject() {
        return "/library/deleteproject";
    }

    public String getLibraryProjectHistory() {
        return "/library/projecthistory";
    }

    public String getLibraryEditComment() {
        return "/library/editcomment";
    }

    public String getLibraryDeleteComment() {
        return "/library/deletecomment";
    }

    public String getLibraryUpdateProjectVersion() {
        return "/library/updateversion";
    }
    
    public String getLibraryAddProjectVersionFromUnreleatedProject() {
        return "/library/addversion";
    }
    
    public String getLibraryRecentChanges() {
        return "/library/recentchanges";
    }
    
    public String getLibraryRevertEvent() {
        return "/library/revertchange";
    }
    
    public String getLibraryRateProject() {
        return "/library/rateproject";
    }

    public String getLibraryDeleteProjectVersion() {
        return "/library/deleteversion";
    }

    public String getLibraryDescribeProject() {
        return "/library/describe";
    }

    public String getLibraryCustomizeAndDeploy() {
        return "/library/customize";
    }

    public String getProjectManagerSaveDuringCustomization() {
        return "/projectmanager/customize-save";
    }

    public String getProjectManagerView() {
        return "/projectmanager/view";
    }

    public String getProjectManagerProjectDetailView() {
        return "/projectmanager/projectdetail";
    }

    public String getProjectManagerPublish() {
        return "/projectmanager/publish";
    }
    public String getProjectManagerDataView() {
        return "/projectmanager/dataview";
    }

    public String getProjectManagerSummaryView() {
        return "/projectmanager/summaryview";
    }

    public String getProjectManagerExportFormCSV() {
        return "/projectmanager/export-csv";
    }

    public String getProjectManagerExportFormExcel() {
        return "/projectmanager/export-excel";
    }

    public String getProjectManagerPublishConfirmation() {
        return "/projectmanager/publishconfirm";
    }

    public String getProjectManagerImportData() {
        return "/projectmanager/import-project-data";
    }

    public String getProjectManagerImportSharedData() {
        return "/projectmanager/import-shared-data";
    }

    public String getProjectManagerDownloadVersion() {
        return "/projectmanager/downloadversion";
    }

    public String getProjectManagerInvite() {
        return "/projectmanager/invite";
    }

    public String getUserRegistration() {
        return "/user/registration";
    }

    public String getUserInitialRegistration() {
        return "/user/initialsetup";
    }

    public String getUserDisplayNextLevel() {
        return "/user/nextlevel";
    }

    public String getUserUpgradeToNextLevel() {
        return "/user/upgrade-to-next-level";
    }

    public String getUserRegistrationDuringCustomization() {
        return "/user/registration_during_customization";
    }

    public String getEmailConfirmation() {
        return "/user/confirmemail";
    }

    public String getUserAccountUpdate() {
        return "/user/account";
    }

    public String getUserPasswordReset() {
        return "/user/resetpassword";
    }

    public String getUserPasswordChange() {
        return "/user/changepassword";
    }

    public String getTutorial() {
        return "/tutorial1";
    }

    public String getManual() {
        return "/manual";
    }

    public String getWhatsNew() {
        return "/whatsnew";
    }

    public String getInfo() {
        return "/info";
    }

    public String getPrivacy() {
        return "/privacy";
    }

    public String getTerms() {
        return "/terms";
    }

    public String getNews() {
        return "/news";
    }

    public String getJobs() {
        return "/jobs";
    }

    public String getFeatures() {
        return "/features";
    }

    public String getDownload() {
        return "/download";
    }

    public String getAdminManageUsers() {
        return "/administration/users";
    }

    public String getAdminManageLibrary() {
        return "/administration/library";
    }
    
    public String getClientApi() {
        return "/client";
    }
    
    public String getProjectThemes() {
        return "/projectThemes";
    }
    
    public String getClientInfo() {
        return "/clientinfo";
    }
    
    public String getExecutable() {
        return "/Tawala.exe";
    }
    
    public String getManualPDF() {
        return "/Tawala_Manual.pdf";
    }
    
    public String getTestExceptionThrower() {
        return "/test_exception_throwing";
    }
    
    public String getLibraryTestDrivePreparation() {
        return "/library/testdrive";
    }

    public String getLibraryOneClickTestDrive() {
        return "/library/oneclicktestdrive";
    }

    public String getLibraryViewSampleData() {
        return "/library/sampledata";
    }

    public String getProjectRunUrlPrefix() {
        return "/p";
    }
    
    public String getFormPreviewUrlPrefix() {
        return "/f";
    }

    public String getLibraryRecordSampleDataUrlPrefix() {
        return "/r";
    }

    public String getLibraryTestDriveProjectUrlPrefix() {
        return "/t";
    }

    public String getUserFileDownloadPrefix() {
        return "/i";
    }

    public String getFaq() {
        return "/faq";
    }
    public String getDesigner() {
        return "/designer";
    }

    public String getAdminViewHibernateStatistics() {
        return "/administration/hibernate_statistics";
    }

    public String getWebpageCode() {
    	return "/projectmanager/webpagecode";
    }

    public String getProjectManagerDeleteVersion() {
    	return "/projectmanager/deleteversion";
    }

	public String getAdminViewUserInfo() {
		return "/administration/view_user_detail";
	}

	public String getHomePage() {
		return "/home";
	}
	
	public String getDisplayComponentRepository() {
		return "/display-component-repository";
	}

	public String getListUserDomains() {
		return "/domain/list";
	}
	
	public String getEditUserDomain() {
		return "/domain/edit";
	}

	public String getDeleteUserDomain() {
		return "/domain/delete";
	}
	
	public String getLandingPagePrefix() {
		return "/solution";
	}
	
	public String getAddSuggestion() {
		return "/domain/add-suggestion";
	}

	public String getAddNotificationRequest() {
		return "/domain/add-notification-request";
	}

	public String getReportCountOfClonedProjects() {
		return "/report/count-of-cloned-projects";
	}
	
	public String getDatabaseSchemaVersion() {
		return "/display-database-schema-version";
	}

	public String getSwitchUser() {
		return "/admin/switch-user";
	}

	public String getRestoreOriginalUser() {
		return "/admin/become-self";
	}
	
	public String getReportAllUserProjects() {
		return "/report/all-user-project-report";
	}
	
	public String getAdminSearchProject() {
		return "/admin/search-project";
	}
	
	public String getForum() {
		return "/jforum";
	}
	
	public String getViewSharedDatasources() {
		return "/projectmanager/view-shared-datasources";
	}
		
	public String getChangeProjectTheme() {
		return "/projectmanager/change-project-theme";
	}

	public String getUserProjectTestDrive() {
		return "/projectmanager/testdrive";
	}

	public String getReportAllEvents() {
		return "/report/all-event-report";
	}
	
	public String getProjectManagerDeleteMultipleVersions() {
		return "/projectmanager/delete-multiple-versions";
	}
	public String getEmailProjectLinks() {
		return "/projectmanager/email-project-links";
	}
	
	public String getCustomizeLastClonedProject() {
		return "/projectmanager/customize-library-project";
	}

	public String getCustomizeExistingProject() {
		return "/projectmanager/customize-project";
	}

	public String getAddNewDataSource() {
		return "/projectmanager/add-data-source";
	}
	
	public String getProjectManagerChangeOnlineStatus() {
		return "/projectmanager/change-online-status";
	}

	public String getProjectManagerExportAllDataDescription() {
		return "/projectmanager/export-all";
	}

	public String getProjectManagerExportAllData() {
		return "/projectmanager/create-export-all";
	}

	public String getProjectManagerRestoreAllData() {
		return "/projectmanager/restore-all-data";
	}

	public String getProjectManagerUpdateProjectDetails() {
		return "/projectmanager/update-project-details";
	}

	public String getHomeNew() {
		return "/home-new";
	}
	
	public String getMyTawalaNewUser() {
		return "/mytawala-newuser";
	}
	
	public String getExamples() {
		return "/examples";
	}
	
	public String getCustomizables() {
		return "/customizables";
	}
	
	public String getProjectManagerBackupDescription() {
		return "/projectmanager/backup";
	}

	public String getProjectManagerCreateBackup() {
		return "/projectmanager/create-backup";
	}

	public String getProjectManagerRestoreProjectFromBackup() {
		return "/projectmanager/restore-from-backup";
	}

	public String getProjectManagerRestoreProjectFromOnlineBackup() {
		return "/projectmanager/restore-from-online-backup";
	}

	public String getProjectManagerCreateBackupSchedule() {
		return "/projectmanager/create-backup-schedule";
	}

	public String getProjectManagerDeleteOnlineBackup() {
		return "/projectmanager/delete-online-backup";
	}

	public String getProjectManagerHideFormFromView() {
		return "/projectmanager/hide-form";
	}

	public String getSaveHideSelectedFormsState() {
		return "/projectmanager/save-hide-selected-forms-state";
	}

	public String getProjectManagerChangeFieldValue() {
		return "/projectmanager/change-field-value";
	}
	public String getCommunityNews() {
		return "/communityNews";
	}
	
	public String getCommunityLibrary() {
		return "/community/library";
	}

	public String getPayPalInstantPaymentNotification() {
		return "/paypal/ipn";
	}

	public String getHelpManual() {
		return "/help/manual";
	}

	public String getViewUserProjectEmail() {
		return "/email/display-project-email";
	}
	
	public String getViewUserProjectEmailBody() {
		return "/email/display-project-email-body";
	}
	
	public String getRetrieveNextProjectEmailPage() {
		return "/email/get-project-emails";
	}

	public String getExportAllProjectEmail() {
		return "/email/export-all-project-emails";
	}

	public String getViewAllUserProjectEmails() {
		return "/email/view-all-project-emails";
	}

	public String getDeleteAllUserProjectEmails() {
		return "/email/delete-all-project-emails";
	}
	
	public String getEditTheme() {
		return "/theme/edit";
	}

	public String getViewSampleProject() {
		return "/theme/sample-project";
	}
	
	public String getRenderUserDefinedTheme() {
		return "/theme/render";
	}

	public String getAddImage() {
		return "/theme/add-image";
	}

	public String getDisplayUserImage() {
		return "/theme/image";
	}
	
	public String getDeleteUserTheme() {
		return "/theme/delete";
	}
	
	public String getLibraryProjectDeployToMyTawala() {
		return "/library/deploy-to-my-tawala";
	}
	
	public String getProjectManagerUpgradeWithNewerVersionFromLibrary() {
		return "/projectmanager/upgrade-with-latest-from-library";
	}
	
	public String getKeepHttpSessionAlive() {
		return "/keep-session-alive";
	}

	public String getUserProjectReport() {
		return "/administration/userProjectReport";
	}
	
	/*
	 * Sports URLS
	 */
	
	public String getSportsHome() {
		return "/sportsdashboards/home";
	}

	public String getSportsPromoPrefix() {
		return "/sportsdashboards/promo";
	}

	public String getSportsFAQ() {
		return "/sportsdashboards/faq";
	}

	public String getSportsFAQIntroduction() {
		return "/sportsdashboards/faq/introduction";
	}

	public String getSportsFAQGettingStarted() {
		return "/sportsdashboards/faq/gettingstarted";
	}

	public String getSportsFAQSetup() {
		return "/sportsdashboards/faq/setup";
	}

	public String getSportsFAQPaypal() {
		return "/sportsdashboards/faq/paypal";
	}

	public String getSportsFAQSecurity() {
		return "/sportsdashboards/faq/security";
	}

	public String getSportsFAQRecruitment() {
		return "/sportsdashboards/faq/recruitment";
	}

	public String getSportsFAQRegistration() {
		return "/sportsdashboards/faq/registration";
	}

	public String getSportsFAQRosters() {
		return "/sportsdashboards/faq/rosters";
	}

	public String getSportsContactUs() {
		return "/sportsdashboards/contactus";
	}

	public String getSportsEmailBoard() {
		return "/sportsdashboards/emailboard";
	}
	
	public String getSportsEmailConfirmation() {
		return "/sportsdashboards/emailconfirmation";
	}
	
    public String getSportsInfo() {
        return "/sportsdashboards/info";
    }

    public String getSportsTerms() {
        return "/sportsdashboards/terms";
    }

    public String getSportsPrivacy() {
        return "/sportsdashboards/privacy";
    }

    public String getSportsJobs() {
        return "/sportsdashboards/jobs";
    }

    public String getSportsKnowledgebase() {
        return "/sportsdashboards/help/knowledgebase";
    }

	/*
	 * End Sports URLS
	 */
    
    public String getAdminUpgradeProjectsWithNewerVersion() {
        return "/administration/upgrade-projects-with-newer-version";
    }

    public String getAdminSportsDashboardManagementConsole() {
        return "/administration/sports-dashboards/overview";
    }

    public String getAdminSportsDashboardRegistrationDetails() {
        return "/administration/sports-dashboards/registration-details";
    }

    public String getAdminSportsDashboardEmailDetails() {
        return "/administration/sports-dashboards/email-details";
    }

    public String getAdminSportsDashboardProjectTaskManagement() {
        return "/administration/sports-console/task-management";
    }

    public String getAdminCreateNewDashboard() {
        return "/administration/sports-console/create-new-dashboard";
    }

    public String getAdminCreateNewClient() {
        return "/administration/sports-console/create-new-client";
    }

    public String getAdminAssignTask() {
        return "/administration/sports-console/assign-task";
    }

    public String getAdminViewDefaultTask() {
        return "/administration/sports-console/task";
    }

    public String getAdminTaskPrepareForRelease() {
        return "/administration/sports-console/prepare-for-release-task";
    }

    public String getAdminTaskCreateInvoice() {
        return "/administration/sports-console/create-invoice-task";
    }

    public String getAdminTaskCreatePurchaseOrder() {
        return "/administration/sports-console/create-purchase-order-task";
    }

    public String getAdminPerformTask() {
        return "/administration/sports-console/perform-task";
    }

    public String getAdminViewProjectWorkflowDetails() {
        return "/administration/sports-console/view-workflow-details";
    }

    public String getAdminListProjectWorkflowsInAParticularState() {
        return "/administration/sports-console/list-workflows-in-a-particular-state";
    }

    public String getAdminAssignUsersToRoles() {
        return "/administration/assign-roles";
    }

    public String getAdminManageUrgentMessage() {
        return "/administration/urgent-message";
    }

    public String getP3PIndexFile() {
    	return "/w3c/p3p.xml";
    }
    
    public String getPrivacyPolicyHtmlDocument() {
    	return "/privacy-policy.html";
    }

    public String getPrivacyPolicy() {
    	return "/w3c/privacy-policy.p3p";
    }
    
    public String getUpdateYourSportsLeagueData() {
    	return "/projectmanager/update-your-sports-league-data";
    }
    
    public String getExcelSpreadsheetCreator() {
    	return "/create-excel-spreadsheet";
    }
    
    public String getDeleteProjectGroup() {
    	return "/projectmanager/delete-project-group";
    }

    public String getManageProjectGroup() {
    	return "/projectmanager/manage-project-group";
    }

    public String getAddProjectToGroup() {
    	return "/projectmanager/add-project-to-group";
    }

    public String getDeleteProjectFromGroup() {
    	return "/projectmanager/delete-project-from-group";
    }

    public String getProjectGroupMenu() {
    	return "/projectmanager/project-group-menu";
    }
    
    public String getProjectGroupRosterReport() {
    	return "/projectmanager/group-roster-report";
    }

    public String getProjectGroupCoachReport() {
    	return "/projectmanager/group-coach-report";
    }

    public String getProjectGroupManageCoaches() {
    	return "/projectmanager/group-coach-management";
    }

    public String getProjectGroupLoadCoachesForProject() {
    	return "/projectmanager/group-coach-load-for-project";
    }
    public String getProjectGroupUpdateCoachStatus() {
    	return "/projectmanager/group-coach-update-status";
    }
    public String getProjectGroupUpdateCoachStatusMemo() {
    	return "/projectmanager/group-coach-update-status-memo";
    }
    
    public String getPrepareTeamRoster() {
    	return "/projectmanager/prepare-team-roster";
    }
}
