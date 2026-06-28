package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.List;

import com.tawala.web.oldhtml.Html;

/**
 * The result of the execution of a ProcessCommand.
 * 
 * TODO: split into a short hierarchy - the ones that produce HTML and the onces 
 * that are about controlling flow (ShowForm).
 * 
 */
public class ExecutionResult {
    public static final ExecutionResult NULL = new ExecutionResult();

    private List<Html> html = new ArrayList<Html>();
    private String nextForm;

    private boolean stopImmediately;
    private String redirectURL;

    public ExecutionResult(Html html) {
        this.html.add(html);
        stopImmediately = false;
    }

    public ExecutionResult(ExecutionResult... results) {
        for (ExecutionResult result : results) {
        	if(result.requireRedirect()) {
        		this.redirectURL = result.getRedirectURL();
        		return;
        	}
            html.addAll(result.html);
            
            if (!hasNextForm() && result.hasNextForm()) 
                this.nextForm = result.nextForm;
            
            if(result.stopImmediately)
                this.stopImmediately = true;
        }
    }

    protected ExecutionResult() {
        stopImmediately = false;
    }

    public ExecutionResult(List<Html> html) {
        stopImmediately = false;
        html.addAll(html);
    }

    public ExecutionResult(String nextFormId) {
        this.nextForm = nextFormId;
        this.stopImmediately = true;
    }

	public ExecutionResult add(ExecutionResult other) {
        return new ExecutionResult(this, other);
    }

    public boolean hasOutput() {
        return !html.isEmpty();
    }

    public List<Html> getHtml() {
        return html;
    }

    public boolean stopImmediately() {
        return stopImmediately;
    }
    
    public boolean requireRedirect() {
    	return redirectURL != null;
    }

    public boolean hasNextForm() {
        return nextForm != null;
    }

    public String getNextForm() {
        return nextForm;
    }

	public String getRedirectURL() {
		return redirectURL;
	}
}
