package com.tawala.project;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.ConfigItem;
import com.scissor.xmlconfig.Factory;
import com.tawala.component.function.FunctionToWebAdapter;
import com.tawala.component.repository.Repository;
import com.tawala.component.web.ResponseCreator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.formatting.AlignableContainerElement;
import com.tawala.project.formatting.Bold;
import com.tawala.project.formatting.ContainerElement;
import com.tawala.project.formatting.DummyTab;
import com.tawala.project.formatting.Font;
import com.tawala.project.formatting.Italics;
import com.tawala.project.formatting.Space;
import com.tawala.project.formatting.Underline;
import com.tawala.web.oldhtml.Block;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;
import com.tawala.web.oldhtml.HtmlReadyString;

public class Paragraph implements FormRenderable {

	private static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();
	static {
		FACTORY.register("field", FieldReference.class);
		FACTORY.ignore("tabPositions");
		FACTORY.register("tab", DummyTab.class);
		FACTORY.registerText(Text.class);
		FACTORY.setKeepWhitespace(true);

		FACTORY.register("font", Font.class);
		FACTORY.register("b", Bold.class);
		FACTORY.register("i", Italics.class);
		FACTORY.register("u", Underline.class);
		FACTORY.register("image", ImageInstance.class);
		FACTORY.register("blank", Blank.class);
		FACTORY.register("fileNameInput", Blank.class);
		FACTORY.register("sp", Space.class);
		Repository.registerWebComponentsWith(FACTORY);
		FunctionToWebAdapter.registerFunctionsWith(FACTORY);
	}

	private final ParagraphImplementation implementation;

	public Paragraph(ConfigElement config) {
		int tabCount = 0;
		for (ConfigElement child : config.childElements()) {
			if (child.getName().equals("tab")) {
				++tabCount;
			}
		}

		implementation = tabCount > 0 && config.child("tabPositions") != null ?
		// We should try to format using more sophisticated layout
		new TabbedParagraphImplementation(config)
				:
				// Ignore the tabs and output plain text
				new SimpleParagraphImplementation(config);
	}

	public boolean isEmpty(ExecutionContext context) {
		return implementation.isEmpty(context);
	}

	public Html toHtml(ExecutionContext context) {
		return implementation.toHtml(context);
	}

	public Html toHtml(ExecutionContext context, FormRenderable extraText) {
		return implementation.toHtml(context, extraText);
	}

	public List<FormRenderable> getContents() {
		return implementation.getContents();
	}

	private static class ParagraphHtml extends Block {
		public ParagraphHtml(String style) {
			super("div", false);
			if(style != null) {
				setAttribute("style", style);
			}
		}
	}
	
	interface ParagraphImplementation extends FormRenderable {
		Html toHtml(ExecutionContext context, FormRenderable extraText);

		List<FormRenderable> getContents();
	}

	private static class SimpleParagraphImplementation extends AlignableContainerElement
			implements ParagraphImplementation {
		public SimpleParagraphImplementation(ConfigElement config) {
			super(config);
		}

		public Html toHtml(ExecutionContext context, FormRenderable extraText) {
			ParagraphHtml paragraph = new ParagraphHtml(getStyle());
			if (getContents().size() == 0) {
				paragraph.add(new HtmlReadyString("&nbsp;"));
			} else {
				paragraph.appendContents(getContents(), context);
			}
			if(extraText != null) {
				paragraph.add(extraText.toHtml(context));
			}
			return paragraph;
		}

		public Html toHtml(ExecutionContext context) {
			return toHtml(context, null);
		}

		@Override
		protected Factory<FormRenderable> getFactory() {
			return FACTORY;
		}
	}

	private static class TabbedParagraphImplementation implements
			ParagraphImplementation {
		public static TabInfo DEFAULT_TAB_INFO = new TabInfo(200 * 20);
		public static class TabInfo {
			final int width;
			final String style;
			
			public TabInfo() {
				width = 0;
				style = null;
			}

			TabInfo(int width) {
				this.width = width;
				this.style = width > 0 ? "width: " + (width / 20) + "px" : null;
			}
		}

		private LinkedHashMap<TabInfo, ContainerElement> contents = new LinkedHashMap<TabInfo, ContainerElement>();

		public TabbedParagraphImplementation(ConfigElement config) {
			List<TabInfo> tabInfos = new ArrayList<TabInfo>();
			List<ConfigElement> tabPositions = config.child("tabPositions")
					.children("tabStop");
			if (tabPositions.size() == 0) {
				throw new IllegalArgumentException(
						"Unable to find tabPositions/tabStop elements");
			}

			int previousPosition = 0;
			for (ConfigElement tabStopElement : tabPositions) {
				int position = tabStopElement.attribute("position").intValue();
				int width = position - previousPosition;
				tabInfos.add(new TabInfo(width));

				previousPosition = position;
			}

			int currentTab = 0;
			ContainerElement tabContainer = new ContainerElement();
			for (ConfigItem configItem : config.children()) {
				if (configItem.getName().equals("tab")) {
					if(tabInfos.size() <= currentTab) {
						tabInfos.add(DEFAULT_TAB_INFO);
					}
					contents.put(tabInfos.get(currentTab++), tabContainer);
					tabContainer = new ContainerElement();
				} else {
					FormRenderable nextObject = FACTORY.make(configItem);
					if (nextObject != null) {
						tabContainer.addElement(nextObject);
					}
				}
			}

			if (tabContainer.getContents().size() > 0) {
				contents.put(new TabInfo(), tabContainer);
			}
		}

		public Html toHtml(ExecutionContext context) {
			return toHtml(context, null);
		}
		public Html toHtml(ExecutionContext context, FormRenderable extraText) {
			HtmlItems result = new Table();
			TableRow singleRow = new TableRow();
			result.add(singleRow);
			
			TableCell lastTab = null;
			for (Map.Entry<TabInfo, ContainerElement> nextTab : contents
					.entrySet()) {
				TableCell tab = new TableCell(nextTab.getKey().style);
				tab.appendContents(nextTab.getValue().getContents(), context);

				singleRow.add(tab);
				lastTab = tab;
			}
			
			if(extraText != null) {
				lastTab.add(extraText.toHtml(context));
			}

			return result;
		}

		public List<FormRenderable> getContents() {
			throw new IllegalStateException(this.getClass()
					+ ".getContents() is not implemented");
		}

		public boolean isEmpty(ExecutionContext context) {
			return false;
		}

		// TODO: This is not correct. Revisit when becomes an issue. SL.
		public ResponseCreator getResponseCreatorForComponentId(String componentId) {
			return null;
		}
	}

	private static class Table extends Block {
		public Table() {
			super("table");
			setAttribute("class", "TAWALA-tabbed-paragraph");
		}
	}

	private static class TableRow extends Block {
		public TableRow() {
			super("tr");
		}
	}

	private static class TableCell extends Block {
		public TableCell(String style) {
			super("td");
			if (style != null) {
				setAttribute("style", style);
			}
		}
	}

	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		return implementation.getResponseCreatorForComponentId(componentId);
	}
}
