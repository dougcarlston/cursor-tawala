package com.tawala.project;

import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.component.web.ResponseCreator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.formatting.Table;
import com.tawala.web.oldhtml.Anchor;
import com.tawala.web.oldhtml.Div;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;

public class TextBlock extends FormItem {

	public static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();

	static {
		FACTORY.register("field", FieldReference.class);
		FACTORY.registerText(Text.class);
		FACTORY.setKeepWhitespace(true);

		FACTORY.register("paragraph", Paragraph.class);
		FACTORY.register("table", Table.class);
		FACTORY.ignore("displayConditions");
	}

	private final Text text;
	private final String style;
	private final boolean condensed;
	protected final List<FormRenderable> contents;

	public TextBlock(ConfigElement config) {
		super(config);
		this.text = new Text(config.text());
		this.style = config.attribute("style").stringValue();
		this.condensed = config.hasAttribute("paddingBottom")
				&& !config.attribute("paddingBottom").booleanValue();
		this.contents = new ArrayList<FormRenderable>();
		contents.addAll(getFactory().makeChildren(config));
	}

	protected Factory<FormRenderable> getFactory() {
		return FACTORY;
	}

	protected boolean useEnclosingDiv() {
		return true;
	}

	public String getTextContents() {
		return text.getContents();
	}

	public boolean hasFields() {
		return false;
	}

	public List<Field> fields() {
		return Collections.emptyList();
	}

	public Html produceHtml(ExecutionContext context) {
		if (context.isPreviewMode() && getId() != null && getId().length() > 0) {
			HtmlItems result = new HtmlItems();
			result.add(new Anchor("anchor-" + getId()));
			result.add(createOutput(context));
			return result;
		} else {
			return createOutput(context);
		}
	}

	protected Html createOutput(ExecutionContext context) {
		HtmlItems result = null;
		if (useEnclosingDiv()) {
			Div div = new Div("class", getClassValue());
			div.setNewLineAfterClosingTag(false);

			result = div;
		} else {
			result = new HtmlItems();
		}
		result.appendContents(contents, context);
		return result;
	}

	private String getClassValue() {
		StringBuilder result = new StringBuilder("text");
		if (style != null) {
			result.append(' ').append(style);
		}
		if (condensed) {
			result.append(' ').append("condensed");
		}
		return result.toString();
	}

	public List<FormRenderable> getContents() {
		return contents;
	}

	public boolean isEmpty(ExecutionContext context) {
		for (FormRenderable renderable : getContents()) {
			if (!renderable.isEmpty(context)) {
				return false;
			}
		}
		return true;
	}

	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		for (FormRenderable formRenderable : contents) {
			ResponseCreator result = formRenderable
					.getResponseCreatorForComponentId(componentId);
			if (result != null) {
				return result;
			}
		}
		return null;
	}
}
