package com.tawala.project;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.formatting.Bold;
import com.tawala.project.formatting.DummyTab;
import com.tawala.project.formatting.FormFont;
import com.tawala.project.formatting.Italics;
import com.tawala.project.formatting.Space;
import com.tawala.project.formatting.Underline;
import com.tawala.web.oldhtml.Html;

public class FormParagraph extends FormRenderableNotHoldingActiveComponents {

	protected static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();
	
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

	protected ParagraphDisplayStrategy displayStrategy;

	public FormParagraph(ConfigElement config) {
		
		displayStrategy = paragraphUsesTabs(config) ? new TabbedParagraphDisplayStrategy(config, FACTORY) : new SimpleParagraphDisplayStrategy(config, FACTORY);
	}

	private boolean paragraphUsesTabs(ConfigElement config) {
		return (countTabs(config) > 0 && config.child("tabPositions") != null);
	}

	private int countTabs(ConfigElement config) {
		int tabCount = 0;
		for (ConfigElement child : config.childElements()) {
			if (child.getName().equals("tab")) {
				++tabCount;
			}
		}
		return tabCount;
	}

	public boolean isEmpty(ExecutionContext context) {
		return displayStrategy.isEmpty(context);
	}

	public Html toHtml(ExecutionContext context) {
		return displayStrategy.toHtml(context);
	}

	public List<FormRenderable> getContents() {
		return displayStrategy.getContents();
	}

}
