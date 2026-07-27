package com.tawala.web.oldhtml;

public class Label extends Block {

    public Label() {
        super("label", true);
    }

    public Label(String attribute, String value) {
    	this();
        this.setAttribute(attribute, value);
    }


}
