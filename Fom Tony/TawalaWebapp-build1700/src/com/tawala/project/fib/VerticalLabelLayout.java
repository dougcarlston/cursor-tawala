package com.tawala.project.fib;

import com.tawala.project.Blank;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Div;
import com.tawala.web.oldhtml.Html;

public class VerticalLabelLayout extends DefaultLayout {

	public VerticalLabelLayout() {
		super("fib vertical");
	}

	@Override
	public Html render(Blank blank, ExecutionContext executionContext) {
		Div result = new Div("class", "blank");
		result.add(blank.defaultRendering(executionContext));
		return result;
	}
}
