package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.TestCase;
import com.tawala.project.Document;
import com.tawala.project.Process;
import com.tawala.project.Project;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.oldhtml.Html;

public class AppendTest extends TestCase {
    private static final String NEWDOC = "newDoc";

    public void testCreateNewDocument() {
        ProjectBuilder builder = new ProjectBuilder();
        builder.addForm("aForm");
        builder.addDocument("aDoc", "hello!");
        FakeExecutionContext context = new FakeExecutionContext(builder.build());

        Process process = makeProcess("<append document=\"" + NEWDOC
                + "\" appendage=\"aDoc\"/>\n");

        ExecutionResult result = process.execute(context);
        assertNull(context.getProject().getDocument(NEWDOC));
        assertEmpty(result.getHtml()); // no show means no content

        assertMatches("<div class=\"document\">.*hello!.*</div>" + NEWLINE,
                renderDocument(NEWDOC, context));
    }

    public void testAppendTwice() {
        ProjectBuilder builder = new ProjectBuilder();
        builder.addForm("aForm");
        builder.addDocument("aDoc", "hello!");
        FakeExecutionContext context = new FakeExecutionContext(builder.build());

        Process process = makeProcess("<append document=\"" + NEWDOC
                + "\" appendage=\"aDoc\"/>\n" + "<append document=\"" + NEWDOC
                + "\" appendage=\"aDoc\"/>\n");

        process.execute(context);
        assertMatches("<div class=\"document\">.*hello!.*</div>" + NEWLINE
                + "<div class=\"document\">.*hello!.*</div>" + NEWLINE,
                renderDocument(NEWDOC, context));
    }

    public void testAppendToExistingDocument() {
        ProjectBuilder builder = new ProjectBuilder();
        builder.addForm("aForm");
        builder.addDocument("doc1", "one");
        builder.addDocument("doc2", "two");
        Project project = builder.build();
        FakeExecutionContext context = new FakeExecutionContext(project);

        Process process = makeProcess("<append document=\"doc1\" appendage=\"doc2\"/>\n");

        process.execute(context);
        assertMatches("<div class=\"document\">.*one.*</div>" + NEWLINE
                + "<div class=\"document\">.*two.*</div>" + NEWLINE,
                renderDocument("doc1", context));
        assertMatches("<div class=\"document\">.*one.*</div>" + NEWLINE,
                renderDocument(project.getDocument("doc1"), context));

    }

    private String renderDocument(String docName, FakeExecutionContext context) {
        return renderDocument(context.getDocument(docName), context);
    }

    private String renderDocument(Document newDoc, FakeExecutionContext context) {
        assertNotNull(newDoc);
        Html html = newDoc.toHtml(context);
        return html.toString();
    }

    private Process makeProcess(String xml) {
        ConfigElement config = parseConfig("<process name=\"proc1\">\n" + xml
                + "</process>\n");
        Process process = new Process(config);
        assertEmpty(config.getUnusedItems());
        return process;
    }

}
