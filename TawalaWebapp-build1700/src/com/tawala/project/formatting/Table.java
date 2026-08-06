package com.tawala.project.formatting;

import java.util.ArrayList;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.web.ResponseCreator;
import com.tawala.project.FormRenderable;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;

public class Table implements FormRenderable {
	private final List<List<Column>> rows = new ArrayList<List<Column>>();
	private final String style;
	/** 0 = none, 1 = Border 1 (default), 2 = Border 2. */
	private final int border;

	public Table(ConfigElement config) {
		List<ConfigElement> rowElements = config.children("row");
		for (ConfigElement row : rowElements) {
			List<Column> columns = new ArrayList<Column>();
			for (ConfigElement columnElement : row.children("cell")) {
				columns.add(new Column(columnElement));
			}
			rows.add(columns);
		}

		this.border = parseBorder(config);
		StringBuilder styleBuilder = new StringBuilder();
		styleBuilder.append("margin-left: ").append(
				config.attribute("indent").intValue() / 20).append("pt");
		styleBuilder.append("; border-collapse: collapse");
		if (border >= 2) {
			styleBuilder.append("; border: 3px solid #000000");
		} else if (border >= 1) {
			styleBuilder.append("; border: 1px solid #000000");
		} else {
			styleBuilder.append("; border: none");
		}
		this.style = styleBuilder.toString();
	}

	/**
	 * Design Border 1 is the default when {@code border} is omitted (legacy XML).
	 * {@code border="0"} / {@code none} = Invisible; {@code 2} = Border 2.
	 */
	private static int parseBorder(ConfigElement config) {
		if (!config.hasAttribute("border")) {
			return 1;
		}
		String raw = config.attribute("border").stringValue();
		if (raw == null || raw.length() == 0) {
			return 1;
		}
		if ("none".equalsIgnoreCase(raw) || "0".equals(raw)) {
			return 0;
		}
		try {
			int n = Integer.parseInt(raw.trim());
			if (n <= 0) {
				return 0;
			}
			if (n >= 2) {
				return 2;
			}
			return 1;
		} catch (NumberFormatException e) {
			return 1;
		}
	}

	public Html toHtml(ExecutionContext context) {
		// HTML border= helps email clients that drop classes / <style>; CSS +
		// inline cell borders remain the primary Border 1 / 2 / none contract.
		String htmlBorder = border <= 0 ? "0" : (border >= 2 ? "2" : "1");
		com.tawala.web.oldhtml.Table result = new com.tawala.web.oldhtml.Table(
				"class", borderClass(), "border", htmlBorder, "cellspacing",
				"0", "cellpadding", "4", "style", style);
		for (List<Column> row : rows) {
			List<com.tawala.web.oldhtml.Table.Column> htmlColumns = new ArrayList<com.tawala.web.oldhtml.Table.Column>(
					row.size());

			for (Column column : row) {
				htmlColumns.add(column.toHtmlColumn(context, border));
			}

			result.addRow(htmlColumns);
		}
		return result;
	}

	private String borderClass() {
		if (border <= 0) {
			return "user user-border-none";
		}
		if (border >= 2) {
			return "user user-border-2";
		}
		return "user user-border-1";
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		for (List<Column> row : rows) {
			for (Column column : row) {
				ResponseCreator result = column
						.getResponseCreatorForComponentId(componentId);
				if (result != null) {
					return result;
				}
			}
		}
		return null;
	}
}
