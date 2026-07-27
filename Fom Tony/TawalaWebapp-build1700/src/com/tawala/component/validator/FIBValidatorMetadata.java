package com.tawala.component.validator;

import com.tawala.component.ComponentMetadata;

public interface FIBValidatorMetadata extends ComponentMetadata {
	Class<? extends FieldValidator> getFibValidator();
}
