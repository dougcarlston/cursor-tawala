package com.scissor.xmlconfig;

import java.lang.reflect.Constructor;
import java.lang.reflect.InvocationTargetException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

import com.scissor.Log;

public class Factory<TYPE> {
    private final List<ConfigMatcher> matchers = new ArrayList<ConfigMatcher>();
    private final Set<String> elementsToIgnore = new HashSet<String>();
    private final Collection<Factory> factoriesToignore = new HashSet<Factory>();
    private Class textClass;
    private boolean keepWhitespace = false;

    public void register(String elementName, Class<? extends TYPE> theClass) {
        checkHasAppropriateConstructor(theClass, ConfigElement.class);
        matchers.add(new ElementNameMatcher(elementName, theClass));
    }

    public void register(String elementName, String requiredAttributeName, Class<? extends TYPE> theClass) {
        checkHasAppropriateConstructor(theClass, ConfigElement.class);
        matchers.add(new ElementAttributeNameMatcher(elementName, requiredAttributeName, theClass));
    }

    public void register(String elementName, String attributeName, String attributeValue, Class<? extends TYPE> theClass) {
        checkHasAppropriateConstructor(theClass, ConfigElement.class);
        matchers.add(new ElementAttributeNameAndValueMatcher(elementName, attributeName, attributeValue, theClass));
	}

    public void registerText(Class theClass) {
        checkHasAppropriateConstructor(theClass, ConfigText.class);
        this.textClass = theClass;
    }

    public void ignore(String elementName) {
        elementsToIgnore.add(elementName);
    }

    public void ignore(Factory factory) {
        factoriesToignore.add(factory);
    }

    public void setKeepWhitespace(boolean keepWhitespace) {
        this.keepWhitespace = keepWhitespace;
    }


    @SuppressWarnings("unchecked")
    public TYPE make(ConfigElement config) {
        if (config == null) throw new NullPointerException("config may not be null");
        if (ignored(config)) {
            return null;
        }
        Class theClass = classToConstruct(config);
        if (theClass == null) {
            Log.warn(this, "No class registered for " + config);
            return null;
        }
        TYPE result;
        try {
            Constructor constructor = constructorFor(theClass, ConfigElement.class);
            result = (TYPE) constructor.newInstance(new Object[]{config});
        } catch (Exception e) {
        	Log.error(this, "Failed to invoke constructor of " + theClass.getName(), e);
            throw new RuntimeException(e);
        }
        return result;
    }

    private boolean ignored(ConfigElement config) {
        if (elementsToIgnore.contains(config.getName())) return true;
        for (Factory pal : factoriesToignore) {
            if (pal.classToConstruct(config) != null) return true;
        }
        return false;
    }

    private Class classToConstruct(ConfigElement config) {
        for (ConfigMatcher configMatcher : matchers) {
            Class result = configMatcher.matchFor(config);
            if (result != null) return result;
        }
        return null;
    }

    @SuppressWarnings("unchecked")
    public TYPE make(ConfigText text) {
        if (text == null) throw new NullPointerException("text may not be null");
        if (textClass == null) {
            throw new IllegalArgumentException("No class registered for text '" + text.text() + "'");
        }
        try {
            Constructor constructor = constructorFor(textClass, ConfigText.class);
            return (TYPE) constructor.newInstance(new Object[]{text});
        } catch (NoSuchMethodException e) {
            throw new RuntimeException(e);
        } catch (IllegalAccessException e) {
            throw new RuntimeException(e);
        } catch (InvocationTargetException e) {
            throw new RuntimeException(e);
        } catch (InstantiationException e) {
            throw new RuntimeException(e);
        }
    }


    public List<TYPE> makeChildren(ConfigElement config) {
        if (config == null) return Collections.emptyList();
        List<ConfigItem> childConfigs = config.children();
        List<TYPE> childObjects = new ArrayList<TYPE>();
        for (ConfigItem childConfig : childConfigs) {
            TYPE childObject = make(childConfig);
            if (childObject != null) childObjects.add(childObject);
        }
        return childObjects;
    }

	public List<TYPE> makeChildren(List<ConfigElement> configElements) {
		if(configElements == null) {
			return Collections.emptyList();
		}
        List<TYPE> childObjects = new ArrayList<TYPE>(configElements.size());
        for (ConfigItem childConfig : configElements) {
            TYPE childObject = make(childConfig);
            if (childObject != null) childObjects.add(childObject);
        }
        return childObjects;
	}

    
    // TODO: This is a sign that typing is wierd.
    public TYPE make(ConfigItem config) {
        TYPE childObject = null;
        if (config instanceof ConfigElement) {
            childObject = make((ConfigElement) config);
        } else {
            ConfigText text = (ConfigText) config;
            if (keepWhitespace || text.isInteresting()) {
                childObject = make(text);
            }
        }
        return childObject;
    }

    @SuppressWarnings("unchecked")
	private Constructor constructorFor(Class type, Class argument) throws NoSuchMethodException {
        return type.getConstructor(new Class[]{argument});
    }

    private void checkHasAppropriateConstructor(Class theClass, Class<? extends ConfigItem> expected) {
        try {
            constructorFor(theClass, expected);
        } catch (NoSuchMethodException e) {
            throw new IllegalArgumentException(theClass + " must have constructor taking " + expected);
        }
    }
}
