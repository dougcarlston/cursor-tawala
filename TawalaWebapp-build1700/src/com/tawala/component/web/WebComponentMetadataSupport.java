package com.tawala.component.web;

import java.util.Locale;

import org.dom4j.Element;

import com.tawala.component.ComponentMetadataSupport;

abstract public class WebComponentMetadataSupport extends
		ComponentMetadataSupport implements WebComponentMetadata {
	public WebComponentMetadataSupport(String id, int version) {
		super(id, "web." + id + ".name", "web." + id + ".description", version);
	}

	public Element toElement(String name, Locale locale) {
		Element result = createComponentElement(name, locale);
		return result;
	}
}
