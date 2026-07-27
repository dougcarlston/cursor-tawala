package com.tawala.project.formatting;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlReadyString;

public class Space extends FormRenderableNotHoldingActiveComponents {
	private static final HtmlReadyString space = new HtmlReadyString("&nbsp;");

	public Space(ConfigElement configElement) {
		//--- Do nothing. The constructor is required by the XMLConfig framework.
	}
	
	public Html toHtml(ExecutionContext context) {
		return space;
	}

	public boolean isEmpty(ExecutionContext context) {
		return true;
	}
}
