package com.tawala.acceptance.project;

import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.project.DataCollectingProjectController;

public class PrivateInvitationTest extends AcceptanceTestCase {
    public void testPrivateInvitation() throws Exception {
        ProjectBuilder builder = new ProjectBuilder();
        
        FormBuilder mainForm = builder.addForm("main", true);
        mainForm.addFib("User's id:", "email", 25);
        
        FormBuilder userEntryPointForm = builder.addForm("userEntry");
        userEntryPointForm.addTextWithFields("Authentication Token: ", "<<" + DataCollectingProjectController.AUTHENTICATION_TOKEN_VARIABLE_NAME + ">>", ".");
        
        DocumentBuilder displayDocument = builder.addDocument("invitation builder");
        displayDocument.addText("This document will display the private invitation link");
        displayDocument.addOldStylePrivateInvitation("Click here...", "", userEntryPointForm.getName(), false, "<string field=\"main:email\" />");
        
        ProcessBlockBuilder processBuilder = builder.addProcess("main post process");
        processBuilder.addShow(displayDocument);
        
        mainForm.setPostProcess(processBuilder);
        
        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner, "Private Invitation Test");
        
        userProject = world.domain().projects().put(userProject);
     
        //--- Test access to userEntryForm without private invitation.
        bot.go(userProject, "userEntry");
        assertContains("Authentication Token: .", bot.getPageText());
        
        //--- Access through the private invitation.
        bot.go(userProject, "main");
        bot.setParameter("email", "test@example.org");
        bot.submit();
        
        assertContains("Click here...", bot.getPageText());
        bot.followLink("Click here...");
        assertContains("Authentication Token: test@example.org.", bot.getPageText());
    }
}
