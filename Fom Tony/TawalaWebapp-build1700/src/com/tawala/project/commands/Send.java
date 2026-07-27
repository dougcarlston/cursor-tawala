package com.tawala.project.commands;

import java.io.IOException;
import java.io.PrintWriter;
import java.io.StringWriter;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.List;

import javax.mail.Address;
import javax.mail.internet.AddressException;
import javax.mail.internet.InternetAddress;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;
import com.tawala.domain.EmailAddress;
import com.tawala.email.Email;
import com.tawala.email.EmailService;
import com.tawala.email.UniqueBodyEmail;
import com.tawala.email.UserProjectEmail;
import com.tawala.email.UniqueBodyEmail.Type;
import com.tawala.project.Document;
import com.tawala.project.FieldReference;
import com.tawala.project.Text;
import com.tawala.project.TextRenderable;
import com.tawala.project.UserProject;
import com.tawala.project.VirtualDocument;
import com.tawala.web.oldhtml.EmbeddedStyle;
import com.tawala.web.oldhtml.OldPage;
import com.tawala.web.oldhtml.RenderingContext;

public class Send extends ProcessCommand {
	public static final String DEFAULT_FROM_ADDRESS = "message@tawala.com";
	private static final Factory<Recipient> RECIPIENT_FACTORY = new Factory<Recipient>();
	private static final Factory<SendBody> BODY_FACTORY = new Factory<SendBody>();
	private static final Factory<TextRenderable> SUBJECT_FACTORY = new Factory<TextRenderable>();

	static {
		RECIPIENT_FACTORY.register("to", "addressLiteral",
				LiteralRecipient.class);
		RECIPIENT_FACTORY.register("cc", "addressLiteral",
				LiteralRecipient.class);
		RECIPIENT_FACTORY
				.register("to", "addressField", DynamicRecipient.class);
		RECIPIENT_FACTORY
				.register("cc", "addressField", DynamicRecipient.class);
		RECIPIENT_FACTORY.ignore("subject");
		RECIPIENT_FACTORY.ignore("from");

		BODY_FACTORY.register("body", "document", DocumentSendBody.class);

		BODY_FACTORY.register("body", StaticTextSendBody.class);

		RECIPIENT_FACTORY.ignore(BODY_FACTORY);

		SUBJECT_FACTORY.registerText(Text.class);
		SUBJECT_FACTORY.register("field", FieldReference.class);
	}

	private final List<Recipient> recipients = new ArrayList<Recipient>();

	private final List<TextRenderable> subjectChunks;

	private final SenderAddressProvider senderAddressProvider;

	private SendBody body;

	public Send(ConfigElement config) {
		super();

		this.recipients.addAll(RECIPIENT_FACTORY.makeChildren(config));
		body = BODY_FACTORY.make(config.child("body"));
		ConfigElement subjectConfigElement = config.child("subject");
		this.subjectChunks = SUBJECT_FACTORY.makeChildren(subjectConfigElement);

		senderAddressProvider = new SenderAddressProvider(config.child("from"));
	}

