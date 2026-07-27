package com.tawala.web.dispatch;

import java.io.IOException;

import org.xml.sax.SAXException;

import com.tawala.web.ServletTestCase;

// TODO: isolate from web.xml usage in superclass so only servlet is tested

public class DispatchServletTest extends ServletTestCase {

    protected void tearDown() throws Exception {
        super.tearDown();
        logs.dumpUnseen(this);
    }

    public void testHomePage() throws IOException, SAXException {
        get("http://ignored/");
        assertContains("Tawala", response.getText());
    }
}
