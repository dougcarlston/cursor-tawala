package com.scissor;


@SuppressWarnings("serial")
public class ImpossibleException extends RuntimeException {
    public ImpossibleException() {
    }

    public ImpossibleException(String message) {
        super(message);
    }

    public ImpossibleException(String message, Throwable cause) {
        super(message, cause);
    }

    public ImpossibleException(Throwable cause) {
        super(cause);
    }
}
