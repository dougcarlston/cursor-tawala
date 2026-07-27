package com.tawala.project.builder;

import com.scissor.XmlBuffer;

public class ForEachMcBuilder extends ProcessBlockBuilder {
	
    public ForEachMcBuilder() {
    	super(Type.forEachMc);
    }

    protected void startTag(XmlBuffer xml) {
        xml.startTag("forEachMc");
    }

    protected void endTag(XmlBuffer xml) {
        xml.endTag("forEachMc");
    }

}
