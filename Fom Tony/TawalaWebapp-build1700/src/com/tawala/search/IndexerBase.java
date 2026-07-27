package com.tawala.search;

import java.io.File;
import java.io.FilenameFilter;
import java.io.IOException;
import java.util.concurrent.locks.ReentrantReadWriteLock;
import java.util.concurrent.locks.ReentrantReadWriteLock.ReadLock;
import java.util.concurrent.locks.ReentrantReadWriteLock.WriteLock;

import org.apache.lucene.analysis.Analyzer;
import org.apache.lucene.index.IndexModifier;
import org.apache.lucene.index.IndexWriter;
import org.apache.lucene.queryParser.ParseException;
import org.apache.lucene.search.IndexSearcher;

import com.scissor.Log;

abstract public class IndexerBase {
    private IndexSearcher searcher;
    private IndexModifier modifier;
    private Analyzer analyzer;
    private String directory = null;

    private final ReentrantReadWriteLock readWriteLock = new ReentrantReadWriteLock();
    private final ReadLock readLock = readWriteLock.readLock();
    private final WriteLock writeLock = readWriteLock.writeLock();

    protected IndexerBase(String directory, Analyzer analyzer) throws IOException {
        this.directory = directory;
        boolean createNewIndexes = prepareDirectory(directory);
        this.modifier = new IndexModifier(directory, analyzer, createNewIndexes);
        this.searcher = new IndexSearcher(directory);
        this.analyzer = analyzer;
    }

    private static boolean prepareDirectory(String fileName) {
        File directory = new File(fileName);
        boolean buildNewIndexes = false;
        if (directory.exists()) {
            clearLocks();
        }
        else {
            if (!directory.mkdirs()){
                Log.error(Indexers.class, "Failed to create directory "
                        + fileName );
                throw new RuntimeException("Failed to create directory "
                        + fileName);
            }
            buildNewIndexes = true;
            
        }
        
        return buildNewIndexes;
    }
    
    private static void clearLocks() {
        String tempDirectory = System.getProperty("java.io.tmpdir");
        if (tempDirectory == null)
            return;

        File[] lockFiles = new File(tempDirectory)
                .listFiles(new FilenameFilter() {

                    public boolean accept(File dir, String name) {
                        return name.matches("lucene-.*("
                                + IndexWriter.WRITE_LOCK_NAME + "|"
                                + IndexWriter.COMMIT_LOCK_NAME + ")");
                    }

                });
        
        for (int i = 0; i < lockFiles.length; i++) {
            lockFiles[i].delete();
        }
    }


    public void destroy() {
        writeLock.lock();
        try {
            if (modifier != null) {
                try {
                    modifier.close();
                    modifier = null;
                } catch (Throwable e) {
                    Log.warn(IndexerBase.class, "Error closing modifier:", e);
                }
            }

            if (searcher != null) {
                try {
                    searcher.close();
                    searcher = null;
                } catch (Throwable e) {
                    Log.warn(IndexerBase.class, "Error closing searcher:", e);
                }
            }
        } finally {
            writeLock.unlock();
        }
    }

    protected void performSearch(SearchCommand searchCommand) throws ParseException, IOException {
        readLock.lock();
        try {
            searchCommand.search(getSearcher());
        } finally {
            readLock.unlock();
        }
    }

    protected void performModification(ModificationCommand modificationCommand)
            throws IOException {
        writeLock.lock();
        try {
            modificationCommand.modify(modifier);
            
            modifier.optimize();
            modifier.flush();

            if (searcher != null) {
                searcher.close();
            }
            searcher = new IndexSearcher(directory);
        } finally {
            writeLock.unlock();
        }
    }

    private IndexSearcher getSearcher() {
        if (searcher == null)
            throw new IllegalStateException("Searcher is not instantiated.");
        return searcher;
    }

    /**
     * @return Returns the analyzer.
     */
    public Analyzer getAnalyzer() {
        return analyzer;
    }
}
