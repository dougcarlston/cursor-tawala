package com.tawala.project.formatting;

import java.util.Formatter;
import java.util.HashMap;
import java.util.Map;

import com.scissor.xmlconfig.ConfigAttribute;
import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.component.function.FunctionToWebAdapter;
import com.tawala.component.repository.Repository;
import com.tawala.project.FieldReference;
import com.tawala.project.FormRenderable;
import com.tawala.project.ImageInstance;
import com.tawala.project.Text;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Block;
import com.tawala.web.oldhtml.Html;

// TODO: optimize by collapsing various tag inside into one inline style.
// For example <font><b><u>text</u></b></font> should result in one <span style="">text</span>
// Also, multiple adjacent <font> tags can be easily aggregated.
public class Font extends ContainerElement implements FormRenderable {
	private static final int TWIPS_TO_PIXEL_CONVERSION_FACTOR = 20;

	private final String styleAttribute;

	private static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();

	private static final Map<String, String> DESKTOP_FACE_TO_HTML_FONT_FAMILY_MAP = new HashMap<String, String>();

	static {
		FACTORY.register("field", FieldReference.class);
		FACTORY.register("b", Bold.class);
		FACTORY.register("u", Underline.class);
		FACTORY.register("i", Italics.class);
		FACTORY.registerText(Text.class);
		FACTORY.setKeepWhitespace(true);
		FACTORY.register("image", ImageInstance.class);
		FACTORY.register("invitation", LinkToProject.class);
		FACTORY.register("link", Link.class);

		Repository.registerWebComponentsWith(FACTORY);
		FunctionToWebAdapter.registerFunctionsWith(FACTORY);

		DESKTOP_FACE_TO_HTML_FONT_FAMILY_MAP.put("Arial",
				"Arial, Helvetica, sans-serif");
	}

	public Font(ConfigElement config) {
		super(FACTORY.makeChildren(config));

		String face = config.attribute("face").stringValue();
		ConfigAttribute sizeAttribute = config.attribute("size");
		int size = sizeAttribute.intValue(TWIPS_TO_PIXEL_CONVERSION_FACTOR * 10)
				/ TWIPS_TO_PIXEL_CONVERSION_FACTOR;
		int color = config.attribute("color").hexValue("0");

		StringBuilder buffer = new StringBuilder();

		if (config.hasAttribute("face")) {
			buffer.append("font-family: ").append(resolveFaceToWebFontFamily(face)).append("; ");
		}
				
		if (config.hasAttribute("size")) {
			buffer.append("font-size: ").append(size).append("pt; ");
		}

		if (config.hasAttribute("color")) {
			buffer.append("color: ").append(new Formatter().format("#%1$06X", color).toString()).append(";");
		}
				
		styleAttribute = buffer.toString();
	}

	public Html toHtml(ExecutionContext context) {
		SpanHtml result = new SpanHtml();
		result.setAttribute("style", styleAttribute);

		result.appendContents(getContents(), context);

		return result;
	}

	public static class SpanHtml extends Block {
		public SpanHtml() {
			super("span", false);
		}
	}

	private static String resolveFaceToWebFontFamily(String face) {
		String result = DESKTOP_FACE_TO_HTML_FONT_FAMILY_MAP.get(face);
		return result == null ? face : result;
	}
}
