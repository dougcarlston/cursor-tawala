package com.tawala.project.builder;

import java.util.ArrayList;
import java.util.List;

import com.scissor.XmlBuffer;
import com.scissor.XmlRenderable;
import com.scissor.xmlconfig.ConfigElement;

public abstract class TagBuilder implements XmlRenderable {
    private final List<XmlRenderable> contents;

    protected TagBuilder() {
        contents = new ArrayList<XmlRenderable>();
        contents.add(new XmlBuffer());
    }

    protected XmlBuffer contents() {
        return (XmlBuffer) contents.get(contents.size() - 1);
    }

    protected boolean hasContent() {
        return contents.size() > 1 || !contents().isEmpty();
    }

    public boolean isEmpty() {
        return !hasContent();
    }

    public void render(XmlBuffer xml) {
        startTag(xml);
        for (XmlRenderable item : contents) {
            item.render(xml);
        }
        endTag(xml);
    }

    protected abstract void startTag(XmlBuffer xml);

    protected abstract void endTag(XmlBuffer xml);

    public void add(XmlRenderable renderable) {
        contents.add(renderable);
        contents.add(new XmlBuffer());
    }

    public ConfigElement asConfig() {
        return new ConfigElement(xmlAsString());
    }

    public String xmlAsString() {
        XmlBuffer xml = new XmlBuffer();
        render(xml);
        return xml.toString();
    }

    public void dumpXml() {
        System.out.println(xmlAsString());

    }

    public String toString() {
        return xmlAsString();
    }

    public String toStringAsElement() {
        XmlBuffer xmlBuffer = new XmlBuffer();
        render(xmlBuffer);
        return xmlBuffer.toStringAsElement();
    }
}
