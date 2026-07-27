package com.tawala.project.formatting;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlReadyString;

public class DummyTab extends FormRenderableNotHoldingActiveComponents {

	public DummyTab(ConfigElement element) {
		
	}
	public Html toHtml(ExecutionContext context) {
		return new HtmlReadyString(" ");
	}
	
	public boolean isEmpty(ExecutionContext context) {
		return true;
	}
}
