package com.tawala.project;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Map;
import java.util.Set;

import com.scissor.xmlconfig.Format;
import com.tawala.component.validator.FieldValidator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.SkipBlock;
import com.tawala.project.theme.ProjectTheme;
import com.tawala.web.oldhtml.Block;
import com.tawala.web.oldhtml.Div;
import com.tawala.web.oldhtml.HtmlForm;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.HtmlString;
import com.tawala.web.oldhtml.OldPage;
import com.tawala.web.oldhtml.Span;

//--- TODO: refactor to contain a list of  FormItems
@SuppressWarnings("serial")
public class FormSegment extends ArrayList<FormItem> {
	public static final Format FORMAT_WITH_EXPLICIT_BACK_BUTTON_DETECTION_FOR_FORMS = new Format(
			"1.14");
	public static final String CONFIRM_NAVIGATION_AWAY_FROM_PAGE = "confirmNavigationAwayFromPage";
	public static final String TAWALA_PROJECT_FORM_NAME = "tawalaProjectForm";
	public static final String SEGMENT_ID = "_segment";
	public static final String FORM_ID = "_form";
	public static final String EDITED_SUBMISSION_ID = "_fste";
	public static final String NEXT_EXPECTED_TOKEN = "_uio01";
	public static final String CUSTOMIZATION_COMPLETE = "_customization_complete";
	public static final String ORIGINAL_URL = "_uio02";

	private final int id;
	private final String formId;
	private final SkipBlocks skipBlocks = new SkipBlocks();

	public FormSegment(int id, String formId) {
		super();
		this.id = id;
		this.formId = formId;
	}

	public boolean add(FormItem item) {
		item.setFormId(formId);
		return super.add(item);
	}

	public OldPage toPage(ExecutionContext context) {
		return toPage(context, null);
	}

	public OldPage toPage(ExecutionContext context, Map<Field, FieldValidator> invalidFields) {
		Form form = context.getProject().getForm(formId);
		ProjectTheme theme = context.getTheme();

		OldPage page = new OldPage();
		page.addPrintStylesheets(theme.getPrintStylesheetURLs());
		page.addScreenStylesheets(theme.getScreenStylesheetURLs());

		displayErrorMessagesIfAny(context, invalidFields, page);

		Div formDiv = new Div("id", "form");

		HtmlForm htmlForm = new HtmlForm();
		htmlForm.setMethod("POST");
		htmlForm.setAttribute("id", TAWALA_PROJECT_FORM_NAME);
		htmlForm.setAttribute("onSubmit", "return onSubmit(this);");
		if (containsFileUploadControls()) {
			htmlForm.setAttribute("enctype", "multipart/form-data");
		}

		if (!context.isPreviewMode()) {
			String url = context.getUserProject().getUrlToForm(
					context.getLink().getId(), context.getEntryPointType(),
					form, context.getOverridingTheme(),
					context.isAdsExplicitlySurpressed());
			htmlForm.setAttribute("action", url);
		}

		HtmlItems html = new HtmlItems();
		for (FormItem item : this) {
			html.add(item.toHtml(context));
		}
		htmlForm.addContents(html);
		htmlForm.add(hiddenField(FORM_ID, context.getProject()
				.getRandomFormName(form)));
		htmlForm.add(hiddenField(SEGMENT_ID, String.valueOf(id)));
		if (!context.isPreviewMode()) {
			if (form.isDetectBackButtonNavigation()
					|| form
							.getProject()
							.getFormat()
							.isLessThan(
									FORMAT_WITH_EXPLICIT_BACK_BUTTON_DETECTION_FOR_FORMS)) {
				htmlForm.add(hiddenField(NEXT_EXPECTED_TOKEN, context
						.generateNextExpectedPostToken()));

			}
			if (form.isDetectBackButtonNavigation()) {
				// --- See default.js for details.
				htmlForm.add(new HtmlReadyString("<script>"
						+ CONFIRM_NAVIGATION_AWAY_FROM_PAGE
						+ " = true;</script>\n"));
			}
		}
		htmlForm.add(hiddenField(ORIGINAL_URL, context.getOriginalLink()));
		if (context.getSubmission().isBeingEdited()) {
			htmlForm.add(hiddenField(EDITED_SUBMISSION_ID, String
					.valueOf(context.getSubmission().getDatabaseId())));
		}
		htmlForm.add(submitButton(context.isPreviewMode()));
		htmlForm.add(new Div("id", "wait.panel"));

		// To indicate that the customization is complete a marker <span>
		// element is added. A piece of Javascript could be added too, but it's
		// not necessarily the easiest way to handle this case.
		boolean customizationComplete = id == 0
				&& Project.SETUP_WIZARD_FORM_NAMES.contains(formId);
		if (customizationComplete) {
			htmlForm.add(new Span("id", CUSTOMIZATION_COMPLETE));
		}

		formDiv.add(htmlForm);
		page.add(formDiv);
		return page;
	}

