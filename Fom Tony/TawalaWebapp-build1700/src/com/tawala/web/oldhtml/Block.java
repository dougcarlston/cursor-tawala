package com.tawala.web.oldhtml;

import java.io.PrintWriter;
import java.util.LinkedHashMap;
import java.util.Map;

import org.springframework.web.util.HtmlUtils;

public class Block extends HtmlItems {
    protected final Map<String, String> attributes = new LinkedHashMap<String, String>();
    protected final String tag;
    protected boolean newline;


    public void setNewLineAfterClosingTag(boolean newline) {
    	this.newline = newline;
    }
    
    public Block(String tag) {
        this(tag, true);
    }

    public Block(String tag, boolean newline) {
        this.tag = tag;
        this.newline = newline;
    }

    public Block(String tag, Html html) {
        this(tag, true, html);
    }

    public Block(String tag, boolean newline, Html html) {
        this(tag, newline);
        add(html);
    }

    public void render(PrintWriter out, RenderingContext context) {
        out.print("<" + tag);
        for (String name : attributes.keySet()) {
            out.print(" ");
            out.print(HtmlUtils.htmlEscape(name));
            out.print("=\"");
            out.print(HtmlUtils.htmlEscape(attributes.get(name)));
            out.print("\"");
        }
        out.print(">");
        if (newlineAfterOpeningTag()) out.print('\n');
        super.render(out, context);
        out.print("</" + tag + ">");
        if (newline) out.print('\n');
    }

    protected boolean newlineAfterOpeningTag() {
        return false;
    }

    public void setAttribute(String name, String value) {
        attributes.put(name, value);
    }
}
