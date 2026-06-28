package com.tawala.component;

import java.util.Locale;

import org.dom4j.Element;
import org.dom4j.QName;

import com.tawala.component.repository.Repository;

public class NumericListParameter extends Parameter {
	private final int minimum;
	private final int maximum;

	public NumericListParameter(String id, String resourceId, boolean required, int minimum, int maximum) {
		super(id, resourceId, ParameterType.NUMERIC_LIST, required);
		this.minimum = minimum;
		this.maximum = maximum;
	}

	@Override
	protected void addTypeSpecificElements(Element result, Locale locale) {
		result.addElement(new QName("minimum", Repository.NAMESPACE)).addAttribute("value", String.valueOf(minimum));
		result.addElement(new QName("maximum", Repository.NAMESPACE)).addAttribute("value", String.valueOf(maximum));
	}
}
