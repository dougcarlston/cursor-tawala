package com.tawala.project.data;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;

public class ExportResult {
	public ExportResult() {
		this.statistics = new ExportStatistics();
	}
	public ExportStatistics statistics;
	public HSSFWorkbook workbook;
}
