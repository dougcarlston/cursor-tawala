package com.scissor.xmlconfig;

import org.dom4j.Attribute;

public class ConfigAttribute extends ConfigItem {
    private String rawValue;
    private String name;

    public ConfigAttribute(ConfigElement parent, Attribute attribute) {
        this(parent, attribute.getName(), attribute.getValue());
    }

    protected ConfigAttribute(ConfigElement parent, String name, String rawValue) {
        super(parent);
        this.name = name;
        this.rawValue = rawValue;
    }

    public String stringValue() {
        return rawValue();
    }

    private String rawValue() {
        markUsed();
        return rawValue;
    }

    public String getName() {
        return "@" + name;
    }

    public boolean booleanValue() {
        return "true".equals(rawValue());
    }

    public int intValue() {
        return Integer.parseInt(rawValue());
    }

    public int intValue(int defaultValue) {
        return rawValue() == null ? defaultValue : Integer.parseInt(rawValue());
    }

    public int hexValue() {
        return Integer.parseInt(rawValue(), 16);
    }

    public int hexValue(String defaultValue) {
        return Integer.parseInt(rawValue == null ? defaultValue : rawValue, 16);
    }

    public void setValue(String newValue) {
        rawValue = newValue;
    }
}
