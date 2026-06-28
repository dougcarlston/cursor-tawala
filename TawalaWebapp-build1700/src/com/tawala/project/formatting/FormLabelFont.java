package com.tawala.project.formatting;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;

public class FormLabelFont extends FormFont {
	
	public FormLabelFont(ConfigElement config) {
		super(config);
	}

	@Override
	public Html toHtml(ExecutionContext context) {
		HtmlItems result = new HtmlItems();
		result.appendContents(getContents(), context);
		return result;
	}

}
