package com.tawala.project.data;

import java.util.ArrayList;
import java.util.List;

public class ImportStatistics {
	private int addedRecords = 0;
	private List<Integer> skippedRows = new ArrayList<Integer>();
	private List<Integer> emptyRows = new ArrayList<Integer>();

	public void incrementAddedRecordCount() {
		addedRecords++;
	}
	
	public void skippedRow(int rowNumber) {
		skippedRows.add(rowNumber);
	}

	public int getAddedRecords() {
		return addedRecords;
	}

	public List<Integer> getSkippedRows() {
		return skippedRows;
	}

	public void emptyRow(int currentRowNumber) {
		emptyRows.add(currentRowNumber);
	}
	
	public List<Integer> getEmptyRows() {
		return emptyRows;
	}
}