	private void displayErrorMessagesIfAny(ExecutionContext context,
			Map<Field, FieldValidator> invalidFields, OldPage page) {
		if (invalidFields != null && invalidFields.size() > 0) {
			Set<String> messages = new LinkedHashSet<String>();
			for (Map.Entry<Field, FieldValidator> mapEntry : invalidFields.entrySet()) {
				messages.add(mapEntry.getValue().getErrorMessage(context));
			}

			Iterator<String> messageIterator = messages.iterator();

			StringBuffer text = new StringBuffer(messageIterator.next());
			while (messageIterator.hasNext()) {
				text.append("\n");
				text.append(messageIterator.next());
			}

			Block html = new Block("div", new HtmlString(text.toString()));
			html.setAttribute("id", "errormsg");
			page.add(html);
		}
	}

	private boolean containsFileUploadControls() {
		for (Field field : fields()) {
			if (field instanceof Blank) {
				Blank blank = (Blank) field;
				if (blank.isFileUpload()) {
					return true;
				}
			}
		}
		return false;
	}

	private Block hiddenField(String name, String value) {
		Block input = new Block("input");
		input.setAttribute("type", "hidden");
		input.setAttribute("name", name);
		input.setAttribute("value", value);
		return input;
	}

	private Block submitButton(boolean makeInactive) {
		Div submitDiv = new Div("class", "submitButton");
		Block input = new Block("input");
		input.setAttribute("class", "button");
		input.setAttribute("type", "submit");
		input.setAttribute("value", "submit");
		if (makeInactive) {
			input.setAttribute("disabled", "true");
		}
		submitDiv.add(input);
		return submitDiv;
	}

	public List<Field> fields() {
		List<Field> fields = new ArrayList<Field>();
		for (FormItem formItem : this) {
			if (formItem.hasFields()) {
				for (Field field : formItem.fields()) {
					fields.add(field);
				}
			}
		}
		return fields;
	}

	public void addSkipBlock(SkipBlock skipBlock) {
		skipBlocks.add(skipBlock);
	}

	public SkipBlocks getSkipBlocks() {
		return skipBlocks;
	}

	public boolean containsItemWithId(String skipTo) {
		for (FormItem item : this) {
			if (item.matchesId(skipTo)) {
				return true;
			}
		}
		return false;

	}

	public FormSegment segmentStaringWithId(String skipTo) {
		if (get(0).matchesId(skipTo))
			return this;
		FormSegment result = new FormSegment(id, formId);
		boolean foundStart = false;
		for (FormItem item : this) {
			if (item.matchesId(skipTo))
				foundStart = true;
			if (foundStart)
				result.add(item);
		}
		return result;

	}

	/**
	 * @param submission
	 * @return the Map of fields which failed validation. The value is the validator which caused the failure.
	 */
	public Map<Field, FieldValidator> validate(FormSubmission submission, ExecutionContext context) {
		Map<Field, FieldValidator> invalidFields = new LinkedHashMap<Field, FieldValidator>();
		for (Field field : fields()) {
			if(! field.wasDisplayedOnPreviousPage(context)) {
				continue;
			}
			List<FieldValidator> validators = field.getFieldValidators();
			if(validators == null || validators.size() == 0) {
				continue;
			}
			Reference reference = new Reference(formId, field
					.getHtmlId());

			for (FieldValidator fieldValidator : validators) {
				if(! fieldValidator.isValid(submission, reference, context)) {
					invalidFields.put(field, fieldValidator);
					break;
				}
			}
		}

		return invalidFields;
	}

	public String toString() {
		return "FormSegment{" + "id=" + id + "}";
	}
}
