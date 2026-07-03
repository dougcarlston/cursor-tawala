package com.tawala.svn;

import java.io.BufferedReader;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLConnection;
import java.util.LinkedHashSet;
import java.util.Set;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.xml.parsers.SAXParser;
import javax.xml.parsers.SAXParserFactory;

import org.xml.sax.Attributes;
import org.xml.sax.SAXException;
import org.xml.sax.helpers.DefaultHandler;

public class CreateLogRecordList {
	private static final String buildUrl = "http://build.tawala.com/buildinfo.txt";
	private static final String devUrl = "http://dev.tawala.com/buildinfo.txt";
	private static final String prodUrl = "http://www.tawala.com/buildinfo.txt";
	private static final String TAGGING_MESSAGE_PREFIX = "Tagging the build with label build-";

	public static void main(String[] args) throws IOException {
		String fileName = null;
		String previousBuildSiteURL = prodUrl;
		String currentBuildSiteURL = devUrl;
		String previousBuildLabel = null;
		String currentBuildLabel = null;

		for (int i = 0; i < args.length; i++) {
			String argument = args[i];
			if (argument.equals("-dev")) {
				previousBuildSiteURL = devUrl;
				currentBuildSiteURL = buildUrl;
			} else if (argument.equals("-f")) {
				fileName = args[++i];
			} else if (argument.equals("-start")) {
				previousBuildLabel = args[++i];
			} else if (argument.equals("-end")) {
				currentBuildLabel = args[++i];
			}
		}

		if (previousBuildLabel == null) {
			previousBuildLabel = getLabelFromBuildInfo(previousBuildSiteURL);
		}
		if (currentBuildLabel == null) {
			currentBuildLabel = getLabelFromBuildInfo(currentBuildSiteURL);
		}

		if (previousBuildLabel.equals(currentBuildLabel)) {
			System.out.println("Both builds have the same label: "
					+ previousBuildLabel);
			return;
		}

		System.out.println("Notes for the builds " + previousBuildLabel
				+ " to " + currentBuildLabel + ":");

		parseFile(fileName, previousBuildLabel, currentBuildLabel);
	}

	private static String getLabelFromBuildInfo(String url)
			throws MalformedURLException, IOException {
		URL nextLevel = new URL(url);
		URLConnection connection = nextLevel.openConnection();
		connection.connect();
		InputStream stream = connection.getInputStream();
		BufferedReader reader = new BufferedReader(
				new InputStreamReader(stream));
		String line = null;

		Pattern pattern = Pattern.compile(".*Build label: build-(.*)");
		while ((line = reader.readLine()) != null) {
			Matcher matcher = pattern.matcher(line);
			if (matcher.matches()) {
				String label = matcher.group(1);
				return label;
			}
		}

		throw new IllegalStateException(
				"Unable to find build label in the build info.");
	}

	private static void parseFile(String fileName, String previousBuildLabel,
			String currentBuildLabel) {
		SAXParserFactory factory = SAXParserFactory.newInstance();
		try {
			SVNLogEventHandler handler = new SVNLogEventHandler(
					currentBuildLabel, previousBuildLabel);
			// Parse the input
			SAXParser saxParser = factory.newSAXParser();
			saxParser.parse(new File(fileName), handler);

			for (String message : handler.getNotes()) {
				System.out.println(message);
			}
		} catch (Throwable t) {
			t.printStackTrace();
		}
	}

	public static class SVNLogEventHandler extends DefaultHandler {
		private String startMessage;
		private String endMessage;
		private String excludeMessagesStartingWith = TAGGING_MESSAGE_PREFIX;
		private Set<String> notes = new LinkedHashSet<String>();
		private boolean captureNotes = false;
		private boolean sectionStarted = false;
		private boolean stopProcessing = false;
		private String currentAuthor;
		private boolean captureAuthor = false;

		public SVNLogEventHandler(String startLabel, String endLabel) {
			this.startMessage = TAGGING_MESSAGE_PREFIX + startLabel;
			this.endMessage = TAGGING_MESSAGE_PREFIX + endLabel;
			this.excludeMessagesStartingWith = TAGGING_MESSAGE_PREFIX;
		}

		public Set<String> getNotes() {
			return notes;
		}

		@Override
		public void startElement(String uri, String localName, String qName,
				Attributes attributes) throws SAXException {
			if (stopProcessing) {
				return;
			}

			if (qName.equals("author")) {
				captureNotes = true;
				captureAuthor = true;
			} else if (qName.equals("msg")) {
				captureNotes = true;
				captureAuthor = false;
			}

		}

		@Override
		public void characters(char[] ch, int start, int length)
				throws SAXException {
			if (captureNotes) {
				captureNotes = false;

				String text = new String(ch, start, length);
				if (sectionStarted) {
					if (text.equals(endMessage)) {
						stopProcessing = true;
					} else if (text.startsWith(excludeMessagesStartingWith)) {
						// -- Do nothing
					} else if (captureAuthor) {
						currentAuthor = text;
					} else if (text.length() > 0) {
						notes.add(currentAuthor + ": " + text);
					}
				} else if (text.equals(startMessage)) {
					sectionStarted = true;
				}
			}
		}
	}
}
