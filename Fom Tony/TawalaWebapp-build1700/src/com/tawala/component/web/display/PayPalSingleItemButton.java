package com.tawala.component.web.display;

import java.math.BigDecimal;
import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.Option;
import com.tawala.component.Parameter;
import com.tawala.component.ParameterRestriction;
import com.tawala.component.ParameterType;
import com.tawala.component.parameter.WorksWithinRecordIteration;
import com.tawala.component.web.WebComponentMetadataSupport;
import com.tawala.payment.ProjectInvoice;
import com.tawala.payment.paypal.PayPalProperties;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.ImageInstance;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.StringConcatenationExpression;
import com.tawala.project.theme.ProjectTheme;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.oldhtml.HiddenInput;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlForm;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlReadyString;
import com.tawala.web.oldhtml.Image;
import com.tawala.web.oldhtml.ImageInput;

public class PayPalSingleItemButton extends WebComponentMetadataSupport {
	public static final String TAWALA_STYLE = "tawala";
	private static final String COMPONENT_ID = "paypal-single-item-button";
	public static final String ITEM_DESCRIPTION_REFERENCE = "item";
	public static final String AMOUNT_REFERENCE = "amount";
	public static final String RETURN_ON_SUCCESS = "successful-payment-return-form-name";
	public static final String RETURN_ON_CANCEL = "cancelled-payment-return-form-name";
	public static final String FIELD_TO_RECORD_STATUS = "status-field";
	public static final String FIELD_TO_RECORD_PAID_AMOUNT = "amount-field";
	public static final String BUTTON_TYPE = "button-type";
	public static final String STYLE_OPTION = "style-option";

	private static final Image PAYPAL_BUY_BUTTON = new Image(
			"https://www.paypal.com/en_US/i/btn/btn_buynow_LG.gif", "Buy Now",
			107, 26);
	private static final Image PAYPAL_BUY_BUTTON_WITH_LOGOS = new Image(
			"https://www.paypal.com/en_US/i/btn/btn_buynowCC_LG.gif",
			"Buy Now", 122, 47);
	private static final Image PAYPAL_PAY_BUTTON = new Image(
			"https://www.paypal.com/en_US/i/btn/btn_paynow_LG.gif", "Pay Now",
			107, 26);
	private static final Image PAYPAL_PAY_BUTTON_WITH_LOGOS = new Image(
			"https://www.paypal.com/en_US/i/btn/btn_paynowCC_LG.gif",
			"Pay Now", 122, 47);

	private static final Image PAYPAL_DONATE_BUTTON = new Image(
			"https://www.paypal.com/en_US/i/btn/btn_donate_LG.gif",
			"Donate Now", 92, 26);
	private static final Image PAYPAL_DONATE_BUTTON_WITH_LOGOS = new Image(
			"https://www.paypal.com/en_US/i/btn/btn_donateCC_LG.gif",
			"Donate Now", 122, 47);

	public static Map<String, PurchaseType> BUTTON_ID_TO_PURCHASE_TYPE_MAP = new HashMap<String, PurchaseType>();
	static {
		BUTTON_ID_TO_PURCHASE_TYPE_MAP.put("buy", new PurchaseType(
				PAYPAL_BUY_BUTTON, "_xclick"));
		BUTTON_ID_TO_PURCHASE_TYPE_MAP.put("buy-logo", new PurchaseType(
				PAYPAL_BUY_BUTTON_WITH_LOGOS, "_xclick"));

		BUTTON_ID_TO_PURCHASE_TYPE_MAP.put("pay", new PurchaseType(
				PAYPAL_PAY_BUTTON, "_xclick"));
		BUTTON_ID_TO_PURCHASE_TYPE_MAP.put("pay-logo", new PurchaseType(
				PAYPAL_PAY_BUTTON_WITH_LOGOS, "_xclick"));

		BUTTON_ID_TO_PURCHASE_TYPE_MAP.put("donate", new PurchaseType(
				PAYPAL_DONATE_BUTTON, "_donations"));
		BUTTON_ID_TO_PURCHASE_TYPE_MAP.put("donate-logo", new PurchaseType(
				PAYPAL_DONATE_BUTTON_WITH_LOGOS, "_donations"));
	}

