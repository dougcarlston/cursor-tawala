package com.tawala.project.commands;

import com.tawala.project.Form;
import com.tawala.project.FormSegment;
import com.tawala.web.Request;

public class SkipExecutionResult extends ExecutionResult {
	private String skipTo;
	public static final SkipExecutionResult NULL = new SkipExecutionResult();

	public SkipExecutionResult(ExecutionResult... results) {
		super(results);
		for (ExecutionResult result : results) {
			if (result instanceof SkipExecutionResult) {
				skipTo = ((SkipExecutionResult) result).skipTo;
			}
		}
	}

	public SkipExecutionResult(String skipTo) {
		this.skipTo = skipTo;
	}

	public SkipExecutionResult add(ExecutionResult other) {
		return new SkipExecutionResult(this, other);
	}

	public String getSkipTo() {
		return skipTo;
	}

	public boolean hasSkipTo() {
		return skipTo != null;
	}

	public FormSegment getDestinationSegment(Form form, Request request) {
		FormSegment next = null;
		if (SkipBlock.SKIP_TO_END.equals(skipTo))
			return null;
		next = getDestinationSegment(form);
		if (next == null)
			next = form.getSegmentAfter(request);
		return next;
	}

	public FormSegment getDestinationSegment(Form form) {
		if (hasSkipTo()) {
			return form.getSegmentForSkip(this.getSkipTo());
		}
		return null;
	}
}
