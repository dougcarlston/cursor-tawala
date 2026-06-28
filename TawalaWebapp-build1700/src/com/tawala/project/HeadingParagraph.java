package com.tawala.project;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.formatting.AlignableContainerElement;
import com.tawala.project.formatting.Space;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;

public class HeadingParagraph extends FormParagraph {

	private static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();

	static {
		FACTORY.ignore("tabPositions");
		FACTORY.setKeepWhitespace(true);
		FACTORY.register("font", HeadingFont.class);
		FACTORY.registerText(Text.class);
		FACTORY.register("field", FieldReference.class);
		FACTORY.register("sp", Space.class);
	}

	public HeadingParagraph(ConfigElement config) {
		super(config);
		displayStrategy = new HeadingParagraphImplementation(config);
	}

	private class HeadingParagraphImplementation extends AlignableContainerElement
			implements ParagraphDisplayStrategy {
		
		public HeadingParagraphImplementation(ConfigElement config) {
			super(config);
		}

		public Html toHtml(ExecutionContext context, FormRenderable extraText) {

			HtmlItems paragraph = new HtmlItems();
			paragraph.appendContents(getContents(), context);
			
			return paragraph;
		}
		

		public Html toHtml(ExecutionContext context) {
			return toHtml(context, null);
		}
		
		@Override
		protected Factory<FormRenderable> getFactory() {
			return FACTORY;
		}
		
	}

}
