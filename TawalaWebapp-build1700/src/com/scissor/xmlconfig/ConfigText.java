package com.scissor.xmlconfig;

import org.dom4j.Text;

public class ConfigText extends ConfigItem {
    private String text;

    public ConfigText(ConfigItem parent, Text text) {
        super(parent);
        this.text = text.getText();
    }

    public String getName() {
        return "text(" + pretty(text) + ")";
    }

    private String pretty(String text) {
        return text.replaceAll("\\n\\s*", "");
    }

    public String text() {
        markUsed();
        return text;
    }

    public boolean isInteresting() {
        return !text.trim().equals("");
    }
}
