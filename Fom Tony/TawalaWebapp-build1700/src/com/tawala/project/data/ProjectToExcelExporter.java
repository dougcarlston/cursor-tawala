package com.tawala.project.data;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Properties;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFCellStyle;
import org.apache.poi.hssf.usermodel.HSSFDataFormat;
import org.apache.poi.hssf.usermodel.HSSFFont;
import org.apache.poi.hssf.usermodel.HSSFName;
import org.apache.poi.hssf.usermodel.HSSFRichTextString;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.hssf.util.HSSFColor;
import org.apache.poi.hssf.util.Region;

import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.FormSubmissionsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.project.Value;
import com.tawala.project.commands.Reference;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.projectmanager.alldataimport.AllDataImportController;

public class ProjectToExcelExporter {
	public static final String BACKUP_VERSION_NUMBER_NAME = "BackupVersionNumber";
	public static final String PROJECT_VERSION_NUMBER_NAME = "ProjectVersionNumber";
	public static final String PROJECT_ID_NAME = "ProjectId";
	public static final String TAWALA_BACKUP_TOKEN_NAME = "TawalaBackupToken";
	public static final String LINK_TO_PROJECT_RESTORE_NAME = "LinkToProjectRestore";
	public static final String FORM_NAME_TO_SHEET_NAME_MAP_NAME = "FormNameToSheetNameMap";

	public static final String OVERVIEW_SHEET_NAME = "Backup Overview";
	public static final int CURRENT_VERSION_NUMBER = 2;
	public static final String UNIQUE_BACKUP_TOKEN = "tawala-backup-file";

	private final HSSFWorkbook workbook;

	private boolean formDataStylesAdded;
	private HSSFCellStyle formDataDefaultDataCellStyle;
	private HSSFCellStyle formDataDateTimeCellStyle;
	private HSSFCellStyle formDataHeaderCellStyle;
	private HSSFFont formDataHeaderFont;

	private HSSFSheet coverSheet;
	private int lastCoverSheetStatisticsRow;

	private HSSFCellStyle formStatsDateCellStyle;
	private Map<String, String> formNameToSheetNameMap = new HashMap<String, String>();

	public ProjectToExcelExporter() {
		this.workbook = new HSSFWorkbook();
	}

	private String translateFormNameIntoSheetName(String formName) {
		String result = formNameToSheetNameMap.get(formName);
		if (result == null) {
			result = addNormalizedFormNameToTheMap(formName,
					normalizeFormName(formName));
		}
		return result;
	}

	private String addNormalizedFormNameToTheMap(String formName,
			String normalizedName) {
		String result = normalizedName;
		int i = 0;
		while (formNameToSheetNameMap.containsValue(result)) {
			String suffix = Integer.toString(++i);
			result = (normalizedName.length() + suffix.length() > 31 ? normalizedName
					.substring(0, 31 - suffix.length())
					: normalizedName)
					+ suffix;
		}
		formNameToSheetNameMap.put(formName, result);
		return result;
	}

	private static String normalizeFormName(String formName) {
		if ((formName == null) || (formName.length() == 0)) {
			throw new IllegalArgumentException("formName is null or blank.");
		}
		StringBuilder result = new StringBuilder(
				formName.length() > 31 ? formName.substring(0, 31) : formName);
		for (int i = 0; i < result.length(); i++) {
			char c = result.charAt(i);
			switch (c) {
			case '/':
			case '\\':
			case '?':
			case '*':
			case '[':
			case ']':
				c = '-';
				break;
			}
			result.setCharAt(i, c);
		}

		return result.toString();
	}

	public void addFormDataSheet(String formName,
			List<FormSubmission> formResponses, Collection<Reference> fields) {
		Date lastCreated = null;

		createFormDataStyles();

		final HSSFSheet sheet = workbook
				.createSheet(translateFormNameIntoSheetName(formName));
		sheet.setDisplayGridlines(false);

		int rowCount = -1;

		rowCount = addFormDataHeaders(sheet, rowCount, fields);

		for (FormSubmission submission : formResponses) {
			short cellNumber = 0;
			HSSFRow row = sheet.createRow(++rowCount);
			HSSFCell cell = null;
			for (Reference reference : fields) {
				cell = row.createCell(cellNumber++);
				List<Value> values = submission.getValues(reference);
				switch (values.size()) {
				case 0:
					cell.setCellValue(new HSSFRichTextString(""));
					break;

				case 1:
					Value storedValue = values.get(0);
					if (storedValue.isNumeric()) {
						cell.setCellValue(storedValue.asNumber().doubleValue());
					} else {
						cell.setCellValue(new HSSFRichTextString(storedValue
								.toString()));
					}
					break;

				default:
					cell.setCellValue(new HSSFRichTextString(
							createCSVEncodedString(values)));
					break;
				}
				cell.setCellStyle(formDataDefaultDataCellStyle);
			}

			// --- Date created
			cell = row.createCell(cellNumber++);
			Date eventDate = submission.getCreationDate();
			cell.setCellValue(eventDate);
			cell.setCellStyle(formDataDateTimeCellStyle);

			if (lastCreated == null
					|| lastCreated.before(submission.getCreationDate())) {
				lastCreated = submission.getCreationDate();
			}

			// --- Date updated
			cell = row.createCell(cellNumber++);
			eventDate = submission.getUpdatedDate();
			if(eventDate != null) {
				cell.setCellValue(eventDate);
			}
			cell.setCellStyle(formDataDateTimeCellStyle);
		}

		if (coverSheet != null) {
			addFormStatistics(formName, formResponses.size(), lastCreated);
		}

	}

