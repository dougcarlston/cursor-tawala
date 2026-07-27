package com.tawala.persistence;

/**
 * A collection of persistent objects.
 */
public abstract class PersistentBunch {
    private final PersistenceStrategy persistenceStrategy;

    public PersistentBunch(PersistenceStrategy persistenceStrategy) {
        this.persistenceStrategy = persistenceStrategy;
    }

    protected PersistenceStrategy getPersistenceStrategy() {
        return persistenceStrategy;
    }
}
