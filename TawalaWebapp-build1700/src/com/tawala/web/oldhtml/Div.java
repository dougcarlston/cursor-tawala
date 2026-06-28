package com.tawala.web.oldhtml;

public class Div extends Block {

    public Div() {
        super("div", true);
    }

    public Div(String attribute, String value) {
    	this();
        this.setAttribute(attribute, value);
    }


}
