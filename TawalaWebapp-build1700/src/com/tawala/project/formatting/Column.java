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
		HtmlItems cellContents = new HtmlItems();
		cellContents.appendContents(getContents(), context);
		
		return new Table.Column(cellContents, "style", "width: " + (width/20) + "pt");
	}
}
