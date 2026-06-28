package com.tawala.search;

import java.io.IOException;
import java.util.List;

import org.apache.lucene.queryParser.ParseException;

import com.tawala.domain.User;

public interface UserIndexer {
	public static final String FIELD_USER_ID = "userId";
	public static final String FIELD_USER_NAME = "user-name";
	public static final String FIELD_USER_FIRST_NAME = "user-first-name";
	public static final String FIELD_USER_LAST_NAME = "user-last-name";
	public static final String FIELD_USER_EMAIL = "user-email";
	public static final String FIELD_USER_ALL = "user-all";

	public void index(final User user) throws IOException;

	public void delete(final long userId) throws IOException;

	public List<Long> searchForUser(final String queryString)
			throws ParseException, IOException;
	
	public void destroy();
}
