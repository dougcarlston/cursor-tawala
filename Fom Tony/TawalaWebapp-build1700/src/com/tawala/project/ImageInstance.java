package com.tawala.project;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.Image;

public class ImageInstance extends FormRenderableNotHoldingActiveComponents {
	public static final String DEFAULT_IMAGE_ALT_NAME = "Project Image";
	private final String id;
	private final String altName;
	private final int width;
	private final int height;

	public ImageInstance(ConfigElement config) {
		this.id = config.attribute("id").stringValue();
		this.altName = DEFAULT_IMAGE_ALT_NAME;
		this.width = config.attribute("width").intValue();
		this.height = config.attribute("height").intValue();
	}

	public Html toHtml(ExecutionContext executionContext) {
		return new Image(executionContext.getUserProject().getImageUrl(false,
				executionContext.getUserProject().getUniqueRandomId(), id),
				altName, width, height);
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public int getHeight() {
		return height;
	}

	public String getId() {
		return id;
	}
}
