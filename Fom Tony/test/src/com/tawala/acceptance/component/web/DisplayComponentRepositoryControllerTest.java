package com.tawala.acceptance.component.web;

import java.io.IOException;

import org.dom4j.Document;
import org.dom4j.DocumentException;
import org.dom4j.DocumentHelper;
import org.xml.sax.SAXException;

import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.repository.Repository;
import com.tawala.component.repository.RepositoryTest;
import com.tawala.web.admin.DisplayComponentRepositoryController;
import com.tawala.web.controller.WellKnown;

public class DisplayComponentRepositoryControllerTest extends
		AcceptanceTestCase {

	public void testNoSignatureArgument() throws Exception {
		bot.go(WellKnown.urls.getDisplayComponentRepository());
		assertEquals("text/xml", bot.getContentType());
		RepositoryTest.assertCorrectRepositoryXML(bot.getPageText());
	}

	public void testMatchingSignature() throws RobotException,
			DocumentException {
		bot.go(WellKnown.urls.getDisplayComponentRepository() + "?"
				+ DisplayComponentRepositoryController.SIGNATURE_PARAMETER
				+ "=" + Repository.getSignature(Repository.DEFAULT_LOCALE));
		assertEquals("text/xml", bot.getContentType());
		Document document = DocumentHelper.parseText(bot.getPageText());

		assertEquals(Repository.REPOSITORY_IS_CURRENT_ROOT_ELEMENT_NAME,
				document.getRootElement().getName());
	}

	public void testNonMatchingSignature() throws RobotException, SAXException,
			IOException {
		bot.go(WellKnown.urls.getDisplayComponentRepository() + "?"
				+ DisplayComponentRepositoryController.SIGNATURE_PARAMETER
				+ "=" + "abc");
		assertEquals("text/xml", bot.getContentType());
		RepositoryTest.assertCorrectRepositoryXML(bot.getPageText());
	}

}
