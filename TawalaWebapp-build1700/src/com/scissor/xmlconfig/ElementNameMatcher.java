package com.scissor.xmlconfig;

public class ElementNameMatcher implements ConfigMatcher {
    private final String name;
    private final Class classToConstruct;

    public ElementNameMatcher(String name, Class classToConstruct) {
        this.name = name;
        this.classToConstruct = classToConstruct;
    }

    public Class matchFor(ConfigElement config) {
        if (name.equals(config.getName())) {
            return classToConstruct;
        } else {
            return null;
        }
    }

}
