package com.tawala.project.builder;

import java.util.ArrayList;
import java.util.Collection;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import com.scissor.XmlBuffer;
import com.tawala.component.ComponentMetadata;
import com.tawala.component.Parameter;
import com.tawala.project.commands.RecordSelector;

public class ComponentBuilder extends TagBuilder {
	private final ComponentMetadata webComponent;
	private final Integer versionNumber;
	private Map<String, Collection<Object>> parameters = new LinkedHashMap<String, Collection<Object>>();

	public ComponentBuilder(ComponentMetadata component,
			String... parametersAndValues) {
		this(component, component.getVersion(), parametersAndValues);
	}
	public ComponentBuilder(ComponentMetadata component, Integer versionNumber,
			String... parametersAndValues) {
		this.webComponent = component;
		this.versionNumber = versionNumber;

		if (parametersAndValues.length % 2 == 1) {
			throw new IllegalArgumentException(
					"The number of arguments in parameters and values array should be even");
		}

		for (int i = 0; i < parametersAndValues.length; i += 2) {
			addParameter(parametersAndValues[i], parametersAndValues[i + 1]);
		}
	}

	public void addTextParameter(String name, String... values) {
		addParameter(name, (Object[]) values);
	}

	public void addPreformattedParameter(String name, String... values) {
		for (String value : values) {
			XmlBuffer xmlBuffer = new XmlBuffer();
			xmlBuffer.preformattedXml(value);
			addParameter(name, xmlBuffer);
		}
	}

	private void addParameter(String name, Object... values) {
		validateParameter(name);

		Collection<Object> previousValues = parameters.get(name);
		if (previousValues == null) {
			previousValues = new ArrayList<Object>();
			parameters.put(name, previousValues);
		}

		for (int i = 0; i < values.length; i++) {
			previousValues.add(values[i]);
		}
	}

	public void addConditionsParameter(String name, List<Object[]> formNames,
			String conditions) {
		XmlBuffer parameter = new XmlBuffer();
		for (Object[] formInfo : formNames) {
			if ((Boolean) formInfo[1]) {
				parameter.tag("form", "name", (String) formInfo[0],
						RecordSelector.EXTERNAL_SHARED_DATA_ATTRIBUTE_NAME,
						"true");

			} else {
				parameter.tag("form", "name", (String) formInfo[0]);
			}
		}
		if (conditions != null) {
			parameter.startTag("conditions");
			parameter.preformattedXml(conditions);
			parameter.endTag("conditions");
		}

		addParameter(name, parameter);
	}

	private void validateParameter(String name) {
		boolean parameterFound = false;
		for (Parameter parameter : webComponent.getParameters()) {
			if (parameter.getId().equals(name)) {
				parameterFound = true;
			}
		}
		if (!parameterFound) {
			throw new IllegalArgumentException("Parameter '" + name
					+ "' is not valid.");
		}
	}

	protected void endTag(XmlBuffer xml) {
		xml.endTag(webComponent.getId(), false);
	}

	protected void startTag(XmlBuffer xml) {
		xml.startTag(webComponent.getId(), false, "version", String
				.valueOf(versionNumber));
		for (Map.Entry<String, Collection<Object>> parameterEntry : parameters
				.entrySet()) {
			for (Object value : parameterEntry.getValue()) {
				xml.startTag(parameterEntry.getKey(), false);
				if (value.getClass() == String.class) {
					xml.text((String) value);
				} else if (value.getClass() == XmlBuffer.class) {
					xml.append((XmlBuffer) value);
				} else {
					throw new IllegalStateException("Unexpected class "
							+ value.getClass());
				}
				xml.endTag(parameterEntry.getKey());
			}
		}
	}
}
