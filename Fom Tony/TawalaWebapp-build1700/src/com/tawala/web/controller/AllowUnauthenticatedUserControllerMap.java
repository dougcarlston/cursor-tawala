package com.tawala.web.controller;

import org.springframework.web.servlet.HandlerInterceptor;
import org.springframework.web.servlet.handler.AbstractUrlHandlerMapping;

import com.tawala.web.admin.DisplayComponentRepositoryController;
import com.tawala.web.admin.DisplaySchemaVersion;
import com.tawala.web.client.ClientApiController;
import com.tawala.web.general.HomePageController;
import com.tawala.web.general.NewHomePageController;
import com.tawala.web.library.CloneAndCustomizeController;
import com.tawala.web.library.CommunityLibraryController;
import com.tawala.web.library.CustomizablesPageController;
import com.tawala.web.library.ExamplesPageController;
import com.tawala.web.library.LibrarySearchController;
import com.tawala.web.library.OneClickTestDriveController;
import com.tawala.web.library.ProjectDescriptionController;
import com.tawala.web.library.TestDriveWithExplanationController;
import com.tawala.web.library.ViewProjectDetailsController;
import com.tawala.web.payment.paypal.InstantPaymentNotificationController;
import com.tawala.web.project.ExcelSpreadsheetCreatorController;
import com.tawala.web.project.KeepHttpSessionAliveController;
import com.tawala.web.project.theme.DisplayUserUploadedFileController;
import com.tawala.web.project.theme.RenderThemeController;
import com.tawala.web.projectmanager.ChangeProjectThemeController;
import com.tawala.web.projectmanager.LastClonedLibraryProjectCustomizationController;
import com.tawala.web.projectmanager.SendLinksByEmailController;
import com.tawala.web.projectmanager.UserProjectTestDriveController;
import com.tawala.web.sportsdashboard.ContactUsController;
import com.tawala.web.sportsdashboard.SendEmailToOrganizationBoardController;
import com.tawala.web.user.EmailVerificationController;
import com.tawala.web.user.InitialRegistrationController;
import com.tawala.web.user.LoginController;
import com.tawala.web.user.LoginDuringCustomizationController;
import com.tawala.web.user.LogoutController;
import com.tawala.web.user.RegistrationDuringCustomizationController;
import com.tawala.web.user.ResetPasswordController;
import com.tawala.web.user.UserRegistrationController;
import com.tawala.web.userdomain.AddNotificationRequestController;
import com.tawala.web.userdomain.AddSuggestionController;

