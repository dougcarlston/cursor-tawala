package com.tawala.project;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Block;
import com.tawala.web.oldhtml.Div;
import com.tawala.web.oldhtml.Html;

public class PageHeader extends FormRenderableNotHoldingActiveComponents {
	private final static Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();
	static {
		FACTORY.register("field", FieldReference.class);
		FACTORY.registerText(Text.class);
		FACTORY.setKeepWhitespace(true);
	}

	private final List<FormRenderable> textContents;
	private final ImageInstance image;

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public PageHeader(ConfigElement configElement) {
		ConfigElement textElement = configElement.child("text");
		textContents = textElement == null ? null : FACTORY
				.makeChildren(textElement);

		ConfigElement imageElement = configElement.child("image");
		image = imageElement == null ? null : new ImageInstance(imageElement);
	}

	public ImageInstance getImageInstance() {
		return image;
	}
	
	public Html toHtml(ExecutionContext context) {
		Block result = new Block("h1");
		result.setAttribute("class", "pageHeading");
		Html headerImage = null;
		if (image == null) {
			headerImage = context.getTheme().getHeaderImageHTML();
		} else {
			headerImage = image.toHtml(context);
			int imageHeight = image.getHeight();
			result.setAttribute("style", "height: " + imageHeight + "px;");
		}
		if (headerImage != null) {
			result.add(headerImage);
		}

		if (textContents != null) {
			Div textContainer = new Div();
			textContainer.appendContents(textContents, context);
			result.add(textContainer);
		}

		return result;
	}

}
