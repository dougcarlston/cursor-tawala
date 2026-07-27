package com.tawala.project;

import java.util.ArrayList;
import java.util.List;

import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;

public class VirtualDocument extends Document {
	private List<Html> appendedHtml = new ArrayList<Html>();

	public VirtualDocument(String name) {
		super(name);
	}

	public VirtualDocument(Document source) {
		super(source);
	}

	public void append(Html html) {
		appendedHtml.add(html);
	}

	public Html toHtml(ExecutionContext context) {
		HtmlItems result = new HtmlItems();
		if (super.hasContent())
			result.add(super.toHtml(context));
		for (Html html : appendedHtml) {
			result.add(html);
		}
		return result;
	}

	public void reset() {
		appendedHtml.clear();
	}
}
