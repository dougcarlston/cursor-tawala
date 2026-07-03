package com.tawala.acceptance.project;

import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.ForEachBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.ProcessBuilder.OperandType;

public class SharedDataTest extends AcceptanceTestCase {
	private UserProject dataSourceDefiningProject;
	
	@Override
	protected void setUp() throws Exception {
		super.setUp();
		
        ProjectBuilder builder = new ProjectBuilder();
        FormBuilder mainForm = builder.addForm("main", true);
        mainForm.setExternalDataSource("User");
        mainForm.addFib("User's id:", "email", 25);
        
        dataSourceDefiningProject = new UserProject(builder.build(), projectOwner, "DataSource Defining Project");
        world.domain().projects().put(dataSourceDefiningProject);
	}
	
    public void testViewingSharedDataFromGet() throws Exception {
    	ProjectBuilder builder = new ProjectBuilder();
    	FormBuilder formBuilder = builder.addForm("main");
        
        DocumentBuilder displayDocument = builder.addDocument("shared data viewer");
        displayDocument.addText("Email:");
        displayDocument.addField("Record:User:email");
        
        ProcessBlockBuilder processBuilder = builder.addProcess("main pre process");
        processBuilder.addGet("RecordList", new Object[] {"User", true});
        ForEachBuilder forEachBuilder = processBuilder.addForEach("Record", "RecordList");
        forEachBuilder.addShow(displayDocument);
        
        formBuilder.setPreProcess(processBuilder);
        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner, "Shared Data Viewer");
        
        userProject = world.domain().projects().put(userProject);
     
        //--- Collect some data.
        bot.go(dataSourceDefiningProject);
        bot.setParameter("email", "joe@test.com");
        bot.submit();

        bot.go(dataSourceDefiningProject);
        bot.setParameter("email", "jim@test.com");
        bot.submit();

        //--- Verify the data is displayed.
        bot.go(userProject);
        assertContains("joe@test.com", bot.getPageText());
        assertContains("jim@test.com", bot.getPageText());
    }

    public void testUpdatingSharedDataWithinForEach() throws Exception {
    	ProjectBuilder builder = new ProjectBuilder();
    	FormBuilder formBuilder = builder.addForm("main");
    	formBuilder.addFib("Updated email:", "newEmail", 20);
        
        DocumentBuilder displayDocument = builder.addDocument("shared data viewer and updater");
        displayDocument.addText("Email:");
        displayDocument.addField("Record:User:email");

        //--- PreProcess
        ProcessBlockBuilder processBuilder = builder.addProcess("main pre process");
        processBuilder.addGet("RecordList", new Object[] {"User", true});
        ForEachBuilder forEachBuilder = processBuilder.addForEach("Record", "RecordList");
        forEachBuilder.addShow(displayDocument);
        formBuilder.setPreProcess(processBuilder);

        //--- PostProcess
        processBuilder = builder.addProcess("main post process");
        processBuilder.addGet("RecordList", new Object[] {"User", true});
        forEachBuilder = processBuilder.addForEach("Record", "RecordList");
        forEachBuilder.addSet("Record:User:email", OperandType.FIELD, "main:newEmail");
        formBuilder.setPostProcess(processBuilder);

        Project project = builder.build();
        UserProject userProject = new UserProject(project, projectOwner, "Shared Data Viewer and Updater");
        
        userProject = world.domain().projects().put(userProject);
     
        //--- Collect some data.
        bot.go(dataSourceDefiningProject);
        bot.setParameter("email", "joe@test.com");
        bot.submit();

        bot.go(dataSourceDefiningProject);
        bot.setParameter("email", "jim@test.com");
        bot.submit();

        //--- Verify the data is displayed.
        bot.go(userProject);
        assertContains("joe@test.com", bot.getPageText());
        assertContains("jim@test.com", bot.getPageText());
        
        //--- Set new value.
        bot.setParameter("newEmail", "jeff@test.com");
        bot.submit();
        
        //--- Verify the data is updated.
        bot.go(userProject);
        assertDoesntContain("joe@test.com", bot.getPageText());
        assertDoesntContain("jim@test.com", bot.getPageText());
        assertContains("jeff@test.com", bot.getPageText());
    }

}
