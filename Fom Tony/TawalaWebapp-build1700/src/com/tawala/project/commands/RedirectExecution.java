package com.tawala.project.commands;

public class RedirectExecution extends ExecutionResult {
	private final String redirectTo;
	public RedirectExecution(String redirectTo) {
		this.redirectTo = redirectTo;
	}
	
	@Override
	public boolean stopImmediately() {
		return true;
	}
	
	@Override
	public boolean requireRedirect() {
		return true;
	}
	
	@Override
	public String getRedirectURL() {
		return redirectTo;
	}
}
