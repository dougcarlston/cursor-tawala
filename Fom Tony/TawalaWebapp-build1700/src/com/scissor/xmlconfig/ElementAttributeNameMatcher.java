package com.scissor.xmlconfig;

public class ElementAttributeNameMatcher implements ConfigMatcher {
    private final String elementName;
    private final String attributeName;
    private final Class classToConstruct;

    public ElementAttributeNameMatcher(String elementName, String attributeName, Class classToConstruct) {
        this.elementName = elementName;
        this.attributeName = attributeName;
        this.classToConstruct = classToConstruct;
    }

    public Class matchFor(ConfigElement config) {
        if (elementName.equals(config.getName()) && config.hasAttribute(attributeName)) {
            return classToConstruct;
        } else {
            return null;
        }
    }

}
