package com.tawala.web.oldhtml;

import java.io.PrintWriter;

import org.springframework.web.util.HtmlUtils;

public class Image implements Html {
    private String url;
    private int width;
    private int height;
    private String altText;

    public Image(String url, String altText, int width, int height) {
        this.url = url;
        this.altText = altText;
        this.width = width;
        this.height = height;
    }
    
    public Image(String url, String altText) {
    	this(url, altText, -1, -1);
	}

	public void render(PrintWriter out, RenderingContext renderingContext) {
        out.print("<img src=\"");
        out.print(renderingContext.renderUrl(url));
        out.print("\" alt=\"");
        out.print(HtmlUtils.htmlEscape(altText));
        out.print('"');
        if(width > 0) {
        	out.print(" width=\"");
        	out.print(width + "px");
        	out.print('"');
        }
        if(height > 0) {
        	out.print(" height=\"");
        	out.print(height + "px");
        	out.print('"');
        }
        out.print(" />");
    }

	public String getAltText() {
		return altText;
	}

	public int getHeight() {
		return height;
	}

	public String getUrl() {
		return url;
	}

	public int getWidth() {
		return width;
	}

	public void setWidth(int width) {
		this.width = width;
	}

	public void setHeight(int height) {
		this.height = height;
	}
}
