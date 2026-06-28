package com.tawala.component;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Locale;

import org.dom4j.DocumentHelper;
import org.dom4j.Element;
import org.dom4j.QName;

import com.tawala.component.repository.Repository;
import com.tawala.message.Message;


public abstract class ComponentMetadataSupport implements ComponentMetadata {
	private final String id;
	private final Message nameMessage;
	private final Message descriptionMessage;
	private final int version;
	private final Collection<Parameter> parameters = new ArrayList<Parameter>();
	
	public ComponentMetadataSupport(String id, String nameMessageId, String descriptionMessageId, int version) {
		this.id = id;
		this.nameMessage = new Message(nameMessageId);
		this.descriptionMessage = new Message(descriptionMessageId);
		this.version = version;
	}
	
	final public String getId() {
		return id;
	}

	final public String getName(Locale locale) {
		return Repository.getMessageSource().getMessage(nameMessage, locale);
	}
	
	final public String getDescription(Locale locale) {
		return Repository.getMessageSource().getMessage(descriptionMessage, locale);
	}
	
	final public int getVersion() {
		return version;
	}
	
	final public Collection<Parameter> getParameters() {
		return parameters;
	}

	final public void addParameters(Parameter ... parameters) {
		for (int i = 0; i < parameters.length; i++) {
			this.parameters.add(parameters[i]);
		}
	}

	final public void addParameter(Parameter parameter) {
		this.parameters.add(parameter);
	}

	protected Element createComponentElement(String name, Locale locale) {
		Element result = DocumentHelper.createElement(new QName(name, Repository.NAMESPACE));
		result.addAttribute("id", getId());
		result.addAttribute("name", getName(locale));
		result.addAttribute("version", String.valueOf(getVersion()));
		result.addElement(new QName("description", Repository.NAMESPACE)).setText(getDescription(locale));
		
		for (Parameter parameter : getParameters()) {
			result.add(parameter.toElement(locale));
		}
		return result;
	}

	public boolean equals(Object obj) {
		if (!ComponentMetadata.class.isAssignableFrom(obj.getClass())) {
			return false;
		}
		
		ComponentMetadata other = (ComponentMetadata)obj;
		return this.getId().equals(other.getId());
	}

	public int hashCode() {
		return getId().hashCode();
	}
}
