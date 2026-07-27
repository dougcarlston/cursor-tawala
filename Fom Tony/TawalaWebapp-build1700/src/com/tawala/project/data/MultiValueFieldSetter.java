package com.tawala.project.data;

import java.io.StringReader;

import com.mindprod.csv.CSVReader;
import com.scissor.Log;
import com.tawala.project.FormSubmission;

public class MultiValueFieldSetter extends BaseFieldSetter {

	private static final char DELIMITER = ',';

	public MultiValueFieldSetter(String fieldName) {
		super(fieldName);
	}

	public void setField(FormSubmission submission, String rawData) {
		CSVReader reader = new CSVReader(new StringReader(rawData), DELIMITER, '"', true /* allow miltiline fields */, true /* trim */);
		try {
			submission.setValues(getFieldName(), reader.getAllFieldsInLine());
		} catch (Exception e) {
			Log.error(this, "Error reading multifield line: ", e);
		}
	}
}
