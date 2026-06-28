package com.tawala.web.admin;

import javax.servlet.ServletContext;

public class UrgentMessage implements Cloneable {
	private static final String URGENT_MESSAGE_ATTRIBUTE = "urgentMessage";
	private String text;
	private String userId;

	public void setText(String text) {
		this.text = text;
	}

	public String getText() {
		return text;
	}

	public static UrgentMessage get(ServletContext context) {
		return (UrgentMessage) context.getAttribute(URGENT_MESSAGE_ATTRIBUTE);
	}
	
	public static void set(ServletContext context, UrgentMessage message) {
		context.setAttribute(URGENT_MESSAGE_ATTRIBUTE, message);
	}

	public String getUserId() {
		return userId;
	}

	public void setUserId(String userId) {
		this.userId = userId;
	}
	
	@Override
	public Object clone() throws CloneNotSupportedException {
		return super.clone();
	}

	public static void remove(ServletContext context) {
		context.removeAttribute(URGENT_MESSAGE_ATTRIBUTE);
	}
}
