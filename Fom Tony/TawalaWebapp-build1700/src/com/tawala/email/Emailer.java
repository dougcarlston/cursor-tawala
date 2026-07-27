package com.tawala.email;

import org.springframework.mail.javamail.JavaMailSender;

public class Emailer {
    static private JavaMailSender sender;
    
    public static JavaMailSender getSender() {
        if(sender == null)
            throw new IllegalStateException("Sender is not initialized");
        return sender;
    }

    public void setSender(JavaMailSender sender) {
        Emailer.sender = sender;
    }
}
