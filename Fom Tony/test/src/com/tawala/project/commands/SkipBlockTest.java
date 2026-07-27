package com.tawala.project.commands;

import com.tawala.TestCase;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.SkipBlockBuilder;

public class SkipBlockTest extends TestCase {
    private User projectOwner = UserTest.aUser("tester");

    public void testSkipToCommand() {
        FormBuilder form = new FormBuilder();
        SkipBlockBuilder skipBuilder = form.addSkip();
        skipBuilder.addSkip("T3");
        SkipBlock skipBlock = skipBuilder.makeSkipBlock();
        FakeExecutionContext context = new FakeExecutionContext(
                new UserProject(ProjectBuilder.buildMinimalisticProject(),
                        projectOwner, "project1"), form.build());

        SkipExecutionResult result = skipBlock.execute(context);
        assertEquals("T3", result.getSkipTo());
    }

    public void testMultipleSkipToCommands() {
        FormBuilder form = new FormBuilder();
        SkipBlockBuilder skipBuilder = form.addSkip();
        skipBuilder.addSkip("T3");
        skipBuilder.addSkip("T5");
        SkipBlock skipBlock = skipBuilder.makeSkipBlock();
        FakeExecutionContext context = new FakeExecutionContext(
                new UserProject(ProjectBuilder.buildMinimalisticProject(),
                        projectOwner, "project1"), form.build());

        SkipExecutionResult result = skipBlock.execute(context);
        assertEquals("T3", result.getSkipTo());
    }

    public void testIfWithOtherwise() {
        FormBuilder form = new FormBuilder();
        SkipBlockBuilder skipBuilder = form.addSkip();
        skipBuilder.addIfSkip("age", "isGreaterThan", "string", "17", "T3",
                "T5");
        SkipBlock skipBlock = skipBuilder.makeSkipBlock();
        FakeExecutionContext context = new FakeExecutionContext(new UserProject(ProjectBuilder
                .buildMinimalisticProject(), projectOwner, "project1"), form
                .build());

        context.setValue("age", "17");
        SkipExecutionResult result = skipBlock.execute(context);
        assertEquals("T5", result.getSkipTo());

        context.setValue("age", "18");
        result = skipBlock.execute(context);
        assertEquals("T3", result.getSkipTo());
    }

    public void testIfWithoutOtherwise() {
        FormBuilder form = new FormBuilder();
        SkipBlockBuilder skipBuilder = form.addSkip();
        skipBuilder.addIfSkip("age", "isGreaterThan", "string", "17", "T3",
                null);
        SkipBlock skipBlock = skipBuilder.makeSkipBlock();
        FakeExecutionContext context = new FakeExecutionContext(new UserProject(ProjectBuilder
                .buildMinimalisticProject(), projectOwner, "project1"), form
                .build());

        context.setValue("age", "18");
        SkipExecutionResult result = skipBlock.execute(context);
        assertEquals("T3", result.getSkipTo());
    }

    public void testTwoIfs() {
        FormBuilder form = new FormBuilder();
        SkipBlockBuilder skipBuilder = form.addSkip();
        skipBuilder.addIfSkip("age", "isGreaterThan", "string", "17", "T3",
                null);
        skipBuilder.addIfSkip("IQ", "isGreaterThan", "string", "100", "T5",
                null);
        SkipBlock skipBlock = skipBuilder.makeSkipBlock();
        FakeExecutionContext context = new FakeExecutionContext(new UserProject(ProjectBuilder
                .buildMinimalisticProject(), projectOwner, "project1"), form
                .build());

        // make both conditions true
        context.setValue("age", "18");
        context.setValue("IQ", "101");
        SkipExecutionResult result = skipBlock.execute(context);
        assertEquals("T3", result.getSkipTo());

        // make first condition false
        context.setValue("age", "16");
        result = skipBlock.execute(context);
        assertEquals("T5", result.getSkipTo());

        // make both conditions false
        context.setValue("IQ", "99");
        result = skipBlock.execute(context);
        assertNull(result.getSkipTo());
    }
}
