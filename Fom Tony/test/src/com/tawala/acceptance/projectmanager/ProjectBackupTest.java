package com.tawala.acceptance.projectmanager;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.util.List;
import java.util.Properties;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;
import org.springframework.web.servlet.mvc.AbstractWizardFormController;
import org.xml.sax.SAXException;

import com.meterware.httpunit.HTMLElement;
import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.protocol.UploadFileSpec;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.batch.backup.ScheduledBackup;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.FormSubmission;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.project.Value;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.project.backup.BackupSchedule;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.Reference;
import com.tawala.project.data.ProjectBackup;
import com.tawala.project.data.ProjectBackupCreator;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.backup.BackupRestoreController;
import com.tawala.web.projectmanager.backup.SetupDailyBackupController;

public class ProjectBackupTest extends AcceptanceTestCase {
	private static final String FIRST_FORM_NAME = "Form 1";
	private static final String SECOND_FORM_NAME = "Form 2";
	private static final String FIRST_FIELD_NAME = "field1";
	private static final String SECOND_FIELD_NAME = "field2";
	private static final String THIRD_FIELD_NAME = "field3";
	private static final String DECLARED_FIELD_NAME = "declared_field";
	private static final String FORTH_FIELD_NAME = "field4";
	private static final String MCQ_FIELD_NAME = "mcq_field";

	private static final String FORM_NAME = "uploadForm";

	private Project project;
	private UserProject userProject;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		WorldInitializer.getDefaultWorld().domain().users()
				.onUserUpgradeToFullyRegistered(projectOwner);

		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm(FIRST_FORM_NAME);
		FibBuilder fibBuilder = formBuilder.addFib("My field:");
		fibBuilder.addBlank(FIRST_FIELD_NAME);
		fibBuilder.addBlank(SECOND_FIELD_NAME);
		fibBuilder.addBlank(THIRD_FIELD_NAME);
		formBuilder.addDeclaredFields(DECLARED_FIELD_NAME);
		formBuilder.addMcWithAlternateLabel(MCQ_FIELD_NAME,
				"You prefer to have:", false, false, "bananas", "oranges",
				"peaches");

		formBuilder = builder.addForm(SECOND_FORM_NAME);
		fibBuilder = formBuilder.addFib();
		fibBuilder.addBlank(FORTH_FIELD_NAME);

		project = builder.build();
		userProject = new UserProject(project, projectOwner, "test");

