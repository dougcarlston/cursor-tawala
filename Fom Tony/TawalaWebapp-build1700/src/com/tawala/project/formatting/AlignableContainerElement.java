package com.tawala.project.formatting;

import com.scissor.xmlconfig.ConfigAttribute;
import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.FormRenderable;

public abstract class AlignableContainerElement extends ContainerElement {
	protected String style;

	public AlignableContainerElement(ConfigElement config) {
		this(config, null);
	}

	public AlignableContainerElement(ConfigElement config, String extraStyleElement) {
		StringBuilder styleBuilder = extraStyleElement == null ? new StringBuilder() : new StringBuilder(extraStyleElement);

		ConfigAttribute indentAttribute = config.attribute("indent");
		if (indentAttribute.stringValue() != null) {
			int indent = indentAttribute.intValue();
			if (indent > 0) {
				if(styleBuilder.length() > 0) {
					styleBuilder.append("; ");
				}
				styleBuilder.append("margin-left: ").append(indent / 20).append("pt");
			}
		}

		String alignment = config.attribute("align").stringValue();
		if (alignment != null && alignment.length() > 0) {
			if(styleBuilder.length() > 0) {
				styleBuilder.append("; ");
			}
			styleBuilder.append("text-align: ").append(alignment);
		}

		this.style = (styleBuilder.length() > 0) ? styleBuilder.toString() : null;

		addElements(getFactory().makeChildren(config));
	}

	protected abstract Factory<FormRenderable> getFactory();

	public String getStyle() {
		return style;
	}
}
