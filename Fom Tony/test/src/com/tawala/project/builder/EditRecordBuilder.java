package com.tawala.project.builder;

import com.scissor.XmlBuffer;

public class EditRecordBuilder extends ProcessBlockBuilder {
	private final String formName;
	private final boolean updateExistingRecord;
	private final ConditionsBuilder conditions;

	public EditRecordBuilder(String formName, boolean updateExistingRecord) {
		super(Type.edit);
		this.formName = formName;
		this.updateExistingRecord = updateExistingRecord;
		conditions = new ConditionsBuilder();
		add(conditions);
	}

	public ConditionsBuilder conditions() {
		return conditions;
	}

	protected void startTag(XmlBuffer xml) {
		xml.startTag("edit", "form", this.formName, "submit", updateExistingRecord ? "update" : "new");
	}

	protected void endTag(XmlBuffer xml) {
		xml.endTag("edit");
	}

}
