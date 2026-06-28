package com.tawala.web.projectmanager;

import java.io.IOException;
import java.io.Writer;
import java.text.SimpleDateFormat;
import java.util.Collection;
import java.util.Iterator;
import java.util.List;

import javax.servlet.http.HttpServletResponse;

import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.Reference;
import com.tawala.project.data.ProjectToExcelExporter;

public class CSVFormDataExportController extends FormDataExportController {

	@Override
	protected void sendData(String formName,
			List<FormSubmission> formResponses, Collection<Reference> fields,
			HttpServletResponse response) throws IOException {
		Writer writer = response.getWriter();
		try {
			doWrite(formResponses, fields, writer);
		} finally {
			writer.close();
		}
	}

	private void doWrite(List<FormSubmission> formResponses,
			Collection<Reference> fields, Writer writer) throws IOException {
		Iterator<Reference> iterator = fields.iterator();
		writer.write(ProjectToExcelExporter.quoteIfRequiredByCSVFormat(iterator
				.next().getFieldName()));
		while (iterator.hasNext()) {
			writer.write(",");
			writer
					.write(ProjectToExcelExporter
							.quoteIfRequiredByCSVFormat(iterator.next()
									.getFieldName()));
		}
		writer.write(",");
		writer.write("Date Created");
		writer.write(",");
		writer.write("Date Updated");
		writer.write("\n");

		SimpleDateFormat dateFormat = new SimpleDateFormat(
				"yyyy/MM/dd HH:mm:ss");

		for (FormSubmission formResponse : formResponses) {
			iterator = fields.iterator();
			writer.write(prepareNextFieldValue(iterator.next(), formResponse));
			while (iterator.hasNext()) {
				writer.write(",");
				writer.write(prepareNextFieldValue(iterator.next(),
						formResponse));
			}
			writer.write(",");
			writer.write(dateFormat.format(formResponse.getCreationDate()));
			writer.write(",");
			writer.write(formResponse.getUpdatedDate() == null ? ""
					: dateFormat.format(formResponse.getUpdatedDate()));
			writer.write("\n");
		}
	}

	private String prepareNextFieldValue(Reference reference,
			FormSubmission formResponse) {
		List<Value> values = formResponse.getValues(reference);
		switch (values.size()) {
		case 0:
			return "";

		case 1:
			return ProjectToExcelExporter.quoteIfRequiredByCSVFormat(values
					.get(0).toString());

		default:
			return ProjectToExcelExporter
					.quoteIfRequiredByCSVFormat(ProjectToExcelExporter
							.createCSVEncodedString(values));
		}
	}

	@Override
	protected String getFileName(String formName) {
		return formName + ".csv";
	}

	@Override
	protected String getContentType() {
		return "application/octet-stream";
	}
}
