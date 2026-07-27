package com.tawala.project.builder;

import com.scissor.XmlBuffer;
import com.tawala.project.commands.RecordSelector;

public class GetBuilder extends ProcessBlockBuilder {
	private String recordListName;
	private ConditionsBuilder conditions;

	public GetBuilder(String recordListName, String... forms) {
		super(Type.get);
		this.recordListName = recordListName;
		contents().startTag("forms");
		for (int i = 0; i < forms.length; i++) {
			contents().tag("form", "name", forms[i]);
		}
		contents().endTag("forms");
		conditions = new ConditionsBuilder();
		add(conditions);
	}

	/*
	 * Each form is an Object array, first element is the form name, the second
	 * is the Boolean indicating whether the form is external datasource
	 */
	public GetBuilder(String recordListName, Object[]... forms) {
		super(Type.get);
		this.recordListName = recordListName;
		contents().startTag("forms");
		for (int i = 0; i < forms.length; i++) {
			contents().tag("form", "name", (String) forms[i][0],
					RecordSelector.EXTERNAL_SHARED_DATA_ATTRIBUTE_NAME,
					forms[i][1].toString());
		}
		contents().endTag("forms");
		conditions = new ConditionsBuilder();
		add(conditions);
	}

	public ConditionsBuilder conditions() {
		return conditions;
	}

	protected void startTag(XmlBuffer xml) {
		xml.startTag("get", "recordList", this.recordListName);
	}

	protected void endTag(XmlBuffer xml) {
		xml.endTag("get");
	}

}
