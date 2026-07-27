package com.tawala.project;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.formatting.Space;
import com.tawala.web.oldhtml.Block;

public class HeadingItem extends TextBlock {

	private static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();

	static {
		FACTORY.register("field", FieldReference.class);
		FACTORY.registerText(Text.class);
		FACTORY.setKeepWhitespace(true);
		FACTORY.register("paragraph", HeadingParagraph.class);
		FACTORY.register("sp", Space.class);
	}

	public HeadingItem(ConfigElement config) {
		super(config);
	}

	@Override
	protected Factory<FormRenderable> getFactory() {
		return FACTORY;
	}

	@Override
	protected Block createOutput(ExecutionContext context) {
		Block block = new Block("h1", true);
		block.setAttribute("class", "heading");
		block.appendContents(contents, context);
		return block;
	}

}
