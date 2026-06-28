package com.tawala.project;

import java.util.Iterator;
import java.util.List;

import org.springframework.web.util.HtmlUtils;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.formatting.Font;
import com.tawala.project.formatting.Font.SpanHtml;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlString;

public class FieldReference extends FormRenderableNotHoldingActiveComponents implements TextRenderable {
	public static String REFERENCE_PREFIX = "tawalaFieldReference_";
	private final String name;

	public FieldReference(ConfigElement config) {
		name = config.attribute("name").stringValue();
	}

	public FieldReference(String name) {
		this.name = name;
	}

	public Html toHtml(ExecutionContext context) {
		if (context.isPreviewMode()) {
			return new HtmlString("<<" + name + ">>");
		} else {
			if (context.isIncludeCustomizationMarkers()) {
				SpanHtml result = new Font.SpanHtml();
				String spanId = REFERENCE_PREFIX + HtmlUtils.htmlEscape(name);
				result.setAttribute("id", spanId);
				result.setAttribute("name", spanId);
				result.add(new HtmlString(buildOutput(context).toString()));
				return result;
			} else {
				return new HtmlString(buildOutput(context).toString());
			}
		}
	}

	private StringBuilder buildOutput(ExecutionContext context) {
		StringBuilder result = new StringBuilder();
		List<Value> values = context.getValues(name);
		if (values == null || values.size() == 0)
			return result;

		Iterator<Value> valueIterator = values.iterator();
		result.append(valueIterator.next());
		while (valueIterator.hasNext()) {
			result.append(", ");
			result.append(valueIterator.next());
		}
		return result;
	}

	public String getName() {
		return name;
	}

	public boolean isEmpty(ExecutionContext context) {
		List<Value> values = context.getValues(name);
		if (values == null || values.size() == 0) {
			return true;
		}

		for (Value value : values) {
			if (value.toString().trim().length() != 0) {
				return false;
			}
		}

		return true;
	}

	public void appendTo(StringBuilder result, ExecutionContext context) {
		result.append(buildOutput(context));
	}
}
