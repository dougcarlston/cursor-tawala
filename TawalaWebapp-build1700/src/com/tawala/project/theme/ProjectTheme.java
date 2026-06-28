package com.tawala.project.theme;

import java.util.List;

import javax.servlet.http.HttpServletRequest;

import com.tawala.web.oldhtml.Image;

public interface ProjectTheme {
	List<String> getScreenStylesheetURLs();
	List<String> getPrintStylesheetURLs();
	Image getHeaderImageHTML();
	String getCSSContents(HttpServletRequest request);
	String getGoogleAdSenseCode();
	String getMainBackgroundColor();
	String getThemeId();
}
