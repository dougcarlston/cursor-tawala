package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;

public class ForEachMc extends ProcessCommand {

	ProcessCommandList commands;

    public ForEachMc(ConfigElement config) {
        super();
    	commands = new ProcessCommandList(config);
    }

    public ExecutionResult execute(ExecutionContext context) {
    	
    	ExecutionResult result = new ExecutionResult();
    	List<String> mcItemIds = context.getForm().getMcItemIds();
    	
    	for (String id : mcItemIds) {
    		context.mapSelection(id);
    		result = result.add(commands.execute(context));
    	}
    	return result;
    }
    
    public ProcessCommandList commands() {
    	return commands;
    }
    
}
