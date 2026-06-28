package com.tawala.persistence;

import java.util.ArrayList;
import java.util.Collection;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import com.tawala.project.FormSubmission;
import com.tawala.project.Project;

public class InMemoryPersistenceStrategy extends PersistenceStrategy {
    private Map<Class, Map<Object, Object>> containers = new LinkedHashMap<Class, Map<Object, Object>>();

    @SuppressWarnings( { "unchecked" })
    public <T> List<T> loadAll(Class<T> aClass) {
        return new ArrayList<T>((Collection<? extends T>) containerFor(aClass)
                .values());
    }

    public <T> T load(Class<T> aClass, String key) {
        return null;
    }

    @Override
    public void save(Object persistentObject) {
        containerFor(persistentObject.getClass()).put(idFor(persistentObject), persistentObject);
    }

    private Object idFor(Object persistentObject) {
        return persistentObject.hashCode();
    }

    public void delete(Object persistentObject) {
        containerFor(persistentObject.getClass()).remove(idFor(persistentObject));
    }

    // TODO: is there a way to figure out the memory used by a persistentObject?
    public long size(Object persistentObject) {
        return 0;
    }

    private <T> Map<Object, Object> containerFor(Class<T> theType) {
        if (!containers.containsKey(theType))
            containers.put(theType, new LinkedHashMap<Object, Object>());
        return containers.get(theType);
    }

    /*
     * @see com.tawala.persistence.PersistenceStrategy#loadResponsesFor(com.tawala.domain.User,
     *      com.tawala.project.Project, java.lang.String)
     */
    @Override
    public List<FormSubmission> loadResponsesFor(Project project, String formName) {
        List<FormSubmission> result = new ArrayList<FormSubmission>();
        Map<Object, Object> formSubmissionContainer = containerFor(FormSubmission.class);
        
        for (Object nextObject : formSubmissionContainer.values()) {
            FormSubmission submission = (FormSubmission)nextObject;
            if(submission.getProject().getId() == project.getId() && submission.getFormName().equals(formName)) {
                result.add(submission);
            }
        }

        return result;
    }
}
