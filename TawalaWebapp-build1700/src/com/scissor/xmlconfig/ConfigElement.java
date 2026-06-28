package com.scissor.xmlconfig;

import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.io.InputStream;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;

import org.dom4j.Attribute;
import org.dom4j.CDATA;
import org.dom4j.Document;
import org.dom4j.DocumentException;
import org.dom4j.Element;
import org.dom4j.Text;
import org.dom4j.io.SAXReader;

import com.scissor.Log;

/**
 * Wraps an XML Element with convenience methods for object configuration.
 */
public class ConfigElement extends ConfigItem {
	private final String name;

	private final LinkedHashMap<String, ConfigAttribute> attributes = new LinkedHashMap<String, ConfigAttribute>();

	private final List<ConfigItem> contents = new ArrayList<ConfigItem>();

	private final Element rawElement;

	private final Format format;

	public ConfigElement(Element element) {
		this(ROOT, element);
	}

	public ConfigElement(String xml) {
		this(ROOT, parseXmlDocument(xml).getRootElement());
	}

	public ConfigElement(InputStream stream) {
		this(ROOT, parseXmlDocument(stream).getRootElement());
	}

	@SuppressWarnings("unchecked")
	public ConfigElement(ConfigItem parent, Element element) {
		super(parent);
		this.rawElement = element;
		this.name = element.getName();
		List<Attribute> list = element.attributes();
		for (Attribute attrib : list) {
			attributes.put(attrib.getName(), new ConfigAttribute(this, attrib));
		}
		List elementContents = element.content();
		for (Object o : elementContents) {
			if (o instanceof Element) {
				contents.add(new ConfigElement(this, (Element) o));
			} else if (o instanceof Text) {
				Text text = (Text) o;
				ConfigText configText = new ConfigText(this, text);
				if (isLame(text))
					configText.markUsed(); // TODO: this is a hack. Is the
											// right solution to make literal
											// doucment text into elements?
				contents.add(configText);
			} else if (o instanceof CDATA) {
				contents.add(new ConfigCDATA(this, (CDATA) o));
			} else {
				System.err.println("Unexpected object " + o);
			}
		}

		if (hasAttribute("format")) {
			this.format = new Format(attribute("format").stringValue());
		} else {
			this.format = null;
		}

	}

	public ConfigElement(File content) throws IOException {
		this(readFile(content));
	}

	private boolean isLame(Text textObject) {
		if (textObject == null)
			return true;
		String text = textObject.getText();
		if (text == null || "".equals(text))
			return true;
		return text.matches("\n\\s*");
	}

	public ConfigAttribute attribute(String name) {
		markUsed();
		ConfigAttribute attribute = attributes.get(name);
		if (attribute == null)
			return new NullAttribute(this, name);
		return attribute;

	}

	public boolean hasAttribute(String name) {
		return attributes.containsKey(name);
	}

	public String getName() {
		return name;
	}

	public List<ConfigItem> getUnusedItems() {
		if (allChildrenUnused() && !isUsed()) {
			return Collections.singletonList((ConfigItem) this);
		}
		List<ConfigItem> result = new ArrayList<ConfigItem>();
		for (ConfigAttribute attribute : attributes.values()) {
			if (!attribute.isUsed())
				result.add(attribute);
		}

		for (ConfigItem configItem : contents) {
			result.addAll(configItem.getUnusedItems());
		}
		return result;

	}

	private boolean allChildrenUnused() {
		for (ConfigAttribute attribute : attributes.values()) {
			if (attribute.isUsed())
				return false;
		}
		return true;
	}

	public List<ConfigItem> children() {
		markUsed();
		return Collections.unmodifiableList(contents);
	}

	public ConfigElement childElement(int index) {
		markUsed();
		int pos = -1;
		for (ConfigItem configItem : contents) {
			if (configItem instanceof ConfigElement) {
				pos++;
				if (index == pos)
					return (ConfigElement) configItem;
			}
		}
		throw new ArrayIndexOutOfBoundsException("index " + index
				+ " is higher than max " + pos);
	}

	public ConfigItem child(int index) {
		markUsed();
		return contents.get(index);
	}

	public ConfigItem childItem(String name) {
		markUsed();
		for (ConfigItem configItem : contents) {
			if (configItem.getName().equals(name)) {
				return configItem;
			}
		}
		return null;
	}

	public boolean hasChild(String name) {
		return child(name) != null;
	}

	public ConfigElement child(String name) {
		for (ConfigElement configElement : childElements()) {
			if (configElement.getName().equals(name)) {
				configElement.markUsed();
				return configElement;
			}
		}
		return null;
	}

	public Element getRawElement() {
		markUsed();
		return rawElement;
	}

	public List<ConfigElement> children(String name) {
		List<ConfigElement> result = new ArrayList<ConfigElement>();
		for (ConfigElement configElement : childElements()) {
			if (configElement.getName().equals(name))
				result.add(configElement);
		}
		return result;
	}

	public List<ConfigElement> childElements() {
		markUsed();
		List<ConfigElement> result = new ArrayList<ConfigElement>();
		for (ConfigItem configItem : contents) {
			if (configItem instanceof ConfigElement) {
				result.add((ConfigElement) configItem);
			}
		}
		return result;

	}

	public void dumpUnused() {
		List<ConfigItem> unusedItems = getUnusedItems();
		for (ConfigItem configItem : unusedItems) {
			Log.warn(this, "unused: " + configItem);
		}

	}

	public String text() {
		markUsed();
		StringBuffer result = new StringBuffer();
		for (ConfigItem item : contents) {
			result.append(item.text());
		}
		return result.toString();
	}

	// TODO: this doesn't seem quite right
	public String firstText() {
		for (ConfigItem item : contents) {
			if (item instanceof ConfigText)
				return ((ConfigText) item).text();
		}
		return "";
	}

	public Format format() {
		if (format != null) {
			return format;
		} else {
			return super.format();
		}
	}

	public boolean hasUnused() {
		return getUnusedItems().size() > 0;
	}

	private static String readFile(File file) throws IOException {
		StringBuffer result = new StringBuffer();
		FileReader reader = new FileReader(file);
		try {
			int read;
			char[] cbuf = new char[4096];
			while ((read = reader.read(cbuf)) > 0) {
				result.append(cbuf, 0, read);
			}
		} finally {
			reader.close();
		}

		return result.toString();
	}

	private static class NullAttribute extends ConfigAttribute {
		public NullAttribute(ConfigElement config, String name) {
			super(config, name, null);
		}
	}

	private static Document parseXmlDocument(String xml) {
		try {
			SAXReader saxReader = new SAXReader();
			saxReader.setMergeAdjacentText(true);
			return saxReader.read(new StringReader(xml));

		} catch (DocumentException e) {
			throw new RuntimeException("unexpected failure parsing XML", e);
		}
	}
	
	private static Document parseXmlDocument(InputStream xml) {
		try {
			SAXReader saxReader = new SAXReader();
			saxReader.setMergeAdjacentText(true);
			return saxReader.read(new NonClosingInputStream(xml));

		} catch (DocumentException e) {
			throw new RuntimeException("unexpected failure parsing XML", e);
		}
	}
	
	private static class NonClosingInputStream extends InputStream {
		private final InputStream underlyingStream;
		
		public NonClosingInputStream(InputStream stream) {
			this.underlyingStream = stream;
		}
		
		@Override
		public int read() throws IOException {
			return underlyingStream.read();
		}
		
		@Override
		public void close() throws IOException {
			//--- Do nothing. This is used to override the default behavior of closing the stream.
		}
	}
}
