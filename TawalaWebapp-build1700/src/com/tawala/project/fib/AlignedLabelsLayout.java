package com.tawala.project.fib;

import java.util.ArrayList;
import java.util.List;

import com.scissor.Log;
import com.tawala.project.Blank;
import com.tawala.project.FillInBlank;
import com.tawala.project.FormParagraph;
import com.tawala.project.FormRenderable;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.Table;

public class AlignedLabelsLayout implements FillInBlankLayout {
	public static final AlignedLabelsLayout LEFT = new AlignedLabelsLayout(
			"left");
	public static final AlignedLabelsLayout RIGHT = new AlignedLabelsLayout(
			"right");

	private final String alignment;

	public AlignedLabelsLayout(String alignment) {
		this.alignment = alignment;
	}

	public final Html render(FillInBlank fib, ExecutionContext executionContext) {
		Table fibTable = new Table("class", "fib");

		for (FormRenderable item : fib.getContents()) {
			if (item instanceof FormParagraph) {
				FormParagraph paragraph = (FormParagraph) item;
				fibTable.addRow(renderParagraph(paragraph, executionContext));
			} else {
				Log.error(this, "Expected FormParagraph only, instead got "
						+ item.getClass());
			}
		}

		return fibTable;
	}

	public final Html render(Blank blank, ExecutionContext executionContext) {
		return blank.defaultRendering(executionContext);
	}

	private List<Table.Column> renderParagraph(FormParagraph paragraph,
			ExecutionContext context) {

		List<Table.Column> row = new ArrayList<Table.Column>();

		row.add(columnOne(paragraph, context));
		row.add(columnTwo(paragraph, context));
		row.add(columnThree(paragraph, context));

		return row;
	}

	private Table.Column columnOne(FormParagraph originalParagraph,
			ExecutionContext context) {
		String[] cellAttributes = { "class", "label " + alignment };

		List<FormRenderable> labelElements = getLabelFromContents(originalParagraph);
		if (labelElements.size() == 0) {
			// --- Needed to create an blank row.
			return new Table.Column(new HtmlReadyString("&nbsp;"),
					cellAttributes);
		} else {
			HtmlItems columnContents = new HtmlItems();
			columnContents.appendContents(labelElements, context);
			return new Table.Column(columnContents, cellAttributes);
		}
	}

	private Table.Column columnTwo(FormParagraph originalParagraph,
			ExecutionContext context) {
		Blank blank = getBlankFromContents(originalParagraph);
		Html contents = blank == null ? new HtmlReadyString(" ") : blank
				.defaultRendering(context);
		return new Table.Column(contents, "class", "blank");
	}

	private Table.Column columnThree(FormParagraph originalParagraph,
			ExecutionContext context) {
		return new Table.Column(getRemainderFromContents(originalParagraph,
				context), "class", "remainder");
	}

	private List<FormRenderable> getLabelFromContents(FormParagraph paragraph) {
		List<FormRenderable> result = new ArrayList<FormRenderable>();

		for (FormRenderable item : paragraph.getContents()) {
			if (item instanceof Blank) {
				break;
			} else {
				result.add(item);
			}
		}

		return result;
	}

	private Blank getBlankFromContents(FormParagraph paragraph) {

		for (FormRenderable item : paragraph.getContents()) {
			if (item instanceof Blank) {
				return ((Blank) item);
			}
		}

		return null;
	}

	private HtmlItems getRemainderFromContents(FormParagraph paragraph,
			ExecutionContext context) {
		HtmlItems remainingItems = new HtmlItems();
		boolean blankFound = false;

		for (FormRenderable item : paragraph.getContents()) {
			if (blankFound) {
				remainingItems.add(item.toHtml(context));
			}
			if (item instanceof Blank) {
				blankFound = true;
			}
		}

		return remainingItems;
	}
}
