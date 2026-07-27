package com.tawala.search;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;
import java.util.List;

import org.apache.lucene.analysis.standard.StandardAnalyzer;
import org.apache.lucene.document.Document;
import org.apache.lucene.document.Field;
import org.apache.lucene.index.IndexModifier;
import org.apache.lucene.index.Term;
import org.apache.lucene.queryParser.ParseException;
import org.apache.lucene.queryParser.QueryParser;
import org.apache.lucene.search.Hit;
import org.apache.lucene.search.Hits;
import org.apache.lucene.search.IndexSearcher;
import org.apache.lucene.search.Query;

import com.tawala.domain.User;

public class InProcessUserIndexer extends IndexerBase implements UserIndexer {
	public InProcessUserIndexer(String directory) throws IOException {
		super(directory, new StandardAnalyzer(Collections.EMPTY_SET));
	}

	public void index(final User user) throws IOException {
		ModificationCommand modificationCommand = new ModificationCommand() {
			public void modify(IndexModifier modifier) throws IOException {
				Document document = createDocument(user);
				doIndex(modifier, document);
			}
		};

		performModification(modificationCommand);
	}

	public static void doIndex(IndexModifier modifier, Document document)
			throws IOException {
		modifier.deleteDocuments(new Term(UserIndexer.FIELD_USER_ID, document
				.get(FIELD_USER_ID)));
		modifier.addDocument(document);
	}

	public void index(final Document document) throws IOException {
		ModificationCommand modificationCommand = new ModificationCommand() {
			public void modify(IndexModifier modifier) throws IOException {
				doIndex(modifier, document);
			}
		};

		performModification(modificationCommand);
	}

	public void delete(final long userId) throws IOException {
		ModificationCommand modificationCommand = new ModificationCommand() {
			public void modify(IndexModifier modifier) throws IOException {
				modifier.deleteDocuments(new Term(UserIndexer.FIELD_USER_ID,
						String.valueOf(userId)));
			}
		};

		performModification(modificationCommand);
	}

	@SuppressWarnings("unchecked")
	public List<Long> searchForUser(final String queryString)
			throws ParseException, IOException {

		final List<Long> result = new ArrayList<Long>();

		SearchCommand searchCommand = new SearchCommand() {
			public void search(IndexSearcher searcher) throws ParseException,
					IOException {
				QueryParser parser = new QueryParser(
						UserIndexer.FIELD_USER_ALL, getAnalyzer());
				Query query = parser.parse(queryString);

				Hits hits = searcher.search(query);
				if (hits.length() == 0)
					return;

				Iterator<Hit> hitIterator = hits.iterator();
				while (hitIterator.hasNext()) {
					Hit hit = hitIterator.next();
					String userIdField = hit.get(UserIndexer.FIELD_USER_ID);
					if (userIdField == null)
						continue;

					result.add(new Long(userIdField));
				}
			}
		};

		performSearch(searchCommand);

		return result;
	}

	public static Document createDocument(final User user) {
		Document document = new Document();
		document.add(new Field(UserIndexer.FIELD_USER_ID, String.valueOf(user
				.getDatabaseId()), Field.Store.YES, Field.Index.UN_TOKENIZED));

		document.add(new Field(UserIndexer.FIELD_USER_NAME, user.getId(),
				Field.Store.NO, Field.Index.TOKENIZED));
		document.add(new Field(UserIndexer.FIELD_USER_ALL, user.getId(),
				Field.Store.NO, Field.Index.TOKENIZED));

		if (user.getFirstName() != null) {
			document.add(new Field(UserIndexer.FIELD_USER_FIRST_NAME, user
					.getFirstName(), Field.Store.NO, Field.Index.TOKENIZED));
			document.add(new Field(UserIndexer.FIELD_USER_ALL, user
					.getFirstName(), Field.Store.NO, Field.Index.TOKENIZED));
		}

		if (user.getLastName() != null) {
			document.add(new Field(UserIndexer.FIELD_USER_LAST_NAME, user
					.getLastName(), Field.Store.NO, Field.Index.TOKENIZED));
			document.add(new Field(UserIndexer.FIELD_USER_ALL, user
					.getLastName(), Field.Store.NO, Field.Index.TOKENIZED));
		}

		if (user.getEmail() != null) {
			document.add(new Field(UserIndexer.FIELD_USER_EMAIL, user
					.getEmail().toString(), Field.Store.NO,
					Field.Index.TOKENIZED));
			document.add(new Field(UserIndexer.FIELD_USER_ALL, user.getEmail()
					.toString(), Field.Store.NO, Field.Index.TOKENIZED));
		}
		return document;
	}
}
