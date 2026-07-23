package com.tawala.project.theme;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.Comparator;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.RequestDispatcher;
import javax.servlet.http.HttpServletRequest;

import org.json.JSONObject;
import org.springframework.mock.web.MockHttpServletResponse;

import com.scissor.Log;
import com.tawala.web.oldhtml.Image;

public class CommonTheme implements ProjectTheme {
	public static class Initializer {
		public void setThemes(List<CommonTheme> themes) {
			List<CommonTheme> visibleTheme = new ArrayList<CommonTheme>();
			List<CommonTheme> invisibleThemes = new ArrayList<CommonTheme>();
			for (CommonTheme theme : themes) {
				if(theme.isVisible()) {
					visibleTheme.add(theme);
				} else {
					invisibleThemes.add(theme);
				}
			}
			
			ALL_THEMES = visibleTheme.toArray(new CommonTheme[0]);
			INVISIBLE_THEMES = invisibleThemes.toArray(new CommonTheme[0]);
			sortVisibleThemes();
			
			CommonTheme.DEFAULT_THEME = getThemeByPath("default");
		}
	}

	// --- See WEB-INF/theme-config.xml for theme details.
	public static CommonTheme[] ALL_THEMES = new CommonTheme[] {new CommonTheme("default", "Default")};
	public static CommonTheme[] INVISIBLE_THEMES = new CommonTheme[0];
	public static CommonTheme DEFAULT_THEME;

	private static final Map<String, String> cachedCSS = new HashMap<String, String>();
	private static String cachedCSSAttributes = null;

	public static final String FORM_LAYOUT_CORE_CSS = "/css/project/form-layout-core.css";

	private String path;
	private String name;
	private Image headerImage;
	private String googleAdSenseCode;
	private String mainBackgroundColor;
	private List<String> screenStylesheetURLs;
	private List<String> printStylesheetURLs;
	private boolean visible = true;

	public CommonTheme(String path, String name) {
		this.path = path;
		this.name = name;
		screenStylesheetURLs = new ArrayList<String>(3);
		screenStylesheetURLs.add("/css/project/default.css");
		screenStylesheetURLs.add("/css/project/" + path + "/project.css");
		/* Layout contracts always last — themes must not override FIB geometry. */
		screenStylesheetURLs.add(FORM_LAYOUT_CORE_CSS);
		screenStylesheetURLs = Collections
				.unmodifiableList(screenStylesheetURLs);

		printStylesheetURLs = new ArrayList<String>(1);
		printStylesheetURLs.add("/css/project/print.css");

	}

	public CommonTheme(String path, String name, Image headerImage) {
		this(path, name);
		this.headerImage = headerImage;
	}

	public String getName() {
		return name;
	}

	public String getPath() {
		return path;
	}

	public String getGoogleAdSenseCode() {
		return googleAdSenseCode;
	}

	public void setGoogleAdSenseCode(String googleAdSenseCode) {
		this.googleAdSenseCode = googleAdSenseCode;
	}

	private static void sortVisibleThemes() {
		Arrays.sort(ALL_THEMES, new Comparator<CommonTheme>() {
			public int compare(CommonTheme o1, CommonTheme o2) {
				return o1.getName().compareTo(o2.getName());
			}
		});
	}

	public static CommonTheme getThemeByPath(String path) {
		CommonTheme result = findThemeInArray(path, ALL_THEMES);
		if(result == null) {
			result = findThemeInArray(path, INVISIBLE_THEMES);
		}

		return result;
	}

	private static CommonTheme findThemeInArray(String path, CommonTheme[] themes) {
		for (CommonTheme theme : themes) {
			if (theme.getPath().equals(path)) {
				return theme;
			}
		}
		
		return null;
	}

	public static String getThemeCSSAttributesAsJSONString(HttpServletRequest request) {
		if(cachedCSSAttributes == null) {
			JSONObject result = new JSONObject();
			for (CommonTheme theme : ALL_THEMES) {
				try {
					Log.info(CommonTheme.class, "Parsing CSS for theme \"" + theme.getPath() + "\"...");
					result.put(theme.getPath(), CSSDocumentHandler.parseCSSAndReturnJSONObject(theme.getCSSContents(request)));
				} catch (Exception e) {
					Log.error(CommonTheme.class, "Unable to parse CSS for theme \"" + theme.getPath() + "\": " + e.getMessage());
				}
			}
			cachedCSSAttributes = result.toString();
		}
		
		return cachedCSSAttributes;
	}
	
	public Image getHeaderImageHTML() {
		return headerImage;
	}

	public String getMainBackgroundColor() {
		return mainBackgroundColor;
	}

	public void setMainBackgroundColor(String mainBackgroundColor) {
		this.mainBackgroundColor = mainBackgroundColor;
	}

	public List<String> getScreenStylesheetURLs() {
		return screenStylesheetURLs;
	}

	public List<String> getPrintStylesheetURLs() {
		return Collections.singletonList("/css/project/print.css");
	}

	public String getCSSContents(HttpServletRequest request) {
		String defaultThemeCSS = retrieveCSS("/css/project/default.css",
				request);
		String themeSpecificCSS = retrieveCSS("/css/project/" + path
				+ "/project.css", request);
		String layoutCoreCSS = retrieveCSS(FORM_LAYOUT_CORE_CSS, request);

		return defaultThemeCSS + "\n" + themeSpecificCSS + "\n" + layoutCoreCSS;
	}

	private String retrieveCSS(String cssPath, HttpServletRequest request) {
		String styleSheet = cachedCSS.get(cssPath);
		if (styleSheet != null)
			return styleSheet;

		RequestDispatcher requestDispatcher = request
				.getRequestDispatcher(cssPath);
		MockHttpServletResponse mockResponse = new MockHttpServletResponse();

		try {
			requestDispatcher.include(request, mockResponse);
			styleSheet = mockResponse.getContentAsString();
		} catch (Exception e) {
			throw new IllegalStateException(
					"Unable to get the style sheet for " + cssPath + ".", e);
		}

		cachedCSS.put(cssPath, styleSheet);

		return styleSheet;
	}

	public String getThemeId() {
		return path;
	}

	public boolean isVisible() {
		return visible;
	}

	public void setVisible(boolean visible) {
		this.visible = visible;
	}

}
