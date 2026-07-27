package com.tawala.email;

abstract public class MailToURLBuilder {
	protected abstract String getSubject();

	protected abstract String getBody();

	public String getMailToURL() {
		String subject = getSubject();
		return "mailto:?"
				+ (subject == null ? "" : "subject="
						+ escapeAccordingToRFC2368(subject) + "&") + "body="
				+ escapeAccordingToRFC2368(getBody());
	}

	protected static String escapeAccordingToRFC2368(String string) {
		StringBuilder result = new StringBuilder();

		for (int i = 0; i < string.length(); i++) {
			char next = string.charAt(i);
			switch (next) {
			case ' ':
				result.append("%20");
				break;

			case '&':
				result.append("%26");
				break;

			case '"':
				result.append("%22");
				break;

			case '=':
				result.append("%3D");
				break;

			case '?':
				result.append("%3F");
				break;

			case '\n':
				result.append("%0D%0A");
				break;

			case '\r':
				// --- Do nothing
				break;

			default:
				result.append(next);
				break;
			}
		}
		return result.toString();
	}

}
