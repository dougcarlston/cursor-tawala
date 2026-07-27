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

public class RosterReportGenerator {

	private ColumnInfo[] columns = new ColumnInfo[] {
			new ColumnInfo(
					new Reference("Record:Registration:FirstName", true),
					"First Name"),
			new ColumnInfo(new Reference("Record:Registration:LastName", true),
					"Last Name"),
			new ColumnInfo(new Reference("Record:Registration:Sex", true),
					"Gender"),
			new ColumnInfo(new Reference("Record:Registration:Birthday", true),
					"Birthday"),
			new ColumnInfo(new Reference("Record:Registration:ParentFirstName",
					true), "Parent First Name"),
			new ColumnInfo(new Reference("Record:Registration:ParentLastName",
					true), "Parent Last Name"),
			new ColumnInfo(new Reference("Record:Registration:Address", true),
					"Address"),
			new ColumnInfo(new Reference("Record:Registration:City", true),
					"City"),
			new ColumnInfo(new Reference("Record:Registration:State", true),
					"State"),
			new ColumnInfo(new Reference("Record:Registration:Zip", true),
					"Zip"),
			new ColumnInfo(new Reference(
					"Record:Registration:Parent2FirstName", true),
					"Second Parent First Name"),
			new ColumnInfo(new Reference("Record:Registration:Parent2LastName",
					true), "Second Parent Last Name"),
			new ColumnInfo(new Reference("Record:Registration:Address2", true),
					"Second Parent Address"),
			new ColumnInfo(new Reference("Record:Registration:City2", true),
					"Second Parent City"),
			new ColumnInfo(new Reference("Record:Registration:State2", true),
					"Second Parent State"),
			new ColumnInfo(new Reference("Record:Registration:Zip2", true),
					"Second Parent Zip"),
			new ColumnInfo(new Reference("Record:Registration:ParentEmail",
					true), "Parent Email"),
			new ColumnInfo(new Reference("Record:Registration:ParentEmail2",
					true), "Additional Email"),
			new ColumnInfo(new Reference("Record:Registration:Phone1", true),
					"Mom's Best Phone"),
			new ColumnInfo(new Reference("Record:Registration:Phone2", true),
					"Dad's Best Phone"),
			new ColumnInfo(new Reference("Record:Registration:Phone3", true),
					"Home Phone"),
			new ColumnInfo(new Reference("Record:Registration:School", true),
					"School"),
			new ColumnInfo(new Reference("Record:Registration:Grade", true),
					"Grade"),
			new ColumnInfo(new Reference("Record:Registration:Parish", true),
					"Parish"),
			new ColumnInfo(new Reference(
					"Record:Registration:CustomAnswer3", true),
					"Exception"),
			new ColumnInfo(new Reference("Record:Registration:MedPlanName",
					true), "Insurance Plan"),
			new ColumnInfo(new Reference("Record:Registration:MedPlanNumber",
					true), "Plan Number"),
			new ColumnInfo(new Reference("Record:Registration:DocName", true),
					"Doctor Name"),
			new ColumnInfo(new Reference("Record:Registration:DocPhone", true),
					"Doctor Phone"),
			new ColumnInfo(
					new Reference("Record:Registration:DocAddress", true),
					"Doctor Address"),
			new ColumnInfo(new Reference("Record:Registration:DonationAmount",
					true), "Donation Amount from Registration Form"),
			new ColumnInfo(new Reference(
					"Record:Registration:OptedOutFromVolunteering", true),
					"Paid Extra to not Volunteer"),
			new ColumnInfo(new Reference("Record:Registration:TeamName", true),
					"Team Name"),
			new ColumnInfo(new Reference("Record:Registration:PaymentReceived",
					true), "Payment Status"),
			new ColumnInfo(new Reference("Record:Registration:WaiverReceived",
					true), "Waiver Status"),
			new ColumnInfo(
					new Reference("Record:Registration:PaidAmount", true),
					"Total Amount Paid with Registration"),
			new ColumnInfo(new Reference("Record:Registration:PaymentMethod",
					true), "Payment Method"),
			new ColumnInfo(new Reference(
					"Record:Registration:CustomFeeCategory", true),
					"Custom Fees"), };

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
			List<CompositeFormSubmission> registrationRecords = retrieveRegistrationsFrom(userProject);
			if (registrationRecords == null || registrationRecords.size() == 0) {
				continue;
			}
			for (CompositeFormSubmission compositeFormSubmission : registrationRecords) {
				addRegistrationRecord(sheet, ++currentRowNumber,
						compositeFormSubmission, userProject, columnStyles,
						maxColumnLengths);
			}

		}

		adjustColumnWidths(sheet, totalColumnCount, maxColumnLengths);

		return workbook;
	}

	private void addRegistrationRecord(HSSFSheet sheet, int currentRowNumber,
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
			String cellValue = value.toString();


			cell = row.createCell(++cellNumber);

			//TODO: Change column to numeric format
			if(columns[j].displayName == "Total Amount Paid with Registration" ||
				columns[j].displayName == "Donation Amount from Registration Form"){
				cell.setCellValue(value.asNumber().intValue());
				cell.setCellType(HSSFCell.CELL_TYPE_NUMERIC);
			}else{
				cell.setCellValue(new HSSFRichTextString(cellValue));
			}
			
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

	public static List<CompositeFormSubmission> retrieveRegistrationsFrom(
			UserProject userProject) {
		ExecutionContext context = new ExecutionContext(WorldInitializer
				.getDefaultWorld().domain(), userProject);

		RecordSelector setupRecordSelector = new RecordSelector(
				Collections
						.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
								"Registration")), BooleanExpression.TRUE,
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
}
