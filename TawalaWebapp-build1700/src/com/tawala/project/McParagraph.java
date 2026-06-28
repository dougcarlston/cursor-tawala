package com.tawala.project;

import java.util.ArrayList;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.formatting.AlignableContainerElement;
import com.tawala.project.formatting.Bold;
import com.tawala.project.formatting.DummyTab;
import com.tawala.project.formatting.FormFont;
import com.tawala.project.formatting.Italics;
import com.tawala.project.formatting.Space;
import com.tawala.project.formatting.Underline;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlReadyString;

public class McParagraph extends FormParagraph {

	private static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();

	static {
		FACTORY.register("field", FieldReference.class);
		FACTORY.ignore("tabPositions");
		FACTORY.register("tab", DummyTab.class);
		FACTORY.registerText(Text.class);
		FACTORY.setKeepWhitespace(true);

		FACTORY.register("font", FormFont.class);
		FACTORY.register("b", Bold.class);
		FACTORY.register("i", Italics.class);
		FACTORY.register("u", Underline.class);
		FACTORY.register("image", ImageInstance.class);
		FACTORY.register("sp", Space.class);
	}

	public McParagraph(ConfigElement config) {
		super(config);
		displayStrategy = new McParagraphImplementation(config);
	}

	private class McParagraphHtml extends HtmlItems {
	}

	private class McParagraphImplementation extends AlignableContainerElement
	implements ParagraphDisplayStrategy {

		public McParagraphImplementation(ConfigElement config) {
			super(config);
		}
		
		public Html toHtml(ExecutionContext context, FormRenderable extraText) {
		
			McParagraphHtml paragraph = new McParagraphHtml();
			
			if (getContents().size() == 0) {
				paragraph.add(new HtmlReadyString("&nbsp;"));
			} else {
				paragraph.appendContents(getContents(), context);
			}
			
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
