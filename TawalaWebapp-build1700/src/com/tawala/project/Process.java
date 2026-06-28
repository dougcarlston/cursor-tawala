package com.tawala.project;

import java.util.HashSet;
import java.util.Set;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.ExecutionResult;
import com.tawala.project.commands.ProcessCommand;
import com.tawala.project.commands.ProcessCommandList;

public class Process {
    private final String name;
    private final ProcessCommandList commands;

    public Process(ConfigElement config) {
        this(config.attribute("name").stringValue(), new ProcessCommandList(config));
    }

    public Process(String name) {
        this(name, new ProcessCommandList());
    }

    private Process(String name, ProcessCommandList commands) {
        assert name != null && !"".equals(name.trim());
        this.name = name;
        this.commands = commands;
    }

    public String getName() {
        return name;
    }

    public ExecutionResult execute(ExecutionContext context) {
        return commands.execute(context);
    }

    public int size() {
        return commands.size();
    }

    public ProcessCommand get(int index) {
        return commands.get(index);
    }

    public boolean add(ProcessCommand o) {
        return commands.add(o);
    }
    
    public Set<String> getFormNamesCanNavigateTo() {
    	Set<String> result = new HashSet<String>();
    	for (ProcessCommand command : commands) {
			command.addFormsNamesCanNavigateTo(result);
		}
    	return result;
    }
}
