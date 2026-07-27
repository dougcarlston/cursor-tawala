package com.tawala.project.formatting;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.project.FieldReference;
import com.tawala.project.FormRenderable;
import com.tawala.project.Text;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;

public class Div extends AlignableContainerElement implements FormRenderable {
	private static Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();
	static {
		FACTORY.setKeepWhitespace(false);
		FACTORY.register("font", Font.class);
		FACTORY.register("division", Div.class);
		FACTORY.register("sp", Space.class);
		FACTORY.register("b", Bold.class);
		FACTORY.register("u", Underline.class);
		FACTORY.register("i", Italics.class);
		FACTORY.register("field", FieldReference.class);
		FACTORY.registerText(Text.class);
	}
	
	public Div(ConfigElement config) {
		super(config);
	}

	public Html toHtml(ExecutionContext context) {
		com.tawala.web.oldhtml.Div result = getStyle() == null ? new com.tawala.web.oldhtml.Div() : 
			new com.tawala.web.oldhtml.Div("style", getStyle());
		
		result.appendContents(getContents(), context);
		
		return result;
	}

	@Override
	protected Factory<FormRenderable> getFactory() {
		return FACTORY;
	}
}
