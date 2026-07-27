package com.tawala.component.web.form;

import java.util.Set;

import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;

public interface MultiChoiceDisplayComponent {
	/**
	 * Create the actual question to be used in a form.
	 * @param context
	 * @return
	 */
	Html toHtml(ExecutionContext context);
	
	/**
	 * Create a view of the selection based on the values selected.
	 * 
	 * @param context
	 * @param selectedValues
	 * @return
	 */
	Html toHtml(ExecutionContext context, Set<String> selectedValues);
}
