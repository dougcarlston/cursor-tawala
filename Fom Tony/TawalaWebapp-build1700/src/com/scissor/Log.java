package com.scissor;

import org.apache.log4j.Appender;
import org.apache.log4j.ConsoleAppender;
import org.apache.log4j.Level;
import org.apache.log4j.Logger;
import org.apache.log4j.PatternLayout;

/**
 * Convenience methods for logging.
 */
public class Log {
    public static Level DEBUG = Level.DEBUG;
    public static Level INFO = Level.INFO;
    public static Level WARN = Level.WARN;
    public static Level ERROR = Level.ERROR;
    public static Level FATAL = Level.FATAL;

    private static final String DEFAULT_FORMAT = "%d{yyyy-MM-dd HH:mm:ss.SSS} [%t] %p %c %x - %m%n";

    static {
        normalLogging();
    }

    public static void normalLogging() {
        captureLogging(new ConsoleAppender(new PatternLayout(DEFAULT_FORMAT)));
    }

    public static void captureLogging(Appender appender, Level level) {
        Logger root = Logger.getRootLogger();
        root.setLevel(level);
        root.removeAllAppenders();
        root.addAppender(appender);
    }

    public static void captureLogging(Appender appender) {
        captureLogging(appender, INFO);
    }

    public static void debug(Object context, String message) {
        loggerFor(context).debug(message);
    }

    public static void debug(Object context, String message, Throwable problem) {
        loggerFor(context).debug(message, problem);
    }

    public static void info(Object context, String message) {
        loggerFor(context).info(message);
    }

    public static void info(Object context, String message, Throwable problem) {
        loggerFor(context).info(message, problem);
    }

    public static void warn(Object context, String message) {
        loggerFor(context).warn(message);
    }

    public static void warn(Object context, String message, Throwable problem) {
        loggerFor(context).warn(message, problem);
    }

    public static void error(Object context, String message) {
        loggerFor(context).error(message);
    }

    public static void error(Object context, String message, Throwable problem) {
        loggerFor(context).error(message, problem);
    }

    private static Logger loggerFor(Object context) {
        if (context instanceof String) {
            return Logger.getLogger((String) context);
        } else if (context instanceof Class) {
            return Logger.getLogger((Class) context);
        } else {
            return Logger.getLogger(context.getClass());
        }
    }

}
