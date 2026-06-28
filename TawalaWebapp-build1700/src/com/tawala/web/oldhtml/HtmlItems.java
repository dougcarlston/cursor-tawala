package com.tawala.web.oldhtml;

import java.io.PrintWriter;
import java.io.StringWriter;
import java.util.ArrayList;
import java.util.List;

import com.tawala.project.FormRenderable;
import com.tawala.project.commands.ExecutionContext;

public class HtmlItems implements Html {
	private final List<Html> contents = new ArrayList<Html>();

	protected void renderContents(PrintWriter out, RenderingContext context) {
		for (Html contentItem : contents) {
			contentItem.render(out, context);
		}
	}

	public void render(PrintWriter out, RenderingContext context) {
		renderContents(out, context);
	}

	public void add(Html html) {
		contents.add(html);
	}

	public void addContents(HtmlItems html) {
		contents.addAll(html.contents);
	}

	public String toString() {
		StringWriter result = new StringWriter();
		render(new PrintWriter(result), new RenderingContext());
		return result.toString();
	}

	public void appendContents(List<FormRenderable> contents,
			ExecutionContext context) {
		for (FormRenderable renderable : contents) {
			add(renderable.toHtml(context));
		}
	}
	
	public boolean isEmpty() {
		return contents.size() == 0;
	}
}
