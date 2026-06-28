package com.tawala.project.data;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Properties;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFName;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.hssf.util.CellReference;

import com.tawala.message.Message;
import com.tawala.project.Field;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;

public class DataImporter {
	private final UserProject userProject;
	private final HSSFWorkbook workbook;
	private final List<Message> errors = new ArrayList<Message>();
	private final List<Message> warnings = new ArrayList<Message>();
	private Map<String, String> sheetNameToFormNameMap;
	private Map<String, String> formNameToSheetNameMap;
	private int backupVersionNumber;
	private int numberOfSystemColumns;

	public List<Message> getErrors() {
		return errors;
	}

	public List<Message> getWarnings() {
		return warnings;
	}

	public DataImporter(UserProject userProject, HSSFWorkbook workbook)
			throws IOException {
		this.userProject = userProject;
		this.workbook = workbook;

		extractSheetNameToFormNameMappings();

		validate(userProject, workbook);
	}

	public static ImportStatistics importData(DataIterator dataIterator,
			FieldSetter[] fieldSetters, int columnCount, boolean skipFirstRow,
			FormSubmissionCreator formSubmissionCreator) {
		ImportStatistics statistics = new ImportStatistics();
		int currentRowNumber = 0;

		try {
			while (dataIterator.hasNext()) {
				List<String> row = dataIterator.next();
				++currentRowNumber;
				if (currentRowNumber == 1 && skipFirstRow) {
					continue;
				}
				
				if(rowIsEmpty(row)) {
					statistics.emptyRow(currentRowNumber);
					continue;
				}

				FormSubmission formSubmission = formSubmissionCreator.create();
				for (int i = 0; i < fieldSetters.length && i < row.size(); i++) {
					FieldSetter fieldSetter = fieldSetters[i];
					if (fieldSetter == null) {
						continue;
					}

					String value = row.get(i);
					if(value.length() != 0) { 
						fieldSetter.setField(formSubmission, value);
					}
				}

				WorldInitializer.getDefaultWorld().domain().storedData()
						.record(formSubmission);
				statistics.incrementAddedRecordCount();
			}
		} finally {
			dataIterator.close();
		}

		return statistics;
	}

	private static boolean rowIsEmpty(List<String> row) {
		for (String column : row) {
			if(column.length() > 0) {
				return false;
			}
		}
		
		return true;
	}

	public Map<String, ImportStatistics> restore() throws IOException {
		if (errors.size() > 0) {
			throw new IllegalStateException(
					"Unable to restore when there are errors.");
		}

		Map<String, ImportStatistics> result = new LinkedHashMap<String, ImportStatistics>();

		for (int sheetNumber = 1; sheetNumber < workbook.getNumberOfSheets(); ++sheetNumber) {
			HSSFSheet currentSheet = workbook.getSheetAt(sheetNumber);

			String sheetName = workbook.getSheetName(sheetNumber);
			String formName = sheetNameToFormNameMap == null ? sheetName
					: sheetNameToFormNameMap.get(sheetName);

			DataIterator dataIterator = new ExcelDataIterator(currentSheet);
			if (!dataIterator.hasNext()) {
				throw new IllegalStateException(
						"No data exists for the sheet #" + sheetNumber);
			}
			Form form = userProject.getProject().getForm(formName);
			if (form == null) {
				throw new IllegalStateException("Unable to find form named '"
						+ formName + "' in the current version of the project.");
			}

			List<String> columns = dataIterator.next();
			columns = columns.subList(0, columns.size() - numberOfSystemColumns);
			
			FieldSetter[] fieldSetters = new FieldSetter[columns.size()];
			int i = 0;
			for (String column : columns) {
				Field field = form.getFieldById(column);
				fieldSetters[i++] = field.getFieldSetter();
			}
			FormSubmissionCreator formSubmissionCreator = new RegularProjectFormSubmissionCreator(
					userProject, form);

			ImportStatistics importStats = importData(dataIterator, fieldSetters, columns.size() + 1,
					false /* the first row has been read already */,
					formSubmissionCreator);
			result.put(formName, importStats);
		}

		return result;
	}

