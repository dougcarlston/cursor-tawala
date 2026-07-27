package com.tawala.component.validator;

import java.util.Locale;

import org.dom4j.Element;

import com.tawala.component.ComponentMetadataSupport;

abstract public class FIBValidatorMetadataSupport extends ComponentMetadataSupport implements FIBValidatorMetadata {
	public FIBValidatorMetadataSupport(String id, int version) {
		super(id, "validator." + id + ".name", "validator." + id + ".description", version);
	}
	
	public Element toElement(String name, Locale locale) {
		Element result = createComponentElement(name, locale);
		return result;
	}
}
