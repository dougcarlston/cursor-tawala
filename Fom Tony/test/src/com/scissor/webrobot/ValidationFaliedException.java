package com.scissor.webrobot;

import java.io.PrintStream;
import java.io.PrintWriter;

/** Indicates that some a page did not pass validation. Thrown only by
 *  PageValidators. Unchecked because normal usage is that we don't care.  */
public class ValidationFaliedException extends RuntimeException {
    private static final long serialVersionUID = 1L;
    private Throwable rootCause;

    public ValidationFaliedException(String message) {
        super(message);
    }

    public ValidationFaliedException(String message, Exception rootCause) {
        this(message);
        this.rootCause = rootCause;
    }


    public Throwable getRootCause() {
        return rootCause;
    }

    public String getMessage() {
        if (rootCause == null) {
            return super.getMessage();
        } else {
            return super.getMessage() + ": root cause: " +
                    rootCause.getClass() + ": " + rootCause.getMessage();
        }
    }

    public void printStackTrace() {
        super.printStackTrace();
        if (rootCause != null) {
            System.err.println("root cause: ");
            rootCause.printStackTrace();
        }
    }

    public void printStackTrace(PrintStream stream) {
        super.printStackTrace(stream);
        if (rootCause != null) {
            stream.println("root cause: ");
            rootCause.printStackTrace(stream);
        }
    }

    public void printStackTrace(PrintWriter writer) {
        super.printStackTrace(writer);
        if (rootCause != null) {
            writer.println("root cause: ");
            rootCause.printStackTrace(writer);
        }
    }

    protected void setRootCause(Throwable rootCause) {
        this.rootCause = rootCause;
    }


}
