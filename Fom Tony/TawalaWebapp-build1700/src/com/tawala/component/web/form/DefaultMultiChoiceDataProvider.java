package com.tawala.component.web.form;

import java.util.ArrayList;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.project.Checkbox;
import com.tawala.project.commands.ExecutionContext;

public class DefaultMultiChoiceDataProvider extends WebComponentMetadataSupport {
	public static final String COMPONENT_ID = "default-mcq";

	public DefaultMultiChoiceDataProvider() {
		super(COMPONENT_ID, 1);
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return Runtime.class;
	}

	public static class Runtime extends MultiChoiceDataProviderRuntimeSupport {
		public Runtime() {
			// --- Nothing to configure
		}

		public Runtime(ConfigElement configElement) {
			// --- Nothing to configure
		}

		public List<Checkbox> getChoices(ExecutionContext context) {
			List<Checkbox> result = new ArrayList<Checkbox>(getQuestion()
					.getDefaultItems().size());
			for (Checkbox checkbox : getQuestion().getDefaultItems()) {
				if (!checkbox.isEmpty(context)) {
					result.add(checkbox);
				}
			}

			if (result.size() == 0
					&& getQuestion().getDefaultItems().size() > 0) {
				return getQuestion().getDefaultItems();
			} else {
				return result;
			}
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}

		public boolean isSafeToGetChoicesWithoutRealContext() {
			return true;
		}
	}
}
