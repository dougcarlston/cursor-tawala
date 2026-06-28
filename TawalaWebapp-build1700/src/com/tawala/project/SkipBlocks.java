package com.tawala.project;

import java.util.ArrayList;

import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.SkipBlock;
import com.tawala.project.commands.SkipExecutionResult;

public class SkipBlocks extends ArrayList<SkipBlock> {
	private static final long serialVersionUID = 1L;

	public SkipExecutionResult execute(ExecutionContext context) {
		for (SkipBlock skipBlock : this) {
			SkipExecutionResult result = skipBlock
					.execute(context);
			context.storeExecutionContextSnapshot();
			if (result.hasSkipTo())
				return result;
		}
		return SkipExecutionResult.NULL;
	}

	public SkipExecutionResult execute(Form form, ExecutionContext context) {
		for (SkipBlock skipBlock : this) {
			SkipExecutionResult result = skipBlock.execute(context);
			if (result.hasSkipTo())
				return result;
		}
		return SkipExecutionResult.NULL;
	}
}
