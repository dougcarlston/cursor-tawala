package com.scissor.xmlconfig;

public class ElementAttributeNameAndValueMatcher implements ConfigMatcher {
	private final String elementName;
	private final String attributeName;
	private final String attributeValue;
	private final Class classToConstruct;

	public ElementAttributeNameAndValueMatcher(String elementName,
			String attributeName, String attributeValue, Class classToConstruct) {
		this.elementName = elementName;
		this.attributeName = attributeName;
		this.attributeValue = attributeValue;
		this.classToConstruct = classToConstruct;
	}

	public Class matchFor(ConfigElement config) {
		if (!elementName.equals(config.getName())) {
			return null;
		}
		ConfigAttribute attribute = config.attribute(attributeName);
		if (attribute == null || attribute.stringValue() == null) {
			return null;
		}

		if (attribute.stringValue().equals(this.attributeValue)) {
			return classToConstruct;
		} else {
			return null;
		}
	}
}
