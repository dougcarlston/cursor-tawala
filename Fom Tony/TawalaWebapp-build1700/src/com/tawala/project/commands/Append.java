package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Document;
import com.tawala.project.VirtualDocument;

public class Append extends ProcessCommand {
    private String targetDocumentId;
    private String appendDocumentid;

    public Append(ConfigElement config) {
        targetDocumentId = config.attribute("document").stringValue();
        appendDocumentid = config.attribute("appendage").stringValue();
    }

    public ExecutionResult execute(ExecutionContext context) {
        VirtualDocument result = context.getVirtualDocument(targetDocumentId);
        if (result == null) {
            Document source = context.getProject().getDocument(targetDocumentId);
            if (source == null) {
                result = new VirtualDocument(targetDocumentId);
            } else {
                result = new VirtualDocument(source);
            }
            context.add(result);
        }
        Document documentToAppend = context.getDocument(appendDocumentid);
        if(documentToAppend != null) {
        	result.append(documentToAppend.toHtml(context));
        }
        
        return ExecutionResult.NULL;
    }
}
