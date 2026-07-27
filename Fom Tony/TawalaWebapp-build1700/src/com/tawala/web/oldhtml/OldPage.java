package com.tawala.web.oldhtml;

import java.io.PrintWriter;
import java.util.ArrayList;
import java.util.Collection;
import java.util.LinkedHashSet;
import java.util.List;
import java.util.Set;

import com.tawala.web.WorldInitializer;
import com.tawala.web.admin.UrgentMessage;
import com.tawala.web.controller.WellKnown;

public class OldPage implements Html {
	private final Set<String> screenStylesheets = new LinkedHashSet<String>();
	private final Set<String> printStylesheets = new LinkedHashSet<String>();
	private final List<String> scripts = new ArrayList<String>();
	private final List<Html> contents = new ArrayList<Html>();
	private String googleAdSenseCode;
	private String headerCSS;
	private Integer sessionExpirationTimeout;
	private final String buildNumber;
	private UrgentMessage urgentMessage;

	public void setHeaderCSS(String headerCSS) {
		this.headerCSS = headerCSS;
	}

	public OldPage() {
/*
		screenStylesheets.add("/scripts/yui/build/fonts/fonts-min.css");
		screenStylesheets
				.add("/scripts/yui/build/button/assets/skins/sam/button.css");
		screenStylesheets
				.add("/scripts/yui/build/container/assets/container.css");
		screenStylesheets
				.add("/scripts/yui/build/container/assets/skins/sam/container.css");
		screenStylesheets
				.add("/scripts/yui/build/datatable/assets/skins/sam/datatable.css");

//		screenStylesheets.add("/css/jquery/ui-lightness/jquery-ui-1.8.12.custom.css");
*/
		screenStylesheets.add("/scripts/yui-2.9.0/build/fonts/fonts-min.css");
		screenStylesheets.add("/scripts/yui-2.9.0/build/button/assets/skins/sam/button.css");
		screenStylesheets.add("/scripts/yui-2.9.0/build/container/assets/container.css");
		screenStylesheets.add("/scripts/yui-2.9.0/build/container/assets/skins/sam/container.css");
		screenStylesheets.add("/scripts/yui-2.9.0/build/datatable/assets/skins/sam/datatable.css");		
		screenStylesheets.add("/scripts/yui-2.9.0/build/paginator/assets/skins/sam/paginator.css");

/*
		scripts.add("/scripts/yui/build/utilities/utilities.js");
		scripts.add("/scripts/yui/build/json/json-min.js");
		scripts.add("/scripts/yui/build/container/container-min.js");
		scripts.add("/scripts/yui/build/datasource/datasource-beta-min.js");
		scripts.add("/scripts/yui/build/datatable/datatable-beta-min.js");
		scripts.add("/scripts/yui/build/animation/animation-min.js");
		scripts.add("/scripts/yui/build/cookie/cookie-beta-min.js");
*/
		scripts.add("/scripts/yui-2.9.0/build/utilities/utilities.js");
		scripts.add("/scripts/yui-2.9.0/build/json/json-min.js");
		scripts.add("/scripts/yui-2.9.0/build/container/container-min.js");
		scripts.add("/scripts/yui-2.9.0/build/datasource/datasource-min.js");
		scripts.add("/scripts/yui-2.9.0/build/datatable/datatable-min.js");
		scripts.add("/scripts/yui-2.9.0/build/paginator/paginator-min.js");
		scripts.add("/scripts/yui-2.9.0/build/animation/animation-min.js");
		scripts.add("/scripts/yui-2.9.0/build/cookie/cookie-min.js");
		
//		scripts.add("/scripts/jquery/jquery-1.5.1.min.js");
//		scripts.add("/scripts/jquery/jquery-ui-1.8.12.custom.min.js");
//		scripts.add("/scripts/jquery/plugins/jquery.tablesorter.min.js");

		scripts.add("/scripts/tiny_mce/tiny_mce.js");

		this.buildNumber = WorldInitializer.getDefaultWorld().getBuildNumber();
	}

	public OldPage(List<Html> html) {
		this();
		contents.addAll(html);
	}

	public OldPage(Html html) {
		this();
		contents.add(html);
	}

