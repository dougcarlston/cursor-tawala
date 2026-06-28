package com.tawala.project;

import com.scissor.xmlconfig.ConfigElement;

public abstract class Question extends FormItem {

    protected Question(ConfigElement element) {
        super(element);
    }

    public boolean hasFields() {
        return true;
    }

    public String htmlPrefix() {
        return getHtmlId() + ":";
    }

    public String getHtmlId() {

        if (getAlternateLabel() != null) {
            return getAlternateLabel();
        } else {
            return getId();
        }
    }

    protected String htmlFormPrefix() {
        String prefix;
        if (getFormId() == null) {
            prefix = "";
        } else {
            prefix = getFormId() + ":";
        }
        return prefix;
    }


}
