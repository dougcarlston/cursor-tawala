package com.tawala.project.formatting;

import java.net.MalformedURLException;
import java.net.URL;
import java.util.HashMap;
import java.util.Map;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.FormRenderableNotHoldingActiveComponents;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.StringConcatenationExpression;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.Image;
import com.tawala.web.oldhtml.NoHtml;

public class Link extends FormRenderableNotHoldingActiveComponents {
	private final StringConcatenationExpression description;
	private final StringConcatenationExpression url;
	private final boolean showInNewWindow;
	private final BooleanExpression displayCondition;
	private static Map<String, Image> fileExtensionToIconMap;
	private static Image DEFAULT_ICON;

	static {
		fileExtensionToIconMap = new HashMap<String, Image>();
		fileExtensionToIconMap.put("pdf", new Image(
				"/images/silk/page_white_acrobat.png", "PDF File", 16, 16));
		fileExtensionToIconMap
				.put("doc", new Image("/images/silk/page_white_word.png",
						"MS Word Document", 16, 16));
		fileExtensionToIconMap
				.put("rtf", new Image("/images/silk/page_white_word.png",
						"MS Word Document", 16, 16));
		fileExtensionToIconMap.put("xls", new Image(
				"/images/silk/page_white_excel.png", "MS Excel Spreadsheet",
				16, 16));
		fileExtensionToIconMap.put("zip", new Image(
				"/images/silk/page_white_compressed.png", "Archive File", 16,
				16));
		fileExtensionToIconMap.put("ppt", new Image(
				"/images/silk/page_white_powerpoint.png",
				"PowerPoint Presentation", 16, 16));

		DEFAULT_ICON = new Image("/images/silk/page_white.png",
				"File", 16, 16);
	}

	public Link(ConfigElement element) {
		this.description = new StringConcatenationExpression(element
				.child("description"));
		this.url = new StringConcatenationExpression(element.child("url"));
		this.showInNewWindow = element.hasChild("new-window");
		ConfigElement displayConditionElement = element
				.child("displayConditions");
		if (displayConditionElement != null) {
			this.displayCondition = BooleanExpression
					.load(displayConditionElement.childElement(0));
		} else {
			this.displayCondition = BooleanExpression.TRUE;
		}
	}

	private Image getIconForURL(String url) {
		int lastDotPosition = url.lastIndexOf('.');
		if (lastDotPosition < 0) {
			return DEFAULT_ICON;
		}
		String extension = url.substring(lastDotPosition + 1);
		extension = extension.toLowerCase();

		Image result = fileExtensionToIconMap.get(extension);
		return result == null ? DEFAULT_ICON : result;
	}

	public Html toHtml(ExecutionContext context) {
		if (!context.isPreviewMode() && !displayCondition.isTrue(context)) {
			return NoHtml.INSTANCE;
		}

		String URL = url.evaluate(context);
		String linkText = description.evaluate(context);

		boolean showIcons = false;
		if(linkText.length() == 0) {
			URLParser parser = new URLParser(URL);
			if(parser.looksLikeFile) {
				showIcons = parser.looksLikeFile;
				linkText = parser.fileName;
			} else {
				showIcons = parser.looksLikeFile;
				linkText = URL;
			}
		} else {
			showIcons = false;
		}
		
		if(context.isPreviewMode()) {
			if(linkText.length() == 0) {
				linkText = "Link";
			}
		}

		com.tawala.web.oldhtml.Link link = new com.tawala.web.oldhtml.Link(URL,
				linkText, showInNewWindow);

		if (showIcons) {
			HtmlItems result = new HtmlItems();
			result.add(getIconForURL(URL));
			result.add(link);
			return result;
		} else {
			return link;
		}
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public static class URLParser {
		public boolean looksLikeFile;
		public String fileName;

		public URLParser(String URLString) {
			try {
				if (URLString.contains("://")) {
					URL url = new URL(URLString);
					this.fileName = url.getFile();
				} else {
					this.fileName = URLString;
				}
				int positionOfLastSlash = fileName.lastIndexOf('/');
				if (positionOfLastSlash > -1) {
					fileName = fileName.substring(positionOfLastSlash + 1);
				}
				this.looksLikeFile = fileName.length() > 0;
			} catch (MalformedURLException e) {
				this.looksLikeFile = false;
				this.fileName = null;
			}
		}
	}
}
