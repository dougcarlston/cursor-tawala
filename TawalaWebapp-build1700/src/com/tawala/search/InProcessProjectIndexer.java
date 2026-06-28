package com.tawala.search;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collection;
import java.util.HashSet;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

import org.apache.lucene.analysis.Analyzer;
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

import com.tawala.project.library.Category;
import com.tawala.project.library.LibraryProject;

public class InProcessProjectIndexer extends IndexerBase implements ProjectIndexer {
	private static Collection<String> WORDS_REMOVED_FROM_STANDARD_STOP_WORD_SET = new HashSet<String>(
			Arrays.asList(new String[] { "will" }));

	public InProcessProjectIndexer(String directory)
			throws IOException {
		super(directory, createAnalyzer());
	}

	private static Analyzer createAnalyzer() {
		Set<String> stopWords = new HashSet<String>(Arrays
				.asList(StandardAnalyzer.STOP_WORDS));
		stopWords.removeAll(WORDS_REMOVED_FROM_STANDARD_STOP_WORD_SET);

		StandardAnalyzer analyzer = new StandardAnalyzer(stopWords);
		return analyzer;
	}

	public void delete(final long projectId) throws IOException {
		ModificationCommand modificationCommand = new ModificationCommand() {

			public void modify(IndexModifier modifier) throws IOException {
				modifier.deleteDocuments(new Term(
						ProjectIndexer.FIELD_ID, String.valueOf(projectId)));
			}

		};

		performModification(modificationCommand);
	}

	public void index(final LibraryProject project) throws IOException {
		ModificationCommand modificationCommand = new ModificationCommand() {
			public void modify(IndexModifier modifier) throws IOException {
				Document document = createDocumentFromProject(project);
				doIndex(modifier, document);
			}
		};

		performModification(modificationCommand);

	}

	public void index(final Document document) throws IOException {
		ModificationCommand modificationCommand = new ModificationCommand() {
			public void modify(IndexModifier modifier) throws IOException {
				doIndex(modifier, document);
			}
		};

		performModification(modificationCommand);

	}

	@SuppressWarnings("unchecked")
	public List<Long> search(int projectLibraryId, final String queryString)
			throws ParseException, IOException {
		
		final StringBuilder actualQueryString = new StringBuilder(queryString);
		actualQueryString.append(" AND " + ProjectIndexer.FIELD_LIBRARY_ID + ":" + projectLibraryId);

		final List<Long> result = new ArrayList<Long>();

		SearchCommand searchCommand = new SearchCommand() {

			public void search(IndexSearcher searcher) throws ParseException,
					IOException {
				QueryParser parser = new QueryParser(
						ProjectIndexer.FIELD_ALL, getAnalyzer());
				Query query = parser.parse(actualQueryString.toString());

				Hits hits = searcher.search(query);
				if (hits.length() == 0)
					return;

				Iterator<Hit> hitIterator = hits.iterator();
				while (hitIterator.hasNext()) {
					Hit hit = hitIterator.next();
					String projectIdField = hit
							.get(ProjectIndexer.FIELD_ID);
					if (projectIdField == null)
						continue;

					result.add(new Long(projectIdField));
				}

			}

		};

		performSearch(searchCommand);

		return result;
	}

	/**
	 * The approach is to use FIELD_PROJECT_ALL as a single field to be searched
	 * for simple queries. Each field is still indexed separately to allow
	 * looking for very specific terms, namely categories. The disadvantage of
	 * this approach is extra space for indexes and loss of the ranking logic
	 * based on the length of the field being searched. The advantage is the
	 * simplified search process.
	 * 
	 * @param project
	 * @throws IOException
	 */
	public static Document createDocumentFromProject(
			final LibraryProject project) {
		Document document = new Document();
		document.add(new Field(ProjectIndexer.FIELD_ID, String
				.valueOf(project.getId()), Field.Store.YES,
				Field.Index.UN_TOKENIZED));
		document.add(new Field(ProjectIndexer.FIELD_LIBRARY_ID, String
				.valueOf(project.getCategory().getLibrary().getId()), Field.Store.NO,
				Field.Index.UN_TOKENIZED));

		document.add(new Field(ProjectIndexer.FIELD_NAME, project
				.getName(), Field.Store.NO, Field.Index.TOKENIZED));
		document.add(new Field(ProjectIndexer.FIELD_ALL, project
				.getName(), Field.Store.NO, Field.Index.TOKENIZED));

		document.add(new Field(ProjectIndexer.FIELD_SHORT_DESCRIPTION,
				project.getShortDescription(), Field.Store.NO,
				Field.Index.TOKENIZED));
		document.add(new Field(ProjectIndexer.FIELD_ALL, project
				.getShortDescription(), Field.Store.NO, Field.Index.TOKENIZED));

		document.add(new Field(
				ProjectIndexer.FIELD_DETAILED_DESCRIPTION, project
						.getLongDescription(), Field.Store.NO,
				Field.Index.TOKENIZED));
		document.add(new Field(ProjectIndexer.FIELD_ALL, project
				.getLongDescription(), Field.Store.NO, Field.Index.TOKENIZED));

		document
				.add(new Field(ProjectIndexer.FIELD_CATEGORY, project
						.getCategory().getName(), Field.Store.NO,
						Field.Index.TOKENIZED));
		document
				.add(new Field(ProjectIndexer.FIELD_ALL, project
						.getCategory().getName(), Field.Store.NO,
						Field.Index.TOKENIZED));

		document.add(new Field(ProjectIndexer.FIELD_AUTHOR, project
				.getAuthorId(), Field.Store.NO, Field.Index.TOKENIZED));
		document.add(new Field(ProjectIndexer.FIELD_ALL, project
				.getAuthorId(), Field.Store.NO, Field.Index.TOKENIZED));

		for (Category parentCategory : project.getCategory().getAllParents()) {
			document.add(new Field(ProjectIndexer.FIELD_CATEGORY,
					parentCategory.getName(), Field.Store.NO,
					Field.Index.TOKENIZED));
			document.add(new Field(ProjectIndexer.FIELD_ALL,
					parentCategory.getName(), Field.Store.NO,
					Field.Index.TOKENIZED));
		}

		document.add(new Field(ProjectIndexer.FIELD_VETTED, (project
				.isVetted() ? "true" : "false"), Field.Store.NO,
				Field.Index.UN_TOKENIZED));

		document.add(new Field(ProjectIndexer.FIELD_UNDER_CONSTRUCTION, (project
				.isUnderConstruction() ? "true" : "false"), Field.Store.NO,
				Field.Index.UN_TOKENIZED));
		
		return document;
	}

	public static void doIndex(IndexModifier modifier, Document document)
			throws IOException {
		modifier.deleteDocuments(new Term(ProjectIndexer.FIELD_ID,
				document.get(FIELD_ID)));
		modifier.addDocument(document);
	}
}