	public ExecutionResult execute(ExecutionContext context) {
		List<Address> toAddresses = new ArrayList<Address>();
		List<Address> ccAddresses = new ArrayList<Address>();

		List<String> errorMessages = new ArrayList<String>();
		
		for (Recipient recipient : recipients) {
			EmailAddress address = null;
			address = recipient.getAddress(context);
			if (recipient.type == RecipientType.TO) {
				try {
					if(address.isEmpty()) {
						Log.warn(this, "Empty TO address");
					} else {
						toAddresses.add(address.asInternetAddress());
					}
				} catch (Exception e) {
					errorMessages.add("Invalid TO address: " + address);
				}
			} else {
				try {
					if(address.isEmpty()) {
						Log.warn(this, "Empty CC address");
					} else {
						ccAddresses.add(address.asInternetAddress());
					}
				} catch (Exception e) {
					errorMessages.add("Invalid CC address: " + address);
				}
			}
		}

		String toAddressString = convertListOfAddressesToString(toAddresses);
		String ccAddressString = convertListOfAddressesToString(ccAddresses);

		EmailAddress emailAddress = senderAddressProvider.getEmail(context);
		String fromAddress = null;
		try {
			fromAddress = emailAddress.asInternetAddress().toString();
		} catch (Exception e) {
			errorMessages.add("Wrong address: " + fromAddress);
		}

		Email email = null;
		String bodyContent = null;
		context.setGeneratingEmailContent(true);
		try {
			bodyContent = body.render(context);
		} finally {
			context.setGeneratingEmailContent(false);
		}
		try {
			email = new UserProjectEmail(context.getUserProject(), fromAddress,
					toAddressString, ccAddressString, buildSubject(context),
					body.getType(), bodyContent);
		} catch (IOException e) {
			Log.error(this, "Error creating email: " + e.getMessage(), e);
			return ExecutionResult.NULL;
		}
		if (errorMessages.size() > 0) {
			StringBuilder errorReason = new StringBuilder(errorMessages.get(0));
			for (int i = 1; i < errorMessages.size(); i++) {
				errorReason.append("\n").append(errorMessages.get(i));
			}
			if(errorReason.length() > Email.MAX_ERROR_LENGTH) {
				errorReason.setLength(Email.MAX_ERROR_LENGTH);
			}
			email.setCustomerErrorReason(errorReason.toString());
			email.setErrorReason(email.getCustomerErrorReason());
			
			EmailService.storeAsFailed(email);
		} else {
			EmailService.enqueueForDelivery(email);
		}


		return ExecutionResult.NULL;
	}

	private static String convertListOfAddressesToString(List<Address> addresses) {
		switch (addresses.size()) {
		case 0:
			return null;

		case 1:
			return addresses.get(0).toString();

		default:
			return InternetAddress.toString((Address[]) addresses
					.toArray(new Address[addresses.size()]));
		}
	}

	private String buildSubject(ExecutionContext context) {
		StringBuilder result = new StringBuilder();
		for (TextRenderable chunk : subjectChunks) {
			chunk.appendTo(result, context);
		}
		return result.toString();
	}

	public List<TextRenderable> getSubject() {
		return subjectChunks;
	}

	public abstract static class SendBody {

		public abstract String render(ExecutionContext context);

		public abstract UniqueBodyEmail.Type getType();
	}

	public static class StaticTextSendBody extends SendBody {
		private String text;

		public StaticTextSendBody(ConfigElement config) {
			this.text = config.text();
		}

		@Override
		public String render(ExecutionContext context) {
			return text;
		}

		@Override
		public Type getType() {
			return UniqueBodyEmail.Type.TEXT;
		}
	}

	public static class DocumentSendBody extends SendBody {
		private final String docId;

		private final boolean resetAfterShowing;
		private final boolean showHeader;

		public DocumentSendBody(ConfigElement config) {
			this.docId = config.attribute("document").stringValue();
			this.resetAfterShowing = config.attribute("reset").booleanValue();
			this.showHeader = config.hasAttribute("showHeader") ? config
					.attribute("showHeader").booleanValue() : true;
		}

		@Override
		public String render(ExecutionContext context) {
			String styleSheet = context.getProject().getTheme().getCSSContents(
					context.getRequest().getHttpRequest());
			EmbeddedStyle style = new EmbeddedStyle(styleSheet);

			OldPage page = new OldPage(style);
			if (context.getProject().getPageHeader() != null && showHeader) {
				page.add(context.getProject().getPageHeader().toHtml(context));
			}
			Document document = context.getDocument(docId);
			page.add(document.toHtml(context));

			if (resetAfterShowing && document instanceof VirtualDocument) {
				((VirtualDocument) document).reset();
			}

			RenderingContext renderingContext = new RenderingContext();
			renderingContext.setEmailDestination(true);
			renderingContext.setRelativeUrlPrefix(context.getUserProject()
					.getProjectUrlPrefix(
							context.getUserProject().getUniqueRandomId()));
			renderingContext.setAbsoluteUrlPrefix("http" + "://"
					+ UserProject.getWebsiteHostName());

			StringWriter out = new StringWriter();
			page.render(new PrintWriter(out), renderingContext);
			return out.toString();
		}

