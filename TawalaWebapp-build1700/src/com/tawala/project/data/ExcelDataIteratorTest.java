package com.tawala.project.data;

import java.util.List;

import junit.framework.TestCase;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;

public class ExcelDataIteratorTest extends TestCase {

	public void testImportingNumbers() {
		HSSFWorkbook workbook = createExcelWorkbook(new double[][] {{123.3, -123.34, 45.0, -77.0}});
		
		ExcelDataIterator iterator = new ExcelDataIterator(workbook.getSheetAt(0));
		List<String> data = iterator.next();
		assertEquals("123.3", data.get(0));
		assertEquals("-123.34", data.get(1));
		assertEquals("45", data.get(2));
		assertEquals("-77", data.get(3));
	}

	private static HSSFWorkbook createExcelWorkbook(double[][] data) {
		HSSFWorkbook workbook = new HSSFWorkbook();
		HSSFSheet sheet = workbook.createSheet();

		for (int i = 0; i < data.length; i++) {
			HSSFRow row = sheet.createRow(i);
			double[] rowCells = data[i];
			for (short j = 0; j < rowCells.length; j++) {
				HSSFCell cell = row.createCell(j);
				cell.setCellValue(rowCells[j]);
			}
		}

		return workbook;
	}

}
