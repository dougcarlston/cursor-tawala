package com.tawala.project;

import com.scissor.xmlconfig.ConfigText;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlString;

public class Text extends FormRenderableNotHoldingActiveComponents implements TextRenderable {
    private final String contents;

    public Text(String contents) {
        this.contents = contents;
    }

    public Text(ConfigText text) {
        this(text.text());
    }

    public Html toHtml(ExecutionContext context) {
        return new HtmlString(contents);
    }

    public String getContents() {
        return contents;
    }

    public String toString() {
        return contents;
    }

	public boolean isEmpty(ExecutionContext context) {
		return contents.trim().length() == 0;
	}

	public void appendTo(StringBuilder result, ExecutionContext context) {
		result.append(contents);
	}
}
