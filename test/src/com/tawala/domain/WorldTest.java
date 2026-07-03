package com.tawala.domain;

import com.tawala.TestCase;
import com.tawala.web.WorldInitializer;

public class WorldTest extends TestCase {

	public void testExtractingBuildNumber() {
		String buildInfoContents =
		"            Built at 2008/11/06 14:15:04\n"
				+ "            Built by sergei\n"
				+ "            Built on M90 in C:\\Common\\Eclipse Workspace\\Tawala\n"
				+ "                Build label: 123\n"
				+ "            Built with:\n"
				+ "            Sun Microsystems Inc. Java 1.6.0_01-b06\n"
				+ "            Apache Ant version 1.6.5 compiled on June 2 2005\n"
				+ "            Windows XP x86 5.1\n"
				+ "            build file C:\\Common\\Eclipse Workspace\\Tawala\\build.xml";

		String buildNumber = WorldInitializer
				.extractBuildNumber(buildInfoContents);

		assertEquals("123", buildNumber);
	}
}
