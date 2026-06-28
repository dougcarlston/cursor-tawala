package com.tawala.project;

import java.io.PrintWriter;
import java.util.List;
import java.util.Set;

import org.springframework.web.util.HtmlUtils;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.project.formatting.ContainerElement;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.HtmlString;
import com.tawala.web.oldhtml.Image;
import com.tawala.web.oldhtml.RenderingContext;

public class Checkbox {
	private final MultipleChoice parent;

	private final String id;

	private final ContainerElement description;

	private static Factory<FormRenderable> DESCRIPTION_FACTORY = new Factory<FormRenderable>();
	static {
		DESCRIPTION_FACTORY.setKeepWhitespace(false);
		DESCRIPTION_FACTORY.register("paragraph", FormParagraph.class);
		DESCRIPTION_FACTORY.registerText(Text.class);
	}

	public Checkbox(MultipleChoice parent, ConfigElement config) {
		this.parent = parent;
		this.id = config.attribute("label").stringValue();
		this.description = new ContainerElement();
		this.description.addElements(DESCRIPTION_FACTORY.makeChildren(config));
	}

	public Checkbox(MultipleChoice parent, String value, String description) {
		this.parent = parent;
		this.id = value;
		this.description = new ContainerElement();
		this.description.addElement(new HtmlString(description));
	}

	public String getId() {
		return id;
	}

	public boolean isEmpty(ExecutionContext context) {
		return description.isEmpty(context);
	}

	public Html toHml(ExecutionContext context) {
		return toHtml(context, null);
	}

	/**
	 * @param context
	 * @param selectedValues
	 *            passed if the checkbox needs to produce a view-only display.
	 * 
	 * @return
	 */
	public Html toHtml(ExecutionContext context, Set<String> selectedValues) {
		HtmlItems result = new HtmlItems();
		if (selectedValues == null) {
			CheckboxHtml checkboxHtml = new CheckboxHtml(this,
					lastSelectedValueIsChecked(context));
			result.add(checkboxHtml);
		} else {
			if (selectedValues.contains(getId())) {
				result.add(new Image("/images/checkbox_on.gif", "Selected", 12,
						12));
			} else {
				result.add(new Image("/images/checkbox_off.gif",
						"Not selected", 12, 12));
			}
			result.add(new HtmlReadyString(" "));
		}

		CheckboxHtml checkbox = new CheckboxHtml(this,
				lastSelectedValueIsChecked(context));

		result.add(new HtmlReadyString("<label for=\"" + checkbox.formId + "-" + HtmlUtils.htmlEscape(checkbox.choiceId) + "\">"));
		appendDescription(context, result);
		result.add(new HtmlReadyString("</label>"));
		return result;
	}

	private boolean lastSelectedValueIsChecked(ExecutionContext context) {
		String recordName = "Record";
		String formName = context.getSubmission().getFormName();
		Form form = context.getForm();
		if (form == null) {
			return false;
		}

		Reference reference = new Reference(getFormId());
		Value value = new Value(id);
		if (context.getSubmission().isFieldSet(reference)) {
			return context.getSubmission().contains(reference, id);
		} else if (form.isDataEntryOnly()) {
			context.mapLastRecord(recordName, formName);
			List<Value> values = context.getValues(fullyQualifiedFieldName(
					recordName, formName));
			if (values == null) {
				return false;
			} else {
				return values.contains(value);
			}
		} else {
			return false;
		}
	}

	private String fullyQualifiedFieldName(String recordName, String formName) {
		return recordName + ":" + formName + ":" + parent.getHtmlId();
	}

	public void appendDescription(ExecutionContext context, HtmlItems result) {
		// TODO: this is very ugly. Hopefully this code will get reworked and we
		// won't have to translate between the XML in project definition layout
		// and HTML.
		if (description.getContents().size() > 0
				&& description.getContents().get(0).getClass().equals(
						FormParagraph.class)) {
			FormParagraph paragraph = (FormParagraph) description.getContents()
					.get(0);
			result.appendContents(paragraph.getContents(), context);
		} else {
			result.appendContents(description.getContents(), context);
		}
	}

	public String getFormId() {
		return parent.getHtmlId();
	}

	private static class CheckboxHtml implements Html {
		private final String formId;

		private final String choiceId;

		private final boolean onlyOne;

		private final boolean checked;

		public CheckboxHtml(Checkbox checkbox, boolean checked) {
			this.formId = checkbox.getFormId();
			this.choiceId = checkbox.getId();
			this.onlyOne = checkbox.parent.onlyOne();
			this.checked = checked;
		}

		public void render(PrintWriter out, RenderingContext renderingContext) {
			out.append("<input");
			if (type() == "checkbox") {
				out.append(" class=\"checkbox\"");
			} else {
				out.append(" class=\"radio\"");
			}
			out.append(" name=\"" + formId + "\"");
			out.append(" id=\"" + formId  + "-" + HtmlUtils.htmlEscape(choiceId) + "\"");
			out.append(" type=\"" + type() + "\"");
			out.append(" value=\"" + HtmlUtils.htmlEscape(choiceId) + "\"");
			if (checked)
				out.append(" checked=\"checked\"");
			out.append(" /> ");
		}

		private String type() {
			return onlyOne ? "radio" : "checkbox";
		}
	}
}
