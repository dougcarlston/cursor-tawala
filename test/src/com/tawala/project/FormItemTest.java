package com.tawala.project;

import java.util.Iterator;

import com.tawala.TestCase;
import com.tawala.project.builder.FormBuilder;

public class FormItemTest extends TestCase {

    public void testSkipToTarget() {
        FormBuilder builder = new FormBuilder();
        builder.addFib("fibAlternate").addBlank();
        builder.addText("textAlternate", "some text");
        builder.addMcWithAlternateLabel("mcAlternate", "Does this work?", "yes", "no");
        Form form = builder.build();

        Iterator<FormItem> i = form.getItems().iterator();
        FormItem fib = i.next();
        assertTrue(fib.matchesId("Q1"));
        assertTrue(fib.matchesId("fibAlternate"));
        FormItem text = i.next();
        assertTrue(text.matchesId("T1"));
        assertTrue(text.matchesId("textAlternate"));
        FormItem mc = i.next();
        assertTrue(mc.matchesId("Q2"));
        assertTrue(mc.matchesId("mcAlternate"));
        assertFalse(i.hasNext());
    }
}
