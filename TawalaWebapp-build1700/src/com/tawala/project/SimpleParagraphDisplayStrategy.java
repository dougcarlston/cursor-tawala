package com.tawala.project;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.formatting.ContainerElement;
import com.tawala.web.oldhtml.Block;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlReadyString;

public class SimpleParagraphDisplayStrategy extends ContainerElement
		implements ParagraphDisplayStrategy {

	public SimpleParagraphDisplayStrategy(ConfigElement config, Factory<FormRenderable> FACTORY) {
		addElements(FACTORY.makeChildren(config));
	}

	public Html toHtml(ExecutionContext context) {
		
		Block paragraph = new Block("div", false);
		
		if (getContents().size() == 0) {
			paragraph.add(new HtmlReadyString("&nbsp;"));
		} else {
			paragraph.appendContents(getContents(), context);
		}
		return paragraph;
	}
}