		@Override
		public Type getType() {
			return UniqueBodyEmail.Type.HTML;
		}

	}

	private static enum RecipientType {
		TO, CC;
	}

	public abstract static class Recipient {
		private final RecipientType type;

		protected Recipient(ConfigElement config) {
			this.type = typeFor(config);
		}

		protected abstract EmailAddress getAddress(ExecutionContext context);

		private static RecipientType typeFor(ConfigElement config) {
			return RecipientType.valueOf(config.getName().toUpperCase());
		}
	}

	public static class LiteralRecipient extends Recipient {
		private final String address;

		public LiteralRecipient(ConfigElement config) {
			super(config);
			this.address = config.attribute("addressLiteral").stringValue();
		}

		@Override
		protected EmailAddress getAddress(ExecutionContext context) {
			return new EmailAddress(address);
		}

	}

	public static class DynamicRecipient extends Recipient {
		private final String fieldName;

		public DynamicRecipient(ConfigElement config) {
			super(config);
			this.fieldName = config.attribute("addressField").stringValue();
		}

		@Override
		protected EmailAddress getAddress(ExecutionContext context) {
			return new EmailAddress(context.getValue(fieldName).toString());
		}

	}

	public static class SenderAddressProvider {
		private final String aliasLiteral;
		private final String aliasFieldReference;
		private final EmailAddressProvider emailAddressProvider;

		public SenderAddressProvider(ConfigElement element) {
			if (element == null) {
				aliasLiteral = null;
				aliasFieldReference = null;
				emailAddressProvider = new ProjectOwnerEmailAddressProvider();
			} else {
				aliasLiteral = element.attribute("aliasLiteral").stringValue();
				aliasFieldReference = element.attribute("aliasField")
						.stringValue();
				if (element.hasAttribute("addressField")) {
					emailAddressProvider = new FieldReferenceEmailAddressProvider(
							element.attribute("addressField").stringValue());
				} else if (element.hasAttribute("addressLiteral")) {
					emailAddressProvider = new LiteralEmailAddressProvider(
							element.attribute("addressLiteral").stringValue());
				} else {
					emailAddressProvider = new ProjectOwnerEmailAddressProvider();
				}
			}
		}

		public EmailAddress getEmail(ExecutionContext context) {
			EmailAddress result = emailAddressProvider.getAddress(context);
			if (result == null) {
				result = new EmailAddress(DEFAULT_FROM_ADDRESS);
			}

			if (aliasLiteral != null) {
				result.setAlias(aliasLiteral);
			} else if (aliasFieldReference != null) {
				String alias = context.getValue(aliasFieldReference).toString();
				if (alias.length() > 0) {
					result.setAlias(alias);
				}
			}

			return result;
		}

		interface EmailAddressProvider {
			EmailAddress getAddress(ExecutionContext executionContext);
		}

		private class ProjectOwnerEmailAddressProvider implements
				EmailAddressProvider {

			public EmailAddress getAddress(ExecutionContext context) {
				return context.getProjectOwner().getEmail();
			}
		}

		private class LiteralEmailAddressProvider implements
				EmailAddressProvider {
			private final String literalAddress;

			public LiteralEmailAddressProvider(String literalAddress) {
				this.literalAddress = literalAddress;
			}

			public EmailAddress getAddress(ExecutionContext context) {
				return new EmailAddress(literalAddress);
			}
		}

		private class FieldReferenceEmailAddressProvider implements
				EmailAddressProvider {
			private final String fieldReference;

			public FieldReferenceEmailAddressProvider(String fieldReference) {
				this.fieldReference = fieldReference;
			}

			public EmailAddress getAddress(ExecutionContext context) {
				String address = context.getValue(fieldReference).toString();
				if (address.length() == 0)
					return null;

				EmailAddress result = new EmailAddress(address);
				try {
					result.asInternetAddress();
					return result;
				} catch (AddressException e) {
					return null;
				} catch (UnsupportedEncodingException e) {
					// Will never happen
					Log.error(this, "Unexpected exception:", e);
					return null;
				}
			}
		}
	}
}
