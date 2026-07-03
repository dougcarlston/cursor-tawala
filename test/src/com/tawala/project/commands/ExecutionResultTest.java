package com.tawala.project.commands;

import java.util.List;

import com.tawala.TestCase;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlParagraph;

public class ExecutionResultTest extends TestCase {
    public void testSomething() {
        ExecutionResult first = new ExecutionResult(new HtmlParagraph("one"));
        ExecutionResult second = new ExecutionResult(new HtmlParagraph("two"));
        ExecutionResult combined = first.add(second);
        assertFalse(first == combined);
        assertFalse(second == combined);
        List<Html> output = combined.getHtml();
        assertEquals(2, output.size());
        assertEquals("<p>one</p>" + NEWLINE, output.get(0).toString());
        assertEquals("<p>two</p>" + NEWLINE, output.get(1).toString());
    }

    public void testNextForm() {
        ExecutionResult first = new ExecutionResult(new HtmlParagraph("one"));
        assertFalse(first.hasNextForm());
        ExecutionResult nextForm = new ExecutionResult("form2");
        assertTrue(nextForm.hasNextForm());
        assertEquals("form2", nextForm.getNextForm());
        ExecutionResult combined = first.add(nextForm);
        assertTrue(combined.hasNextForm());
        assertEquals("form2", combined.getNextForm());
        assertEquals("<p>one</p>" + NEWLINE, combined.getHtml().get(0).toString());
    }
}
