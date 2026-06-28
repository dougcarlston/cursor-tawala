package com.tawala.project.data;

import org.dom4j.Element;
import org.json.JSONException;
import org.json.JSONObject;

import com.scissor.xmlconfig.ConfigElement;

public class MultiChoiceField implements StoredField {
	private static final String NAME_ATTRIBUTE_NAME = "name";
	private static final String ONE_ONLY_ATTRIBUTE_NAME = "onlyone";
	private static final String CHOICES_ATTRIBUTE_NAME = "choices";

	public static final String TYPE = "mcq";

	private final String name;
	private final int choiceCount;
	private final boolean oneOnly;

	public MultiChoiceField(String name, int choiceCount, boolean oneOnly) {
		this.name = name;
		this.choiceCount = choiceCount;
		this.oneOnly = oneOnly;
	}

	public MultiChoiceField(ConfigElement configElement) {
		this.name = configElement.attribute(NAME_ATTRIBUTE_NAME).stringValue();
		this.choiceCount = configElement.attribute(CHOICES_ATTRIBUTE_NAME)
				.intValue();
		this.oneOnly = configElement.attribute(ONE_ONLY_ATTRIBUTE_NAME)
				.booleanValue();
	}

	public void toXml(Element dataSourceElement) {
		Element field = dataSourceElement.addElement(StoredField.ELEMENT_NAME);
		field.addAttribute(NAME_ATTRIBUTE_NAME, name);
		field.addAttribute(TYPE_ATTRIBUTE_NAME, TYPE);
		field.addAttribute(CHOICES_ATTRIBUTE_NAME, Integer
				.toString(choiceCount));
		field.addAttribute(ONE_ONLY_ATTRIBUTE_NAME, Boolean.toString(oneOnly));
	}

	public String getName() {
		return name;
	}

	@Override
	public int hashCode() {
		final int PRIME = 31;
		int result = 1;
		result = PRIME * result + choiceCount;
		result = PRIME * result + ((name == null) ? 0 : name.hashCode());
		result = PRIME * result + (oneOnly ? 1231 : 1237);
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		final MultiChoiceField other = (MultiChoiceField) obj;
		if (choiceCount != other.choiceCount)
			return false;
		if (name == null) {
			if (other.name != null)
				return false;
		} else if (!name.equals(other.name))
			return false;
		if (oneOnly != other.oneOnly)
			return false;
		return true;
	}

	public FieldSetter getFieldSetter() {
		return oneOnly ? new SingleValueFieldSetter(getName())
				: new MultiValueFieldSetter(getName());
	}

	public JSONObject getDataTableEditorInfo() throws JSONException {
		JSONObject result = new JSONObject();
		result.put("editor", "textbox");
		return result;
	}
}
