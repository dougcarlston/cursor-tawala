package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Document;
import com.tawala.project.VirtualDocument;

public class ShowDocument extends ProcessCommand {
    private String documentName;
    private boolean resetAfterShowing;

    public ShowDocument(String documentName, boolean resetOnCompletion) {
        assert documentName != null;
        this.documentName = documentName;
        this.resetAfterShowing = resetOnCompletion;
    }

    public ShowDocument(String documentName) {
        this(documentName, false);
    }

    public ShowDocument(ConfigElement config) {
        this(config.attribute("document").stringValue(), config.attribute(
                "reset").booleanValue());
    }

    public String getDocumentName() {
        return documentName;
    }

    public ExecutionResult execute(ExecutionContext context) {
        // --- TODO: should it throw an exception?
        Document document = context.getDocument(documentName);
        if (document == null) {
            return ExecutionResult.NULL;
        } else {
            ExecutionResult result = new ExecutionResult(document
                    .toHtml(context));
            
            if (resetAfterShowing && document instanceof VirtualDocument) {
                VirtualDocument virtualDocument = (VirtualDocument) document;
                virtualDocument.reset();
            }
            
            return result;
        }
    }
}
