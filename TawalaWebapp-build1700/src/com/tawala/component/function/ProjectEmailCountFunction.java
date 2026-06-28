package com.tawala.component.function;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.ComponentRuntimeSupport;
import com.tawala.component.ReturnType;
import com.tawala.email.EmailService;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;

public class ProjectEmailCountFunction extends FunctionMetadataSupport {
	private static final String COMPONENT_ID = "project-email-count";

	public ProjectEmailCountFunction() {
		super(COMPONENT_ID, 1);
	}

	public ReturnType getReturnType() {
		return ReturnType.INT;
	}

	public Class<? extends Function> getRuntimeClass() {
		return Runtime.class;
	}

	public static class Runtime extends ComponentRuntimeSupport implements
			Function {

		public Runtime() {
		}

		public Runtime(ConfigElement configElement) {
			super(configElement);
		}

		public Value execute(ExecutionContext executionContext) {
			return new Value(String.valueOf(EmailService
					.getProjectEmailCount(executionContext.getUserProject()
							.getId())));
		}
	}
}
