package com.tawala.component.web.display;

import java.util.Collections;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterRestriction;
import com.tawala.component.ParameterType;
import com.tawala.component.parameter.WorksWithinRecordIteration;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.StringConcatenationExpression;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.Image;

public class DisplayImage extends WebComponentMetadataSupport {
	private static final String COMPONENT_ID = "display-image";
	public static final String SOURCE = "source";
	public static final String WIDTH = "width";
	public static final String HEIGHT = "height";
	public static final String ALT_TITLE = "alt_title";

	public DisplayImage() {
		super(COMPONENT_ID, 1);

		addParameters(new Parameter[] {
				new Parameter(
						SOURCE,
						COMPONENT_ID + "_" + SOURCE,
						ParameterType.EXPRESSION,
						true,
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.never,
										null))),
				new Parameter(WIDTH, COMPONENT_ID + "_" + WIDTH,
						ParameterType.EXPRESSION, false),
				new Parameter(HEIGHT, COMPONENT_ID + "_" + HEIGHT,
						ParameterType.EXPRESSION, false),
				new Parameter(ALT_TITLE, COMPONENT_ID + "_" + ALT_TITLE,
						ParameterType.EXPRESSION, false)

		});
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return RuntimeProcessor.class;
	}

	public static class RuntimeProcessor extends
			FormRenderableNotHoldingActiveComponents {
		private final StringConcatenationExpression sourceExpression;
		private final StringConcatenationExpression widthExpression;
		private final StringConcatenationExpression heightExpression;
		private final StringConcatenationExpression alternativeTitleExpression;

		public RuntimeProcessor(ConfigElement configElement) {
			sourceExpression = new StringConcatenationExpression(configElement
					.child(SOURCE));
			widthExpression = new StringConcatenationExpression(configElement
					.child(WIDTH));
			heightExpression = new StringConcatenationExpression(configElement
					.child(HEIGHT));
			alternativeTitleExpression = new StringConcatenationExpression(
					configElement.child(ALT_TITLE));
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}

		public Html toHtml(ExecutionContext context) {
			String sourceURL = sourceExpression.evaluate(context);
			String width = widthExpression.evaluate(context);
			String height = heightExpression.evaluate(context);
			String altTitle = alternativeTitleExpression.evaluate(context);
			if (sourceURL.length() == 0) {
				return new HtmlReadyString("");
			}

			Image image = new Image(sourceURL, altTitle);

			if (width.length() > 0) {
				try {
					image.setWidth(Integer.parseInt(width));
				} catch (NumberFormatException e) {
					Log.error(this, "Error parsing width: " + e);
				}
			}
			if (height.length() > 0) {
				try {
					image.setHeight(Integer.parseInt(height));
				} catch (NumberFormatException e) {
					Log.error(this, "Error parsing height: " + e);
				}
			}

			return image;
		}
	}
}
