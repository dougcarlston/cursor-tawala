package com.tawala.web.project;

import java.io.IOException;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.HashMap;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFCellStyle;
import org.apache.poi.hssf.usermodel.HSSFDataFormat;
import org.apache.poi.hssf.usermodel.HSSFFont;
import org.apache.poi.hssf.usermodel.HSSFRichTextString;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;

public class ExcelSpreadsheetCreatorController implements Controller {

	private static final String FIRST_DATA_ROW_MARKER = "<<first data row>>";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		HSSFWorkbook workbook = generateSpreadsheet(request);

		response.setContentType("application/vnd.ms-excel");
		response.setHeader("Content-Disposition", "attachment; filename=\""
				+ "DataExport" + ".xls\";");
		workbook.write(response.getOutputStream());

		return null;
	}

	private HSSFWorkbook generateSpreadsheet(HttpServletRequest request) {
		HSSFWorkbook workbook = null;
		HSSFSheet sheet = null;
		final short totalColumnCount = Short.parseShort(request
				.getParameter("columns"));
		final int dataRowCount = Integer.parseInt(request.getParameter("rows"));

		Map<Short, Integer> maxColumnLengths = new HashMap<Short, Integer>();
		int currentRowNumber = -1;

		final HSSFCellStyle[] columnStyles = new HSSFCellStyle[totalColumnCount];

		boolean templatedOutput = false;
		String templateId = request.getParameter("template");
		if (templateId != null) {
			try {
				workbook = ExcelDataExportTemplate
						.getTemplateWorkbookById(templateId);
				sheet = workbook.getSheetAt(0);

				int startRow = sheet.getFirstRowNum();
				int endRow = sheet.getLastRowNum();
				boolean markerFound = false;
				for (currentRowNumber = startRow; currentRowNumber <= endRow; currentRowNumber++) {
					HSSFRow row = sheet.getRow(currentRowNumber);
					HSSFCell firstCell = row.getCell((short) 0);
					if (firstCell == null
							|| firstCell.getCellType() != HSSFCell.CELL_TYPE_STRING) {
						continue;
					}
					if (firstCell.getRichStringCellValue().getString().equals(
							FIRST_DATA_ROW_MARKER)) {
						for (short i = 0; i < columnStyles.length; i++) {
							HSSFCell cell = row.getCell(i);
							columnStyles[i] = cell.getCellStyle();
						}
						currentRowNumber--;
						markerFound = true;
						break;
					}
				}

				if (markerFound) {
					templatedOutput = true;
				} else {
					Log.error(this, "Unable to find first data row maker \""
							+ FIRST_DATA_ROW_MARKER
							+ "\" in template with id = " + templateId + ".");
				}

			} catch (IOException e) {
				Log.error(this, "Error creating worksheet from template "
						+ templateId + ":", e);
			}
		}

		boolean handleSpecialTimestamps = false;
		
		if (!templatedOutput) {
			workbook = new HSSFWorkbook();
			sheet = workbook.createSheet("Report");
			sheet.setDisplayGridlines(true);

			currentRowNumber = addTitleAndHeaders(workbook, sheet, request, -1,
					totalColumnCount, maxColumnLengths);

			HSSFCellStyle defaultDataCellStyle = workbook.createCellStyle();
			defaultDataCellStyle.setWrapText(true);

			HSSFCellStyle dateTimeDataCellStyle = workbook.createCellStyle();
			dateTimeDataCellStyle.setDataFormat(HSSFDataFormat.getBuiltinFormat("m/d/yy h:mm"));
			
			HSSFRow headerRow = sheet.getRow(0);
			for (int i = 0; i < columnStyles.length; i++) {
				String headerCellText = headerRow.getCell((short)i).getRichStringCellValue().getString();
				if (headerCellText.equals("__Created__") || headerCellText.equals("__Updated__")) {
					// these are special column headers generated in the Itemization Table
					columnStyles[i] = dateTimeDataCellStyle;
					handleSpecialTimestamps = true;
				}
				else {
					columnStyles[i] = defaultDataCellStyle;
				}
			}
		}

		for (int i = 0; i < dataRowCount; i++) {
			HSSFRow row = sheet.createRow(++currentRowNumber);

			for (short j = 0; j < totalColumnCount; j++) {
				String cellValue = request.getParameter("d" + i + "_" + j);
				if (cellValue == null) {
					continue;
				}
				
				cellValue = cleanCellValue(cellValue);
				
				HSSFCell cell = row.createCell(j);
				
				try {
					cell.setCellValue(Double.parseDouble(cellValue));
				} catch (NumberFormatException e) {
					if (handleSpecialTimestamps == true && (j == totalColumnCount - 1 || j == totalColumnCount - 2 )) {
						try {
							SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy/MM/dd HH:mm:ss");
							cell.setCellValue(dateFormat.parse(cellValue));
						} catch (ParseException e1) {
							cell.setCellValue(new HSSFRichTextString(cellValue));
						}
					}
					else {
						cell.setCellValue(new HSSFRichTextString(cellValue));
					}
				}

				HSSFCellStyle cellStyle = columnStyles[j];
				if (cellStyle != null) {
					cell.setCellStyle(cellStyle);
				}

				if (!templatedOutput) {
					updateMaxColumnLengthIfNeeded(maxColumnLengths, j,
							cellValue.length());
				}
			}
		}

		if (!templatedOutput) {
			adjustColumnWidths(sheet, totalColumnCount, maxColumnLengths);
		}

		return workbook;
	}

	private static String cleanCellValue(String cellValue) {
		StringBuilder result = new StringBuilder(cellValue.length());
		for (int i = 0; i < cellValue.length(); i++) {
			char currentChar = cellValue.charAt(i);
			switch (currentChar) {
			case '\r':
				//-- remove it
				break;

			default:
				result.append(currentChar);
				break;
			}
		}
		return result.toString();
	}

	private void adjustColumnWidths(HSSFSheet sheet,
			final short totalColumnCount, Map<Short, Integer> maxColumnLengths) {
		for (short i = 0; i < totalColumnCount; i++) {
			// --- Keep the column length in some reasonable boundaries.
			Integer maxLength = maxColumnLengths.get(i);
			int normalizedLength = maxLength == null ? 5 : Math.max(5, maxLength);
			normalizedLength = Math.min(normalizedLength, 50);
			normalizedLength += 3;

			sheet.setColumnWidth(i, (short) (normalizedLength * 256));
		}
	}

	private void updateMaxColumnLengthIfNeeded(
			Map<Short, Integer> maxColumnLengths, short columnNumber,
			int columnLength) {
		Integer value = maxColumnLengths.get(columnNumber);
		int currentLength = value == null ? 0 : value;

		if (columnLength > currentLength) {
			maxColumnLengths.put(columnNumber, columnLength);
		}
	}

	private int addTitleAndHeaders(final HSSFWorkbook workbook,
			final HSSFSheet sheet, HttpServletRequest request,
			int currentRowNumber, short totalColumnCount,
			Map<Short, Integer> maxColumnLengths) {
		HSSFCell cell = null;

		// --- Add header
		HSSFCellStyle headerCellStyle = workbook.createCellStyle();
		headerCellStyle.setBorderBottom(HSSFCellStyle.BORDER_DOUBLE);
		headerCellStyle.setBorderTop(HSSFCellStyle.BORDER_DOUBLE);
		headerCellStyle.setBorderLeft(HSSFCellStyle.BORDER_THIN);
		headerCellStyle.setAlignment(HSSFCellStyle.ALIGN_CENTER);
		headerCellStyle.setWrapText(true);

		HSSFFont headerFont = workbook.createFont();
		headerFont.setBoldweight(HSSFFont.BOLDWEIGHT_BOLD);
		headerFont.setFontHeightInPoints((short) 10);
		headerCellStyle.setFont(headerFont);

		HSSFRow headerRow = sheet.createRow(++currentRowNumber);

		short cellNumber = -1;
		for (short i = 0; i < totalColumnCount; i++) {
			cellNumber++;
			cell = headerRow.createCell(cellNumber);
			cell.setCellStyle(headerCellStyle);

			String header = request.getParameter("h" + i);
			if (header == null) {
				header = "";
			}
			
			cell.setCellValue(new HSSFRichTextString(cleanCellValue(header)));
			updateMaxColumnLengthIfNeeded(maxColumnLengths, i, header.length());
		}

		return currentRowNumber;
	}

}
