package com.tawala.project.commands;

import java.util.List;

import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Document;
import com.tawala.project.Form;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProcessBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.oldhtml.Html;

public class ShowTest extends TestCase {
    private User projectOwner = UserTest.aUser("tester");

    public void testExecution() {
        Project project = ProjectBuilder.buildMinimalisticProject();
        Document doc = new Document("docname", "doccontent");
        project.add(doc);
        Form form = new Form("form");
        project.add(form);
        ShowDocument show = new ShowDocument("docname");
        ExecutionResult result = show.execute(new FakeExecutionContext(
                new UserProject(project, projectOwner, "bar"), form));
        assertContains("doccontent", result.getHtml().toString());
    }

    public void testExecutionWithIntermediateVariableChange() {
    	ProjectBuilder projectBuilder = new ProjectBuilder();
    	
    	DocumentBuilder documentBuilder = projectBuilder.addDocument("showname");
    	documentBuilder.addField("name");
    	
    	ProcessBlockBuilder processBlockBuilder = projectBuilder.addProcess("multishow");
    	processBlockBuilder.addSet("name", ProcessBuilder.OperandType.VALUE,  "foo");
    	processBlockBuilder.addShow(documentBuilder);
    	processBlockBuilder.addSet("name", ProcessBuilder.OperandType.VALUE,  "bar");
    	processBlockBuilder.addShow(documentBuilder);
    	
    	projectBuilder.addForm("form", processBlockBuilder);
    	
        Project project = projectBuilder.build();

        ExecutionResult result = project.getProcess(processBlockBuilder.getName()).execute(new FakeExecutionContext(
                new UserProject(project, projectOwner, "complicated"), project.getForm("form")));
        List<Html> html = result.getHtml();
        assertMatches("<div class=\"document\">.*foo.*</div>" + NEWLINE, html.get(0)
                .toString());
        assertMatches("<div class=\"document\">.*bar.*</div>" + NEWLINE, html.get(1)
                .toString());
        assertEquals(2, html.size());
    }
}
