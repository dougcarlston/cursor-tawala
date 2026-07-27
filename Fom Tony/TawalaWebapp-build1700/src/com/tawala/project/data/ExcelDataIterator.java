package com.tawala.project.data;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFDateUtil;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.scissor.Log;

public class ExcelDataIterator implements DataIterator {
	private InputStream inputStream;
	private Iterator rowIterator;

	public ExcelDataIterator(HSSFSheet sheet) {
		rowIterator = sheet.rowIterator();
		this.inputStream = null;
	}

	public ExcelDataIterator(InputStream inputStream) throws IOException {
		this(new HSSFWorkbook(inputStream).getSheetAt(0));
		this.inputStream = inputStream;
	}

	public void close() {
		if(inputStream != null) {
			try {
				inputStream.close();
			} catch (IOException e) {
				Log.warn(this, "Error closing the stream: ", e);
			}
		}
	}

	public boolean hasNext() {
		return rowIterator.hasNext();
	}

	public List<String> next() {
		List<String> result = new ArrayList<String>();

		HSSFRow row = (HSSFRow) rowIterator.next();
		Iterator cellIterator = row.cellIterator();
		while (cellIterator.hasNext()) {
			HSSFCell cell = (HSSFCell) cellIterator.next();
			//--- Fill the row with empty columns in case some of the cells are not set.
			while(cell.getCellNum() > result.size()) {
				result.add("");
			}
			switch (cell.getCellType()) {
			case HSSFCell.CELL_TYPE_BLANK:
				result.add("");
				break;

			case HSSFCell.CELL_TYPE_BOOLEAN:
				result.add(String.valueOf(cell.getBooleanCellValue()));
				break;

			case HSSFCell.CELL_TYPE_ERROR:
				result.add("error");
				break;

			case HSSFCell.CELL_TYPE_FORMULA:
				result.add(cell.getCellFormula());
				break;

			case HSSFCell.CELL_TYPE_NUMERIC:
				if (HSSFDateUtil.isCellDateFormatted(cell)) {
					result.add(String.valueOf(cell.getDateCellValue()));
				} else {
					double doublelValue = cell.getNumericCellValue();
					long longValue = (long)doublelValue;
					if(doublelValue == longValue) {
						result.add(String.valueOf(longValue));
					} else {
						result.add(String.valueOf(doublelValue));
					}
				}
				break;

			case HSSFCell.CELL_TYPE_STRING:
				result.add(cell.getRichStringCellValue().getString());
				break;
			}
		}
		return result;
	}

	public void remove() {
		throw new IllegalStateException("remove() is not supported.");
	}
}
