/**
 * 
 */
package com.tawala.web.project;

import java.io.PrintWriter;

public class BackButtonNavigationPage extends IncorrectNavigationPage {

	public BackButtonNavigationPage(String originalLink) {
		super(originalLink);
	}

	@Override
	protected void renderErrorMessage(PrintWriter out) {
		out
				.println("An error occured while running this application. <br />\n"
						+ "<br />\n"
						+ "Either your previous session has expired or you used your browser's BACK button to navigate back to a previous page." +
								"<br />\n" +
								"Note: This site uses cookies. If you have cookies turned off in your browser you need to enable them." +
								"<br />\n");
	}
}