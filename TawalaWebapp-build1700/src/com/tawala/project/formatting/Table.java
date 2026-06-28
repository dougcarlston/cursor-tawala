package com.tawala.project.formatting;

import java.util.ArrayList;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.component.web.ResponseCreator;
import com.tawala.project.FormRenderable;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.web.oldhtml.Html;

public class Table implements FormRenderable {
	private final List<List<Column>> rows = new ArrayList<List<Column>>();
	private final String style;

	public Table(ConfigElement config) {
		List<ConfigElement> rowElements = config.children("row");
		for (ConfigElement row : rowElements) {
			List<Column> columns = new ArrayList<Column>();
			for (ConfigElement columnElement : row.children("cell")) {
				columns.add(new Column(columnElement));
			}
			rows.add(columns);
		}
		
		this.style = "margin-left: " + (config.attribute("indent").intValue()/20) + "pt";
	}
	
	public Html toHtml(ExecutionContext context) {
		com.tawala.web.oldhtml.Table result = new com.tawala.web.oldhtml.Table("style", style);
		for (List<Column> row : rows) {
			List<com.tawala.web.oldhtml.Table.Column> htmlColumns = new ArrayList<com.tawala.web.oldhtml.Table.Column>(row.size());
			
			for (Column column : row) {
				htmlColumns.add(column.toHtmlColumn(context));
			}
			
			result.addRow(htmlColumns);
		}
		return result;
	}

	public boolean isEmpty(ExecutionContext context) {
		return false;
	}

	public ResponseCreator getResponseCreatorForComponentId(String componentId) {
		for (List<Column> row : rows) {
			for (Column column : row) {
				ResponseCreator result = column.getResponseCreatorForComponentId(componentId);
				if(result != null) {
					return result;
				}
			}
		}
		return null;
	}
}
