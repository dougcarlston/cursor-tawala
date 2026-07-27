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
import com.tawala.web.oldhtml.Block;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlReadyString;

public class FibParagraph extends FormParagraph {

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
		FACTORY.register("blank", Blank.class);
		FACTORY.register("fileNameInput", Blank.class);
		FACTORY.register("sp", Space.class);
	}

	public FibParagraph(ConfigElement config) {
		super(config);
		displayStrategy = new FibParagraphImplementation(config);
	}

	private class FibParagraphHtml extends Block {

		public FibParagraphHtml(String style) {
			super("div", false);
		}
	}

	private class FibParagraphImplementation extends AlignableContainerElement
	implements ParagraphDisplayStrategy {

		public FibParagraphImplementation(ConfigElement config) {
			super(config);
		}
		
		public Html toHtml(ExecutionContext context, FormRenderable extraText) {
		
			FibParagraphHtml paragraph = new FibParagraphHtml(getStyle());
			
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
		
		public List<Blank> getAllBlanks() {
			List<Blank> result = new ArrayList<Blank>();
			for (Object nextElement : getContents()) {
				if (nextElement instanceof Blank) {
					result.add((Blank) nextElement);
				}
			}
			return result;
		}

		@Override
		protected Factory<FormRenderable> getFactory() {
			return FACTORY;
		}
	}

}
