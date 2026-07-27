package com.scissor.webrobot;

import com.scissor.ScissorException;

public class RobotException extends ScissorException {
    private static final long serialVersionUID = 1L;

    public RobotException(String message) {
        super(message);
    }

    public RobotException(String message, Exception rootCause) {
        super(message, rootCause);
    }

    public RobotException(Exception rootCause) {
        super(rootCause);
    }
}
