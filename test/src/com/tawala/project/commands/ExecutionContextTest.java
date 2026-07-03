package com.tawala.project.commands;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.ProjectTest;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.WorldInitializer;

public class ExecutionContextTest extends com.tawala.TestCase {
    private User projectOwner = UserTest.aUser("tester");

    public ExecutionContextTest() {
        addUserNameToDelete(projectOwner.getId());
    }
    
    @Override
    protected void setUp() throws Exception {
        super.setUp();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
        WorldInitializer.getDefaultWorld().domain().users().addOrSave(projectOwner);
    }

    public void testGetQualifier() {

        ExecutionContext context = new FakeExecutionContext(ProjectTest
                .projectHelloBob());
        context.mapRecord("Record1", new CompositeFormSubmission("RecordList"));

        assertEquals("Record1", context.getQualifier("Record1:Q1:a"));
        assertEquals("Record1", context.getQualifier("Record1:Field"));
        assertEquals("Q1", context.getQualifier("Q1:a"));
        assertEquals("", context.getQualifier("Score"));
    }
    
    public void testVerifyBackwardCompatibilityOfSetVariable() {
    	ProjectBuilder builder = new ProjectBuilder();
    	builder.addForm("main");
    	builder.setFormat("1.3");
    	
    	Project project = builder.build();
		ExecutionContext context = new FakeExecutionContext(project);
    	context.setValue("var", "123");
    	assertEquals("123", context.getVariables().getValue(new Reference("var")).toString());
    	assertEquals("123", context.getSubmission().getValue(new Reference("var")).toString());
    	
    	builder.setFormat("1.6");
    	project = builder.build();
		context = new FakeExecutionContext(project);
    	context.setValue("var", "123");
    	assertEquals("123", context.getVariables().getValue(new Reference("var")).toString());
    	assertEquals("", context.getSubmission().getValue(new Reference("var")).toString());
    }
    
    public void testVerifyCorrectSubmissionRetrieval() {
    	ProjectBuilder builder = new ProjectBuilder();
    	FormBuilder form1 = builder.addForm("form1");
    	String fieldName = "question1";
		form1.addFib("First form question:", "Question1", fieldName, 20);
    	
    	FormBuilder form2 = builder.addForm("form2");
    	form2.addFib("Second form question:", "Question1", fieldName, 20);
    	
    	Project project = builder.build();
    	
    	UserProject userProject = new UserProject(project, projectOwner, "test");
    	
    	userProject = WorldInitializer.getDefaultWorld().domain().projects().put(userProject);
    	
		ExecutionContext context = new FakeExecutionContext(userProject, userProject.getProject().getForm("form1"), fieldName, "123");
		assertEquals("123", context.getValue("form1:" + fieldName).toString());
		assertEquals("", context.getValue("form2:" + fieldName).toString());
    }

}
