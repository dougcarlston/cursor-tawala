package com.tawala.search;

import java.io.IOException;

import org.apache.lucene.index.IndexModifier;

public interface ModificationCommand {
    void modify(IndexModifier indexModifier) throws IOException;
}
