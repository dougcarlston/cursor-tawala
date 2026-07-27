/**
 * 
 */
package com.tawala.web.project;

import java.io.PrintWriter;

import com.tawala.component.web.ResponseCreator;
import com.tawala.project.FormRenderable;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.RenderingContext;

abstract public class IncorrectNavigationPage implements FormRenderable, Html {
	private final String originalLink;

	public IncorrectNavigationPage(String originalLink) {
		this.originalLink = originalLink;
	}

	public final Html toHtml(ExecutionContext executionContext) {
		return this;
	}

	public final void render(PrintWriter out, RenderingContext renderingContext) {
		out.println("<div align=\"center\">\n");

		renderErrorMessage(out);

		out.print("<br />\n");

		if (originalLink == null) {
			out
					.print("If you didn't use the Back button please start your session again by using the link you were provided.\n");
		} else {
			out.print("You can continue by navigating to <a href=\""
					+ originalLink + "\" id=\"originalLink\">this page</a>.\n");
		}

		out.print("</div>\n" + "<br />");
	}

	protected abstract void renderErrorMessage(PrintWriter out);

	public final boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public final ResponseCreator getResponseCreatorForComponentId(
			String componentId) {
		return null;
	}
}