	public void render(PrintWriter out, RenderingContext context) {
		if(! context.isEmailDestination()) {
			out.print("<!DOCTYPE html>\n");
		}
		
		out.print("<html>\n");
		out.print("<head>\n");
		out.print("<title>SportsDashboards</title>\n");
		out.print("<meta name=\"robots\" content=\"noarchive\">\n");
		
		addStylesheets(out, "print", printStylesheets);
		addStylesheets(out, "screen", screenStylesheets);

		if (headerCSS != null) {
			out.print("<style type=\"text/css\" id=\"headerCSS\">" + headerCSS
					+ "</style>\n");
		}

		for (String script : scripts) {
			String scriptURL = addBuildNumberToPreventCaching(script);
			out.print("<script type=\"text/javascript\" src=\"" + scriptURL
					+ "\" ></script>\n");
		}

		if (sessionExpirationTimeout != null) {
			out.print("<script type=\"text/javascript\">\n"
					+ "var sessionExpirationTimeout = "
					+ sessionExpirationTimeout + ";\n"
					+ "Event.onDOMReady(initTimeoutDialog);\n" + "</script>\n");
		}

		out.print("</head>\n");
		out.print("<body class=\"yui-skin-sam\">\n");

		if (sessionExpirationTimeout != null) {
			out
					.print("<div id=\"timeoutDialogDiv\">\n"
							+ "<div class=\"hd\">Are you still here?</div>\n"
							+ "<div class=\"bd\">\n"
							+ "<div id=\"timeoutDialogText\">Your session is going to time out in 5 minutes.</div>\n"
							+ "<form method=\"GET\" action=\""
							+ WellKnown.urls.getKeepHttpSessionAlive()
							+ "\">\n" + "</form>\n" +
							// --
							"</div>" +
							// --
							"</div>\n");
		}

		if (!contents.isEmpty()) {
			out.print("<div id=\"outerContainer\">\n");
			out.print("<div id=\"tawalaProjectContainer\">\n");
			
			if(! context.isEmailDestination() && urgentMessage != null) {
				out.print("<div class=\"urgent-message\">\n");
				out.print(urgentMessage.getText());
				out.print("</div>");
			}

			if (googleAdSenseCode != null) {
				out.print("\n");
				out.print("<div id=\"tawalaRightSideAdsContainer\">\n");
				out.print(googleAdSenseCode);
				out.print("</div>\n");
				out.print("\n");
			}

			for (Html item : contents) {
				if (item != null) {
					item.render(out, context);
				}
			}

			out.print("<div id=\"innerTawalaFooter\"><div id=\"tawalaFooterLogo\" onclick=\"location.href='http://www.tawala.com'\"></div></div>\n");
			out.print("</div><!-- end tawalaProjectContainer -->\n");
			out.print("<div id=\"tawalaFooter\" onclick=\"location.href='http://www.tawala.com'\"></div>\n");
			
			out.print("</div><!-- end outerContainer -->\n");
		}
		out.print("</body>\n");
		out.print("</html>\n");
	}

	private String addBuildNumberToPreventCaching(String url) {
		return url + (url.indexOf('?') > 0 ? "&" : "?") + "x=" + buildNumber;
	}

	private void addStylesheets(PrintWriter out, String media,
			Set<String> stylesheets) {
		for (String stylesheet : stylesheets) {
			if (stylesheet == null) {
				continue;
			}
			String styleSheetURL = addBuildNumberToPreventCaching(stylesheet);
			out.print("<link rel=\"stylesheet\" href=\"" + styleSheetURL
					+ "\" type=\"text/css\" media=\"" + media + "\" />\n");
		}
	}

	public void add(Html html) {
		contents.add(html);
	}

	public void addToFront(Html html) {
		contents.add(0, html);
	}

	public void addAll(Collection<Html> html) {
		contents.addAll(html);
	}

	public void addContents(OldPage sourcePage) {
		if (sourcePage != null) {
			for (Html item : sourcePage.contents) {
				contents.add(item);
			}
		}
	}

	public void addScreenStylesheets(List<String> stylesheets) {
		screenStylesheets.addAll(stylesheets);
	}

	public void addPrintStylesheets(List<String> stylesheets) {
		printStylesheets.addAll(stylesheets);
	}

	public void addScript(String scriptName) {
		scripts.add(scriptName);
	}

	public boolean isEmpty() {
		return contents.size() == 0;
	}

	public String getGoogleAdSenseCode() {
		return googleAdSenseCode;
	}

	public void setGoogleAdSenseCode(String googleAdSenseCode) {
		this.googleAdSenseCode = googleAdSenseCode;
	}

	public void setSessionExpirationTimeout(int minutes) {
		sessionExpirationTimeout = minutes;
	}

	public UrgentMessage getUrgentMessage() {
		return urgentMessage;
	}

	public void setUrgentMessage(UrgentMessage urgentMessage) {
		this.urgentMessage = urgentMessage;
	}
}
