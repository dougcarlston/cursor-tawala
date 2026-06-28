package com.tawala.component.function;

import com.tawala.component.ComponentMetadata;
import com.tawala.component.ReturnType;

public interface FunctionMetadata extends ComponentMetadata {
	ReturnType getReturnType();
	Class<? extends Function> getRuntimeClass();
}
