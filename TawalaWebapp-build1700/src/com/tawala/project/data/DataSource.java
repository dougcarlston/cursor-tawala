package com.tawala.project.data;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

import org.dom4j.Element;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;

public class DataSource {
	private static final Factory<StoredField> FIELD_FACTORY = new Factory<StoredField>();
	static {
		FIELD_FACTORY.register(StoredField.ELEMENT_NAME, StoredField.TYPE_ATTRIBUTE_NAME, MultiChoiceField.TYPE, MultiChoiceField.class);
		FIELD_FACTORY.register(StoredField.ELEMENT_NAME, StoredField.TYPE_ATTRIBUTE_NAME, StringField.TYPE, StringField.class);
	}
	
	private final String name;
	private List<StoredField> fields = new ArrayList<StoredField>();
	
	public DataSource(String name) {
		this.name = name;
	}
	
	public DataSource(ConfigElement configElement) {
		this.name = configElement.attribute("name").stringValue();
		fields = FIELD_FACTORY.makeChildren(configElement);
	}

	public String getName() {
		return name;
	}
	
	public void addField(StoredField field) {
		fields.add(field);
	}
	
	public StoredField getFieldById(String fieldId) {
		for (StoredField field : fields) {
			if(field.getName().equals(fieldId)) {
				return field;
			}
		}
		return null;
	}
	
	public Collection<StoredField> getFields() {
		return fields;
	}

	public void toXml(Element rootElement) {
		Element dataSourceElement = rootElement.addElement("datasource");
		dataSourceElement.addAttribute("name", name);
		
		for (StoredField field : fields) {
			field.toXml(dataSourceElement);
		}
	}

	@Override
	public int hashCode() {
		final int PRIME = 31;
		int result = 1;
		result = PRIME * result + ((fields == null) ? 0 : fields.hashCode());
		result = PRIME * result + ((name == null) ? 0 : name.hashCode());
		return result;
	}

	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		final DataSource other = (DataSource) obj;
		if (name == null) {
			if (other.name != null)
				return false;
		} else if (!name.equals(other.name))
			return false;	if (fields == null) {
			if (other.fields != null)
				return false;
		} else if (!fields.equals(other.fields))
			return false;
	
		return true;
	}
}