	private static final Option[] BUTTON_OPTIONS = new Option[] {
			new Option("buy", "paypal.button.buy"),
			new Option("buy-logo", "paypal.button.buy-logo"),

			new Option("pay", "paypal.button.pay"),
			new Option("pay-logo", "paypal.button.pay-logo"),

			new Option("donate", "paypal.button.donate"),
			new Option("donate-logo", "paypal.button.donate-logo"),

	};

	public PayPalSingleItemButton() {
		super(COMPONENT_ID, 1);

		addParameters(new Parameter[] {
				new Parameter(BUTTON_TYPE, COMPONENT_ID + "_" + BUTTON_TYPE,
						ParameterType.ENUMERATION, true)
						.addOptions(BUTTON_OPTIONS),
				new Parameter(
						ITEM_DESCRIPTION_REFERENCE,
						COMPONENT_ID + "_" + ITEM_DESCRIPTION_REFERENCE,
						ParameterType.EXPRESSION,
						true,
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.never,
										null))),
				new Parameter(
						AMOUNT_REFERENCE,
						COMPONENT_ID + "_" + AMOUNT_REFERENCE,
						ParameterType.EXPRESSION,
						true,
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.never,
										null))),
				new Parameter(RETURN_ON_SUCCESS, COMPONENT_ID + "_"
						+ RETURN_ON_SUCCESS, ParameterType.FORM_NAME, true),
				new Parameter(RETURN_ON_CANCEL, COMPONENT_ID + "_"
						+ RETURN_ON_CANCEL, ParameterType.FORM_NAME, true),
				new Parameter(
						FIELD_TO_RECORD_STATUS,
						COMPONENT_ID + "_" + FIELD_TO_RECORD_STATUS,
						ParameterType.BLANK_FIELD_NAME,
						true,
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.never,
										null))),
				new Parameter(
						FIELD_TO_RECORD_PAID_AMOUNT,
						COMPONENT_ID + "_" + FIELD_TO_RECORD_PAID_AMOUNT,
						ParameterType.BLANK_FIELD_NAME,
						true,
						Collections
								.singletonList((ParameterRestriction) new WorksWithinRecordIteration(
										WorksWithinRecordIteration.When.never,
										null))),
				new Parameter(STYLE_OPTION, COMPONENT_ID + "_" + STYLE_OPTION,
						ParameterType.ENUMERATION, true)
						.addOptions(new Option[] {
								new Option("paypal", COMPONENT_ID
										+ ".option.style.paypay-default"),
								new Option(TAWALA_STYLE, COMPONENT_ID
										+ ".option.style.tawala-style") }), });
	}

	@SuppressWarnings("unchecked")
	public Class getRuntimeProcessingClass() {
		return RuntimeProcessor.class;
	}

	public static class RuntimeProcessor extends
			FormRenderableNotHoldingActiveComponents {
		private final PurchaseType purchaseType;
		private final StringConcatenationExpression amountExpression;
		private final StringConcatenationExpression itemDescriptionExpression;
		private final String currencyCode = "USD";
		private final String returnFormName;
		private final String cancelReturnFormName;
		private final String fieldToRecordStatus;
		private final String fieldToRecordAmount;
		private final boolean useTawalaStyling;

		public RuntimeProcessor(ConfigElement configElement) {
			String buttonId = configElement.child(BUTTON_TYPE).text();
			purchaseType = BUTTON_ID_TO_PURCHASE_TYPE_MAP.get(buttonId);
			itemDescriptionExpression = new StringConcatenationExpression(
					configElement.child(ITEM_DESCRIPTION_REFERENCE));
			amountExpression = new StringConcatenationExpression(configElement
					.child(AMOUNT_REFERENCE));
			returnFormName = configElement.child(RETURN_ON_SUCCESS).text();
			cancelReturnFormName = configElement.child(RETURN_ON_CANCEL).text();
			fieldToRecordStatus = configElement.child(FIELD_TO_RECORD_STATUS)
					.text();
			ConfigElement fieldToRecordAmountElement = configElement
					.child(FIELD_TO_RECORD_PAID_AMOUNT);

			fieldToRecordAmount = fieldToRecordAmountElement == null ? null
					: fieldToRecordAmountElement.text();
			ConfigElement styleElement = configElement.child(STYLE_OPTION);
			useTawalaStyling = styleElement != null
					&& TAWALA_STYLE.equals(styleElement.text());
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}

		public Html toHtml(ExecutionContext context) {
			if (context.getUserProject().getUser().getPayPalAccountId() == null) {
				return displayInactiveButton(
						context,
						"This project is not set up to provide PayPal integration. Please contact Tawala at info@tawala.com to inquire about the details.");
			} else {
				BigDecimal amount = null;
				String amountString = amountExpression.evaluate(context);
				try {
					amount = new BigDecimal(amountString);
				} catch (NumberFormatException e) {
					Log.error(this,
							"Error occurred trying to parse the amount of the transaction. Amount: '"
									+ amountString + "', user project id: "
									+ context.getUserProject().getId()
									+ ", submission data: "
									+ context.getSubmission().toString());
				}
				if (amount == null) {
					return displayInactiveButton(context,
							"We are sorry - an error occurred calculating the payment amount.");
				} else {
					return displayValidButton(context, amount);
				}
			}
		}

		private Html displayInactiveButton(ExecutionContext context,
				String message) {
			HtmlForm form = new HtmlForm();
			form.setMethod("post");

			ImageInput imageInput = new ImageInput("submit", "submit", "0",
					purchaseType.image);
			imageInput
					.setOnClickHandler("displayPayPalIntegrationNotCompleteMessage(); return false;");
			form.add(imageInput);

			HtmlItems result = new HtmlItems();
			result.add(form);
			result
					.add(new HtmlReadyString(
							"<script>\n"
									+ "function displayPayPalIntegrationNotCompleteMessage() {"
									+ " alert('"
									+ message
									+ "');\n"
									+ "}\n"
									+ "YAHOO.util.Event.on(window, \"load\", displayPayPalIntegrationNotCompleteMessage);\n"
									+ "</script>"));

			return result;
		}

		private Html displayValidButton(ExecutionContext context,
				BigDecimal amount) {
			ProjectInvoice invoice = new ProjectInvoice(
					context.getSubmission(), context.getUserProject(), amount,
					fieldToRecordStatus, fieldToRecordAmount);
			if (context.isPreviewMode()) {
				invoice.setId("123456");
			} else {
				ProjectsHibernateImpl.createProjectInvoice(invoice);
			}

			HtmlForm form = new HtmlForm();
			form.setAttribute("action", PayPalProperties.getSingleton()
					.getPaypalSiteURL()
					+ "/cgi-bin/webscr");
			form.setMethod("post");

			form.add(new HiddenInput("cmd", purchaseType.payPalCommand));
			form.add(new HiddenInput("business", context.getUserProject()
					.getUser().getPayPalAccountId()));
			form.add(new HiddenInput("item_name", itemDescriptionExpression
					.evaluate(context)));
			form.add(new HiddenInput("amount", amount.toPlainString()));
			form.add(new HiddenInput("currency_code", currencyCode));
			form.add(new HiddenInput("return", getURLForFormName(context,
					returnFormName)));
			form.add(new HiddenInput("cancel_return", getURLForFormName(
					context, cancelReturnFormName)));
			form.add(new HiddenInput("invoice", invoice.getId()));
			form.add(new HiddenInput("notify_url", PayPalProperties
					.getSingleton()
					.getInstantPaymentNotificationCallBackSiteURL()
					+ WellKnown.urls.getPayPalInstantPaymentNotification()));
			// --- 0 - ask, 1 - don't ask, 2 - must provide.
			form.add(new HiddenInput("no_shipping", "1"));
			if (useTawalaStyling) {
				addStyleControllingFields(context, form);
			}

			form
					.add(new ImageInput("submit", "submit", "0",
							purchaseType.image));

			return form;
		}

		private void addStyleControllingFields(ExecutionContext context,
				HtmlForm form) {
			ProjectTheme theme = context.getTheme();
			if (theme.getMainBackgroundColor() != null) {
				form.add(new HiddenInput("cpp_payflow_color", theme
						.getMainBackgroundColor()));
			}

			ImageInstance headerImageInstance = context.getProject()
					.getPageHeader() == null ? null : context.getProject()
					.getPageHeader().getImageInstance();
			if (headerImageInstance != null) {
				String imageURL = context.getUserProject().getImageUrl(true,
						context.getLink().getId(), headerImageInstance.getId());
				form.add(new HiddenInput("cpp_header_image", imageURL));
			}
		}

		private static String getURLForFormName(ExecutionContext context,
				String formName) {
			return context.getUserProject().getUrlToForm(context.getLink(),
					context.getEntryPointType(),
					context.getProject().getForm(formName));
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