	public static String createCSVEncodedString(List<Value> values) {
		StringBuilder stringBuilder = new StringBuilder(
				quoteIfRequiredByCSVFormat(values.get(0).toString()));
		for (int i = 1; i < values.size(); i++) {
			stringBuilder.append(",");
			stringBuilder.append(quoteIfRequiredByCSVFormat(values.get(i)
					.toString()));
		}
		String result = stringBuilder.toString();
		return result;
	}

	public static String quoteIfRequiredByCSVFormat(String string) {
		StringBuilder result = new StringBuilder();
		boolean alreadyQuoted = false;
		for (int i = 0; i < string.length(); i++) {
			char nextChar = string.charAt(i);
			switch (nextChar) {
			case '"':
				result.append('"');

			case '\n':
			case '\r':
			case ',':
				if (!alreadyQuoted) {
					alreadyQuoted = true;
					result.insert(0, '\"');
				}

			default:
				result.append(nextChar);
				break;
			}
		}
		if (alreadyQuoted) {
			result.append('"');
		}

		return result.toString();
	}

	private void addFormStatistics(String formName, int numberOfRecords,
			Date lastCreated) {
		HSSFRow row = coverSheet.createRow(++lastCoverSheetStatisticsRow);
		HSSFCell cell = row.createCell((short) 1);
		cell.setCellValue(new HSSFRichTextString(formName));

		cell = row.createCell((short) 2);
		cell.setCellValue(numberOfRecords);

		if (lastCreated != null) {
			cell = row.createCell((short) 3);
			cell.setCellStyle(formStatsDateCellStyle);
			cell.setCellValue(lastCreated);
		}
	}

	private void createFormDataStyles() {
		if (formDataStylesAdded) {
			return;
		}

		formDataDefaultDataCellStyle = workbook.createCellStyle();
		formDataDefaultDataCellStyle
				.setBorderBottom(HSSFCellStyle.BORDER_DOTTED);
		formDataDefaultDataCellStyle
				.setBottomBorderColor(HSSFColor.GREY_25_PERCENT.index);
		formDataDefaultDataCellStyle.setBorderLeft(HSSFCellStyle.BORDER_DOTTED);
		formDataDefaultDataCellStyle
				.setLeftBorderColor(HSSFColor.GREY_25_PERCENT.index);

		formDataDateTimeCellStyle = workbook.createCellStyle();
		formDataDateTimeCellStyle.setBorderBottom(HSSFCellStyle.BORDER_DOTTED);
		formDataDateTimeCellStyle
				.setBottomBorderColor(HSSFColor.GREY_25_PERCENT.index);
		formDataDateTimeCellStyle.setBorderLeft(HSSFCellStyle.BORDER_DOTTED);
		formDataDateTimeCellStyle
				.setLeftBorderColor(HSSFColor.GREY_25_PERCENT.index);
		formDataDateTimeCellStyle.setDataFormat(HSSFDataFormat
				.getBuiltinFormat("m/d/yy h:mm"));

		formDataHeaderCellStyle = workbook.createCellStyle();
		formDataHeaderCellStyle.setBorderBottom(HSSFCellStyle.BORDER_DOUBLE);
		formDataHeaderCellStyle.setBorderTop(HSSFCellStyle.BORDER_DOUBLE);
		formDataHeaderCellStyle.setBorderLeft(HSSFCellStyle.BORDER_THIN);
		formDataHeaderCellStyle.setAlignment(HSSFCellStyle.ALIGN_CENTER);
		formDataHeaderCellStyle.setWrapText(true);

		formDataHeaderFont = workbook.createFont();
		formDataHeaderFont.setBoldweight(HSSFFont.BOLDWEIGHT_BOLD);
		formDataHeaderFont.setFontHeightInPoints((short) 10);
		formDataHeaderCellStyle.setFont(formDataHeaderFont);

		formDataStylesAdded = true;
	}

