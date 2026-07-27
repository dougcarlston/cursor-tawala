package com.tawala.component.repository;

import java.io.File;
import java.io.IOException;
import java.io.StringReader;

import javax.xml.XMLConstants;
import javax.xml.transform.stream.StreamSource;
import javax.xml.validation.Schema;
import javax.xml.validation.SchemaFactory;
import javax.xml.validation.Validator;

import junit.framework.TestCase;

import org.springframework.beans.factory.xml.XmlBeanFactory;
import org.springframework.core.io.FileSystemResource;
import org.springframework.core.io.Resource;
import org.xml.sax.SAXException;

public class RepositoryTest extends TestCase {

	@Override
	protected void setUp() throws Exception {
		super.setUp();

		Resource res = new FileSystemResource(
				"web/WEB-INF/component-config.xml");
		XmlBeanFactory factory = new XmlBeanFactory(res);
		factory.preInstantiateSingletons();
	}

	public void testBuildingXMLPresentation() throws Exception {
		String xml = Repository.getXMLPresentation();
		assertNotNull(xml);
		
		assertCorrectRepositoryXML(xml);
	}

	public static void assertCorrectRepositoryXML(String xml) throws SAXException, IOException {
		Schema schema = compileSchema("web/WEB-INF/xmlschema/component-repository.xsd");
		Validator validator = schema.newValidator();
		validator.validate(new StreamSource(new StringReader(xml)));
	}

	private static Schema compileSchema(String schema) throws SAXException {
		SchemaFactory sf = SchemaFactory
				.newInstance(XMLConstants.W3C_XML_SCHEMA_NS_URI);
		return sf.newSchema(new File(schema));
	}
}
