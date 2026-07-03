package com.scissor.webrobot;

import java.io.PrintWriter;
import java.net.MalformedURLException;

import com.meterware.httpunit.WebRequest;
import com.meterware.httpunit.WebResponse;

/** Keeps track of what a robot is up to. By default, doesn't do much. */
public class Tracker {
    private boolean verbose = false;
    private PrintWriter out;

    public boolean isVerbose() {
        return verbose;
    }

    public void setVerbose(boolean verbose) {
        this.verbose = verbose;
    }

    public void note(String message) {
        if (verbose) out().println(clean(message));
    }

    protected PrintWriter out() {
        if (out == null) out = new PrintWriter(System.out);
        return out;
    }

    protected String clean(String message) {
        StringBuffer sb = new StringBuffer();
        char[] chars = message.toCharArray();
        for (int i = 0; i < chars.length; i++) {
            char c = chars[i];
            switch (c) {
                case '\n':
                    sb.append(" ");
                    break;
                case '\r':
                    sb.append(" ");
                    break;
                default:
                    sb.append(c);
            }
        }
        return sb.toString();
    }


    public void goingTo(WebRobot webRobot, WebRequest request) {
        try {
            note("going to " + request.getURL());
        } catch (MalformedURLException e) {
            grumble("request can't parse url", e);
        }
    }


    public void gotResponse(WebRobot bot, WebResponse response) {
        String title = null;
        try {
            title = bot.pageTitle();
        } catch (RuntimeException e) {
            title = "";
        }
        note("arrived at " + title + " <" + response.getURL() + ">");
    }


    protected void grumble(String message, Exception e) {
        System.err.println(message + ":");
        e.printStackTrace(System.err);
    }


}
