package com.tawala.acceptance.projectmanager;

import java.io.IOException;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.web.controller.WellKnown;

public class ThemeBuilderTest extends AcceptanceTestCase {
	public void testMainPage() throws RobotException, IOException {
		projectOwner.setAdministrator(true);
		world.domain().users().addOrSave(projectOwner);
		
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("createUserTheme");

		assertContains(
				"<iframe frameborder=\"0\" height=\"600px\" id=\"sampleproject\" name=\"sampleproject\" src=\""
						+ WellKnown.urls.getViewSampleProject()
						+ "\" width=\"100%\" onload=\"Tawala.Theme.applyCustomStyleToPreviewFrame();\">></iframe>",
				bot.getPageText());

		// --- TODO: test save
	}

	public void testPrevewPane() throws RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getViewSampleProject());
		assertDoesntContain("We are very sorry!", bot.getPageText());
		assertContains("Main Heading", bot.getPageText());
	}
}
