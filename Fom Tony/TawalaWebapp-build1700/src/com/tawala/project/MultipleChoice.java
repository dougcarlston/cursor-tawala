package com.tawala.project;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.component.validator.FieldValidator;
import com.tawala.component.validator.NonEmptyMCQValidator;
import com.tawala.component.web.ResponseCreator;
import com.tawala.component.web.form.DefaultMultiChoiceDataProvider;
import com.tawala.component.web.form.DefaultMultiChoiceDisplayComponent;
import com.tawala.component.web.form.DynamicMultiChoiceDataProvider;
import com.tawala.component.web.form.HorizontalMultiChoiceDisplayComponent;
import com.tawala.component.web.form.MultiChoiceDataProvider;
import com.tawala.component.web.form.MultiChoiceDisplayComponent;
import com.tawala.component.web.form.MultiColumnMultiChoiceDisplayComponent;
import com.tawala.component.web.form.VerticalMultiChoiceDisplayComponent;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.data.FieldSetter;
import com.tawala.project.data.MultiChoiceField;
import com.tawala.project.data.MultiValueFieldSetter;
import com.tawala.project.data.SingleValueFieldSetter;
import com.tawala.project.data.StoredField;
import com.tawala.project.formatting.ContainerElement;
import com.tawala.project.formatting.ValidationSupport;
import com.tawala.web.WorldInitializer;
import com.tawala.web.oldhtml.Anchor;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;

public class MultipleChoice extends Question implements Field {
	private final boolean onlyOne;
	private final boolean required;
	private final String style;
	private final int columnCount;
	private final ContainerElement questionText;
	private final List<Checkbox> items = new ArrayList<Checkbox>();
	private final MultiChoiceDataProvider dataProvider;
	protected MultiChoiceDisplayComponent displayComponent;
	private final boolean displayCondensed;
	private List<FieldValidator> validators;

	private static Factory<FormRenderable> QUESTION_FACTORY = new Factory<FormRenderable>();
	static {
		QUESTION_FACTORY.setKeepWhitespace(false);
		QUESTION_FACTORY.register("paragraph", McParagraph.class);
		QUESTION_FACTORY.registerText(Text.class);
	}

	private static Factory<MultiChoiceDataProvider> DATA_PROVIDER_COMPONENT_FACTORY = new Factory<MultiChoiceDataProvider>();
	static {
		DATA_PROVIDER_COMPONENT_FACTORY.register(
				DefaultMultiChoiceDataProvider.COMPONENT_ID,
				DefaultMultiChoiceDataProvider.Runtime.class);
		DATA_PROVIDER_COMPONENT_FACTORY.register(
				DynamicMultiChoiceDataProvider.COMPONENT_ID,
				DynamicMultiChoiceDataProvider.Runtime.class);
	}

	public MultipleChoice(ConfigElement config) {
		super(config);
		this.onlyOne = config.attribute("onlyone").booleanValue();
		this.required = config.attribute("required").booleanValue();
		this.style = config.attribute("style").stringValue();
		this.displayCondensed = config.hasAttribute("paddingBottom")
				&& !config.attribute("paddingBottom").booleanValue();
		columnCount = (config.hasAttribute("columnCount") ? config.attribute(
				"columnCount").intValue() : 0);
		questionText = new ContainerElement(QUESTION_FACTORY
				.makeChildren(config.child("question")));
		List<ConfigElement> choices = config.children("choice");
		for (ConfigElement choice : choices) {
			items.add(new Checkbox(this, choice));
		}

		ConfigElement displayConfigElement = config.child("data-provider");

		if (displayConfigElement == null) {
			dataProvider = new DefaultMultiChoiceDataProvider.Runtime();
		} else {
			List<MultiChoiceDataProvider> components = DATA_PROVIDER_COMPONENT_FACTORY
					.makeChildren(displayConfigElement);
			if (components.size() == 0) {
				dataProvider = new DefaultMultiChoiceDataProvider.Runtime();
			} else {
				dataProvider = components.get(0);
			}
		}

		dataProvider.setQuestion(this);

		String style = config.attribute("style").stringValue();
		if (style == null) {
			displayComponent = new DefaultMultiChoiceDisplayComponent(this);
		} else if (style.equals("vertical")) {
			displayComponent = new VerticalMultiChoiceDisplayComponent(this);
		} else if (style.equals("horizontal")) {
			displayComponent = new HorizontalMultiChoiceDisplayComponent(this);
		} else if (style.equals("multicolumn")) {
			displayComponent = new MultiColumnMultiChoiceDisplayComponent(this);
		} else {
			Log.error(this, "Unknown MCQ style '" + style + "'");
			displayComponent = new DefaultMultiChoiceDisplayComponent(this);
		}
		
		if(isRequired()) {
			if(validators == null) {
				validators = new ArrayList<FieldValidator>(1);
			}
			validators.add(new NonEmptyMCQValidator(this));
		}
	}

