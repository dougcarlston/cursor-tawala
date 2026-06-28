package com.tawala.search;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.List;

import org.apache.lucene.document.Document;
import org.apache.lucene.queryParser.ParseException;

import com.tawala.project.library.LibraryProject;
import com.tawala.web.search.SearchURL;

public class ProjectIndexerHttpProxy extends HttpProxySupport implements
		ProjectIndexer {
	private final URL deleteUrl;
	private final URL indexUrl;
	private final URL searchUrl;

	public ProjectIndexerHttpProxy(String urlPrefix)
			throws MalformedURLException {
		deleteUrl = new URL(urlPrefix + SearchURL.DELETE_PROJECT.url());
		indexUrl = new URL(urlPrefix + SearchURL.INDEX_PROJECT.url());
		searchUrl = new URL(urlPrefix + SearchURL.SEARCH_PROJECT.url());
	}

	public void delete(long projectId) throws IOException {
		handlePut(deleteUrl, Long.valueOf(projectId));
	}

	public void destroy() {
		// Do nothing
	}

	public void index(LibraryProject project) throws IOException {
		Document document = InProcessProjectIndexer
				.createDocumentFromProject(project);
		handlePut(indexUrl, document);
	}

	@SuppressWarnings("unchecked")
	public List<Long> search(int projectLibraryId, String queryString)
			throws ParseException, IOException {
		return (List<Long>) handleGet(searchUrl, queryString, Integer.valueOf(projectLibraryId));
	}

}
