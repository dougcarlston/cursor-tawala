package com.tawala.acceptance.projectmanager;

import java.io.IOException;

import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.email.Email;
import com.tawala.email.EmailService;
import com.tawala.email.UniqueBodyEmail;
import com.tawala.email.UserProjectEmail;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.controller.WellKnown;

public class ProjectEmailsTest extends AcceptanceTestCase {
	private UserProject userProject;

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		projectOwner.setAdministrator(true);
		world.domain().users().addOrSave(projectOwner);

		world.domain().users().onUserUpgradeToFullyRegistered(projectOwner);

		userProject = new UserProject(
				ProjectBuilder.buildMinimalisticProject(), projectOwner, "test");
		world.domain().projects().put(userProject);

		UserProjectEmail email = new UserProjectEmail(userProject,
				"from@tawala.com", "to@tawala.com", null, "Test email",
				UniqueBodyEmail.Type.TEXT, "Test body");
		email.setState(Email.State.ERROR);
		email.setCustomerErrorReason("Test failure reason");
		EmailService.saveEmail(email);

	}

	protected void navigateToProjectEmailPage() throws RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");
		bot.followLink("viewAllProjectEmailsLink");
	}

	public void testProjectEmailExport() throws RobotException, IOException {
		navigateToProjectEmailPage();
		bot.followLink("exportEmailsLink");

		assertEquals("application/vnd.ms-excel", bot.getContentType());
		HSSFWorkbook workbook = new HSSFWorkbook(bot.lastResponse()
				.getInputStream());
		assertEquals(1, workbook.getNumberOfSheets());

		HSSFSheet sheet = workbook.getSheet("Emails");
		assertNotNull(sheet);
		assertEquals(3, sheet.getLastRowNum());

		HSSFRow row = sheet.getRow(3);
		assertEquals("from@tawala.com", row.getCell((short) 0)
				.getRichStringCellValue().getString());
		assertEquals("to@tawala.com", row.getCell((short) 1)
				.getRichStringCellValue().getString());
		assertEquals("Test email", row.getCell((short) 3)
				.getRichStringCellValue().getString());
}
}
