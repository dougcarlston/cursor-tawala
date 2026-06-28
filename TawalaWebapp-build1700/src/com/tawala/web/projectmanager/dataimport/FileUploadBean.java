/**
 * 
 */
package com.tawala.web.projectmanager.dataimport;

import java.io.IOException;
import java.util.Collection;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.springframework.web.multipart.MultipartFile;

import com.tawala.project.data.DataIterator;
import com.tawala.project.data.ExcelDataIterator;
import com.tawala.project.data.StoredField;

public abstract class FileUploadBean {
	private MultipartFile data;
	private int columnCount;
	private boolean skipFirstRow;
	private boolean deleteOldData;
	private boolean firstRowIsLikelyHeaders;
	private List<List<String>> sampleData;
	private Map<String, String> mapping = new LinkedHashMap<String, String>();
	private Collection<StoredField> fields;
	private boolean excelSpreadsheet = true;

	public boolean isExcelSpreadsheet() {
		return excelSpreadsheet;
	}

	public void setExcelSpreadsheet(boolean excelSpreadsheet) {
		this.excelSpreadsheet = excelSpreadsheet;
	}

	public Collection<StoredField> getFields() {
		return fields;
	}

	public void setFields(Collection<StoredField> fields) {
		this.fields = fields;
	}

	/**
	 * @return Returns the mapping.
	 */
	public Map<String, String> getMapping() {
		return mapping;
	}

	/**
	 * @param mapping
	 *            The mapping to set.
	 */
	public void setMapping(Map<String, String> mapping) {
		this.mapping = mapping;
	}

	public int getColumnCount() {
		return columnCount;
	}

	public void setColumnCount(int columnCount) {
		this.columnCount = columnCount;
	}

	public List<List<String>> getSampleData() {
		return sampleData;
	}

	public void setSampleData(List<List<String>> sampleData) {
		this.sampleData = sampleData;
	}

	public void setData(MultipartFile data) {
		this.data = data;
	}

	public abstract String getDataSourceDescription();

	public abstract String getReturnURL();

	public boolean isSkipFirstRow() {
		return skipFirstRow;
	}

	public void setSkipFirstRow(boolean skipFirstRow) {
		this.skipFirstRow = skipFirstRow;
	}

	public boolean isDeleteOldData() {
		return deleteOldData;
	}

	public void setDeleteOldData(boolean deleteOldData) {
		this.deleteOldData = deleteOldData;
	}

	public boolean isFirstRowIsLikelyHeaders() {
		return firstRowIsLikelyHeaders;
	}

	public void setFirstRowIsLikelyHeaders(boolean firstRowIsLikelyHeaders) {
		this.firstRowIsLikelyHeaders = firstRowIsLikelyHeaders;
	}

	public boolean isEmptyFile() throws IOException {
		return data == null || data.getBytes().length == 0;
	}

	public DataIterator getDataIterator() {
		try {
			if(excelSpreadsheet) {
				return new ExcelDataIterator(data.getInputStream());
			} else {
				return new CSVDataIterator(data.getInputStream());
			}
		} catch (IOException e) {
			throw new RuntimeException(
					"Unable to get uploaded file input stream:", e);
		}
	}

	public MultipartFile getData() {
		return data;
	}
	
	public HSSFWorkbook getWorkbook() throws IOException {
		return new HSSFWorkbook(data.getInputStream());
	}

	public void countColumns() {
		this.columnCount = 1;
		DataIterator dataIterator = getDataIterator();
		try {
			while (dataIterator.hasNext()) {
				List<String> nextRow = dataIterator.next();
				for (int i = nextRow.size() - 1; i > columnCount && i >=0; i--) {
					String value = nextRow.get(i);
					if(value.length() > 0) {
						columnCount = i + 1;
					}
				}
			}
		} finally {
			dataIterator.close();
		}
	}
}