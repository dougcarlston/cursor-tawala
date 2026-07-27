package com.tawala.acceptance.customization;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;

public class FormDataUpdateTest extends AcceptanceTestCase {

    private ProjectBuilder builder;
    private FormBuilder customizeForm;
    private FormBuilder nonCustomizeForm;
    private FormBuilder dataEntryOnlyForm;

	private UserProject createProject() {
		builder = new ProjectBuilder();

        customizeForm = builder.addForm("Customize");
        customizeForm.addMc("MC Item 1", "Choice A", "Choice B", "Choice C");
        customizeForm.addFib("FIB Item 1", 10);

        nonCustomizeForm = builder.addForm("Form 2");
        nonCustomizeForm.addMc("MC Item 1", "Choice A", "Choice B", "Choice C");
        nonCustomizeForm.addFib("FIB Item 1", 10);

        dataEntryOnlyForm = builder.addForm("Form 3", false, true);
        dataEntryOnlyForm.addMc("MC Item 1", "Choice A", "Choice B", "Choice C");
        dataEntryOnlyForm.addFib("FIB Item 1", 10);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner, "testBlankEntryUpdate");
        world.domain().projects().put(userProject);
        
        return userProject;
	}

/*
    public void testSelectionRetainedForCustomizeForm() throws RobotException {
        
    	UserProject userProject = createProject();

        bot.go(userProject, customizeForm.getName());
        bot.setParameter("Q1", "b");
        bot.submit();

        bot.back();
        bot.clearResponses();
        bot.go(userProject, customizeForm.getName());
        
        assertEquals("b", bot.getParameter("Q1"));
    }
*/
    public void testSelectionNotRetainedForNonCustomizeForm() throws RobotException {

    	UserProject userProject = createProject();

        bot.go(userProject, nonCustomizeForm.getName());
        bot.setParameter("Q1", "b");
        bot.submit();

        bot.back();
        bot.clearResponses();
        bot.go(userProject, nonCustomizeForm.getName());
        
        assertNotEquals("b", bot.getParameter("Q1"));
        assertNull(bot.getParameter("Q1"));
    }
    
/*    
    public void testEntryRetainedForCustomizeForm() throws RobotException {
        
    	UserProject userProject = createProject();

        bot.go(userProject, customizeForm.getName());
        bot.setParameter("Q2:a", "Blank One");
        bot.submit();

        bot.back();
        bot.clearResponses();
        bot.go(userProject, customizeForm.getName());
        
        assertEquals("Blank One", bot.getParameter("Q2:a"));
    }
*/
    public void testEntryNotRetainedForNonCustomizeForm() throws RobotException {

    	UserProject userProject = createProject();

        bot.go(userProject, nonCustomizeForm.getName());
        bot.setParameter("Q2:a", "Blank One");
        bot.submit();

        bot.back();
        bot.clearResponses();
        bot.go(userProject, customizeForm.getName());
        
        assertNotEquals("Blank One", bot.getParameter("Q2:a"));
        assertEquals("", bot.getParameter("Q2:a"));
    }
    
    public void testEntryRetainedForDataEntryOnlyForm() throws RobotException {
        
    	UserProject userProject = createProject();

        bot.go(userProject, dataEntryOnlyForm.getName());
        bot.setParameter("Q2:a", "Blank One");
        bot.submit();

        bot.back();
        bot.clearResponses();
        bot.go(userProject, dataEntryOnlyForm.getName());
        
        assertEquals("Blank One", bot.getParameter("Q2:a"));
    }

}
