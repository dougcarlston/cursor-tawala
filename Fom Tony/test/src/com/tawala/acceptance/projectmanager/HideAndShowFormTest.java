package com.tawala.acceptance.projectmanager;

import java.util.Set;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.xml.sax.SAXException;

import com.meterware.httpunit.HTMLElement;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.controller.WellKnown;

public class HideAndShowFormTest extends AcceptanceTestCase {
	public static final String FIRST_FORM_NAME = "form1";
	public static final String SECOND_FORM_NAME = "form2";

	protected UserProject userProject;
	
	@Override
	protected void setUp() throws Exception {
		super.setUp();
		
		ProjectBuilder builder = new ProjectBuilder();
		builder.addForm(FIRST_FORM_NAME);
		builder.addForm(SECOND_FORM_NAME);
		userProject = new UserProject(builder.build(), projectOwner, "test");
		
		userProject = world.domain().projects().put(userProject);
		bot.logInAs(projectOwner);

		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");
	}

	public void testHideAndShowAll() throws RobotException, SAXException, InterruptedException {
			//--- Hide the first form
		HTMLElement linkToHide = bot.lastResponse().getElementWithID("hideFormCheckbox1");
		String onChangeHandler = linkToHide.getAttribute("onchange");
		assertNotNull(onChangeHandler);
		
		String onChangeHandlerPattern = "Tawala.ProjectManager.saveFormSelection\\('([^']+)' \\+ this.checked\\)";
		Pattern pattern = Pattern.compile(onChangeHandlerPattern);
		Matcher matcher = pattern.matcher(onChangeHandler);
		assertTrue(matcher.find());

		String saveFormVisibilityURL = matcher.group(1);
		bot.go(saveFormVisibilityURL + "true");
		
		UserProject updatedProject = world.domain().projects().getWithProjectRuntime(projectOwner.getId(), userProject.getName());
		Set<String> hiddenForms = updatedProject.getProject().getFormNamesSelectedInProjectManager();
		assertEquals(1, hiddenForms.size());
		
		assertTrue(hiddenForms.contains(FIRST_FORM_NAME));

		//--- Hide the second form
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("projectDetailsLink1");

		linkToHide = bot.lastResponse().getElementWithID("hideFormCheckbox2");
		onChangeHandler = linkToHide.getAttribute("onchange");
		assertNotNull(onChangeHandler);
		
		pattern = Pattern.compile(onChangeHandlerPattern);
		matcher = pattern.matcher(onChangeHandler);
		assertTrue(matcher.find());

		saveFormVisibilityURL = matcher.group(1);
		bot.go(saveFormVisibilityURL + "true");
		
		updatedProject = world.domain().projects().getWithProjectRuntime(projectOwner.getId(), userProject.getName());
		hiddenForms = updatedProject.getProject().getFormNamesSelectedInProjectManager();
		assertEquals(2, hiddenForms.size());
		
		assertTrue(hiddenForms.contains(FIRST_FORM_NAME));
		assertTrue(hiddenForms.contains(SECOND_FORM_NAME));
	}
	// TODO: test persistence of the hide/show button.
}
