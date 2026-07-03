package com.scissor.webrobot;

import java.util.Hashtable;

import junit.framework.TestCase;

import com.meterware.httpunit.WebClient;
import com.meterware.servletunit.ServletRunner;

public abstract class WebRobotTestCase extends TestCase {
    protected ServletRunner runner = new ServletRunner();

    protected void registerPage(String url, String text) {
        Hashtable<String, String> initParams = new Hashtable<String, String>();
        initParams.put("text", text);
        runner.registerServlet(url, SampleServlet.class.getName(), initParams);
    }

    protected WebClient makeClient() {
        WebClient client = runner.newClient();
        return client;
    }

    public static void assertEquals(String[] expected, String[] actual) {
        assertEquals("array size", expected.length, actual.length);
        for (int i = 0; i < expected.length; i++) {
            assertEquals("at index " + i, expected[i], actual[i]);
        }
    }

}
