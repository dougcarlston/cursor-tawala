package com.tawala.project.data;

import java.util.Iterator;
import java.util.List;

public interface DataIterator extends Iterator<List<String>> {
	void close();
}
