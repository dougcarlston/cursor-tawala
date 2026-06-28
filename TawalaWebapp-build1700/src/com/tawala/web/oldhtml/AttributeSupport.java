package com.tawala.web.oldhtml;

import java.io.PrintWriter;
import java.util.LinkedHashMap;
import java.util.Map;

import org.springframework.web.util.HtmlUtils;

public class AttributeSupport {
    private Map<String, String> attributes = null;

    public AttributeSupport(String ... attributes) {
		for (int i = 0; i < attributes.length; i += 2) {
			setAttribute(attributes[i], attributes[i + 1]);
		}
    }
    
	public final void setAttribute(String name, String value) {
		if(attributes == null) {
			attributes = new LinkedHashMap<String, String>();
		}
		attributes.put(name, value);
	}
	
	public void renderAttributes(PrintWriter out) {
		if(attributes == null) {
			return;
		}
		for (String name : attributes.keySet()) {
			out.print(" ");
			out.print(HtmlUtils.htmlEscape(name));
			out.print("=\"");
			out.print(HtmlUtils.htmlEscape(attributes.get(name)));
			out.print("\"");
		}
	}
}