	private int addFormDataHeaders(final HSSFSheet sheet, int rowCount,
			Collection<Reference> fields) {
		HSSFRow headerRow = sheet.createRow(++rowCount);

		HSSFCell cell = null;
		short cellNumber = 0;
		for (Reference reference : fields) {
			cell = headerRow.createCell(cellNumber++);
			cell.setCellStyle(formDataHeaderCellStyle);
			cell.setCellValue(new HSSFRichTextString(reference.getFieldName()));
		}

		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(formDataHeaderCellStyle);
		cell.setCellValue(new HSSFRichTextString("Date Created"));
		sheet.setColumnWidth(cellNumber, (short) (20 * 256));

		cellNumber++;
		
		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(formDataHeaderCellStyle);
		cell.setCellValue(new HSSFRichTextString("Date Updated"));
		sheet.setColumnWidth(cellNumber, (short) (20 * 256));

		return rowCount;
	}

	public HSSFWorkbook getWorkBook() {
		return workbook;
	}

	public void addCoverSheet(UserProject project) {
		coverSheet = workbook.createSheet(OVERVIEW_SHEET_NAME);
		coverSheet.setDisplayGridlines(false);
		coverSheet.setProtect(true);

		final short totalCellCount = 5; // --- Actually, -1.
		int rowCount = -1;

		// --- Add Title
		HSSFCellStyle titleCellStyle = workbook.createCellStyle();
		titleCellStyle.setWrapText(true);
		titleCellStyle.setAlignment(HSSFCellStyle.ALIGN_CENTER);
		titleCellStyle.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);

		HSSFFont titleFont = workbook.createFont();
		titleFont.setBoldweight(HSSFFont.BOLDWEIGHT_BOLD);
		titleFont.setFontHeightInPoints((short) 12);
		titleFont.setItalic(true);
		titleCellStyle.setFont(titleFont);

		HSSFRow row = coverSheet.createRow(++rowCount);
		HSSFCell cell = row.createCell((short) 0);
		cell.setCellValue(new HSSFRichTextString("Backup of "
				+ project.getName()));
		cell.setCellStyle(titleCellStyle);
		row.setHeightInPoints(30);

		coverSheet.addMergedRegion(new Region(rowCount, (short) 0, rowCount,
				totalCellCount));

