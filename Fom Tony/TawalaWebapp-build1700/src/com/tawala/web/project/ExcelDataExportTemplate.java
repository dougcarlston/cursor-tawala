package com.tawala.web.project;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.util.Collection;
import java.util.LinkedHashMap;
import java.util.Map;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.core.io.Resource;

import com.tawala.domain.notification.BaseNotification;

public class ExcelDataExportTemplate {
	private static Map<String, byte[]> templates = new LinkedHashMap<String, byte[]>();

	public static HSSFWorkbook getTemplateWorkbookById(String id) throws IOException {
		byte[] template = templates.get(id);
		if(template == null) {
			throw new IllegalArgumentException("Unable to find template by id '" + id + "'");
		}
		return new HSSFWorkbook(new ByteArrayInputStream(template));
	}

	public static Collection<String> getTemplateIds() {
		return templates.keySet();
	}

	public static class Initializer {
		public void setTemplates(Map<String, Resource> templateMap) throws IOException {
			for (Map.Entry<String, Resource> mapEntry : templateMap.entrySet()) {
				String templateId = mapEntry.getKey();
				Resource file = mapEntry.getValue();
				templates.put(templateId, BaseNotification.getResourceAsByteArray(file).toByteArray());
			}
		}
	}
}