	public boolean hasFields() {
		return true;
	}

	public List<Field> fields() {
		return Collections.singletonList((Field) this);
	}

	public Html produceHtml(ExecutionContext context) {
		Html displayHTML = displayComponent.toHtml(context);
		if (context.isPreviewMode()) {
			HtmlItems result = new HtmlItems();
			result.add(new Anchor("anchor-" + getHtmlId()));
			result.add(displayHTML);
			return result;
		} else {
			if(isRequired()) {
				HtmlItems result = new HtmlItems();
				result.add(displayHTML);
				result.add(ValidationSupport.createRegisterFormValidatorsScript(context, getContainerDivId(), validators));
				return result;
			} else {
				return displayHTML;
			}
		}
	}

	public boolean onlyOne() {
		return onlyOne;
	}

	public boolean isRequired() {
		return required;
	}

	public String getStyle() {
		return style;
	}

	public int getColumnCount() {
		return columnCount;
	}

	/**
	 * @return Returns the items.
	 */
	public List<Checkbox> getItems(ExecutionContext executionContext) {
		return dataProvider.getChoices(executionContext);
	}

	public List<Checkbox> getDefaultItems() {
		return items;
	}

	public ContainerElement getQuestionText() {
		return questionText;
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public MultiChoiceDataProvider getDataProvider() {
		return dataProvider;
	}

	public StoredField getStoredFieldDefinition() {
		// --- TODO: items size works only for DefaultDataProvider.
		return new MultiChoiceField(getHtmlId(), items.size(), onlyOne());
	}

	public boolean canBeUsedToReplaceText() {
		return false;
	}

	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		return null;
	}

	public FieldSetter getFieldSetter() {
		return onlyOne ? new SingleValueFieldSetter(getHtmlId())
				: new MultiValueFieldSetter(getHtmlId());
	}

	public MultiChoiceDisplayComponent getDisplayComponent() {
		return displayComponent;
	}

	public JSONObject getDataTableEditorInfo(UserProject userProject)
			throws JSONException {
		JSONObject result = new JSONObject();
		if (getDataProvider().isSafeToGetChoicesWithoutRealContext()) {
			result.put("editor", onlyOne() ? "dropdown" : "checkbox");

			JSONObject editorOption = new JSONObject();

			ExecutionContext context = new ExecutionContext(WorldInitializer
					.getDefaultWorld().domain(), userProject);
			List<Checkbox> choices = getDataProvider().getChoices(context);
			if (onlyOne) {
				JSONArray values = new JSONArray();
				JSONObject removeSelection = new JSONObject();
				removeSelection.put("text", "No value");
				removeSelection.put("value", "");
				values.put(removeSelection);
				for (Checkbox checkbox : choices) {
					values.put(checkbox.getId());
				}

				editorOption.put("dropdownOptions", values);
			} else {
				JSONArray values = new JSONArray();
				for (Checkbox checkbox : choices) {
					values.put(checkbox.getId());
				}
				editorOption.put("checkboxOptions", values);
			}

			result.put("editorOptions", editorOption);
		} else {
			// We can't build a list of options. Let's give the user the ability to type the value in.
			result.put("editor", "textbox");
		}

		return result;
	}

	public boolean isDisplayCondensed() {
		return displayCondensed;
	}

	public boolean expectsFileUpload() {
		return false;
	}

	public List<FieldValidator> getFieldValidators() {
		return validators;
	}
	
	public String getContainerDivId() {
		return getHtmlId() + "Container";
	}

	public boolean wasDisplayedOnPreviousPage(ExecutionContext context) {
		return shouldBeDisplayed(context);
	}
}
