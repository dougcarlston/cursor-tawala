package com.tawala.project;

import java.util.List;

import com.tawala.TestCase;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.ExecutionResult;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.project.commands.Send;
import com.tawala.project.commands.ShowDocument;
import com.tawala.web.oldhtml.Html;

public class ProcessTest extends TestCase {
    public void testConstructFromXml() {
        String xml = "<process name=\"ShowDoc\">\n"
                + "                  <show document=\"Hello\"/>\n"
                + "                  <send><subject>foo</subject><body>bar</body></send>\n"
                + "            </process>\n";
        Process process = new Process(parseConfig(xml));
        assertEquals("ShowDoc", process.getName());
        assertEquals(2, process.size());
        ShowDocument show = (ShowDocument) process.get(0);
        assertEquals("Hello", show.getDocumentName());
        Send send = (Send) process.get(1);
        StringBuilder subjectBuilder = new StringBuilder();
        send.getSubject().get(0).appendTo(subjectBuilder, null);
		assertEquals("foo", subjectBuilder.toString());
    }

    public void testManualConstruction() {
        Process process = new Process("someName");
        assertEquals("someName", process.getName());
        assertEquals(0, process.size());
        process.add(new ShowDocument("aDoc"));
        assertEquals(1, process.size());
    }

    public void testBasicPostProcessExecution() {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder db = builder.addDocument("doc1", "content");
        ProcessBlockBuilder proc1 = builder.addProcess("proc1");
        proc1.addShow(db);
        builder.addForm("form1", proc1);

        ExecutionResult result = execute(builder, "proc1");
        List<Html> html = result.getHtml();
        assertContains("content", html.get(0).toString());
        assertEquals(1, html.size());
    }

    public void testMultipleShow() {
        ProjectBuilder builder = new ProjectBuilder();
        DocumentBuilder doc1 = builder.addDocument("doc1", "hello");
        DocumentBuilder doc2 = builder.addDocument("doc2", "world");
        com.tawala.project.builder.ProcessBlockBuilder proc1 = builder
                .addProcess("proc1");
        proc1.addShow(doc1);
        proc1.addShow(doc2);
        builder.addForm("form1", proc1);

        ExecutionResult result = execute(builder, "proc1");
        List<Html> html = result.getHtml();
        assertEquals(2, html.size());
        assertContains("hello", html.get(0).toString());
        assertContains("world", html.get(1).toString());
    }

    private ExecutionResult execute(ProjectBuilder builder, String processName) {
        Project project = builder.build();
        Process process = project.getProcess(processName);
        FakeExecutionContext context = new FakeExecutionContext(project);
        return process.execute(context);
    }

}
