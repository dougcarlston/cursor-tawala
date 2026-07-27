package com.tawala.project.data;

import org.dom4j.Element;
import org.json.JSONException;
import org.json.JSONObject;

import com.scissor.xmlconfig.ConfigElement;

public class StringField implements StoredField {
	public static final String TYPE = "string";
	private final String name;

	public StringField(String name) {
		this.name = name;
	}

	public StringField(ConfigElement configElement) {
		this.name = configElement.attribute("name").stringValue();
	}

	public void toXml(Element dataSourceElement) {
		Element field = dataSourceElement.addElement(StoredField.ELEMENT_NAME);
		field.addAttribute("name", name);
		field.addAttribute(StoredField.TYPE_ATTRIBUTE_NAME, TYPE);
	}

	public String getName() {
		return name;
	}

	@Override
	public int hashCode() {
		final int PRIME = 31;
		int result = 1;
		result = PRIME * result + ((name == null) ? 0 : name.hashCode());
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
		final StringField other = (StringField) obj;
		if (name == null) {
			if (other.name != null)
				return false;
		} else if (!name.equals(other.name))
			return false;
		return true;
	}

	public static boolean isValidFieldName(String fieldName) {
		char firstCharacter = fieldName.charAt(0);
		if (!(Character.isLetter(firstCharacter) || firstCharacter == '_')) {
			return false;
		}
		if (fieldName.startsWith("__")) {
			return false;
		}
		if (fieldName.indexOf(':') > -1) {
			return false;
		}

		return true;
	}

	public FieldSetter getFieldSetter() {
		return new SingleValueFieldSetter(getName());
	}

	public JSONObject getDataTableEditorInfo() throws JSONException {
		JSONObject result = new JSONObject();
		result.put("editor", "textbox");
		return result;
	}
}
