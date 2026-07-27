package com.tawala.web.projectmanager.dataimport;

import java.io.EOFException;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.List;

import com.mindprod.csv.CSVReader;
import com.tawala.project.data.DataIterator;

public class CSVDataIterator implements DataIterator {
	private CSVReader csvReader;
	private List<String> nextRow;
	private boolean endOfFile;

	public CSVDataIterator(InputStream stream) {
		csvReader = new CSVReader(new InputStreamReader(stream), ',', '"',
				true, false);
	}

	public boolean hasNext() {
		if (endOfFile) {
			return false;
		}
		if (nextRow == null) {
			readRow();
		}
		return !endOfFile;
	}

	private void readRow() {
		try {
			nextRow = csvReader.getAllFieldsInLine();
		} catch (EOFException e) {
			nextRow = null;
			endOfFile = true;
		} catch (IOException e) {
			throw new RuntimeException("Error reading the CSV data: ", e);
		}
	}

	public List<String> next() {
		if (nextRow == null) {
			throw new IllegalStateException(
					"Attempt to read next row without issuing hasNext() first.");
		}

		List<String> result = nextRow;
		nextRow = null;
		return result;
	}

	public void remove() {
		throw new IllegalStateException("remove() is not supported");
	}

	public void close() {
		try {
			csvReader.close();
		} catch (IOException e) {
			throw new RuntimeException("Error closing imported data input stream.", e);
		}
	}
}
