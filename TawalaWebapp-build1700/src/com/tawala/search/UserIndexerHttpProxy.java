package com.tawala.search;

import java.io.IOException;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.List;

import org.apache.lucene.document.Document;
import org.apache.lucene.queryParser.ParseException;

import com.tawala.domain.User;
import com.tawala.web.search.SearchURL;

public class UserIndexerHttpProxy extends HttpProxySupport implements UserIndexer {
	private final URL deleteUrl;
	private final URL searchUrl;
	private final URL indexUrl;
	public UserIndexerHttpProxy(String urlPrefix) throws MalformedURLException {
		this.deleteUrl = new URL(urlPrefix + SearchURL.DELETE_USER.url());
		this.searchUrl = new URL(urlPrefix + SearchURL.SEARCH_USER.url());
		this.indexUrl = new URL(urlPrefix + SearchURL.INDEX_USER.url());
	}

	public void delete(long userId) throws IOException {
		handlePut(deleteUrl, Long.valueOf(userId));
	}

	public void destroy() {
		// Do nothing
	}

	public void index(User user) throws IOException {
		Document userDocument = InProcessUserIndexer.createDocument(user);
		handlePut(indexUrl, userDocument);
		
	}

	@SuppressWarnings("unchecked")
	public List<Long> searchForUser(String queryString) throws ParseException,
			IOException {
		return (List<Long>)handleGet(searchUrl, queryString);
	}
}
