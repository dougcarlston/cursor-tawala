package com.tawala.component.repository;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashSet;
import java.util.LinkedHashSet;
import java.util.Locale;
import java.util.Set;

import org.dom4j.DocumentHelper;
import org.dom4j.Element;
import org.dom4j.QName;

import com.tawala.component.ComponentMetadata;
import com.tawala.message.Message;

public class Category<Type extends ComponentMetadata> {
	private Message name;
	private Set<Type> components = new LinkedHashSet<Type>();
	private Collection<Category> subcategories = new ArrayList<Category>();
	
	public Category(String nameKey, Type[] components) {
		this.name = new Message(nameKey);
		for (Type component : components) {
			addComponent(component);
		}
	}

	public void addSubcategory(Category subCategory) {
		subcategories.add(subCategory);
	}
	
	public String getName(Locale locale) {
		return Repository.getMessageSource().getMessage(name, locale);
	}
	
	public Collection<Type> getComponents() {
		return components;
	}
	
	public void addComponent(Type component) {
		if(! components.add(component)) {
			throw new IllegalStateException("Component " + component + " already exists in the " + this);
		}
	}
	
	@Override
	public String toString() {
		return "Category \"" + getName(Repository.getDefaultLocale()) + "\"";
	}

	public Element toElement(Locale locale) {
		Element result = DocumentHelper.createElement(new QName("category", Repository.NAMESPACE));
		result.addAttribute("name", getName(locale));
		for (Category subcategory : subcategories) {
			result.add(subcategory.toElement(locale));
		}
		
		for (Type component : components) {
			Element componentElement = DocumentHelper.createElement(new QName("element-id", Repository.NAMESPACE));
			componentElement.setText(component.getId());
			result.add(componentElement);
		}

		return result;
	}

	public Set<Type> getAllComponents() {
		if(subcategories.size() == 0) {
			return components;
		}
		
		Set<Type> result = new HashSet<Type>(components);
		for (Category<Type> subcategory : subcategories) {
			result.addAll(subcategory.getAllComponents());
		}
		
		return components;
	}
}
