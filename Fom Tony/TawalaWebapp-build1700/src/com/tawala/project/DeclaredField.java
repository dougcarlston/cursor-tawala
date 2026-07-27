package com.tawala.project;

import java.util.List;

import org.json.JSONException;
import org.json.JSONObject;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.validator.FieldValidator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.data.FieldSetter;
import com.tawala.project.data.SingleValueFieldSetter;
import com.tawala.project.data.StoredField;
import com.tawala.project.data.StringField;

public class DeclaredField implements Field {
	private final String name;

	public DeclaredField(ConfigElement configElement) {
		this.name = configElement.attribute("name").stringValue();
	}
	
	public String getHtmlId() {
		return name;
	}

	public String getId() {
		return name;
	}

	public StoredField getStoredFieldDefinition() {
		return new StringField(name);
	}

	public boolean isRequired() {
		return false;
	}

	public boolean canBeUsedToReplaceText() {
		return false;
	}

	public FieldSetter getFieldSetter() {
		return new SingleValueFieldSetter(getHtmlId());
	}

	public JSONObject getDataTableEditorInfo(UserProject userProject) throws JSONException{
		JSONObject result = new JSONObject();
		result.put("editor", "textbox");
		return result;
	}

	public boolean expectsFileUpload() {
		return false;
	}

	public List<FieldValidator> getFieldValidators() {
		return null;
	}

	public boolean wasDisplayedOnPreviousPage(ExecutionContext context) {
		throw new IllegalStateException("Unexpected call to " + this.getClass());
	}
}
