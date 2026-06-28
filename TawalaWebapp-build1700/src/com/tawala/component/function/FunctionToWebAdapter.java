package com.tawala.component.function;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.component.repository.Repository;
import com.tawala.project.FormRenderable;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.Value;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlString;

public class FunctionToWebAdapter extends FormRenderableNotHoldingActiveComponents {
	private final static Factory<Function> FACTORY = new Factory<Function>();
	
	static {
		Repository.registerFunctionsWith(FACTORY);
	}
	
	final private Function function;
	
	public FunctionToWebAdapter(ConfigElement configElement) {
		this.function = FACTORY.make(configElement); 
	}

	public Html toHtml(ExecutionContext context) {
		Value executionResult = function.execute(context);
		if (executionResult == null) {
			return null;
		} else {
			return new HtmlString(executionResult.toString());
		}
	}

	public static void registerFunctionsWith(Factory<FormRenderable> factory) {
		for (FunctionMetadata functionMetadata : Repository.getAllFunctionMetadata()) {
			factory.register(functionMetadata.getId(), FunctionToWebAdapter.class);
		}
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}
}
