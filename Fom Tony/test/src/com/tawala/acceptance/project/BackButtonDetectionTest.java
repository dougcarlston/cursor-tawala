package com.tawala.acceptance.project;

import java.util.List;

import com.meterware.httpunit.WebForm;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.FormSubmission;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.ExecutionContext;

public class BackButtonDetectionTest extends AcceptanceTestCase {
	private UserProject userProject;
	private boolean originalDetectBackButtonValue;
	
	@Override
	protected void setUp() throws Exception {
		super.setUp();
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm("main");
		formBuilder.addFib("Your email address:", "Email", "Email", 10);
		formBuilder.addFib("Your address", "Address" , "Address1", 20);
		
		this.userProject = world.domain().projects().put(new UserProject(builder.build(),
				projectOwner, "test"));
		
		originalDetectBackButtonValue = ExecutionContext.DETECT_BACK_BUTTON_NAVIGATION;
	}
	
	@Override
	protected void tearDown() throws Exception {
		ExecutionContext.DETECT_BACK_BUTTON_NAVIGATION = originalDetectBackButtonValue;
		super.tearDown();
	}

	public void testBackButtonNavigationHandling() throws RobotException {
		useBackButton();
		
		assertContains("you used your browser's BACK button", bot.getPageText());

		List<FormSubmission> submissions = world.domain().storedData().responsesFor(userProject.getProject(), "main");
		assertEquals(1, submissions.size());
	}

	public void testOverridingBackButtonNavigationHandling() throws RobotException {
		ExecutionContext.DETECT_BACK_BUTTON_NAVIGATION = false;
		
		useBackButton();
		
		assertContains("Thank you!", bot.getPageText());

		List<FormSubmission> submissions = world.domain().storedData().responsesFor(userProject.getProject(), "main");
		assertEquals(2, submissions.size());
	}

	private void useBackButton() throws RobotException {
		bot.go(userProject, "main");
		
		WebForm originalForm = bot.getForm(0);
		originalForm.setParameter("Email", "test@example.com");
		originalForm.setParameter("Address1", "123 Main St.");
		bot.submit(originalForm);
		
		//--- Attempt to navigate back and resubmit the original form.
		originalForm.setParameter("Email", "another@example.com");
		bot.submit(originalForm);
	}
}
