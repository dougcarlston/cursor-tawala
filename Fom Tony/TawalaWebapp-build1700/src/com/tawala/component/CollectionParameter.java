package com.tawala.component;

import java.util.ArrayList;
import java.util.List;
import java.util.Locale;

import org.dom4j.Element;
import org.dom4j.QName;

import com.tawala.component.repository.Repository;

public class CollectionParameter extends Parameter {
	private final String sourceId;
	private List<Parameter> nestedParameters = new ArrayList<Parameter>();

	public CollectionParameter(String id, String resourceId, String sourceId, boolean required) {
		super(id, resourceId, ParameterType.COLLECTION, required);
		this.sourceId = sourceId;
	}

	@Override
	protected void addTypeSpecificElements(Element result, Locale locale) {
		result.addElement(new QName("source-parameter", Repository.NAMESPACE)).addAttribute("source-id", sourceId);
		
		for (Parameter parameter : nestedParameters) {
			result.add(parameter.toElement(locale));
		}
	}
	
	public CollectionParameter addNestedParameters(Parameter [] parameters) {
		for (int i = 0; i < parameters.length; i++) {
			nestedParameters.add(parameters[i]);
		}
		return this;
	}
}
