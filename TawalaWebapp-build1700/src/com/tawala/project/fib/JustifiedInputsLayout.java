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
import com.tawala.web.oldhtml.Table;

public class JustifiedInputsLayout implements FillInBlankLayout {
	private final String labelAlignment;

	public JustifiedInputsLayout(String labelAlignment) {
		this.labelAlignment = labelAlignment;
	}

	public Html render(FillInBlank fib, ExecutionContext context) {
		Table fibTable = new Table("class", "fib justified");

		for (FormRenderable item : fib.getContents()) {
			if (item instanceof FormParagraph) {
				FormParagraph paragraph = (FormParagraph) item;
				fibTable.addRow(renderParagraph(paragraph, context));
			} else {
				Log.error(this, "Expected FormParagraph, instead got: "
						+ item.getClass());
			}
		}

		return fibTable;
	}

	public Html render(Blank blank, ExecutionContext executionContext) {
		return blank.defaultRendering(executionContext);
	}

	public List<Table.Column> renderParagraph(FormParagraph paragraph,
			ExecutionContext context) {

		// --- Separate the paragraph contents into two groups
		List<FormRenderable> firstColumnItems = new ArrayList<FormRenderable>();
		List<FormRenderable> secondColumnItems = new ArrayList<FormRenderable>();

		List<FormRenderable> currentContainer = firstColumnItems;
		for (FormRenderable item : paragraph.getContents()) {
			if (item instanceof Blank) {
				currentContainer = secondColumnItems;
			}
			currentContainer.add(item);
		}

		boolean hasItemsForSecondColumn = secondColumnItems.size() != 0;

		// --- Create the row with two columns.

		List<Table.Column> result = new ArrayList<Table.Column>();

		result
				.add(columnOne(firstColumnItems, hasItemsForSecondColumn,
						context));
		if (hasItemsForSecondColumn) {
			result.add(columnTwo(secondColumnItems, context));
		}

		return result;
	}

	private Table.Column columnOne(List<FormRenderable> items,
			boolean hasItemsForSecondColumn, ExecutionContext context) {
		HtmlItems contents = new HtmlItems();
		contents.appendContents(items, context);
		if (hasItemsForSecondColumn) {
			return new Table.Column(contents, "class", "label "
					+ labelAlignment);
		} else {
			return new Table.Column(contents, "colspan", "2");
		}
	}

	private Table.Column columnTwo(List<FormRenderable> items,
			ExecutionContext context) {

		if (items.size() == 1 && items.get(0) instanceof Blank) {
			Blank blank = (Blank) items.get(0);
			return new Table.Column(blank.defaultRendering(context), "class",
					"blank");
		} else {
			String[] tableAttributes = items.get(items.size() - 1) instanceof Blank ? new String[] {
					"class", "remainder", "width", "100%" }
					: new String[] { "class", "remainder" };
			Table table = new Table(tableAttributes);
			List<Table.Column> columns = new ArrayList<Table.Column>();

			HtmlItems columnContents = new HtmlItems();
			int itemCount = 0;
			for (FormRenderable item : items) {
				++itemCount;
				if (item instanceof Blank) {
					if (!columnContents.isEmpty()) {
						columns.add(new Table.Column(columnContents, "class",
								"right"));
						columnContents = new HtmlItems();
					}

					Blank blank = (Blank) item;
					if (itemCount == items.size()) {
						// Last blank in the paragraph should be treated
						// differently
						columns.add(new Table.Column(blank
								.defaultRendering(context), "class", "right",
								"style", "padding-top: 0px;"));

					} else {
						columns.add(new Table.Column(blank
								.defaultRendering(context), "class", "blank"));
					}

				} else {
					columnContents.add(item.toHtml(context));
				}
			}

			if (!columnContents.isEmpty()) {
				columns.add(new Table.Column(columnContents, "class", "left"));
			}

			table.addRow(columns);

			return new Table.Column(table, "style",
					"padding-bottom: 0px; padding-top: 0px;");
		}
	}
}
