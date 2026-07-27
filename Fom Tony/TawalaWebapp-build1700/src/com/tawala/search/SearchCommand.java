package com.tawala.search;

import java.io.IOException;

import org.apache.lucene.queryParser.ParseException;
import org.apache.lucene.search.IndexSearcher;

public interface SearchCommand {
    void search(IndexSearcher indexSearcher) throws ParseException, IOException;
}
