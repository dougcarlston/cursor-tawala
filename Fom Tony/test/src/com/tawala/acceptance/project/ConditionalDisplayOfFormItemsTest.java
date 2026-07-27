package com.tawala.acceptance.project;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ConditionsBuilder;
import com.tawala.project.builder.FibBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class ConditionalDisplayOfFormItemsTest extends AcceptanceTestCase {
	private static final String FIB_NAME = "email_fib";
	private UserProject project;
	
	@Override
	protected void setUp() throws Exception {
		super.setUp();

		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder mainFormBuilder = builder.addForm("main");
		mainFormBuilder.addFib("Show FIB:", "ShowFib", "ShowFib", 10);
		mainFormBuilder.addBreak();
		
		FibBuilder fibBuilder = new FibBuilder("Conditional FIB");
		
		fibBuilder.addBlank("a", FIB_NAME, 20, true);
		ConditionsBuilder conditionsBuilder = fibBuilder.getDisplayConditions();
		conditionsBuilder.addComparison("equals", "main:ShowFib", "string", "Yes");
		
		mainFormBuilder.add(fibBuilder);
		project = world.domain().projects().put(new UserProject(builder.build(), projectOwner, "test"));
	}

	public void testDisplayFIBIfConditionFalse() throws RobotException {
		bot.go(project, "main");
		bot.setParameter("ShowFib", "No");
		
		bot.submit();
		
		assertDoesntContain(FIB_NAME, bot.getPageText());
	}

	public void testDisplayFIBIfConditionTrue() throws RobotException {
		bot.go(project, "main");
		bot.setParameter("ShowFib", "yes");
		
		bot.submit();
		
		assertContains(FIB_NAME, bot.getPageText());
	}
}
