package com.tawala.project;

import java.util.ArrayList;
import java.util.List;

import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.Html;
import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;

public class HeadingFont extends FormRenderableNotHoldingActiveComponents {

	private static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();

	static {
		FACTORY.register("field", FieldReference.class);
		FACTORY.registerText(Text.class);
		FACTORY.setKeepWhitespace(true);
	}

	protected final List<FormRenderable> contents;

	public HeadingFont(ConfigElement config) {
		contents = new ArrayList<FormRenderable>();
		contents.addAll(FACTORY.makeChildren(config));
	}
	
	public boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public Html toHtml(ExecutionContext context) {
		HtmlItems result = new HtmlItems();
		result.appendContents(contents, context);

		return result;
	}

}
