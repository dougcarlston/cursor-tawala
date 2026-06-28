package com.tawala.web.projectmanager.projectgroup;

import java.util.Collections;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFCellStyle;
import org.apache.poi.hssf.usermodel.HSSFDataFormat;
import org.apache.poi.hssf.usermodel.HSSFFont;
import org.apache.poi.hssf.usermodel.HSSFRichTextString;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.tawala.domain.ProjectGroup;
import com.tawala.project.CompositeFormSubmission;
import com.tawala.project.FormSubmission;
import com.tawala.project.UserProject;
import com.tawala.project.Value;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.ExecutionContext;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.Reference;
import com.tawala.project.commands.RecordSelector.FormDataProvider;
import com.tawala.web.WorldInitializer;

public class CoachReportGenerator {

	private ColumnInfo[] columns = new ColumnInfo[] {
			new ColumnInfo(
					new Reference("Record:Coach:FirstName", true),
					"First Name"),
			new ColumnInfo(
					new Reference("Record:Coach:MiddleName", true),
					"Middle Name"),
			new ColumnInfo(new Reference("Record:Coach:LastName", true),
					"Last Name"),
			new ColumnInfo(
					new Reference("Record:Coach:BirthMonth", true),
					"Birthday Month"),
			new ColumnInfo(new Reference("Record:Coach:BirthDay", true),
					"Birthday Day"),
			new ColumnInfo(
					new Reference("Record:Coach:BirthYear", true),
					"Birthday Year"),
			new ColumnInfo(new Reference("Record:Coach:Age", true),
					"Age"),
			new ColumnInfo(new Reference("Record:Coach:Email", true),
					"Email"),
			new ColumnInfo(
					new Reference("Record:Coach:HomePhone", true),
					"Home Phone"),
			new ColumnInfo(
					new Reference("Record:Coach:WorkPhone", true),
					"Work Phone"),
			new ColumnInfo(
					new Reference("Record:Coach:CellPhone", true),
					"Cell Phone"),
			new ColumnInfo(new Reference("Record:Coach:Street", true),
					"Address"),
			new ColumnInfo(new Reference("Record:Coach:City", true),
					"City"),
			new ColumnInfo(new Reference("Record:Coach:State", true),
					"State"),
			new ColumnInfo(new Reference("Record:Coach:Zip", true),
					"Zip"),
			new ColumnInfo(new Reference("Record:Coach:CoachingTeams",
					true), "Coaching Team"),
			new ColumnInfo(new Reference(
					"Record:Coach:AssistantCoachingTeams", true),
					"Assistant Coaching Team"),
			new ColumnInfo(new Reference(
					"Record:Coach:StatusID", true),
					"Coach Status"),
			new ColumnInfo(new Reference(
					"Record:Coach:StatusMemo", true),
					"Note") };

	public HSSFWorkbook generateReport(ProjectGroup group) {
		HSSFWorkbook workbook = null;
		HSSFSheet sheet = null;
		final short totalColumnCount = (short) (columns.length + 3);

		Map<Short, Integer> maxColumnLengths = new HashMap<Short, Integer>();
		int currentRowNumber = -1;

		final HSSFCellStyle[] columnStyles = new HSSFCellStyle[totalColumnCount];

		workbook = new HSSFWorkbook();
		sheet = workbook.createSheet("Report");
		sheet.setDisplayGridlines(true);

		HSSFCellStyle defaultDataCellStyle = null;
		defaultDataCellStyle = workbook.createCellStyle();
		defaultDataCellStyle.setWrapText(true);

		for (int i = 0; i < columnStyles.length; i++) {
			columnStyles[i] = defaultDataCellStyle;
		}
		HSSFCellStyle defaultDateAndTimeStyle = workbook.createCellStyle();
		defaultDateAndTimeStyle.setDataFormat(HSSFDataFormat
				.getBuiltinFormat("m/d/yy h:mm"));
		defaultDateAndTimeStyle.setAlignment(HSSFCellStyle.ALIGN_LEFT);
		columnStyles[columnStyles.length - 1] = defaultDateAndTimeStyle;

		currentRowNumber = addTitleAndHeaders(workbook, sheet, group, columns,
				-1, totalColumnCount, maxColumnLengths);

		for (UserProject userProject : group.getUserProjects()) {
			List<CompositeFormSubmission> registrationRecords = retrieveCoachesFrom(userProject);
			if (registrationRecords == null || registrationRecords.size() == 0) {
				continue;
			}
			for (CompositeFormSubmission compositeFormSubmission : registrationRecords) {
				addCoachRecord(sheet, ++currentRowNumber,
						compositeFormSubmission, userProject, columnStyles,
						maxColumnLengths);
			}

		}

		adjustColumnWidths(sheet, totalColumnCount, maxColumnLengths);

		return workbook;
	}

