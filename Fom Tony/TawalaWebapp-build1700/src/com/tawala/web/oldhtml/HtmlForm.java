package com.tawala.web.oldhtml;


public class HtmlForm extends Block {

    public HtmlForm() {
        super("form");
    }

    public void setMethod(String method) {
        setAttribute("method", method);
    }

    protected boolean newlineAfterOpeningTag() {
        return true;
    }

}
