package com.scissor;

@SuppressWarnings("serial")
public class UnimplementedException extends RuntimeException {
    public UnimplementedException() {
    }

    public UnimplementedException(String message) {
        super(message);
    }

    public UnimplementedException(String message, Throwable cause) {
        super(message, cause);
    }

    public UnimplementedException(Throwable cause) {
        super(cause);
    }
}