public class AllowUnauthenticatedUserControllerMap extends
		AbstractUrlHandlerMapping {
	public AllowUnauthenticatedUserControllerMap() {
		setAlwaysUseFullPath(true);
		setInterceptors(new HandlerInterceptor[] {
				new UserAccessTicketInterceptor(), new NDCSetupInterceptor(),
				new VisitorTrackerInterceptor(),
				new UserInfoPreparationInterceptor(),
				new ForumCookieSetupInterceptor() });

		registerHandler(WellKnown.urls.getUserRegistration(),
				new UserRegistrationController());
		registerHandler(WellKnown.urls.getUserInitialRegistration(),
				new InitialRegistrationController());
		registerHandler(
				WellKnown.urls.getUserRegistrationDuringCustomization(),
				new RegistrationDuringCustomizationController());
		registerHandler(WellKnown.urls.getLibraryDescribeProject(),
				new ProjectDescriptionController());
		registerHandler(WellKnown.urls.getTutorial(),
				new URLToViewNameDisplayController());
		registerHandler(WellKnown.urls.getEmailConfirmation(),
				new EmailVerificationController());

		registerHandler(WellKnown.urls.getHome(), new HomeController());
		registerHandler(WellKnown.urls.getLogin(), new LoginController());
		registerHandler(WellKnown.urls.getLoginDuringCustomization(),
				new LoginDuringCustomizationController());

		registerHandler(WellKnown.urls.getLogout(), new LogoutController());
		registerHandler(WellKnown.urls.getUserPasswordReset(),
				new ResetPasswordController());

		registerHandler(WellKnown.urls.getManual(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getWhatsNew(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getFeatures(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getInfo(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getTerms(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getPrivacy(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getNews(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getJobs(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getClientApi(),
				new ClientApiController());

		registerHandler(WellKnown.urls.getProjectThemes(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getClientInfo(), new StaticFileHandler(
				"/clientinfo.xml"));

		registerHandler(WellKnown.urls.getTestExceptionThrower(),
				new TestExceptionThrower());

		registerHandler(WellKnown.urls.getLibrarySearch(),
				new LibrarySearchController());

		registerHandler(WellKnown.urls.getLibraryTestDrivePreparation(),
				new TestDriveWithExplanationController());

		registerHandler(WellKnown.urls.getLibraryOneClickTestDrive(),
				new OneClickTestDriveController());

		registerHandler(WellKnown.urls.getHomePage(), new HomePageController());

		registerHandler(WellKnown.urls.getFaq(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getLibraryCustomizeAndDeploy(),
				new CloneAndCustomizeController());
		registerHandler(WellKnown.urls.getLibraryProjectDetailView(),
				new ViewProjectDetailsController());
		registerHandler(WellKnown.urls.getDisplayComponentRepository(),
				new DisplayComponentRepositoryController());
		registerHandler(WellKnown.urls.getAddSuggestion(),
				new AddSuggestionController());
		registerHandler(WellKnown.urls.getAddNotificationRequest(),
				new AddNotificationRequestController());

		registerHandler(WellKnown.urls.getDatabaseSchemaVersion(),
				new DisplaySchemaVersion());
		registerHandler(WellKnown.urls.getChangeProjectTheme(),
				new ChangeProjectThemeController());
		registerHandler(WellKnown.urls.getUserProjectTestDrive(),
				new UserProjectTestDriveController());
		registerHandler(WellKnown.urls.getEmailProjectLinks(),
				new SendLinksByEmailController());
		registerHandler(WellKnown.urls.getCustomizeLastClonedProject(),
				new LastClonedLibraryProjectCustomizationController());

		registerHandler(WellKnown.urls.getHomeNew(),
				new NewHomePageController());

		registerHandler(WellKnown.urls.getExamples(),
				new ExamplesPageController());

		registerHandler(WellKnown.urls.getCustomizables(),
				new CustomizablesPageController());

		registerHandler(WellKnown.urls.getDesigner(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getMyTawalaNewUser(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getExecutable(), new StaticFileHandler(
				"Tawala.exe"));

		registerHandler(WellKnown.urls.getCommunityNews(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getCommunityLibrary(),
				new CommunityLibraryController());

		registerHandler(WellKnown.urls.getPayPalInstantPaymentNotification(),
				new InstantPaymentNotificationController());

		registerHandler(WellKnown.urls.getHelpManual(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getRenderUserDefinedTheme(),
				new RenderThemeController());

		registerHandler(WellKnown.urls.getDisplayUserImage(),
				new DisplayUserUploadedFileController());

		registerHandler(WellKnown.urls.getKeepHttpSessionAlive(),
				new KeepHttpSessionAliveController());

		/*
		 * SportsDashboards pages
		 */
		registerHandler(WellKnown.urls.getSportsHome(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsFAQ(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsFAQIntroduction(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsFAQGettingStarted(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsFAQSetup(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsFAQPaypal(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsFAQSecurity(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsFAQRecruitment(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsFAQRegistration(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsFAQRosters(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsInfo(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsTerms(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsPrivacy(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsJobs(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsContactUs(),
				new ContactUsController());

		registerHandler(WellKnown.urls.getSportsEmailBoard(),
				new SendEmailToOrganizationBoardController());

		registerHandler(WellKnown.urls.getSportsEmailConfirmation(),
				new URLToViewNameDisplayController());

		registerHandler(WellKnown.urls.getSportsKnowledgebase(),
				new URLToViewNameDisplayController());

		/*
		 * end SportsDashboards pages
		 */

		registerHandler(WellKnown.urls.getP3PIndexFile(),
				new StaticFileHandler(WellKnown.urls.getP3PIndexFile()));

		registerHandler(WellKnown.urls.getPrivacyPolicyHtmlDocument(),
				new StaticFileHandler(WellKnown.urls
						.getPrivacyPolicyHtmlDocument()));

		registerHandler(WellKnown.urls.getPrivacyPolicy(),
				new StaticFileHandler(WellKnown.urls.getPrivacyPolicy()));

		registerHandler(WellKnown.urls.getExcelSpreadsheetCreator(),
				new ExcelSpreadsheetCreatorController());
	}
}
