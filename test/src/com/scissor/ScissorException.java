package com.scissor;

import java.io.PrintStream;
import java.io.PrintWriter;

public class ScissorException extends Exception {
    private static final long serialVersionUID = 1L;

    Throwable rootCause = null;

    public ScissorException(String message) {
        this(message, null);
    }

    public ScissorException(String message, Exception rootCause) {
        super(message);
        this.setRootCause(rootCause);
    }

    public ScissorException(Exception rootCause) {
        super();
        this.setRootCause(rootCause);
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
