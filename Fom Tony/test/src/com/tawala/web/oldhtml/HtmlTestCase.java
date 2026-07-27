package com.tawala.web.oldhtml;

import java.io.PrintWriter;
import java.io.StringWriter;

import com.tawala.TestCase;
import com.tawala.project.FormRenderable;
import com.tawala.project.Value;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.FakeExecutionContext;

public abstract class HtmlTestCase extends TestCase {
	public static String renderHtml(Html html) {
		StringWriter out = new StringWriter();
		html.render(new PrintWriter(out), new RenderingContext());
		String result = out.toString();
		return result.replace("\r\n", "\n");
	}

	public static String render(Value value) {
		StringWriter out = new StringWriter();
		new HtmlString(value.toString()).render(new PrintWriter(out), new RenderingContext());
		String result = out.toString();
		return result.replace("\r\n", "\n");
	}

	public static String render(FormRenderable formRenderable) {
		FakeExecutionContext context = new FakeExecutionContext(ProjectBuilder
				.buildMinimalisticProject());

		return renderHtml(formRenderable.toHtml(context));
	}
}
