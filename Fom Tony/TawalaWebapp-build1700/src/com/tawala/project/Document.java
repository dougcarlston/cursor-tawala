package com.tawala.project;

import java.util.ArrayList;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.component.function.FunctionToWebAdapter;
import com.tawala.component.repository.Repository;
import com.tawala.component.web.ResponseCreator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.formatting.Table;
import com.tawala.web.oldhtml.Div;
import com.tawala.web.oldhtml.Html;

public class Document implements FormRenderable {
	private static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();

	static {
		FACTORY.setKeepWhitespace(false);

		FACTORY.register("paragraph", Paragraph.class);
		FACTORY.register("table", Table.class);
		Repository.registerWebComponentsWith(FACTORY);
		FunctionToWebAdapter.registerFunctionsWith(FACTORY);
	}
	
	private final String name;
	private final List<FormRenderable> contents;

	public Document(ConfigElement config) {
		this(config.attribute("name").stringValue());
		ConfigElement xmlData = config.child("xmlData");
		contents.addAll(FACTORY.makeChildren(xmlData));
		ConfigElement rawHtml = config.child("rawHtmlData");
		if(rawHtml != null) {
			rawHtml.markUsed();
		}
	}

	public Document(String name) {
		this.name = name;
		contents = new ArrayList<FormRenderable>();
	}

	public Document(String name, String text) {
		this(name);
		contents.add(new Text(text));
	}

	public Document(String name, List<FormRenderable> items) {
		this(name);
		contents.addAll(items);
	}

	protected Document(Document other) {
		this.name = other.name;
		this.contents = other.contents;
	}

	public String getName() {
		return name;
	}

	public Html toHtml(ExecutionContext context) {
		Div div = new Div("class", "document");
		div.appendContents(contents, context);
		return div;
	}

	public String toString() {
		return "Document{" + "name='" + name + "'" + "}";
	}

	public boolean hasContent() {
		return !contents.isEmpty();
	}

	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		for (FormRenderable formRenderable : contents) {
			ResponseCreator responseCreator = formRenderable.getResponseCreatorForComponentId(componentId);
			if(responseCreator != null) {
				return responseCreator;
			}
		}
		return null;
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}
}
