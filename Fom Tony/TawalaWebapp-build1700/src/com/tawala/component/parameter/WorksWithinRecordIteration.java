package com.tawala.component.parameter;

import org.dom4j.DocumentHelper;
import org.dom4j.Element;
import org.dom4j.QName;

import com.tawala.component.ParameterRestriction;
import com.tawala.component.repository.Repository;

public class WorksWithinRecordIteration implements ParameterRestriction {
	private static final String ELEMENT_NAME = "works-within-record-iteration";
	private final String recordListName;
	private final When when;
	
	public WorksWithinRecordIteration(When when, String recordListName) {
		this.when = when;
		this.recordListName = recordListName;
	}
	

	public Element toElement() {
		Element result = DocumentHelper.createElement(new QName(ELEMENT_NAME, Repository.NAMESPACE));
		result.addAttribute("when", when.toString());
		if(recordListName != null) {
			result.addAttribute("record-list-name", recordListName);
		}
		return result;
	}

	public static enum When {
		always,
		never
	}
}