	private void extractSheetNameToFormNameMappings() throws IOException {
		sheetNameToFormNameMap = new HashMap<String, String>();
		formNameToSheetNameMap = new HashMap<String, String>();

		HSSFCell cell = getNamedCell(workbook,
				ProjectToExcelExporter.FORM_NAME_TO_SHEET_NAME_MAP_NAME);
		if (cell == null || cell.getCellType() != HSSFCell.CELL_TYPE_STRING) {
			//--- fall back on the original sheet name/form name mapping.
			for(int i=1; i < workbook.getNumberOfSheets(); i++) {
				String sheetName = workbook.getSheetName(i);
				sheetNameToFormNameMap.put(sheetName, sheetName);
				formNameToSheetNameMap.put(sheetName, sheetName);
			}
		} else {
			Properties formNameToSheetNameProperties = new Properties();
			formNameToSheetNameProperties.load(new ByteArrayInputStream(cell
					.getRichStringCellValue().getString().getBytes()));

			for (Map.Entry<Object, Object> mapEntry : formNameToSheetNameProperties
					.entrySet()) {
				String formName = (String) mapEntry.getKey();
				String sheetName = (String) mapEntry.getValue();
				sheetNameToFormNameMap.put(sheetName, formName);
				formNameToSheetNameMap.put(formName, sheetName);
			}
		}
	}

