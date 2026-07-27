package com.tawala.web;

import java.util.List;
import javax.servlet.http.HttpServletRequest;
import org.dom4j.Element;
import com.tawala.project.data.DataSource;

public class DataSourceQueryResponse extends ApiResponse {
    protected final List<DataSource> dataSources;

    public DataSourceQueryResponse(List<DataSource> dataSources) {
        this.dataSources = dataSources;
    }

    protected void addContents(Element root, HttpServletRequest request) {
        Element dataSourcesElement = root.addElement("datasources");
        if(dataSources != null && dataSources.size() > 0) {
        	for (DataSource dataSource : dataSources) {
				dataSource.toXml(dataSourcesElement);
			}
        }
    }

    protected String status() {
        return "success";
    }
}