		HSSFCellStyle reportDescriptionCellStyle = workbook.createCellStyle();
		reportDescriptionCellStyle.setWrapText(true);
		reportDescriptionCellStyle.setAlignment(HSSFCellStyle.ALIGN_RIGHT);
		reportDescriptionCellStyle
				.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);

		HSSFFont reportDescriptionFont = workbook.createFont();
		reportDescriptionFont.setFontHeightInPoints((short) 10);
		reportDescriptionFont.setItalic(true);
		reportDescriptionFont.setColor(HSSFColor.GREY_50_PERCENT.index);
		reportDescriptionCellStyle.setFont(reportDescriptionFont);

		// --- Date Generated
		row = coverSheet.createRow(++rowCount);
		cell = row.createCell((short) 0);
		cell.setCellStyle(reportDescriptionCellStyle);
		cell.setCellValue(new HSSFRichTextString("Generated on "
				+ new SimpleDateFormat("EEEE MMM d, yyyy hh:mm z")
						.format(new Date())));
		row.setHeightInPoints(20);
		coverSheet.addMergedRegion(new Region(rowCount, (short) 0, rowCount,
				totalCellCount));

		// --- Project Version
		row = coverSheet.createRow(++rowCount);
		cell = row.createCell((short) 0);
		cell.setCellStyle(reportDescriptionCellStyle);
		cell.setCellValue(new HSSFRichTextString("Project version #"
				+ project.getDeployedVersion().getVersionNumber()));
		row.setHeightInPoints(20);
		coverSheet.addMergedRegion(new Region(rowCount, (short) 0, rowCount,
				totalCellCount));

		// --- Special tokens
		HSSFFont invisibleFont = workbook.createFont();
		invisibleFont.setColor(HSSFColor.WHITE.index);
		HSSFCellStyle invisibleStyle = workbook.createCellStyle();
		invisibleStyle.setFont(invisibleFont);
		invisibleStyle.setHidden(true);

		row = coverSheet.createRow(++rowCount);
		cell = row.createCell((short) 0);
		cell.setCellStyle(invisibleStyle);
		cell.setCellValue(CURRENT_VERSION_NUMBER);
		nameTheCell(BACKUP_VERSION_NUMBER_NAME, OVERVIEW_SHEET_NAME, cell
				.getCellNum(), rowCount);

		cell = row.createCell((short) 1);
		cell.setCellStyle(invisibleStyle);
		cell.setCellValue(new HSSFRichTextString(UNIQUE_BACKUP_TOKEN));
		nameTheCell(TAWALA_BACKUP_TOKEN_NAME, OVERVIEW_SHEET_NAME, cell
				.getCellNum(), rowCount);

		cell = row.createCell((short) 2);
		cell.setCellStyle(invisibleStyle);
		cell.setCellValue(project.getId());
		nameTheCell(PROJECT_ID_NAME, OVERVIEW_SHEET_NAME, cell.getCellNum(),
				rowCount);

		cell = row.createCell((short) 3);
		cell.setCellStyle(invisibleStyle);
		cell.setCellValue(project.getDeployedVersion().getVersionNumber());
		nameTheCell(PROJECT_VERSION_NUMBER_NAME, OVERVIEW_SHEET_NAME, cell
				.getCellNum(), rowCount);

		cell = row.createCell((short) 4);
		cell.setCellStyle(invisibleStyle);
		cell.setCellValue(new HSSFRichTextString(
				createFormNameToSheetNameMap(project)));
		nameTheCell(FORM_NAME_TO_SHEET_NAME_MAP_NAME, OVERVIEW_SHEET_NAME, cell
				.getCellNum(), rowCount);

		// --- Backup Overview Text
		HSSFCellStyle overviewStyle = workbook.createCellStyle();
		overviewStyle.setWrapText(true);

		row = coverSheet.createRow(++rowCount);
		cell = row.createCell((short) 1);
		cell.setCellStyle(overviewStyle);
		row.setHeightInPoints(40);

		HSSFRichTextString overviewText = new HSSFRichTextString(
				"This file is the backup of a Tawala project. "
						+ "Don't modify the overview sheet because it contains important information required to restore the data.");
		cell.setCellValue(overviewText);
		Region overviewRegion = new Region(rowCount, (short) 1, rowCount,
				totalCellCount);
		coverSheet.addMergedRegion(overviewRegion);

		HSSFCellStyle linkStyle = workbook.createCellStyle();
		linkStyle.setAlignment(HSSFCellStyle.ALIGN_CENTER);
		linkStyle.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);
		HSSFFont linkFont = workbook.createFont();
		linkFont.setBoldweight(HSSFFont.BOLDWEIGHT_BOLD);
		linkFont.setColor(HSSFColor.BLUE.index);
		linkFont.setUnderline(HSSFFont.U_SINGLE);
		linkFont.setFontHeightInPoints((short) 12);
		linkStyle.setFont(linkFont);

		String restoreURL = "http://" + UserProject.getWebsiteHostName()
				+ WellKnown.urls.getProjectManagerRestoreAllData() + '?'
				+ AllDataImportController.PROJECT_ID_PARAMETER + '='
				+ project.getId();
		String formula = "HYPERLINK(\""
				+ restoreURL
				+ "\", \"Click here to import the data back into the project\")";
		row = coverSheet.createRow(rowCount);
		row.setHeightInPoints(25);

		cell = row.createCell((short) 0);
		cell.setCellStyle(invisibleStyle);
		nameTheCell(LINK_TO_PROJECT_RESTORE_NAME, OVERVIEW_SHEET_NAME, cell
				.getCellNum(), rowCount);

		cell.setCellFormula(formula);
		coverSheet.addMergedRegion(new Region(rowCount, (short) 0, rowCount,
				totalCellCount));

		++rowCount;

		// --- Form statistics title
		HSSFFont formStatsTitleFont = workbook.createFont();
		formStatsTitleFont.setBoldweight(HSSFFont.BOLDWEIGHT_BOLD);
		formStatsTitleFont.setFontHeightInPoints((short) 11);
		formStatsTitleFont.setItalic(false);

		HSSFCellStyle formStatsTitleCellStyle = workbook.createCellStyle();
		formStatsTitleCellStyle.setWrapText(true);
		formStatsTitleCellStyle.setAlignment(HSSFCellStyle.ALIGN_CENTER);
		formStatsTitleCellStyle.setFont(formStatsTitleFont);
		formStatsTitleCellStyle
				.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);

		row = coverSheet.createRow(++rowCount);
		cell = row.createCell((short) 0);
		cell.setCellStyle(formStatsTitleCellStyle);
		cell.setCellValue(new HSSFRichTextString("Form Data Statistics"));
		row.setHeightInPoints(15);
		coverSheet.addMergedRegion(new Region(rowCount, (short) 0, rowCount,
				totalCellCount));

		// --- Form statistics headers
		HSSFFont formStatsHeaderFont = workbook.createFont();
		formStatsHeaderFont.setBoldweight(HSSFFont.BOLDWEIGHT_BOLD);
		formStatsHeaderFont.setFontHeightInPoints((short) 10);
		formStatsHeaderFont.setItalic(false);

		HSSFCellStyle formStatsHeaderCellStyle = workbook.createCellStyle();
		formStatsHeaderCellStyle.setWrapText(true);
		formStatsHeaderCellStyle.setAlignment(HSSFCellStyle.ALIGN_CENTER);
		formStatsHeaderCellStyle.setFont(formStatsHeaderFont);
		formStatsHeaderCellStyle
				.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);

		row = coverSheet.createRow(++rowCount);
		cell = row.createCell((short) 1);
		cell.setCellStyle(formStatsHeaderCellStyle);
		cell.setCellValue(new HSSFRichTextString("Form Name"));
		coverSheet.setColumnWidth((short) 1, (short) (40 * 256));

		formStatsHeaderCellStyle = workbook.createCellStyle();
		formStatsHeaderCellStyle.setWrapText(true);
		formStatsHeaderCellStyle.setAlignment(HSSFCellStyle.ALIGN_RIGHT);
		formStatsHeaderCellStyle.setFont(formStatsHeaderFont);
		formStatsHeaderCellStyle
				.setVerticalAlignment(HSSFCellStyle.VERTICAL_CENTER);

		cell = row.createCell((short) 2);
		cell.setCellStyle(formStatsHeaderCellStyle);
		cell.setCellValue(new HSSFRichTextString("Number of Records"));
		coverSheet.setColumnWidth((short) 2, (short) (25 * 256));

		cell = row.createCell((short) 3);
		cell.setCellStyle(formStatsHeaderCellStyle);
		cell.setCellValue(new HSSFRichTextString("Last Created"));
		coverSheet.setColumnWidth((short) 3, (short) (25 * 256));

		formStatsDateCellStyle = workbook.createCellStyle();
		formStatsDateCellStyle.setDataFormat(HSSFDataFormat
				.getBuiltinFormat("m/d/yy h:mm"));

		lastCoverSheetStatisticsRow = rowCount;
	}

	private String createFormNameToSheetNameMap(UserProject project) {
		for (Form form : project.getProject().getForms()) {
			addNormalizedFormNameToTheMap(form.getName(),
					normalizeFormName(form.getName()));
		}

		Properties properties = new Properties();
		for (Map.Entry<String, String> mapEntry : formNameToSheetNameMap
				.entrySet()) {
			properties.setProperty(mapEntry.getKey(), mapEntry.getValue());
		}
		ByteArrayOutputStream out = new ByteArrayOutputStream();
		try {
			properties.store(out, null);
		} catch (IOException e) {
			throw new IllegalStateException(e);
		}
		return new String(out.toByteArray());
	}

	private void nameTheCell(String cellName, String sheetName,
			short cellNumber, int rowNumber) {
		HSSFName name = workbook.createName();
		name.setNameName(cellName);
		String reference = '\'' + sheetName + '\'' + '!' + '$'
				+ (char) ('A' + cellNumber) + '$' + (rowNumber + 1) + ':' + '$'
				+ (char) ('A' + cellNumber) + '$' + (rowNumber + 1);
		name.setReference(reference);
	}

	public static ExportResult exportAllProjectData(UserProject project) {
		ExportResult result = new ExportResult();
		result.statistics.currentProjectVersion = project.getDeployedVersion()
				.getVersionNumber();

		ProjectToExcelExporter exporter = new ProjectToExcelExporter();
		exporter.addCoverSheet(project);

		for (Form form : project.getProject().getForms()) {
			Collection<Reference> fieldReferences = new ArrayList<Reference>();
			for (Field field : form.getAllFields()) {
				fieldReferences.add(new Reference(field.getHtmlId()));
			}

			List<FormSubmission> responsesForForm = new FormSubmissionsHibernateImpl()
					.responsesFor(project.getProject(), form);
			exporter.addFormDataSheet(form.getName(), responsesForForm,
					fieldReferences);
			result.statistics.totalRecordCount += responsesForForm.size();
		}

		result.workbook = exporter.getWorkBook();
		return result;
	}

	public Map<String, String> getFormNameToSheetNameMap() {
		return formNameToSheetNameMap;
	}
}
