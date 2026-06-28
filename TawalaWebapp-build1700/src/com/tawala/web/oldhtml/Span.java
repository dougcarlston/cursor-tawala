package com.tawala.web.oldhtml;

public class Span extends Block {

    public Span() {
        super("span", true);
    }

    public Span(String attribute, String value) {
    	this();
        this.setAttribute(attribute, value);
    }


}