		world.domain().projects().put(userProject);
	}

	protected void navigateToBackupPage() throws RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		// --- Extract the link to the description page.
		// --- It's stored in a JavaScript variable.
		Pattern pattern = Pattern.compile("var linkToBackup = '([^']*)'",
				Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());

		String link = matcher.group(1);
		bot.go(link);

		// --- Extract the link to the backup itself. It's stored in a
		// JavaScript
		// variable.
		pattern = Pattern.compile("\\s*parent.backupURL = '([^']*)'",
				Pattern.MULTILINE);
		matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());

		link = matcher.group(1);
		bot.go(link);
	}

	protected void navigateToFirstRestorePage(User user) throws RobotException {
		bot.logInAs(user);
		bot.go(WellKnown.urls.getProjectManagerView());

		// --- Extract the link to the first page of restore wizard.
		// --- It's stored in a JavaScript variable.
		Pattern pattern = Pattern.compile("var linkToRestore = '([^']*)'",
				Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());

		String link = matcher.group(1);
		bot.go(link);

	}

	public void testBasicBackup() throws RobotException, IOException {
		navigateToBackupPage();

		assertEquals("application/octet-stream", bot.getContentType());
		assertMatches("attachment; filename=\"" + userProject.getName()
				+ "-\\d\\d\\d\\d\\d\\d\\d\\d_\\d\\d\\d\\d\\.backup\";", bot
				.lastResponse().getHeaderField("Content-Disposition"));

		ProjectBackup backup = ProjectBackupCreator.recreateFromZip(bot
				.lastResponse().getInputStream());
		Properties backupProperties = backup.getBackupProperties();

		assertEquals(ProjectBackupCreator.CURRENT_VERSION, backupProperties
				.get(ProjectBackupCreator.PROPERTY_BACKUP_VERSION_NUMBER));
		assertEquals(userProject.getName(), backupProperties
				.get(ProjectBackupCreator.PROPERTY_PROJECT_NAME));
		assertEquals(Long.toString(userProject.getId()), backupProperties
				.get(ProjectBackupCreator.PROPERTY_PROJECT_ID));
		assertEquals(Integer.toString(userProject.getDeployedVersion()
				.getVersionNumber()), backupProperties
				.get(ProjectBackupCreator.PROPERTY_PROJECT_VERSION_NUMBER));

		assertNotNull(backup.getProject().getForm(FIRST_FORM_NAME));
		assertNotNull(backup.getLinks());
		assertEquals(1, backup.getLinks().size());
		assertEquals(userProject.getUniqueRandomId(), backup.getLinks().get(0)
				.getId());
	}

	public void testBackupOfProjectWithLongFormNames() throws RobotException,
			IOException {
		// --- Create a version of the project with an invalid name.
		ProjectBuilder builder = new ProjectBuilder();
		String formName = "Long and ugly name with a bunch of special characters []{}<>?)_*&*&@#!%@#@#";
		FormBuilder formBuilder = builder.addForm(formName);
		FibBuilder fibBuilder = formBuilder.addFib("My field:");
		fibBuilder.addBlank(FIRST_FIELD_NAME);
		fibBuilder.addBlank(SECOND_FIELD_NAME);
		fibBuilder.addBlank(THIRD_FIELD_NAME);
		formBuilder.addDeclaredFields(DECLARED_FIELD_NAME);

		formBuilder = builder.addForm(SECOND_FORM_NAME);
		fibBuilder = formBuilder.addFib();
		fibBuilder.addBlank(FORTH_FIELD_NAME);

		project = builder.build();
		userProject = new UserProject(project, projectOwner, "test");

		userProject = world.domain().projects().put(userProject);

		// --- Record some data
		bot.go(userProject, userProject.getProject().getForm(formName));
		WebForm form = bot.getForm(0);
		form.setParameter(FIRST_FIELD_NAME, "1");
		form.setParameter(SECOND_FIELD_NAME, "2");
		form.setParameter(THIRD_FIELD_NAME, "3");
		bot.submit(form);

		bot.go(userProject, userProject.getProject().getForm(formName));
		form = bot.getForm(0);
		form.setParameter(FIRST_FIELD_NAME, "11");
		form.setParameter(SECOND_FIELD_NAME, "22");
		form.setParameter(THIRD_FIELD_NAME, "33");
		bot.submit(form);

		bot.go(userProject, userProject.getProject().getForm(SECOND_FORM_NAME));
		form = bot.getForm(0);
		form.setParameter(FORTH_FIELD_NAME, "4");
		bot.submit(form);

		navigateToBackupPage();

		assertEquals("application/octet-stream", bot.getContentType());
		assertMatches("attachment; filename=\"" + userProject.getName()
				+ "-\\d\\d\\d\\d\\d\\d\\d\\d_\\d\\d\\d\\d\\.backup\";", bot
				.lastResponse().getHeaderField("Content-Disposition"));

		ByteArrayOutputStream data = captureTheBackupData();

		ProjectBackup backup = ProjectBackupCreator
				.recreateFromZip(new ByteArrayInputStream(data.toByteArray()));
		Properties backupProperties = backup.getBackupProperties();

		assertEquals(ProjectBackupCreator.CURRENT_VERSION, backupProperties
				.get(ProjectBackupCreator.PROPERTY_BACKUP_VERSION_NUMBER));
		assertEquals(userProject.getName(), backupProperties
				.get(ProjectBackupCreator.PROPERTY_PROJECT_NAME));
		assertEquals(Long.toString(userProject.getId()), backupProperties
				.get(ProjectBackupCreator.PROPERTY_PROJECT_ID));
		assertEquals(Integer.toString(userProject.getDeployedVersion()
				.getVersionNumber()), backupProperties
				.get(ProjectBackupCreator.PROPERTY_PROJECT_VERSION_NUMBER));

		assertNotNull(backup.getProject().getForm(formName));
		assertNotNull(backup.getLinks());
		assertEquals(1, backup.getLinks().size());
		assertEquals(userProject.getUniqueRandomId(), backup.getLinks().get(0)
				.getId());

		world.domain().storedData().purgeProjectResponses(
				userProject.getProject());

		// -- Restore the data
		navigateToFirstRestorePage(projectOwner);

		// --- File submission step
		form = bot.getForm(FORM_NAME);

		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\test-backup.zip", new ByteArrayInputStream(data
						.toByteArray()), "application/zip") });

		bot.submit(form, BackupRestoreController.PARAM_TARGET + "1");

		// --- Import approval step
		form = bot.getForm(FORM_NAME);
		form.setCheckbox("confirmRestore", true);
		bot.submit(form, BackupRestoreController.PARAM_FINISH);

		// --- Confirmation screen
		assertContains("Project " + userProject.getName()
				+ " has been restored.", bot.lastResponse().getText());

		UserProject newProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), userProject.getName());
		assertNotNull(newProject);

		// --- Validate restored data
		List<FormSubmission> submissions = world.domain().storedData()
				.responsesFor(userProject.getProject(), formName);
		assertEquals(2, submissions.size());
		FormSubmission submission = submissions.get(0);
		assertEquals("1", submission.getValue(new Reference(FIRST_FIELD_NAME))
				.toString());
		assertEquals("2", submission.getValue(new Reference(SECOND_FIELD_NAME))
				.toString());
		assertEquals("3", submission.getValue(new Reference(THIRD_FIELD_NAME))
				.toString());

		submission = submissions.get(1);
		assertEquals("11", submission.getValue(new Reference(FIRST_FIELD_NAME))
				.toString());
		assertEquals("22", submission
				.getValue(new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("33", submission.getValue(new Reference(THIRD_FIELD_NAME))
				.toString());

	}

	public void testRestoreOfDeletedProject() throws RobotException,
			IOException {
		// --- Add some data to the project.
		recordData();

		// --- Add a couple of links
		LinkToUserProject regularLink = LinkToUserProject
				.createUnauthenticatedLink(userProject);
		world.domain().projects().addLinkToProject(regularLink);

		LinkToUserProject privateLink = new LinkToUserProject(userProject,
				"authentication token");
		world.domain().projects().addLinkToProject(privateLink);

		// --- Create the backup
		navigateToBackupPage();

		ByteArrayOutputStream data = captureTheBackupData();

		// --- Delete the project
		world.domain().projects().delete(userProject, world);

		navigateToFirstRestorePage(projectOwner);

		// --- File submission step
		WebForm form = bot.getForm(FORM_NAME);

		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\test-backup.zip", new ByteArrayInputStream(data
						.toByteArray()), "application/zip") });

		bot.submit(form, BackupRestoreController.PARAM_TARGET + "1");

		// --- Import approval step
		// --- Test project name validation
		String newProjectName = "New Project Name";
		UserProject duplicateProject = new UserProject(ProjectBuilder
				.buildMinimalisticProject(), projectOwner, newProjectName);
		world.domain().projects().put(duplicateProject);

		form = bot.getForm(FORM_NAME);
		form.setParameter("projectName", newProjectName);
		bot.submit(form, BackupRestoreController.PARAM_FINISH);

		assertContains("You already have a project named \"" + newProjectName
				+ "\"", bot.getPageText());

		// --- Remove the duplicate and continue
		world.domain().projects().delete(duplicateProject, world);

		form = bot.getForm(FORM_NAME);
		form.setParameter("projectName", newProjectName);
		bot.submit(form, BackupRestoreController.PARAM_FINISH);

		// --- Confirmation screen
		assertContains("Project " + newProjectName + " has been restored.", bot
				.lastResponse().getText());

		// --- Verify data
		UserProject newProject = world.domain().projects()
				.getWithProjectRuntime(projectOwner.getId(), newProjectName);
		assertNotNull(newProject);

		assertNotEquals(userProject.getId(), newProject.getId());
		assertEquals(userProject.getUniqueRandomId(), newProject
				.getUniqueRandomId());
		assertEquals(userProject.getProject().getPropertiesAsString(),
				newProject.getProject().getPropertiesAsString());

		verifyRestoredData(newProject);

		verifyLinks(regularLink, privateLink, newProject);
	}

	private ByteArrayOutputStream captureTheBackupData() throws IOException {
		ByteArrayOutputStream data = new ByteArrayOutputStream();
		InputStream inputStream = bot.lastResponse().getInputStream();
		byte[] buffer = new byte[1024];
		int readBytes = 0;
		while ((readBytes = inputStream.read(buffer)) != -1) {
			data.write(buffer, 0, readBytes);
		}
		return data;
	}

	public void testRestoreOfExistingProject() throws RobotException,
			IOException {
		// --- Add some data to the project.
		recordData();

		// --- Add a couple of links
		LinkToUserProject regularLink = LinkToUserProject
				.createUnauthenticatedLink(userProject);
		world.domain().projects().addLinkToProject(regularLink);

		LinkToUserProject privateLink = new LinkToUserProject(userProject,
				"authentication token");
		world.domain().projects().addLinkToProject(privateLink);

		// --- Create the backup
		navigateToBackupPage();

		ByteArrayOutputStream data = captureTheBackupData();

		String projectXMLAtTheTimeOfBackup = userProject.getProject()
				.getProjectXmlDefinition();

		// --- Record additional data
		bot.go(userProject, userProject.getProject().getForm(FIRST_FORM_NAME));
		WebForm form = bot.getForm(0);
		form.setParameter(FIRST_FIELD_NAME, "1 - new");
		form.setParameter(SECOND_FIELD_NAME, "2 - new");
		form.setParameter(THIRD_FIELD_NAME, "3 - new");
		bot.submit(form);

		// --- Add a new version of the project
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm(FIRST_FORM_NAME);
		FibBuilder fibBuilder = formBuilder.addFib("My field:");
		fibBuilder.addBlank(FIRST_FIELD_NAME);

		project = builder.build();
		userProject = new UserProject(project, projectOwner, "test");

		world.domain().projects().put(userProject);
		// --- Retrieve the versions
		userProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), userProject.getName());

		navigateToFirstRestorePage(projectOwner);

		// --- File submission step
		form = bot.getForm(FORM_NAME);

		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\test-backup.zip", new ByteArrayInputStream(data
						.toByteArray()), "application/zip") });

		bot.submit(form, BackupRestoreController.PARAM_TARGET + "1");

		// --- Import approval step
		form = bot.getForm(FORM_NAME);
		form.setCheckbox("confirmRestore", true);
		bot.submit(form, BackupRestoreController.PARAM_FINISH);

		// --- Confirmation screen
		assertContains("Project " + userProject.getName()
				+ " has been restored.", bot.lastResponse().getText());

		UserProject newProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), userProject.getName());
		assertNotNull(newProject);

		// --- Validate data
		assertEquals(userProject.getDeployedVersion().getVersionNumber() + 1,
				newProject.getDeployedVersion().getVersionNumber());
		assertEquals(projectXMLAtTheTimeOfBackup, newProject.getProject()
				.getProjectXmlDefinition());

		assertEquals(userProject.getId(), newProject.getId());
		assertEquals(userProject.getUniqueRandomId(), newProject
				.getUniqueRandomId());
		assertEquals(userProject.getProject().getPropertiesAsString(),
				newProject.getProject().getPropertiesAsString());

		verifyRestoredData(newProject);

		verifyLinks(regularLink, privateLink, newProject);
	}

	public void testRestoreOfExistingProjectUnderDifferentName()
			throws RobotException, IOException {
		// --- Add some data to the project.
		recordData();

		// --- Add a couple of links
		LinkToUserProject regularLink = LinkToUserProject
				.createUnauthenticatedLink(userProject);
		world.domain().projects().addLinkToProject(regularLink);

		LinkToUserProject privateLink = new LinkToUserProject(userProject,
				"authentication token");
		world.domain().projects().addLinkToProject(privateLink);

		// --- Create the backup
		navigateToBackupPage();

		ByteArrayOutputStream data = captureTheBackupData();

		String projectXMLAtTheTimeOfBackup = userProject.getProject()
				.getProjectXmlDefinition();

		navigateToFirstRestorePage(projectOwner);

		// --- File submission step
		WebForm form = bot.getForm(FORM_NAME);

		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\test-backup.zip", new ByteArrayInputStream(data
						.toByteArray()), "application/zip") });

		bot.submit(form, BackupRestoreController.PARAM_TARGET + "1");

		// --- Import approval step
		form = bot.getForm(FORM_NAME);
		form.setCheckbox("restoreAsNewProject", true);
		String newProjectName = "New Project Name";
		form.setParameter("projectName", newProjectName);
		bot.submit(form, BackupRestoreController.PARAM_FINISH);

		// --- Confirmation screen
		assertContains("Project " + newProjectName + " has been restored.", bot
				.lastResponse().getText());

		UserProject newProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), newProjectName);
		assertNotNull(newProject);

		// --- Validate data
		assertEquals(1, newProject.getDeployedVersion().getVersionNumber());
		assertEquals(projectXMLAtTheTimeOfBackup, newProject.getProject()
				.getProjectXmlDefinition());

		assertNotEquals(userProject.getId(), newProject.getId());
		assertNotEquals(userProject.getUniqueRandomId(), newProject
				.getUniqueRandomId());
		assertNotEquals(userProject.getProject().getPropertiesAsString(),
				newProject.getProject().getPropertiesAsString());

		verifyRestoredData(newProject);
	}

	public void testRestoreOfLegacyProjectsWithEmptyProperties()
			throws RobotException, IOException {
		// --- Set the project properties to be null;
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				Project project = (Project) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(Project.class,
								userProject.getProject().getId());
				project.setPropertiesAsString(null);
				return null;
			}
		});

		// --- Create the backup
		navigateToBackupPage();

		ByteArrayOutputStream data = captureTheBackupData();

		// --- Delete the project
		world.domain().projects().delete(userProject, world);

		// --- Start restoring...
		navigateToFirstRestorePage(projectOwner);

		// --- File submission step
		WebForm form = bot.getForm(FORM_NAME);

		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\test-backup.zip", new ByteArrayInputStream(data
						.toByteArray()), "application/zip") });

		bot.submit(form, BackupRestoreController.PARAM_TARGET + "1");

		// --- Import approval step
		form = bot.getForm(FORM_NAME);
		bot.submit(form, BackupRestoreController.PARAM_FINISH);

		// --- Confirmation screen
		assertContains("Project " + userProject.getName()
				+ " has been restored.", bot.lastResponse().getText());

		UserProject newProject = world.domain().projects().getWithVersions(
				projectOwner.getId(), userProject.getName());
		assertNotNull(newProject);

		assertNull(newProject.getProject().getPropertiesAsString());
	}

	public void testRestoreOfProjectByADifferentUserUnderADifferentName()
			throws RobotException, IOException {
		// --- Add some data to the project.
		recordData();

		// --- Add a couple of links
		LinkToUserProject regularLink = LinkToUserProject
				.createUnauthenticatedLink(userProject);
		world.domain().projects().addLinkToProject(regularLink);

		LinkToUserProject privateLink = new LinkToUserProject(userProject,
				"authentication token");
		world.domain().projects().addLinkToProject(privateLink);

		// --- Create the backup
		navigateToBackupPage();

		ByteArrayOutputStream data = captureTheBackupData();

		String projectXMLAtTheTimeOfBackup = userProject.getProject()
				.getProjectXmlDefinition();

		User anotherUser = UserTest.aUser("testUser2");
		world.domain().users().addOrSave(anotherUser);

		addUserNameToDelete(anotherUser.getId());

		navigateToFirstRestorePage(anotherUser);

		// --- File submission step
		WebForm form = bot.getForm(FORM_NAME);

		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\test-backup.zip", new ByteArrayInputStream(data
						.toByteArray()), "application/zip") });

		bot.submit(form, BackupRestoreController.PARAM_TARGET + "1");

		// --- Import approval step
		form = bot.getForm(FORM_NAME);
		form.setCheckbox("restoreAsNewProject", true);
		String newProjectName = "New Project Name";
		form.setParameter("projectName", newProjectName);
		bot.submit(form, BackupRestoreController.PARAM_FINISH);

		// --- Confirmation screen
		assertContains("Project " + newProjectName + " has been restored.", bot
				.lastResponse().getText());

		UserProject newProject = world.domain().projects().getWithVersions(
				anotherUser.getId(), newProjectName);
		assertNotNull(newProject);

		// --- Validate data
		assertEquals(1, newProject.getDeployedVersion().getVersionNumber());
		assertEquals(projectXMLAtTheTimeOfBackup, newProject.getProject()
				.getProjectXmlDefinition());

		assertNotEquals(userProject.getId(), newProject.getId());
		assertNotEquals(userProject.getUniqueRandomId(), newProject
				.getUniqueRandomId());
		assertNotEquals(userProject.getProject().getPropertiesAsString(),
				newProject.getProject().getPropertiesAsString());

		verifyRestoredData(newProject);
	}

	public void testRestoreOfProjectByADifferentUserUnderTheSameName()
			throws RobotException, IOException {
		// --- Add some data to the project.
		recordData();

		// --- Add a couple of links
		LinkToUserProject regularLink = LinkToUserProject
				.createUnauthenticatedLink(userProject);
		world.domain().projects().addLinkToProject(regularLink);

		LinkToUserProject privateLink = new LinkToUserProject(userProject,
				"authentication token");
		world.domain().projects().addLinkToProject(privateLink);

		// --- Create the backup
		navigateToBackupPage();

		ByteArrayOutputStream data = captureTheBackupData();

		String projectXMLAtTheTimeOfBackup = userProject.getProject()
				.getProjectXmlDefinition();

		User anotherUser = UserTest.aUser("testUser2");
		world.domain().users().addOrSave(anotherUser);

		addUserNameToDelete(anotherUser.getId());

		navigateToFirstRestorePage(anotherUser);

		// --- File submission step
		WebForm form = bot.getForm(FORM_NAME);

		form.setParameter("data", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\test-backup.zip", new ByteArrayInputStream(data
						.toByteArray()), "application/zip") });

		bot.submit(form, BackupRestoreController.PARAM_TARGET + "1");

		// --- Import approval step
		form = bot.getForm(FORM_NAME);
		form.setCheckbox("restoreAsNewProject", false);

		// --- Import approval step
		form = bot.getForm(FORM_NAME);
		form.setCheckbox("confirmRestore", true);
		bot.submit(form, BackupRestoreController.PARAM_FINISH);

		// --- Confirmation screen
		assertContains("Project " + userProject.getName()
				+ " has been restored.", bot.lastResponse().getText());

		UserProject newProject = world.domain().projects().getWithVersions(
				anotherUser.getId(), userProject.getName());
		assertNotNull(newProject);

		// --- Validate data
		assertEquals(1, newProject.getDeployedVersion().getVersionNumber());
		assertEquals(projectXMLAtTheTimeOfBackup, newProject.getProject()
				.getProjectXmlDefinition());

		assertNotEquals(userProject.getId(), newProject.getId());
		assertNotEquals(userProject.getUniqueRandomId(), newProject
				.getUniqueRandomId());
		assertNotEquals(userProject.getProject().getPropertiesAsString(),
				newProject.getProject().getPropertiesAsString());

		verifyRestoredData(newProject);
	}

	public void testSchedulingOfOnlineBackup() throws RobotException {
		// --- Test that available to admins only
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");
		assertDoesntContain("No backups are currently scheduled to run", bot
				.getPageText());

		// --- Make the user admin
		projectOwner.setAdministrator(true);
		world.domain().users().addOrSave(projectOwner);

		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		// --- Test schedule creation
		assertContains("No backups are currently scheduled to run", bot
				.getPageText());
		WebForm form = bot.getForm("setupDailyBackupScheduleForm");
		assertNotNull(form);

		form.setParameter(SetupDailyBackupController.HOUR_PARAMETER, "3");
		bot.submit(form);

		assertMatches(
				"Next backup will run on [A-za-z]+ \\d?\\d around 3:00AM", bot
						.getPageText());

		// --- Test schedule update
		form = bot.getForm("updateDailyBackupScheduleForm");
		form.setParameter(SetupDailyBackupController.HOUR_PARAMETER, "16");
		bot.submit(form);

		assertMatches(
				"Next backup will run on [A-za-z]+ \\d?\\d around 4:00PM", bot
						.getPageText());

		// --- Test schedule deletion
		form = bot.getForm("updateDailyBackupScheduleForm");
		bot.submit(form, SetupDailyBackupController.STOP_PARAMETER);
		assertContains("No backups are currently scheduled to run", bot
				.getPageText());
	}

	public void testDeletingOnlineBackup() throws RobotException {
		// --- Make the user admin
		projectOwner.setAdministrator(true);
		world.domain().users().addOrSave(projectOwner);

		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		// --- Test schedule creation
		WebForm form = bot.getForm("setupDailyBackupScheduleForm");
		form.setParameter(SetupDailyBackupController.HOUR_PARAMETER, "3");
		bot.submit(form);

		simulateBatchRun();

		// --- Refresh project details page.
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");
		assertContains("Available backups:", bot.getPageText());

		// --- Test Deletion
		form = bot.getForm("deleteOnlineBackup1");
		bot.submit(form);

		assertContains(
				"There are no online backups available to restore for this project.",
				bot.getPageText());
	}

	public void testRestoringOnlineBackupIntoTheSameProject()
			throws RobotException, SAXException {
		// --- Add some data to the project.
		recordData();

		// --- Make the user admin
		projectOwner.setAdministrator(true);
		world.domain().users().addOrSave(projectOwner);

		// --- Create a schedule
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		WebForm form = bot.getForm("setupDailyBackupScheduleForm");
		form.setParameter(SetupDailyBackupController.HOUR_PARAMETER, "3");
		bot.submit(form);

		simulateBatchRun();

		// --- Purge data
		world.domain().storedData().purgeProjectResponses(
				userProject.getProject());
		assertEquals(0, world.domain().storedData().responseCount(
				userProject.getProject()));

		// --- Refresh project details page.
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");
		assertContains("Available backups:", bot.getPageText());

		HTMLElement button = bot.lastResponse().getElementWithID(
				"restoreOnlineBackup1");
		String backupId = button.getAttribute("value");

		// --- Extract the link to the description page.
		// --- It's stored in a JavaScript variable.
		Pattern pattern = Pattern.compile(
				"var linkToRestoreOnlineBackupURL = '([^']*)'",
				Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());

		String link = matcher.group(1);
		String urlToRestore = link + backupId;
		bot.go(urlToRestore);

		form = bot.getForm("restoreForm");
		assertNotNull(form);

		form.setCheckbox("confirmRestore", true);
		bot.submit(form, AbstractWizardFormController.PARAM_FINISH);

		assertContains(" has been restored.", bot.getPageText());

		verifyRestoredData(userProject);
	}

	public void testRestoringOnlineBackupIntoANewProject()
			throws RobotException, SAXException {
		// --- Add some data to the project.
		recordData();

		// --- Make the user admin
		projectOwner.setAdministrator(true);
		world.domain().users().addOrSave(projectOwner);

		// --- Create a schedule
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		WebForm form = bot.getForm("setupDailyBackupScheduleForm");
		form.setParameter(SetupDailyBackupController.HOUR_PARAMETER, "3");
		bot.submit(form);

		simulateBatchRun();

		// --- Refresh project details page.
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");
		assertContains("Available backups:", bot.getPageText());

		HTMLElement button = bot.lastResponse().getElementWithID(
				"restoreOnlineBackup1");
		String backupId = button.getAttribute("value");

		// --- Extract the link to the description page.
		// --- It's stored in a JavaScript variable.
		Pattern pattern = Pattern.compile(
				"var linkToRestoreOnlineBackupURL = '([^']*)'",
				Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());

		String link = matcher.group(1);
		String urlToRestore = link + backupId;
		bot.go(urlToRestore);

		form = bot.getForm("restoreForm");
		assertNotNull(form);

		form.setCheckbox("restoreAsNewProject", true);
		form.setParameter("projectName", "Another Project");
		bot.submit(form, AbstractWizardFormController.PARAM_FINISH);

		assertContains(" has been restored.", bot.getPageText());

		UserProject newProject = world.domain().projects().get(
				projectOwner.getId(), "Another Project");
		assertNotNull(newProject);

		verifyRestoredData(newProject);
	}

	private void simulateBatchRun() {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {

			public Object doInTransaction(TransactionStatus status) {
				List<BackupSchedule> schedules = ProjectsHibernateImpl
						.getBackupSchedules(userProject);
				assertEquals(1, schedules.size());

				ScheduledBackup.createBackup(schedules.get(0));
				return null;
			}
		});
	}

	private void verifyLinks(LinkToUserProject regularLink,
			LinkToUserProject privateLink, UserProject newProject) {
		String previousURL = userProject.getUrlToForm(
				EntryPointType.REAL_PROJECT, userProject.getProject().getForm(
						FIRST_FORM_NAME));
		String newURL = newProject.getUrlToForm(EntryPointType.REAL_PROJECT,
				userProject.getProject().getForm(FIRST_FORM_NAME));
		assertEquals(previousURL, newURL);

		// --- Original link
		LinkToUserProject newLink = world.domain().projects()
				.getWithProjectRuntime(userProject.getUniqueRandomId());
		assertNotNull(newLink);
		assertFalse(newLink.isAuthenticated());
		assertNull(newLink.getAuthenticationToken());

		// --- Added links
		newLink = world.domain().projects().getWithProjectRuntime(
				regularLink.getId());
		assertNotNull(newLink);
		assertFalse(newLink.isAuthenticated());
		assertNull(newLink.getAuthenticationToken());

		newLink = world.domain().projects().getWithProjectRuntime(
				privateLink.getId());
		assertNotNull(newLink);
		assertTrue(newLink.isAuthenticated());
		assertEquals("authentication token", newLink.getAuthenticationToken());
	}

	private void verifyRestoredData(UserProject newProject) {
		List<FormSubmission> submissions = world.domain().storedData()
				.responsesFor(newProject.getProject(), FIRST_FORM_NAME);
		assertEquals(2, submissions.size());
		FormSubmission submission = submissions.get(0);
		assertEquals("1", submission.getValue(new Reference(FIRST_FIELD_NAME))
				.toString());
		assertEquals("2", submission.getValue(new Reference(SECOND_FIELD_NAME))
				.toString());
		assertEquals("3", submission.getValue(new Reference(THIRD_FIELD_NAME))
				.toString());
		List<Value> mcqValues = submission.getValues(new Reference(
				MCQ_FIELD_NAME));
		assertEquals(1, mcqValues.size());
		assertEquals("c", mcqValues.get(0).toString());

		submission = submissions.get(1);
		assertEquals("11", submission.getValue(new Reference(FIRST_FIELD_NAME))
				.toString());
		assertEquals("22", submission
				.getValue(new Reference(SECOND_FIELD_NAME)).toString());
		assertEquals("33", submission.getValue(new Reference(THIRD_FIELD_NAME))
				.toString());
		List<Value> values = submission
				.getValues(new Reference(MCQ_FIELD_NAME));

		assertEquals(3, values.size());
		assertEquals("a", values.get(0).toString());
		assertEquals("b", values.get(1).toString());
		assertEquals("c", values.get(2).toString());

		submissions = world.domain().storedData().responsesFor(
				newProject.getProject(), SECOND_FORM_NAME);
		assertEquals(1, submissions.size());
		submission = submissions.get(0);
		assertEquals("4", submission.getValue(new Reference(FORTH_FIELD_NAME))
				.toString());
	}

	private void recordData() throws RobotException {
		bot.go(userProject, userProject.getProject().getForm(FIRST_FORM_NAME));
		WebForm form = bot.getForm(0);
		form.setParameter(FIRST_FIELD_NAME, "1");
		form.setParameter(SECOND_FIELD_NAME, "2");
		form.setParameter(THIRD_FIELD_NAME, "3");
		form.setParameter(MCQ_FIELD_NAME, new String[] { "c" });
		bot.submit(form);

		bot.go(userProject, userProject.getProject().getForm(FIRST_FORM_NAME));
		form = bot.getForm(0);
		form.setParameter(FIRST_FIELD_NAME, "11");
		form.setParameter(SECOND_FIELD_NAME, "22");
		form.setParameter(THIRD_FIELD_NAME, "33");
		form.setParameter(MCQ_FIELD_NAME, new String[] { "a", "b", "c" });
		bot.submit(form);

		bot.go(userProject, userProject.getProject().getForm(SECOND_FORM_NAME));
		form = bot.getForm(0);
		form.setParameter(FORTH_FIELD_NAME, "4");
		bot.submit(form);
	}
}
