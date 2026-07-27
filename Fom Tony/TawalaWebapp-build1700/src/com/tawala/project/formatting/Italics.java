package com.tawala.project.formatting;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.web.oldhtml.HtmlItems;

public class Italics extends TextFormattingContainerElement {
    public Italics(ConfigElement config) {
        super(config);
    }

	@Override
	protected HtmlItems getHtmlElement() {
		return new com.tawala.web.oldhtml.Italics();
	}
}
