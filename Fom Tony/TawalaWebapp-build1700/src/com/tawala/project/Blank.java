package com.tawala.project;

import java.util.ArrayList;
import java.util.List;

import org.json.JSONException;
import org.json.JSONObject;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.repository.Repository;
import com.tawala.component.validator.FieldValidator;
import com.tawala.component.validator.NonEmptyFIBValidator;
import com.tawala.component.web.ResponseCreator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.project.data.FieldSetter;
import com.tawala.project.data.SingleValueFieldSetter;
import com.tawala.project.data.StoredField;
import com.tawala.project.data.StringField;
import com.tawala.project.fib.FillInBlankLayout;
import com.tawala.project.formatting.ValidationSupport;
import com.tawala.web.oldhtml.Anchor;
import com.tawala.web.oldhtml.AttributeSupport;
import com.tawala.web.oldhtml.Block;
import com.tawala.web.oldhtml.FileInput;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlString;
import com.tawala.web.oldhtml.TextInput;

public class Blank implements Field, FormRenderable {
	private final String id;
	private final String alternateLabel;
	private String fibPrefix = "";
	private final int length;
	private final int height;
	private boolean required;
	private final boolean isFileUpload;
	private List<FieldValidator> validators;
	private FormItem htmlContainer;

	public Blank(ConfigElement config) {
		this(config.attribute("label").stringValue(), config.attribute(
				"alternateLabel").stringValue(), config.attribute("length")
				.intValue(50), config.attribute("height").intValue(1), config
				.attribute("required").booleanValue(), config.getName().equals(
				"fileNameInput"));
		validators = Repository.instantiateFIBValidators(config
				.child("validator"));
		addRequiredValidatorIfNeeded();
	}
	
	

	private void addRequiredValidatorIfNeeded() {
		if (required) {
			if (validators == null) {
				validators = new ArrayList<FieldValidator>(1);
			}
			validators.add(new NonEmptyFIBValidator());
		}
	}

	public Blank(String id, int length) {
		this(id, length, 1);
	}

	public Blank(String id, int length, int height) {
		this(id, null, length, height, false, false);
	}

	private Blank(String id, String alternateLabel, int length, int height,
			boolean required, boolean isFileUpload) {
		this.id = id;
		this.alternateLabel = alternateLabel;
		this.length = Math.min(length, 60);
		this.height = Math.max(height, 1);
		this.required = required;
		this.isFileUpload = isFileUpload;
	}

	public int getLength() {
		return length;
	}

	public String getId() {
		if (isFileUpload) {
			return fibPrefix;
		} else {
			return id;
		}
	}

	// TODO: this should handle encoding
	public String getHtmlId() {
		if (isFileUpload) {
			return fibPrefix;
		} else {
			return blankId();
		}
	}

	private String blankId() {
		if (alternateLabel == null) {
			return fibPrefix + getId();
		} else {
			return alternateLabel;
		}
	}

	public boolean isRequired() {
		return required;
	}

	void setParentPrefix(String htmlIdPrefix) {
		this.fibPrefix = htmlIdPrefix;
	}

	public Html toHtml(ExecutionContext context) {
		FillInBlankLayout currentFillInBlankRenderer = context
				.getCurrentFillInBlankLayoutManager();
		if (currentFillInBlankRenderer == null) {
			return defaultRendering(context);
		} else {
			return currentFillInBlankRenderer.render(this, context);
		}
	}

	public Html defaultRendering(ExecutionContext context) {
		HtmlItems htmlItems = new HtmlItems();
		if (context.isPreviewMode()) {
			htmlItems.add(new Anchor("anchor-" + getHtmlId()));
		}
		AttributeSupport inputElement = null;
		if (isFileUpload) {
			FileInput fileInput = new FileInput(getHtmlId(), getLength());
			inputElement = fileInput;
			htmlItems.add(fileInput);
		} else {
			TextInput textInput = new TextInput(getHtmlId(), getLength(),
					getHeight(), getLastEnteredValue(context));
			inputElement = textInput;

			htmlItems.add(textInput);
		}
		if (validators != null && validators.size() > 0) {
			inputElement.setAttribute("onblur",
					"Tawala.validation.validate(this);");
			htmlItems.add(ValidationSupport.createRegisterFormValidatorsScript(context,
					TextInput.elementId(getHtmlId()), validators));
		}

		if (isRequired()) {
			Block star = new Block("span", false, new HtmlString(" *"));
			star.setAttribute("class", "qinfo");
			htmlItems.add(star);
		}
		return htmlItems;
	}

	private Value getLastEnteredValue(ExecutionContext context) {

		String recordName = "Record";
		String formName = context.getSubmission().getFormName();
		Form form = context.getForm();
		if (form == null) {
			return Value.NULL;
		}

		Reference reference = new Reference(getHtmlId());
		if (context.getSubmission().isFieldSet(reference)) {
			return context.getSubmission().getValue(reference);
		} else {
			if (form.isDataEntryOnly()) {
				context.mapLastRecord(recordName, formName);

				return context.getValue(fullyQualifiedFieldName(recordName,
						formName));
			} else {
				return Value.NULL;
			}
		}
	}

	private String fullyQualifiedFieldName(String recordName, String formName) {
		return recordName + ":" + formName + ":" + getHtmlId();
	}

	public String getAlternateLabel() {
		return alternateLabel;
	}

	int getHeight() {
		return height;
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public StoredField getStoredFieldDefinition() {
		return new StringField(blankId());
	}

	public boolean canBeUsedToReplaceText() {
		return true;
	}

	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		return null;
	}

	public FieldSetter getFieldSetter() {
		return new SingleValueFieldSetter(getHtmlId());
	}

	public JSONObject getDataTableEditorInfo(UserProject userProject)
			throws JSONException {
		JSONObject result = new JSONObject();
		if (height > 1) {
			result.put("editor", "textarea");
			result.put("formatter", "formatMultilineText");
		} else {
			result.put("editor", "textbox");
		}
		return result;
	}

	public boolean isFileUpload() {
		return isFileUpload;
	}

	public void setRequired(boolean required) {
		this.required = required;
		addRequiredValidatorIfNeeded();
	}

	public boolean expectsFileUpload() {
		return this.isFileUpload;
	}

	public List<FieldValidator> getFieldValidators() {
		return validators;
	}

	public boolean wasDisplayedOnPreviousPage(ExecutionContext context) {
		return getHtmlContainer().shouldBeDisplayed(context);
	}

	public FormItem getHtmlContainer() {
		return htmlContainer;
	}

	public void setHtmlContainer(FormItem containerElement) {
		this.htmlContainer = containerElement;
	}
}
