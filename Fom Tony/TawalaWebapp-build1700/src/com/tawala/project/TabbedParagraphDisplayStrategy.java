package com.tawala.project;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.ConfigItem;
import com.scissor.xmlconfig.Factory;
import com.tawala.component.web.ResponseCreator;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.formatting.ContainerElement;
import com.tawala.web.oldhtml.Block;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.HtmlItems;

public class TabbedParagraphDisplayStrategy implements ParagraphDisplayStrategy {
	
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

	public TabbedParagraphDisplayStrategy(ConfigElement config, Factory<FormRenderable> FACTORY) {
		List<TabInfo> tabInfos = new ArrayList<TabInfo>();
		List<ConfigElement> tabPositions = config.child("tabPositions").children("tabStop");
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
		HtmlItems result = new Table();
		TableRow singleRow = new TableRow();
		result.add(singleRow);
		
		for (Map.Entry<TabInfo, ContainerElement> nextTab : contents
				.entrySet()) {
			TableCell tab = new TableCell(nextTab.getKey().style);
			tab.appendContents(nextTab.getValue().getContents(), context);

			singleRow.add(tab);
		}
		
		return result;
	}

	public List<FormRenderable> getContents() {
		List<FormRenderable> result = new ArrayList<FormRenderable>();

		for (ContainerElement nextTab : contents.values()) {
			for (Object nextElement : nextTab.getContents()) {
				if (nextElement instanceof Blank) {
					result.add((Blank) nextElement);
				}
			}
		}
		return result;
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
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
		for (ContainerElement nextTab : contents.values()) {
			for (FormRenderable formRenderable : nextTab.getContents()) {
				ResponseCreator result = formRenderable.getResponseCreatorForComponentId(componentId);
				if(result != null) {
					return result;
				}
			}
		}
		return null;
	}
}
