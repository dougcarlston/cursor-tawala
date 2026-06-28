package com.tawala.search;

import java.io.IOException;
import java.util.List;

import org.apache.lucene.queryParser.ParseException;

import com.tawala.project.library.LibraryProject;

public interface ProjectIndexer {
	public static final String FIELD_ID = "projectId";
	public static final String FIELD_LIBRARY_ID = "libraryId";
	public static final String FIELD_NAME = "name";
	public static final String FIELD_SHORT_DESCRIPTION = "short-description";
	public static final String FIELD_DETAILED_DESCRIPTION = "detailed-description";
	public static final String FIELD_CATEGORY = "category";
	public static final String FIELD_AUTHOR = "author";
	public static final String FIELD_VETTED = "vetted";
	public static final String FIELD_UNDER_CONSTRUCTION = "under-construction";
	public static final String FIELD_ALL = "all";

	public void delete(final long projectId) throws IOException;

	public void index(final LibraryProject project) throws IOException;

	public List<Long> search(final int projectLibraryId, final String queryString)
			throws ParseException, IOException;

	public void destroy();
}
