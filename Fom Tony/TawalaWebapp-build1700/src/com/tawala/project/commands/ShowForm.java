package com.tawala.project.commands;

import java.util.Set;

import com.scissor.xmlconfig.ConfigElement;

public class ShowForm extends ProcessCommand {
    private String formName;

    private ShowForm(String formName) {
        this.formName = formName;
    }

    public ShowForm(ConfigElement config) {
        this(config.attribute("form").stringValue());
    }

    public String getFormName() {
        return formName;
    }

    public ExecutionResult execute(ExecutionContext context) {
        return new ExecutionResult(formName);
    }
    
    @Override
    public void addFormsNamesCanNavigateTo(Set<String> result) {
    	result.add(formName);
    }
}
