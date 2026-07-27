package com.tawala.persistence;

import java.util.List;

import com.tawala.project.FormSubmission;
import com.tawala.project.Project;

/**
 * Pluggable description of how persistent objects get stored.
 */
public abstract class PersistenceStrategy {
    public abstract <T> List<T> loadAll(Class<T> aClass);

    public abstract <T> T load(Class<T> aClass, String key);

    public abstract void save(Object persistentObject);

    public abstract void delete(Object persistentObject);

    public abstract long size(Object persistentObject);

    abstract public List<FormSubmission> loadResponsesFor(Project project, String formName);
}
