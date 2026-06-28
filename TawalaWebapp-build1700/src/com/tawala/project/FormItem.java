package com.tawala.project;

import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.NoHtml;

public abstract class FormItem implements FormRenderable {
    private final String id;
    private final String alternateLabel;
    private String formId;
    private BooleanExpression displayCondition;

    protected FormItem(ConfigElement element) {
        this.id = element.attribute("label").stringValue();
        this.alternateLabel = element.attribute("alternateLabel").stringValue();
        ConfigElement displayConditionElement = element.child("displayConditions");
        if(displayConditionElement != null) {
        	this.displayCondition = BooleanExpression.load(displayConditionElement.childElement(0));
        }
    }

    public final Html toHtml(ExecutionContext context) {
    	if(context.isPreviewMode() || shouldBeDisplayed(context)) {
    		return produceHtml(context);
    	} else {
    		return NoHtml.INSTANCE; 
    	}
    }
    
    public abstract boolean hasFields();
    public abstract List<Field> fields();
    public abstract Html produceHtml(ExecutionContext context);

    public boolean matchesId(String skipTo) {
        if (skipTo == null) return false;
        return skipTo.equals(id) || skipTo.equals(alternateLabel);
    }

    String getFormId() {
        return formId;
    }

    protected void setFormId(String formId) {
        this.formId = formId;
    }

    public String getId() {
        return id;
    }

    public String getAlternateLabel() {
        return alternateLabel;
    }
    
    public boolean shouldBeDisplayed(ExecutionContext context) {
    	return displayCondition == null || displayCondition.isTrue(context);
    }
}
