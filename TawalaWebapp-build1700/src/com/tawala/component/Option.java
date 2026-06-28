package com.tawala.component;

import java.util.Locale;

import org.dom4j.DocumentHelper;
import org.dom4j.Element;
import org.dom4j.QName;

import com.tawala.component.repository.Repository;
import com.tawala.message.Message;

public class Option {
	private final String value;
	private final Message descriptionMessage;

	public Option(String value, String descriptionMessageId) {
		this.value = value;
		this.descriptionMessage = new Message(descriptionMessageId);
	}

	public Element toElement(Locale locale) {
		Element result = DocumentHelper.createElement(new QName("choice",
				Repository.NAMESPACE));
		result.addAttribute("value", value);
		result.addAttribute("description", getDescription(locale));

		return result;
	}

	public String getDescription(Locale locale) {
		return Repository.getMessageSource().getMessage(descriptionMessage,
						locale);
	}

	public String getValue() {
		return value;
	}
}