	private void validate(UserProject project, HSSFWorkbook backup) {
		// --- Test for the overview sheet being present.
		if (!ProjectToExcelExporter.OVERVIEW_SHEET_NAME.equals(backup
				.getSheetName(0))) {
			errors
					.add(new Message(
							"data.all-data-import.incorrect.excel.file"));
			return;
		}

		// --- Validate for presence of the special Tawala token.
		HSSFCell cell = getNamedCell(backup,
				ProjectToExcelExporter.TAWALA_BACKUP_TOKEN_NAME);
		if (cell == null
				|| cell.getCellType() != HSSFCell.CELL_TYPE_STRING
				|| !ProjectToExcelExporter.UNIQUE_BACKUP_TOKEN.equals(cell
						.getRichStringCellValue().getString())) {
			errors
					.add(new Message(
							"data.all-data-import.incorrect.excel.file"));
			return;
		}

		// --- Get backup version number
		cell = getNamedCell(backup, ProjectToExcelExporter.BACKUP_VERSION_NUMBER_NAME);
		if(cell == null || cell.getCellType() != HSSFCell.CELL_TYPE_NUMERIC) {
			//--- TODO: in the future it would need to be more elaborate to deal with obsolete versions.
			errors.add(new Message("data.all-data-import.incorrect.excel.file"));
			return;
		}
		
		this.backupVersionNumber = (int)cell.getNumericCellValue();
		this.numberOfSystemColumns = this.backupVersionNumber > 1 ? 2 : 1;

		// --- Compare project id and version numbers
		cell = getNamedCell(backup, ProjectToExcelExporter.PROJECT_ID_NAME);
		if (cell != null && cell.getCellType() == HSSFCell.CELL_TYPE_NUMERIC) {
			long projectId = (long) cell.getNumericCellValue();
			if (projectId == project.getId()) {
				HSSFCell versionNumberCell = getNamedCell(backup,
						ProjectToExcelExporter.PROJECT_VERSION_NUMBER_NAME);
				if (versionNumberCell != null
						&& versionNumberCell.getCellType() == HSSFCell.CELL_TYPE_NUMERIC) {
					int originalVersionNumber = (int) versionNumberCell
							.getNumericCellValue();
					if (originalVersionNumber != project.getDeployedVersion()
							.getVersionNumber()) {
						warnings.add(new Message(
								"data.all-data-import.versions.dont.match",
								new Object[] {
										originalVersionNumber,
										project.getDeployedVersion()
												.getVersionNumber() }));
					}
				}
			} else {
				warnings.add(new Message(
						"data.all-data-import.projects.dont.match"));
			}
		}

		// --- Check for the presence of the forms in the backup file.
		List<String> missingForms = new ArrayList<String>();
		for (Form form : project.getProject().getForms()) {
			HSSFSheet sheet = backup.getSheet(formNameToSheetNameMap.get(form.getName()));
			if (sheet == null) {
				missingForms.add(form.getName());
			}
		}
		if (missingForms.size() == 1) {
			warnings.add(new Message(
					"data.all-data-import.form.is.missing.from.file",
					new Object[] { missingForms.get(0) }));
		} else if (missingForms.size() > 1) {
			warnings.add(new Message(
					"data.all-data-import.forms.are.missing.from.file",
					new Object[] { createListOfItems(missingForms) }));
		}

		// --- Check for the presence of the forms in the project.
		missingForms = new ArrayList<String>();
		for (int i = 1; i < backup.getNumberOfSheets(); i++) {
			String sheetName = backup.getSheetName(i);
			String formName = sheetNameToFormNameMap.get(sheetName);
			Form form = project.getProject().getForm(formName);
			if (form == null) {
				missingForms.add(formName);
				continue;
			}

			// --- Get the headers from the file
			DataIterator dataIterator = new ExcelDataIterator(workbook
					.getSheetAt(i));
			if (!dataIterator.hasNext()) {
				errors.add(new Message(
						"data.all-data-import.form.sheet.is.empty", sheetName));
				continue;
			}
			List<String> fieldNames = dataIterator.next();

			// --- Check the fields to be present in the project.
			// --- Exclude the last field (date created).
			for (int j = 0; j < fieldNames.size() - numberOfSystemColumns; j++) {
				String fieldName = fieldNames.get(j);
				try {
					form.getFieldById(fieldName);
				} catch (IllegalArgumentException e) {
					errors.add(new Message(
							"data.all-data-import.field.not.found.in.project",
							formName, fieldName));
				}
			}
			// --- Check the fields to be present in the backup file.
			for (Field field : form.getAllFields()) {
				if (!fieldNames.contains(field.getHtmlId())) {
					warnings.add(new Message(
							"data.all-data-import.field.not.found.in.file",
							formName, field.getHtmlId()));
				}
			}
		}
		if (missingForms.size() == 1) {
			errors.add(new Message(
					"data.all-data-import.form.is.missing.from.project",
					new Object[] { missingForms.get(0) }));
		} else if (missingForms.size() > 1) {
			errors.add(new Message(
					"data.all-data-import.forms.are.missing.from.project",
					new Object[] { createListOfItems(missingForms) }));
		}
	}

	public static HSSFCell getNamedCell(HSSFWorkbook workbook,
			String definedName) {
		int index = workbook.getNameIndex(definedName);
		if (index < 0) {
			return null;
		}

		HSSFName name = workbook.getNameAt(index);
		String reference = name.getReference();

		CellReference cellReference = new CellReference(reference);
		String sheetName = cellReference.getSheetName();
		if (sheetName.startsWith("'")) {
			sheetName = sheetName.substring(1);
		}
		if (sheetName.endsWith("'")) {
			sheetName = sheetName.substring(0, sheetName.length() - 1);
		}
		HSSFSheet sheet = workbook.getSheet(sheetName);
		if (sheet == null) {
			return null;
		}

		return sheet.getRow(cellReference.getRow()).getCell(
				cellReference.getCol());
	}

	private static StringBuilder createListOfItems(List<String> items) {
		StringBuilder listOfForms = new StringBuilder("\"" + items.get(0)
				+ "\"");
		for (int i = 1; i < items.size(); i++) {
			listOfForms.append(", \"").append(items.get(i)).append("\"");
		}
		return listOfForms;
	}
}
