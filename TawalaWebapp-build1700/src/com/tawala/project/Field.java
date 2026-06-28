package com.tawala.project;

import java.util.List;

import org.json.JSONException;
import org.json.JSONObject;

import com.tawala.component.validator.FieldValidator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.data.FieldSetter;
import com.tawala.project.data.StoredField;

public interface Field {
    String getId();
    String getHtmlId();
    boolean isRequired();
	StoredField getStoredFieldDefinition();
	boolean canBeUsedToReplaceText();
	FieldSetter getFieldSetter();
	boolean expectsFileUpload();
	JSONObject getDataTableEditorInfo(UserProject userProject) throws JSONException;
	List<FieldValidator> getFieldValidators();
	boolean wasDisplayedOnPreviousPage(ExecutionContext context);
}
