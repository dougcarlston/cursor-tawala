package com.tawala.acceptance.project;

import java.util.List;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;
import com.tawala.project.commands.Reference;

public class ShowURLTest extends AcceptanceTestCase {
    public void testShowUrlInPreprocess() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        FormBuilder mainForm = builder.addForm("main");
        
        ProcessBlockBuilder processBuilder = builder.addProcess("preprocess");
        processBuilder.addShowURLAsStringValue("http://www.google.com");
        mainForm.setPreProcess(processBuilder);
        Project project = builder.build();
        
        UserProject userProject = new UserProject(project, projectOwner, "test of show url");
        world.domain().projects().put(userProject);

		bot.getClient().getClientProperties().setAutoRedirect(false);
        bot.go(userProject);

        validateCorrectRedirect(userProject);
    }

    public void testShowUrlInPostProcess() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        FormBuilder mainForm = builder.addForm("main");
        
        ProcessBlockBuilder processBuilder = builder.addProcess("preprocess");
        processBuilder.addShowURLAsStringValue("http://www.google.com");
        mainForm.setPostProcess(processBuilder);
        Project project = builder.build();
        
        UserProject userProject = new UserProject(project, projectOwner, "test of show url");
        world.domain().projects().put(userProject);
        
        bot.go(userProject);

        bot.getClient().getClientProperties().setAutoRedirect(false);
        bot.submit();
        
        validateCorrectRedirect(userProject);
    }

    public void testShowUrlInPostProcessAndConfirmDataPersisted() throws RobotException {
        ProjectBuilder builder = new ProjectBuilder();
        FormBuilder mainForm = builder.addForm("main");
        mainForm.addField("id");
        
        ProcessBlockBuilder processBuilder = builder.addProcess("post-process");
        processBuilder.addSet("main:id", OperandType.VALUE, "123");
        processBuilder.addShowURLAsStringValue("http://www.google.com");
        mainForm.setPostProcess(processBuilder);
        Project project = builder.build();
        
        UserProject userProject = new UserProject(project, projectOwner, "test of show url");
        world.domain().projects().put(userProject);
        
        bot.go(userProject);

        bot.getClient().getClientProperties().setAutoRedirect(false);
        bot.submit();
        
        validateCorrectRedirect(userProject);
        
        List<FormSubmission> data = world.domain().storedData().responsesFor(userProject.getProject(), "main");
        assertEquals(1, data.size());
        assertEquals("123", data.get(0).getValue(new Reference("main:id", true)).toString());
    }

	private void validateCorrectRedirect(UserProject userProject) throws RobotException {
        assertEquals(302, bot.lastResponse().getResponseCode());
        assertEquals("http://www.google.com", bot.lastResponse().getHeaderField("Location"));
	}
}
