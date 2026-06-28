package com.tawala.component.function;

import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;

public interface Function {
	Value execute(ExecutionContext executionContext);
}
