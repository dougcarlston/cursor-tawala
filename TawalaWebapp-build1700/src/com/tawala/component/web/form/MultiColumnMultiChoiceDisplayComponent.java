package com.tawala.component.web.form;

import java.util.ArrayList;
import java.util.List;
import java.util.Set;

import com.tawala.project.Checkbox;
import com.tawala.project.FormRenderable;
import com.tawala.project.MultipleChoice;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Block;
import com.tawala.web.oldhtml.Div;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.Label;
import com.tawala.web.oldhtml.Span;

public class MultiColumnMultiChoiceDisplayComponent implements
		MultiChoiceDisplayComponent {
	private MultipleChoice question;

	public MultiColumnMultiChoiceDisplayComponent(MultipleChoice question) {
		this.question = question;
	}

	public Html toHtml(ExecutionContext context, Set<String> selectedValues) {
		Div mcDiv = new Div("id", question.getContainerDivId());

		String attributeValue = getQuestion().onlyOne() ? "mcRadio multicolumn"
				: "mcCheckbox multicolumn";
		mcDiv.setAttribute("class", attributeValue
				+ DefaultMultiChoiceDisplayComponent
						.addCondensedAttribute(getQuestion()
								.isDisplayCondensed()));

		if (selectedValues == null) {
			boolean isQuestionEmpty = true;
			for (FormRenderable element : getQuestion().getQuestionText()
					.getContents()) {
				if (!element.isEmpty(context)) {
					isQuestionEmpty = false;
					break;
				}
			}

			if (!isQuestionEmpty) {
				Label questionLabel = new Label("class", "question");
				questionLabel.appendContents(getQuestion().getQuestionText()
						.getContents(), context);

				if (getQuestion().isRequired()) {
					questionLabel
							.add(DefaultMultiChoiceDisplayComponent.REQUIRED_INDICATOR);
				}

				mcDiv.add(questionLabel);
			}
		}

		mcDiv.add(new McAnswerTable(context, selectedValues));

		return mcDiv;
	}

	private class McAnswerTable extends Block {
		public McAnswerTable(ExecutionContext context,
				Set<String> selectedValues) {
			super("table", true);
			setAttribute("class", "answer");

			TableBody tableBody = new TableBody(context, selectedValues);
			add(tableBody);
		}
	}

	private class TableBody extends Block {
		public TableBody(ExecutionContext context, Set<String> selectedValues) {
			super("tbody", true);

			int choiceCount = getQuestion().getItems(context).size();
			int columnCount = (getQuestion().getColumnCount() == 0 ? (int) Math
					.sqrt((double) choiceCount) : getQuestion()
					.getColumnCount());
			columnCount = columnCount == 0 ? 1 : columnCount;
			int rowCount = (choiceCount / columnCount)
					+ (choiceCount % columnCount > 0 ? 1 : 0);

			List<Checkbox> checkboxes = new ArrayList<Checkbox>(columnCount
					* rowCount);

			for (int row = 0; row < rowCount; row++) {
				for (int column = 0; column < columnCount; column++) {
					int sourceIndex = column * rowCount + row;

					if (sourceIndex < choiceCount) {
						Checkbox checkbox = getQuestion().getItems(context)
								.get(sourceIndex);
						checkboxes.add(checkbox);
					} else {
						checkboxes.add(null);
					}
				}
			}

			for (int row = 0, itemIndex = 0; row < rowCount; row++) {

				TableRow tableRow = new TableRow();
				this.add(tableRow);

				for (int column = 0; column < columnCount; column++) {

					TableColumn tableColumn = new TableColumn();
					tableRow.add(tableColumn);

					Span span = new Span("class", "answer");
					span.setNewLineAfterClosingTag(false);
					tableColumn.add(span);

					Checkbox checkbox = checkboxes.get(itemIndex++);
					span.add(checkbox == null ? new HtmlReadyString(" ")
							: checkbox.toHtml(context, selectedValues));
				}
			}
		}
	}

	private class TableRow extends Block {
		public TableRow() {
			super("tr", false);
		}
	}

	private class TableColumn extends Block {
		public TableColumn() {
			super("td", false);
		}
	}

	public MultipleChoice getQuestion() {
		return question;
	}

	public Html toHtml(ExecutionContext context) {
		return toHtml(context, null);
	}
}
