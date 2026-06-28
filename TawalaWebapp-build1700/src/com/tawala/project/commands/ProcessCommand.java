package com.tawala.project.commands;

import java.util.Set;

import com.scissor.xmlconfig.ConfigElement;

public abstract class ProcessCommand {
    public static final ProcessCommand NULL = new NullCommand();

    public ProcessCommand() {
    }
    
    public ProcessCommand(ConfigElement configElement) {
    	if(configElement == null) {
    		return;
    	}
    	ConfigElement comments = configElement.child("comment");
    	if(comments != null) {
    		comments.markUsed();
    	}
    }
    
    public abstract ExecutionResult execute(ExecutionContext context);
    
    public void addFormsNamesCanNavigateTo(Set<String> result) {
    	//--- Does nothing by default.
    }

    private static class NullCommand extends ProcessCommand {
        public ExecutionResult execute(ExecutionContext context) {
            return ExecutionResult.NULL;
        }
    }
}
