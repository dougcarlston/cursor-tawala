package com.tawala.component.function;

import java.util.Locale;

import org.dom4j.Element;

import com.tawala.component.ComponentMetadataSupport;

abstract public class FunctionMetadataSupport extends ComponentMetadataSupport implements FunctionMetadata {
	public FunctionMetadataSupport(String id, int version) {
		super(id, "function." + id + ".name", "function." + id + ".description", version);
	}
	
	public Element toElement(String name, Locale locale) {
		Element result = createComponentElement(name, locale);
		result.addAttribute("return-type", getReturnType().getId());
		return result;
	}
}
