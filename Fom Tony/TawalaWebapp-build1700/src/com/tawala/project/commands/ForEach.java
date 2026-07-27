package com.tawala.project.commands;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.CompositeFormSubmission;

public class ForEach extends ProcessCommand {
	private ProcessCommandList commands;
	private String recordName;
	private String recordListName;

    public ForEach(ConfigElement config) {
    	super(config);
    	commands = new ProcessCommandList(config);
    	recordName = config.attribute("record").stringValue();
    	recordListName = config.attribute("recordList").stringValue();
    }

    public ExecutionResult execute(ExecutionContext context) {
    	ExecutionResult result = new ExecutionResult();
    	List<CompositeFormSubmission> recordList = context.getRecordList(recordListName);
    	for (CompositeFormSubmission record : recordList) {
    		context.mapRecord(recordName, record);
    		ExecutionResult recordProcessingResult = commands.execute(context);
			result = result.add(recordProcessingResult);
			if(recordProcessingResult.stopImmediately()) {
				break;
			}
    	}
    	context.removeRecordMapping(recordName);
    	return result;
    }
    
    public ProcessCommandList commands() {
    	return commands;
    }
    
    public String recordName() {
    	return recordName;
    }
    
    public String recordListName() {
    	return recordListName;
    }
    

}
