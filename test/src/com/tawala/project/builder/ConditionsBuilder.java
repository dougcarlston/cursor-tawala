/**
 * 
 */
package com.tawala.project.builder;

import com.scissor.XmlBuffer;

public class ConditionsBuilder extends TagBuilder {
	private boolean omitEnclosingTag = false;

	public void addComparison(String comparison, String field, String type,
			String nameOrValue) {
		contents().startTag(comparison, "field", field);

		addFieldOrValue(contents(), type, nameOrValue);
		contents().endTag(comparison);

	}

	public void addMCToMCComparison(String comparison, String field1,
			String field2) {

		contents().startTag(comparison, "field", field1);
		contents().tag("string", "field", field2);
		contents().endTag(comparison);
	}

	public void addMCComparison(String field, String comparison,
			String nameOrValue) {
		contents().tag(comparison, "field", field, "value", nameOrValue);
	}

	public void addMCComparisonAdvancedIfStyle(String field, String comparison,
			String nameOrValue) {
		contents().startTag(comparison, true, "field", field);
		contents().tag("string", false, "value", nameOrValue);
		contents().endTag(comparison);
	}

	private void addFieldOrValue(XmlBuffer buf, String type, String nameOrValue) {
		if (type.equals("field")) {
			addField(buf, nameOrValue);
		} else {
			addValue(buf, nameOrValue);
		}
	}

	private void addValue(XmlBuffer buf, String value) {
		buf.tag("string", "value", value);
	}

	private void addField(XmlBuffer buf, String name) {
		buf.tag("field", "name", name);
	}

	protected void startTag(XmlBuffer xml) {
		if (!omitEnclosingTag) {
			xml.startTag("conditions");
		}
	}

	protected void endTag(XmlBuffer xml) {
		if (!omitEnclosingTag) {
			xml.endTag("conditions");
		}
	}

	public void startAnd() {
		contents().startTag("and");
	}

	public void endAnd() {
		contents().endTag("and");
	}

	public void startOr() {
		contents().startTag("or");
	}

	public void endOr() {
		contents().endTag("or");
	}

	public boolean isOmitEnclosingTag() {
		return omitEnclosingTag;
	}

	public void setOmitEnclosingTag(boolean omitEnclosingTag) {
		this.omitEnclosingTag = omitEnclosingTag;
	}
}