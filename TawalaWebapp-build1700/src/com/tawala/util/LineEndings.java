package com.tawala.util;

import java.util.regex.Pattern;

/**
 * Carriage returns in field text. XStream {@code PrettyPrintWriter} writes a
 * raw {@code \r} as the six characters {@code &#x0D;} in submission CLOB XML.
 * Spring {@code HtmlUtils.htmlEscape} does not invent that entity; it only
 * makes an already-stored {@code &} visible. Convert CR (and those entities)
 * to {@code \n} before persist and before textarea escape.
 */
public final class LineEndings {
	private static final Pattern CR_ENTITY_OR_RETURN = Pattern
			.compile("(?i)(?:&#x0*d;|&#13;|\r)\n?");

	private LineEndings() {
	}

	public static String toUnixNewlines(String raw) {
		if (raw == null) {
			return "";
		}
		return CR_ENTITY_OR_RETURN.matcher(raw).replaceAll("\n");
	}
}
