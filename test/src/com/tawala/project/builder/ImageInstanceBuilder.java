package com.tawala.project.builder;

import com.scissor.XmlBuffer;

public class ImageInstanceBuilder extends TagBuilder {
    private int width;
    private int height;
    private String id;

    public ImageInstanceBuilder(String id, int width, int height) {
        this.id = id;
        this.height = height;
        this.width = width;
    }

    protected void startTag(XmlBuffer xml) {
        xml.tag("image", true, "id", id, "width", Integer.toString(width),
                "height", Integer.toString(height));
    }

    protected void endTag(XmlBuffer xml) {
        // --- Do nothing
    }
}
