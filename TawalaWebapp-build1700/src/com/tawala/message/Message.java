package com.tawala.message;

import org.springframework.context.support.DefaultMessageSourceResolvable;

public class Message extends DefaultMessageSourceResolvable {
    private static final long serialVersionUID = 1L;

    public Message(String code) {
        super(code);
    }

    public Message(String code, Object ... arguments) {
        super(new String[]{code}, arguments);
    }
}
