package com.tawala.acceptance.report;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.web.controller.WellKnown;

public class AllUserProjectReportTest extends AcceptanceTestCase {
	
	public void testUnauthorizedAccess() throws RobotException {
		bot.go(WellKnown.urls.getReportAllUserProjects());
        assertTrue(bot.getPath().localPart().startsWith(WellKnown.urls.getLogin()));

		//--- Project owner is not an admin.
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getReportAllUserProjects());
        assertEquals(WellKnown.urls.getSportsHome(), bot.getPath().localPart());
	}
	
	public void testReport() throws RobotException {
		projectOwner.setAdministrator(true);
		world.domain().users().addOrSave(projectOwner);
		
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getAdminManageUsers());
		bot.go(WellKnown.urls.getUserProjectReport());		
		bot.followLink("allUserProjectsReportLink");
		
		assertContains("application/vnd.ms-excel", bot.getContentType());
	}
}
