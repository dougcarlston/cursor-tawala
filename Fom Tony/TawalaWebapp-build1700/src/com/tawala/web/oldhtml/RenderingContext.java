package com.tawala.web.oldhtml;

public class RenderingContext {
	private String relativeUrlPrefix;
	private String absoluteUrlPrefix;
	private boolean emailDestination;

	public void setRelativeUrlPrefix(String urlPrefix) {
		this.relativeUrlPrefix = urlPrefix;
	}

	public String renderUrl(String url) {
		if (relativeUrlPrefix == null) {
			return url;
		}

		if (url.contains("://")) {
			return url;
		} else {
			if (url.startsWith("/")) {
				return absoluteUrlPrefix + url;
			} else {
				return relativeUrlPrefix + url;
			}
		}
	}

	public void setAbsoluteUrlPrefix(String absoluteUrlPrefix) {
		this.absoluteUrlPrefix = absoluteUrlPrefix;
	}

	public boolean isEmailDestination() {
		return emailDestination;
	}

	public void setEmailDestination(boolean emailDestination) {
		this.emailDestination = emailDestination;
	}
}
