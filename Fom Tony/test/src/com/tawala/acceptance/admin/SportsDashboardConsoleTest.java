package com.tawala.acceptance.admin;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.WellKnown;

public class SportsDashboardConsoleTest extends AcceptanceTestCase {
	@Override
	protected void setUp() throws Exception {
		super.setUp();
		projectOwner.setAdministrator(true);
		WorldInitializer.getDefaultWorld().domain().users().addOrSave(projectOwner);
	}
	
	/*
	 * The main purpose of the test is to make sure all the queries ran without errors.
	 */
	public void testOverViewPage() throws RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getAdminSportsDashboardManagementConsole());
		assertContains("weeklyRegistrations", bot.getPageText());
		assertContains("weeklyEmails", bot.getPageText());
	}
}
