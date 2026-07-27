package com.scissor;

import org.springframework.web.util.HtmlUtils;

public class XmlBuffer implements XmlRenderable {
    private final StringBuffer buffer;
    private static final String HEADER = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n";

    public XmlBuffer() {
        this.buffer = new StringBuffer();
    }


    public String toString() {
        return HEADER + buffer.toString();
    }

    public String toStringAsElement() {
        return buffer.toString();
    }

    public void startTag(String tag, String... attributes) {
        startTag(tag, true, attributes);

    }

    public void startTag(String tag, boolean newline, String... attributes) {
        append("<");
        append(tag);
        attributes(attributes);
        append(">");
        if (newline) newLine();
    }


    public void tag(String tag, String... attributes) {
        tag(tag, true, attributes);
    }

    public void tag(String tag, boolean newline, String... attributes) {
        append("<");
        append(tag);
        attributes(attributes);
        append("/>");
        if (newline) newLine();
    }

    private void attributes(String... attributes) {
        for (int i = 0; i < attributes.length; i += 2) {
            String name = attributes[i];
            String value = attributes[i + 1];
            if (name != null && value != null) attribute(name, value);
        }
    }

    private void attribute(String name, String value) {
        append(" ");
        append(name);
        append("=\"");
        append(HtmlUtils.htmlEscape(value));
        append("\"");
    }

    private void newLine() {
        append("\n");
    }

    private void append(String tag) {
        buffer.append(tag);
    }

    public void text(String tag, String text) {
        startTag(tag, false);
        endTag(tag);
    }

    public void endTag(String tag) {
        endTag(tag, true);
    }

    public void endTag(String tag, boolean newline) {
        append("</");
        append(tag);
        append(">");
        if (newline) newLine();
    }

    public void text(String text) {
        append(HtmlUtils.htmlEscape(text));
    }

    public void textAsCDATA(String text) {
        append("<![CDATA[");
        append(text);
        append("]]>");
    }

    public void append(XmlBuffer another) {
        buffer.append(another.buffer);
    }

    public boolean isEmpty() {
        return buffer.length() == 0;
    }

    public void render(XmlBuffer xml) {
        xml.append(this);
    }

	public void preformattedXml(String preformattedXml) {
		buffer.append(preformattedXml);
	}
}
