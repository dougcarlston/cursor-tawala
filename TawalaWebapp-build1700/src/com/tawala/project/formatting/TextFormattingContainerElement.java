package com.tawala.project.formatting;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.component.function.FunctionToWebAdapter;
import com.tawala.component.repository.Repository;
import com.tawala.component.web.ResponseCreator;
import com.tawala.project.FieldReference;
import com.tawala.project.FormRenderable;
import com.tawala.project.ImageInstance;
import com.tawala.project.Text;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;

abstract public class TextFormattingContainerElement extends ContainerElement implements FormRenderable {
	private static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();
    
    static {
        FACTORY.register("field", FieldReference.class);
        FACTORY.registerText(Text.class);
        FACTORY.setKeepWhitespace(true);
        FACTORY.register("image", ImageInstance.class);
        FACTORY.register("b", Bold.class);
        FACTORY.register("i", Italics.class);
        FACTORY.register("u", Underline.class);
        FACTORY.register("invitation", LinkToProject.class);
		FACTORY.register("link", Link.class);
        
		Repository.registerWebComponentsWith(FACTORY);
		FunctionToWebAdapter.registerFunctionsWith(FACTORY);
    }

    public TextFormattingContainerElement(ConfigElement config) {
        super(FACTORY.makeChildren(config));
    }

	public final Html toHtml(ExecutionContext context) {
		HtmlItems result = getHtmlElement();

		result.appendContents(getContents(), context);
		
		return result;
	}

	protected abstract HtmlItems getHtmlElement();
	
	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		for (FormRenderable formRenderable : getContents()) {
			ResponseCreator responseCreator = formRenderable.getResponseCreatorForComponentId(componentId);
			if(responseCreator != null) {
				return responseCreator;
			}
		}
		return null;
	}
}
