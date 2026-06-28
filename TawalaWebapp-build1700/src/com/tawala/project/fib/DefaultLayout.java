package com.tawala.project.fib;

import com.tawala.project.Blank;
import com.tawala.project.FillInBlank;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Div;
import com.tawala.web.oldhtml.Html;

public class DefaultLayout implements FillInBlankLayout {
	private final String cssClassName;

	public DefaultLayout() {
		this.cssClassName = "fib";
	}

	public DefaultLayout(String cssClassName) {
		this.cssClassName = cssClassName;
	}

	public final Html render(FillInBlank fib, ExecutionContext executionContext) {
		Div fibDiv = new Div("class", cssClassName);
		fibDiv.appendContents(fib.getContents(), executionContext);
		return fibDiv;
	}

	public Html render(Blank blank, ExecutionContext executionContext) {
		return blank.defaultRendering(executionContext);
	}
}
