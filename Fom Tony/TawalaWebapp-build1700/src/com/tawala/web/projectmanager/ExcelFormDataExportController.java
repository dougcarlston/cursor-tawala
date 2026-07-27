package com.tawala.web.projectmanager;

import java.io.IOException;
import java.util.Collection;
import java.util.List;

import javax.servlet.http.HttpServletResponse;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.tawala.project.FormSubmission;
import com.tawala.project.commands.Reference;
import com.tawala.project.data.ProjectToExcelExporter;

public class ExcelFormDataExportController extends FormDataExportController {

	@Override
	protected void sendData(String formName,
			List<FormSubmission> formResponses, Collection<Reference> fields,
			HttpServletResponse response) throws IOException {

		ProjectToExcelExporter exporter = new ProjectToExcelExporter();
		exporter.addFormDataSheet(formName, formResponses, fields);
		HSSFWorkbook workbook = exporter.getWorkBook();
		workbook.write(response.getOutputStream());
	}

	@Override
	protected String getFileName(String formName) {
		return formName + ".xls";
	}

	@Override
	protected String getContentType() {
		return "application/vnd.ms-excel";
	}
}
