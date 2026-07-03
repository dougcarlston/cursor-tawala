package com.tawala.acceptance.component.web.display;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.runtime.PopularChoiceAlgorithmTest;
import com.tawala.project.Project;
import com.tawala.project.UserProject;

public class PopularChoiceTableTest extends AcceptanceTestCase {
	private UserProject userProject;
	@Override
	protected void setUp() throws Exception {
		super.setUp();

		Project project = PopularChoiceAlgorithmTest.buildGetTogetherProject();

		userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);
	}

	public void testDisplayNoChoice() throws RobotException {
		bot.go(userProject, PopularChoiceAlgorithmTest.REPORT_FORM);
		String expectedDocumentOutput = "<div class=\"document\">.*No choices were ranked \"first\".*</div>";
		assertMatches(expectedDocumentOutput, bot.getPageText());
	}

	public void testDisplayNonEmptyTable() throws Exception {
		// Just one record, "a" is the choice
		addData("Joe", new String[] { "a", "b" }, "a");

		bot.go(userProject, PopularChoiceAlgorithmTest.REPORT_FORM);
		String expectedDocumentOutput = "<tr class=\"odd\"><td>Joe</td><td><img src=\"/images/silk/tick.gif\" alt=\"X\" /></td></tr>";

		assertContains(expectedDocumentOutput, bot.getPageText());

		// Another record, now "b" is the choice. Both people reported.
		addData("Jim", new String[] { "b", "c" }, "b");
		bot.go(userProject, PopularChoiceAlgorithmTest.REPORT_FORM);

		expectedDocumentOutput = "<tr class=\"odd\"><td>Joe</td><td>&nbsp;</td></tr>";
		assertContains(expectedDocumentOutput, bot.getPageText());
		expectedDocumentOutput = "<tr class=\"even\"><td>Jim</td><td><img src=\"/images/silk/tick.gif\" alt=\"X\" /></td></tr>";
		assertContains(expectedDocumentOutput, bot.getPageText());

		// Yet another record, "b" is still the choice. New person is not
		// reported.
		addData("Sarah", new String[] { "d" }, "d");
		bot.go(userProject, PopularChoiceAlgorithmTest.REPORT_FORM);
		assertFalse(bot.getPageText().contains("Sarah"));
	}

	private void addData(String name, String[] ableChoices, String preferChoice)
			throws Exception {
		bot.go(userProject, PopularChoiceAlgorithmTest.FORM_NAME);

		bot.setParameter(PopularChoiceAlgorithmTest.NAME_FIELD, name);
		bot.setParameters(PopularChoiceAlgorithmTest.ABLE_TO_ATTEND_FIELD, ableChoices);
		if (preferChoice != null) {
			bot.setParameter(PopularChoiceAlgorithmTest.PREFER_TO_ATTEND_FIELD, preferChoice);
		}

		bot.submit();
	}

}
