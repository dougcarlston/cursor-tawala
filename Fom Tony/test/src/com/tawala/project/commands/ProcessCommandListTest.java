package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Document;
import com.tawala.project.Form;
import com.tawala.project.Process;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.oldhtml.Html;

public class ProcessCommandListTest extends TestCase {
    private User projectOwner = UserTest.aUser("tester");

    public void testConstruction() {
        ShowDocument showDoc = (ShowDocument) makeCommand("<show document=\"doc1\"/>");
        assertEquals("doc1", showDoc.getDocumentName());
        
        ShowForm showForm = (ShowForm) makeCommand("<show form=\"form2\"/>");
        assertEquals("form2", showForm.getFormName());
        
        Show show = (Show) makeCommand("<show><url><string value=\"http://www.google.com\" /></url></show>");
        assertEquals("http://www.google.com", show.getUrlExpression().evaluate(null));
    }

    private ProcessCommand makeCommand(String commandXml) {
        ConfigElement xml = parseConfig("<foo>" + commandXml + "</foo>");
        ProcessCommandList commands = new ProcessCommandList(xml);
        assertEquals(1, commands.size());
        return commands.get(0);
    }

    public void testExecution() {
        ProcessCommandList commands = new ProcessCommandList();
        commands.add(new ShowDocument("one"));
        commands.add(new ShowDocument("two"));

        Project project = ProjectBuilder.buildMinimalisticProject();
        project.add(new Document("one", "one"));
        project.add(new Document("two", "two"));
        ExecutionResult result = commands.execute(new FakeExecutionContext(
                new UserProject(project, projectOwner, "aProject"), new Form(
                        "aForm")));
        List<Html> html = result.getHtml();
        assertContains("one", html.get(0).toString());
        assertContains("two", html.get(1).toString());
        assertEquals(2, html.size());
    }

    public void testShowFormExecution() {
        ProcessCommandList commands = new ProcessCommandList();
        commands.add(makeCommand("<show form=\"form1\" />"));
        commands.add(new ShowDocument("doc1"));

        Project project = ProjectBuilder.buildMinimalisticProject();
        Document document = new Document("doc1", "contents");
        project.add(document);

        ExecutionResult result = commands.execute(new FakeExecutionContext(
                new UserProject(project, projectOwner, "aProject"), new Form(
                        "form1")));
        assertFalse("Empty html", result.hasOutput());
        assertTrue("Next form", result.hasNextForm());
        assertEquals("Next form", "form1", result.getNextForm());
    }

    public void testShowFormExecutionWithinIfStatement() {
        ProcessCommandList commands = new ProcessCommandList();

        ProcessCommandList thenBlock = new ProcessCommandList();
        thenBlock.add(makeCommand("<show form=\"form1\" />"));

        BooleanExpression condition = new Equals("form1:Q1:a", "abc");
        If ifCommand = new If(condition, thenBlock);

        commands.add(ifCommand);
        commands.add(new ShowDocument("doc1"));

        Project project = ProjectBuilder.buildMinimalisticProject();
        Document document = new Document("doc1", "contents");
        project.add(document);

        Form form = new Form(
                parseConfig("<form name=\"form1\" process=\"process1\">\n"
                        + "<items>\n"
                        + "<fib label=\"Q1\">Name?<blank label=\"a\" length=\"20\"/></fib>\n"
                        + "</items>\n" + "</form>\n"));

        project.add(form);

        Process process = new Process("process1");
        process.add(ifCommand);
        project.add(process);

        ExecutionResult result = commands.execute(new FakeExecutionContext(
                new UserProject(project, projectOwner, "aProject"), form,
                "Q1:a", "abc"));
        assertFalse("Empty html", result.hasOutput());
        assertTrue("Next form", result.hasNextForm());
        assertEquals("Next form", "form1", result.getNextForm());
    }
}
