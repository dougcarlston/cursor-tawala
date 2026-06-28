package com.tawala.project.formatting;

import java.util.List;

import org.json.JSONException;
import org.springframework.web.util.HtmlUtils;

import com.scissor.Log;
import com.tawala.component.validator.FieldValidator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Block;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.TextInput;

public class ValidationSupport {

	public static Block createRegisterFormValidatorsScript(
			ExecutionContext context, String id, List<FieldValidator> validators) {
		Block javaScriptValidationCode = new Block("script");
		javaScriptValidationCode.setNewLineAfterClosingTag(false);
		for (FieldValidator validator : validators) {
			try {
				javaScriptValidationCode.add(new HtmlReadyString(
						"Tawala.validation.register('"
								+ HtmlUtils.htmlEscape(id) + "', "
								+ validator.getJavaScriptFunctionName(context)
								+ ", "
								+ validator.getJavaScriptParameters(context)
								+ ");"));
			} catch (JSONException e) {
				Log.error(FieldValidator.class, "Error adding a validator to "
						+ id, e);
			}
		}
		return javaScriptValidationCode;
	}

}
