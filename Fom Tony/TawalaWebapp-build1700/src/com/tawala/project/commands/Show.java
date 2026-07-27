package com.tawala.project.commands;

import com.scissor.xmlconfig.ConfigElement;

public class Show extends ProcessCommand {
	private StringConcatenationExpression urlExpression;

    public Show(ConfigElement config) {
        this.urlExpression = new StringConcatenationExpression((config.child("url")));
    }

    public StringConcatenationExpression getUrlExpression() {
		return urlExpression;
	}

	public ExecutionResult execute(ExecutionContext context) {
    	String url = urlExpression.evaluate(context);
    	if(url.length() == 0) {
    		return ExecutionResult.NULL;
    	}
        return new RedirectExecution(url);
    }
}
