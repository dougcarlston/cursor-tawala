package com.tawala.project;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Block;

public class SubheadingItem extends HeadingItem {

	public SubheadingItem(ConfigElement config) {
		super(config);
	}

	@Override
	protected Block createOutput(ExecutionContext context) {
		Block block = new Block("h2", true);
		block.setAttribute("class", "subheading");

		block.appendContents(contents, context);
		return block;
	}

}
