package com.tawala.component;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Locale;

import org.dom4j.DocumentHelper;
import org.dom4j.Element;
import org.dom4j.QName;

import com.tawala.component.repository.Repository;
import com.tawala.message.Message;

public class Parameter {
	final private String id;
	final private String resourceId;
	final private Message nameMessage;
	final private Message descriptionMessage;
	final private ParameterType type;
	final private boolean required;
	
	final private Collection<Option> options = new ArrayList<Option>();
	private Message defaultValue;
	final private List<ParameterRestriction> restrictions;
	

	public Parameter(String id, String resourceId, ParameterType type, boolean required) {
		this(id, resourceId, type, required, null);
	}

	public Parameter(String id, String resourceId, ParameterType type, boolean required, List<ParameterRestriction> restrictions) {
		this.id = id;
		this.resourceId = resourceId;
		this.nameMessage = new Message("parameter." + resourceId + ".name");
		this.descriptionMessage = new Message("parameter." + resourceId + ".description");
		this.type = type;
		this.required = required;
		this.restrictions = restrictions;
	}

	public Element toElement(Locale locale) {
		Element result = DocumentHelper.createElement(new QName("parameter", Repository.NAMESPACE));
		result.addAttribute("id", id);
		result.addAttribute("type", type.getId());
		result.addAttribute("name", Repository.getMessageSource().getMessage(nameMessage, locale));
		result.addAttribute("required", String.valueOf(required));
		result.addElement(new QName("description", Repository.NAMESPACE)).setText(Repository.getMessageSource().getMessage(descriptionMessage, locale));

		if(options.size() > 0 && defaultValue != null) {
			throw new IllegalStateException("Options and default values are mutually exclusive.");
		}
		
		for (Option option : options) {
			result.add(option.toElement(locale));
		}
		
		if (defaultValue != null) {
			result.addElement(new QName("default-value", Repository.NAMESPACE)).setText(Repository.getMessageSource().getMessage(defaultValue, locale));
		}
		
		if(restrictions != null && restrictions.size() > 0) {
			for (ParameterRestriction restriction : restrictions) {
				result.add(restriction.toElement());
			}
		}
		
		addTypeSpecificElements(result, locale);
		
		return result;
	}

	/** 
	 * Placeholder to be overwritten by extending classes
	 */
	protected void addTypeSpecificElements(Element result, Locale locale) {}

	
	public Parameter addOptions(Option... options ) {
		for (Option option : options) {
			this.options.add(option);
		}
		return this;
	}

	public Parameter useDefaultValue() {
		defaultValue = new Message("parameter." + resourceId + ".default" );
		return this;
	}
	
	public String getId() {
		return id;
	}
}
