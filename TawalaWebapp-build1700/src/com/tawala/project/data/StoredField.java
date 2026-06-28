package com.tawala.project.data;

import org.dom4j.Element;
import org.json.JSONException;
import org.json.JSONObject;

public interface StoredField {
	String TYPE_ATTRIBUTE_NAME = "type";
	String ELEMENT_NAME = "field";
	
	String getName();
	void toXml(Element dataSourceElement);
	FieldSetter getFieldSetter();
	JSONObject getDataTableEditorInfo() throws JSONException;
}

