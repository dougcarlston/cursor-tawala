package com.tawala.component.web.form;

import java.util.List;
import java.util.Set;

import com.tawala.component.web.ResponseCreator;
import com.tawala.project.Checkbox;
import com.tawala.project.FormRenderable;
import com.tawala.project.MultipleChoice;
import com.tawala.project.Paragraph;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Div;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlString;
import com.tawala.web.oldhtml.Label;
import com.tawala.web.oldhtml.Span;

public class DefaultMultiChoiceDisplayComponent implements
		MultiChoiceDisplayComponent {
	protected static final HtmlString REQUIRED_INDICATOR = new HtmlString(" *");
	private MultipleChoice question;

	public DefaultMultiChoiceDisplayComponent(MultipleChoice question) {
		this.question = question;
	}

	MultipleChoice getQuestion() {
		return question;
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public Html toHtml(ExecutionContext context, Set<String> selectedValues) {
		boolean viewOnlyMode = selectedValues != null;
		
		Div mcDiv = new Div("id", getQuestion().getContainerDivId());
		if (getQuestion().onlyOne()) {
			mcDiv.setAttribute("class", getOnlyOneStyleClass() + addCondensedAttribute(getQuestion().isDisplayCondensed()));
		} else {
			mcDiv.setAttribute("class", getMultipleStyleClass() + addCondensedAttribute(getQuestion().isDisplayCondensed()));
		}

		if (!viewOnlyMode) {
			addQuestionLabel(context, mcDiv);
		}

		displayChoices(context, mcDiv, selectedValues);

		return mcDiv;
	}

	static String addCondensedAttribute(boolean displayCondensed) {
		return (displayCondensed ? " condensed" : "");
	}

	private void addQuestionLabel(ExecutionContext context, Div mcDiv) {
		Label qLabel = new Label("class", "question");
		
		boolean isEmpty = false;

		List<FormRenderable> questionContents = getQuestion()
				.getQuestionText().getContents();
		// TODO: see comments in Checkbox.toHtml().
		if (questionContents.size() > 0
				&& questionContents.get(0) instanceof Paragraph) {
			Paragraph paragraph = (Paragraph) questionContents.get(0);
			
			isEmpty = paragraph.isEmpty(context);
			
			qLabel.add(getQuestion().isRequired() ? paragraph.toHtml(
					context, REQUIRED_INDICATOR) : paragraph
					.toHtml(context));
		} else {
			for (FormRenderable renderable : questionContents) {
				isEmpty = renderable.isEmpty(context);
				if(! isEmpty) {
					break;
				}
			}
			qLabel.appendContents(questionContents, context);
			if (getQuestion().isRequired()) {
				qLabel.add(REQUIRED_INDICATOR);
			}
		}

		if(! isEmpty) {
			mcDiv.add(qLabel);
		}
	}

	protected String getMultipleStyleClass() {
		return "mcCheckbox";
	}

	protected String getOnlyOneStyleClass() {
		return "mcRadio";
	}

	protected void displayChoices(ExecutionContext context,
			HtmlItems enclosingContainer, Set<String> selectedValues) {
		for (Checkbox checkbox : getQuestion().getDataProvider().getChoices(
				context)) {
			Span aSpan = new Span("class", "answer");
			aSpan.add(checkbox.toHtml(context, selectedValues));
			enclosingContainer.add(aSpan);
		}
	}

	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		return null;
	}

	public Html toHtml(ExecutionContext context) {
		return toHtml(context, null);
	}
}
