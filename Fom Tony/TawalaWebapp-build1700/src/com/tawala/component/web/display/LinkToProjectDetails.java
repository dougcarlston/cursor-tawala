package com.tawala.component.web.display;

import java.util.Collections;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Option;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterRestriction;
import com.tawala.component.ParameterType;
import com.tawala.component.parameter.WorksWithinRecordIteration;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.UserProject;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.StringConcatenationExpression;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.Image;
import com.tawala.web.oldhtml.Link;
import com.tawala.web.projectmanager.ViewProjectManagerDetailsController;

public class LinkToProjectDetails extends WebComponentMetadataSupport {
	public static final String OPEN_IN_CURRENT_WINDOW_OPTION = "current-window";
	public static final String OPEN_IN_NEW_WINDOW_OPTION = "new-window";
	private static final String COMPONENT_ID = "link-to-project-details";
	public static final String LINK_DESCRIPTION = "description";
	public static final String OPEN_PREFERENCE = "open-preference";

	private static final Option[] OPENING_OPTIONS = new Option[] {
			new Option(OPEN_IN_CURRENT_WINDOW_OPTION,
					"option.open-in-current-window"),
			new Option(OPEN_IN_NEW_WINDOW_OPTION, "option.open-in-new-window"),

	};

	public LinkToProjectDetails() {
		super(COMPONENT_ID, 1);

		addParameters(new Parameter[] {
				new Parameter(
						LINK_DESCRIPTION,
						COMPONENT_ID + "_" + LINK_DESCRIPTION,
						ParameterType.EXPRESSION,
						true,
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.never,
										null))),
				new Parameter(OPEN_PREFERENCE, COMPONENT_ID + "_"
						+ OPEN_PREFERENCE, ParameterType.ENUMERATION, true)
						.addOptions(OPENING_OPTIONS) });
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return RuntimeProcessor.class;
	}

	public static class RuntimeProcessor extends
			FormRenderableNotHoldingActiveComponents {
		private final StringConcatenationExpression descriptionExpression;
		private final String openingOption;

		public RuntimeProcessor(ConfigElement configElement) {
			openingOption = configElement.child(OPEN_PREFERENCE).text();
			descriptionExpression = new StringConcatenationExpression(
					configElement.child(LINK_DESCRIPTION));
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}

		public Html toHtml(ExecutionContext context) {
			StringBuilder urlBuilder = new StringBuilder();
			urlBuilder.append("http").append("://").append(
					UserProject.getWebsiteHostName());
			urlBuilder.append(WellKnown.urls.getProjectManagerProjectDetailView())
					.append('?');
			urlBuilder.append(
					ViewProjectManagerDetailsController.PARAMETER_PROJECT_NAME)
					.append('=').append(context.getUserProject().getName());
			
			String text = descriptionExpression.evaluate(context);
			boolean openInNewWindow = openingOption
					.equals(OPEN_IN_NEW_WINDOW_OPTION);

			Link result = new Link(urlBuilder.toString(), text, openInNewWindow);

			return result;
		}
	}

	public static class PurchaseType {
		Image image;
		String payPalCommand;

		PurchaseType(Image image, String command) {
			this.image = image;
			this.payPalCommand = command;
		}
	}
}
