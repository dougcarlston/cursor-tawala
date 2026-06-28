package com.tawala.project;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import com.scissor.Log;
import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.ConfigText;
import com.scissor.xmlconfig.Factory;
import com.tawala.component.web.ResponseCreator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.fib.AlignedLabelsLayout;
import com.tawala.project.fib.DefaultLayout;
import com.tawala.project.fib.FillInBlankLayout;
import com.tawala.project.fib.JustifiedInputsLayout;
import com.tawala.project.fib.VerticalLabelLayout;
import com.tawala.web.oldhtml.Div;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlString;

public class FillInBlank extends Question {
	private static final Map<String, FillInBlankLayout> LAYOUT_MANAGER_BY_STYLE_NAME_MAP = new HashMap<String, FillInBlankLayout>();
	static {
		LAYOUT_MANAGER_BY_STYLE_NAME_MAP.put("freeform", new DefaultLayout());
		LAYOUT_MANAGER_BY_STYLE_NAME_MAP
				.put("topLabels", new VerticalLabelLayout());
		LAYOUT_MANAGER_BY_STYLE_NAME_MAP.put("leftAlignLabels",
				AlignedLabelsLayout.LEFT);
		LAYOUT_MANAGER_BY_STYLE_NAME_MAP.put("rightAlignLabels",
				AlignedLabelsLayout.RIGHT);
		LAYOUT_MANAGER_BY_STYLE_NAME_MAP.put("leftAlignLabelsJustified",
				new JustifiedInputsLayout("left"));
		LAYOUT_MANAGER_BY_STYLE_NAME_MAP.put("rightAlignLabelsJustified",
				new JustifiedInputsLayout("right"));
	}

	private static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();

	static {
		FACTORY.registerText(FibText.class);
		FACTORY.register("paragraph", FormParagraph.class);
		FACTORY.register("blank", Blank.class);
		FACTORY.register("fileNameInput", Blank.class);
		FACTORY.setKeepWhitespace(true);
		FACTORY.ignore("displayConditions");
	}

	protected List<FormRenderable> contents = new ArrayList<FormRenderable>();

	private List<Blank> blanks = new ArrayList<Blank>();
	private FillInBlankLayout layoutManager;
	//--- Used for file uploads only.
	private boolean required = false;

	public FillInBlank(ConfigElement element) {
		super(element);
		contents.addAll(FACTORY.makeChildren(element));
		boolean fileUploadFIB = element.getName().equals("file");
		this.required = element.attribute("required").booleanValue();
		addBlanks(fileUploadFIB);

		String styleName = element.attribute("style").stringValue();
		if (styleName != null) {
			layoutManager = LAYOUT_MANAGER_BY_STYLE_NAME_MAP.get(styleName);
			if (layoutManager == null) {
				Log.error(this, "Unable to find renderer for style '"
						+ styleName + "'");
			}
		}
		if (layoutManager == null) {
			layoutManager = new DefaultLayout();
		}
	}

	private void addBlanks(boolean fileUploadFIB) {
		for (Object renderable : contents) {
			if (renderable instanceof Blank) {
				add((Blank) renderable, fileUploadFIB);
			} else if (renderable instanceof FormParagraph) {
				for (Object nextElement : ((FormParagraph) renderable)
						.getContents()) {
					if (nextElement instanceof Blank) {
						add((Blank) nextElement, fileUploadFIB);
					}
				}
			}
		}
	}

	private void add(Blank blank, boolean fileUploadFIB) {
		blank.setHtmlContainer(this);
		
		if(fileUploadFIB) {
			blank.setParentPrefix(getHtmlId());
			blank.setRequired(required);
		} else {
			blank.setParentPrefix(this.htmlPrefix());			
		}
		blanks.add(blank);
	}

	// TODO: do we need to create another array?
	public List<Field> fields() {
		return new ArrayList<Field>(getBlanks());
	}

	public Html produceHtml(ExecutionContext context) {
		context.setCurrentFillInBlankLayoutManager(layoutManager);
		try {
			Html result = layoutManager.render(this, context);
			return result;
		} finally {
			context.setCurrentFillInBlankLayoutManager(null);
		}
	}

	public List<Blank> getBlanks() {
		return blanks;
	}

	public List<FormRenderable> getContents() {
		return contents;
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public static class FibText extends FormRenderableNotHoldingActiveComponents {
		private final String contents;

		public FibText(ConfigText configText) {
			this.contents = configText.text();
		}

		public Html toHtml(ExecutionContext context) {
			Div qDiv = new Div("class", "question");
			qDiv.add(new HtmlString(contents));
			return qDiv;
		}

		public String getContents() {
			return contents;
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}
	}

	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		return null;
	}
}
