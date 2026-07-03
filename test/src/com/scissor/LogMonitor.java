package com.scissor;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import junit.framework.Assert;
import junit.framework.TestCase;

import org.apache.log4j.Level;
import org.apache.log4j.spi.LoggingEvent;

public class LogMonitor extends org.apache.log4j.AppenderSkeleton {
    List<LoggingEvent> events = new ArrayList<LoggingEvent>();
    Set<LoggingEvent> seen = new HashSet<LoggingEvent>();
    Set<EventMatcher> ignore = new HashSet<EventMatcher>();

    public int size() {
        return events.size();
    }

    public LoggingEvent get(int index) {
        return events.get(index);
    }

    protected void append(LoggingEvent event) {
        if (!ignored(event)) {
            events.add(event);
        }
    }

    private boolean ignored(LoggingEvent event) {
        for (EventMatcher matcher : ignore) {
            if (matcher.matches(event))
                return true;
        }
        return false;
    }

    public boolean requiresLayout() {
        return false;
    }

    public void close() {
    }

    public void ignoreMessage(Level level, String message) {
        ignore.add(new EventMatcher(level, message));
    }

    public void checkNothingElse() {
        for (LoggingEvent event : events) {
            if (!seen.contains(event)) {
                fail("expected nothing, found " + asString(event));
            }
        }
    }

    public void lastMessageOk() {
        seen.add(firstUnseen());
    }

    public void checkMessage(int index, Level level, String message) {
        checkMessage(level, get(index), message);
    }

    public void checkMessage(Level level, String message) {
        checkMessage(level, firstUnseen(), message);
    }

    private void checkMessage(Level level, LoggingEvent event, String message) {
        Assert.assertEquals(level, event.getLevel());
        Assert.assertEquals(message, event.getMessage());
        seen.add(event);
    }

    private LoggingEvent firstUnseen() {
        for (LoggingEvent event : events) {
            if (!seen.contains(event))
                return event;
        }
        fail("no more events");
        return null; // unreached
    }

    // --- TODO: validate that we really should this one. Alternatives include
    // "skipUntil". SL.
    public boolean containsMessage(Level level, String message) {
        for (LoggingEvent event : events) {
            if (event.getLevel().equals(level)
                    && event.getMessage().equals(message))
                return true;
        }
        return false;
    }

    public void checkMessageCount(int expected) {
        int actual = size();
        if (expected != actual) {
            String message = "expected " + expected + " log messages, found "
                    + actual;
            if (actual == 0) {
                fail(message);
            } else {
                LoggingEvent event = get(0);
                fail(message + ": " + asString(event));
            }
        }
    }

    private String asString(LoggingEvent event) {
        if (event.getThrowableInformation() == null
                || event.getThrowableInformation().getThrowable() == null)
            return event.getLevel() + ": " + event.getMessage();
        else {
            StringWriter writer = new StringWriter();
            event.getThrowableInformation().getThrowable().printStackTrace(
                    new PrintWriter(writer));
            return event.getLevel() + ": " + event.getMessage() + "\n" + writer;
        }
    }

    private void fail(String message) {
        Assert.fail(message);
    }

    public void checkJustOneMessage(Level level, String message) {
        checkMessageCount(1);
        checkMessage(0, level, message);
    }

    public void dumpUnseen(TestCase caller) {
        dumpUnseen(caller, Level.ALL);
    }

    public void dumpUnseenErrors(com.tawala.TestCase caller) {
        dumpUnseen(caller, Level.ERROR);
    }

    private void dumpUnseen(TestCase caller, Level level) {
        String contextName = caller.getClass().getName() + "."
                + caller.getName();
        boolean headerPrinted = false;
        for (LoggingEvent event : events) {
            if (!seen.contains(event)
                    && event.getLevel().isGreaterOrEqual(level)) {
                if (!headerPrinted) {
                    System.err.println(contextName + ":");
                    headerPrinted = true;
                }
                System.err.println("    unexpected log message <"
                        + asString(event) + ">");
            }
        }
    }

    private static class EventMatcher {
        private final Level level;
        private final String message;

        public EventMatcher(Level level, String message) {
            // TODO: implement
            this.level = level;
            this.message = message;
        }

        public boolean matches(LoggingEvent event) {
            if (!level.equals(event.getLevel()))
                return false;
            return message.equals(event.getMessage().toString());
        }
    }
}
