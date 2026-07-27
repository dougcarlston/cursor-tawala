package com.tawala.acceptance;

import java.util.List;

import com.scissor.webrobot.RobotException;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.Reference;

public class PostProcessTest extends AcceptanceTestCase {
	private UserProject userProject;
	
	@Override
	protected void setUp() throws Exception {
		super.setUp();
        ProjectBuilder builder = new ProjectBuilder();

        FormBuilder form = builder.addForm("Form 1");
        form.addText("First page");
        form.addFib("Question:", "question", 10);

        ProcessBlockBuilder process = builder.addProcess("Process 1");
        process.addShow(form);

        form.setPostProcess(process);

        Project project = builder.build();

        userProject = new UserProject(project, projectOwner, "test");
        world.domain().projects().put(userProject);
	}

    public void testSavingPost() throws RobotException {
    	//--- First page
        bot.go(userProject);
        bot.setParameter("question", "123");
        //--- Second page
        bot.submit();

        List<FormSubmission> submissions = world.domain().storedData().responsesFor(userProject.getProject(), "Form 1");
        assertEquals(1, submissions.size());
        
        FormSubmission firstOne = submissions.get(0);
        assertEquals("123", firstOne.getValue(new Reference("question")).toString());
    }

}
