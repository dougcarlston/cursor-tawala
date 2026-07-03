package com.tawala.project.builder;

import java.util.ArrayList;
import java.util.List;

import com.scissor.XmlBuffer;
import com.scissor.XmlRenderable;

public class PageHeaderBuilder implements XmlRenderable {
	private List<XmlRenderable> textElements = new ArrayList<XmlRenderable>();
	private ImageInstanceBuilder imageInstanceBuilder;

	public void render(XmlBuffer xml) {
		xml.startTag("pageHeader", false);
		if (textElements.size() > 0) {
			xml.startTag("text", false);
			for (XmlRenderable textElement : textElements) {
				textElement.render(xml);
			}
			xml.endTag("text");
		}
		if (imageInstanceBuilder != null) {
			imageInstanceBuilder.render(xml);
		}
		xml.endTag("pageHeader");
	}

	public void addText(String text) {
		XmlBuffer textElement = new XmlBuffer();
		textElement.text(text);
		textElements.add(textElement);
	}

	public void addField(String id) {
		XmlBuffer fieldElement = new XmlBuffer();
		fieldElement.tag("field", false, "name", id);
		textElements.add(fieldElement);
	}

	public void setImage(ImageInstanceBuilder imageInstanceBuilder) {
		this.imageInstanceBuilder = imageInstanceBuilder;
	}
}
