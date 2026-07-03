package com.tawala.project.builder;

import com.scissor.XmlBuffer;

public class ForEachBuilder extends ProcessBlockBuilder {
	
	private String recordName;
	private String recordListName;
	
    public ForEachBuilder(String recordName, String recordListName) {
    	super(Type.forEach);
    	this.recordName = recordName;
    	this.recordListName = recordListName;
    }

    protected void startTag(XmlBuffer xml) {
        xml.startTag("foreach", "record", recordName, "recordList", recordListName);
    }

    protected void endTag(XmlBuffer xml) {
        xml.endTag("foreach");
    }

}
