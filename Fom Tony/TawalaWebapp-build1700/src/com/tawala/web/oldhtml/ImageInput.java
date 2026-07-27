package com.tawala.web.oldhtml;

import java.io.PrintWriter;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;

import org.springframework.web.util.HtmlUtils;

public class ImageInput implements Html {
	private final String name;
	private final String value;
	private final Image image;
	private final String border;
	private String onClickHandler;

	public String getOnClickHandler() {
		return onClickHandler;
	}

	public void setOnClickHandler(String onClickHandler) {
		this.onClickHandler = onClickHandler;
	}

	public ImageInput(String name, String value, String border, Image image) {
		this.name = name;
		this.value = value;
		this.image = image;
		this.border = border;
	}
	
	public void render(PrintWriter out, RenderingContext renderingContext) {
        try {
            out.print("<input type=\"image\" name=\"");
			out.print(URLEncoder.encode(name, "UTF-8"));
			out.print("\" value=\"");
	        out.print(URLEncoder.encode(value, "UTF-8"));
			out.print("\" border=\"");
	        out.print(URLEncoder.encode(border, "UTF-8"));
			out.print("\" src=\"");
	        out.print(renderingContext.renderUrl(image.getUrl()));
			out.print("\" alt=\"");
	        out.print(HtmlUtils.htmlEscape(image.getAltText()));
	        out.print("\"");
	        if(onClickHandler != null) {
	        	out.print(" onclick=\"");
	        	out.print(onClickHandler);
	        	out.print("\"");
	        }
	        out.print("/>\n");
		} catch (UnsupportedEncodingException e) {
			throw new Error("Unexpected exception: ", e);
		}
	}
}
