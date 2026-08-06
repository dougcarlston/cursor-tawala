package com.tawala.project.formatting;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.FormRenderable;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.Table;

public class Column extends ContainerElement {
	private static Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();
	static {
		FACTORY.setKeepWhitespace(false);
		FACTORY.register("division", Div.class);
	}

	private final int width;

	public Column(ConfigElement config) {
		this.width = config.attribute("width").intValue();
		addElements(FACTORY.makeChildren(config));
	}

	public Table.Column toHtmlColumn(ExecutionContext context) {
		return toHtmlColumn(context, 1);
	}

	public Table.Column toHtmlColumn(ExecutionContext context, int border) {
		HtmlItems cellContents = new HtmlItems();
		cellContents.appendContents(getContents(), context);

		StringBuilder style = new StringBuilder();
		style.append("width: ").append(width / 20).append("pt");
		if (border <= 0) {
			style.append("; border: none");
		} else {
			style.append("; border: 1px solid #000000");
		}
		return new Table.Column(cellContents, "style", style.toString());
	}
}