	private void addCoachRecord(HSSFSheet sheet, int currentRowNumber,
			CompositeFormSubmission submission, UserProject userProject,
			final HSSFCellStyle[] columnStyles,
			Map<Short, Integer> maxColumnLengths) {
		HSSFRow row = sheet.createRow(currentRowNumber);

		short cellNumber = -1;

		// --- Club
		HSSFCell cell = row.createCell(++cellNumber);
		String userName = userProject.getUser().getId();
		cell.setCellValue(new HSSFRichTextString(userName));
		updateMaxColumnLengthIfNeeded(maxColumnLengths, cellNumber, userName
				.length());

		// --- Sport
		cell = row.createCell(++cellNumber);
		cell.setCellValue(new HSSFRichTextString(userProject.getName()));
		updateMaxColumnLengthIfNeeded(maxColumnLengths, cellNumber, userProject
				.getName().length());

		for (short j = 0; j < columns.length; j++) {
			Reference fieldReference = columns[j].fieldReference;
			FormSubmission record = submission
					.getFormSubmission(fieldReference);
			if (record == null) {
				throw new IllegalStateException("Couldn't find record for "
						+ fieldReference);
			}

			Value value = record.getValue(fieldReference);
			if (value == null) {
				continue;
			}

			String cellValue = "";
			if(columns[j].displayName == "Coach Status") {
				Map<String, String> statusMap = getStatusMap(userProject);
				cellValue= statusMap.get(value.toString());
			}else{
				cellValue = value.toString();
			}
			
			cell = row.createCell(++cellNumber);
			cell.setCellValue(new HSSFRichTextString(cellValue));
			HSSFCellStyle cellStyle = columnStyles[j];
			if (cellStyle != null) {
				cell.setCellStyle(cellStyle);
			}

			updateMaxColumnLengthIfNeeded(maxColumnLengths, cellNumber,
					cellValue.length());
		}

		// --- Date created
		cell = row.createCell(++cellNumber);
		cell.setCellValue(submission.getFormSubmission(
				columns[0].fieldReference).getCreationDate());
		cell.setCellStyle(columnStyles[columnStyles.length - 1]);

		updateMaxColumnLengthIfNeeded(maxColumnLengths, cellNumber, 14);
	}

	private int addTitleAndHeaders(final HSSFWorkbook workbook,
			final HSSFSheet sheet, ProjectGroup group, ColumnInfo[] columns,
			int currentRowNumber, short totalColumnCount,
			Map<Short, Integer> maxColumnLengths) {

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

		short cellNumber = 0;

		addHeader(headerRow, cellNumber++, headerCellStyle, "Club",
				maxColumnLengths);

		addHeader(headerRow, cellNumber++, headerCellStyle, "Sport",
				maxColumnLengths);

		for (short i = 0; i < columns.length; i++) {
			String header = columns[i].displayName;
			if (header == null) {
				header = "";
			}
			addHeader(headerRow, cellNumber++, headerCellStyle, header,
					maxColumnLengths);
		}

		addHeader(headerRow, cellNumber++, headerCellStyle,
				"Registration Date", maxColumnLengths);

		return currentRowNumber;
	}

	private void addHeader(HSSFRow headerRow, short cellNumber,
			HSSFCellStyle headerCellStyle, String columnName,
			Map<Short, Integer> maxColumnLengths) {
		HSSFCell cell;
		cell = headerRow.createCell(cellNumber);
		cell.setCellStyle(headerCellStyle);
		cell.setCellValue(new HSSFRichTextString(columnName));
		updateMaxColumnLengthIfNeeded(maxColumnLengths, cellNumber, columnName
				.length());
	}

	private void adjustColumnWidths(HSSFSheet sheet,
			final short totalColumnCount, Map<Short, Integer> maxColumnLengths) {
		for (short i = 0; i < totalColumnCount; i++) {
			// --- Keep the column length in some reasonable boundaries.
			int normalizedLength = Math.max(5, maxColumnLengths.get(i));
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

	public static List<CompositeFormSubmission> retrieveCoachesFrom(
			UserProject userProject) {
		ExecutionContext context = new ExecutionContext(WorldInitializer
				.getDefaultWorld().domain(), userProject);

		RecordSelector setupRecordSelector = new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								"Coach")), BooleanExpression.TRUE,
				RecordSelector.DEFAULT_RECORD_LIST_NAME);

		return setupRecordSelector.getRecords(context);
	}

	public static class ColumnInfo {
		public ColumnInfo(Reference fieldReference, String displayName) {
			this.fieldReference = fieldReference;
			this.displayName = displayName;
		}

		Reference fieldReference;
		String displayName;
	}
	
	private Map<String, String> getStatusMap(UserProject userProject) {
		Map<String, String> statusMap = new HashMap<String, String>();
		
		ExecutionContext context = new ExecutionContext(WorldInitializer
				.getDefaultWorld().domain(), userProject);

		RecordSelector recordSelector = new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								"CoachStatus")), BooleanExpression.TRUE,
				RecordSelector.DEFAULT_RECORD_LIST_NAME);

		List<CompositeFormSubmission> records = recordSelector
				.getRecords(context);

		Reference statusIdField = new Reference("Record:CoachStatus:ID", true);
		for (CompositeFormSubmission compositeFormSubmission : records) {
			FormSubmission submission = compositeFormSubmission
					.getFormSubmission(statusIdField);
			try{
				statusMap.put(submission.getValue("ID").toString(), submission.getValue("Name").toString());
			} catch(Exception e) {
				throw new IllegalStateException(
						"Unable to create statusMap: ", e);				
			}
		}
		return statusMap;
	}
}
