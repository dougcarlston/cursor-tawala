package com.tawala.component;

import java.util.Collection;
import java.util.Locale;

import org.dom4j.Element;

public interface ComponentMetadata {
	String getId();
	String getName(Locale locale);
	String getDescription(Locale locale);
	Element toElement(String name, Locale locale);
	Collection<Parameter> getParameters();
	int getVersion();
}
