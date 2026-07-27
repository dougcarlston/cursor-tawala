package com.tawala.acceptance.payment;

import java.io.IOException;
import java.math.BigDecimal;
import java.net.MalformedURLException;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import org.xml.sax.SAXException;

import com.meterware.httpunit.PostMethodWebRequest;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.component.web.display.PayPalSingleItemButton;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.payment.ProjectInvoice;
import com.tawala.payment.ProjectInvoiceEvent;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.ComponentBuilder;
import com.tawala.project.builder.DocumentBuilder;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProcessBlockBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.SiteRobot;
import com.tawala.web.controller.WellKnown;

public class PayPalPaymentTest extends AcceptanceTestCase {
	private static final String MAIN_FORM = "Main";

	private Project buildProject(String amount) {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm(MAIN_FORM);
		formBuilder.addDeclaredFields("Status", "Amount");
		projectBuilder.addForm("SuccessfulPayment");
		projectBuilder.addForm("CancelledPayment");

		DocumentBuilder documentBuilder = projectBuilder
				.addDocument("PaymentPage");
		ComponentBuilder payPalButton = new ComponentBuilder(
				new PayPalSingleItemButton());

		payPalButton.addPreformattedParameter(
				PayPalSingleItemButton.ITEM_DESCRIPTION_REFERENCE,
				"<string value=\"My item\" />");
		payPalButton.addPreformattedParameter(
				PayPalSingleItemButton.AMOUNT_REFERENCE, "<string value=\""
						+ amount + "\" />");
		payPalButton.addTextParameter(PayPalSingleItemButton.RETURN_ON_SUCCESS,
				"SuccessfulPayment");
		payPalButton.addTextParameter(PayPalSingleItemButton.RETURN_ON_CANCEL,
				"CancelledPayment");
		payPalButton.addTextParameter(PayPalSingleItemButton.BUTTON_TYPE,
				PayPalSingleItemButton.BUTTON_ID_TO_PURCHASE_TYPE_MAP.keySet()
						.iterator().next());
		payPalButton.addPreformattedParameter(
				PayPalSingleItemButton.FIELD_TO_RECORD_STATUS, MAIN_FORM + ":"
						+ "Status");
		payPalButton.addPreformattedParameter(
				PayPalSingleItemButton.FIELD_TO_RECORD_PAID_AMOUNT, MAIN_FORM
						+ ":" + "Amount");

		documentBuilder.addComponent(payPalButton);

		ProcessBlockBuilder processBlockBuilder = projectBuilder
				.addProcess("Main Post Process");
		processBlockBuilder.addShow(documentBuilder);

		formBuilder.setPostProcess(processBlockBuilder);

		return projectBuilder.build();
	}

	public void testBehaviorOfUserWithoutPaypalAccount() throws RobotException {
		Project project = buildProject("100");

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		bot.go(userProject, MAIN_FORM);
		bot.submit();
		assertContains(
				"This project is not set up to provide PayPal integration", bot
						.getPageText());
	}

	public void testBehaviorWhenAmountIsIncorrect() throws RobotException {
		Project project = buildProject("wrong amount");

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);

		projectOwner.setPayPalAccountId("test@example.org");
		world.domain().users().addOrSave(projectOwner);

		bot.go(userProject, MAIN_FORM);
		bot.submit();
		assertContains(
				"error occurred calculating the payment amount",
				bot.getPageText());
	}

	@SuppressWarnings("unchecked")
	public void testInstantPaymentNotification() throws RobotException,
			MalformedURLException, IOException, SAXException {
		Project project = buildProject("100");

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);


		projectOwner.setPayPalAccountId("test@example.org");
		world.domain().users().addOrSave(projectOwner);

		bot.go(userProject, MAIN_FORM);
		bot.submit();
		assertContains(
				"<input type=\"hidden\" name=\"item_name\" value=\"My item\"/>",
				bot.getPageText());

		Pattern pattern = Pattern.compile(
				"<input type=\"hidden\" name=\"invoice\" value=\"([^\"]+)\"/>",
				Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());
		String invoiceNumber = matcher.group(1);

		assertNotNull(invoiceNumber);

		ProjectInvoice invoice = (ProjectInvoice) TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.get(ProjectInvoice.class, invoiceNumber);
		assertNotNull(invoice);
		assertEquals("New", invoice.getStatus());
		assertEquals(MAIN_FORM + ":" + "Status", invoice.getStatusFieldName());
		assertEquals(MAIN_FORM + ":" + "Amount", invoice.getAmountFieldName());

		// ---- Simulate IPN
		PostMethodWebRequest request = new PostMethodWebRequest(SiteRobot.ROOT
				+ WellKnown.urls.getPayPalInstantPaymentNotification());
		request.setParameter("invoice", invoiceNumber);
		request.setParameter("mc_gross", "33.50");
		request.setParameter("payment_status", "Completed");
		request.setParameter("payment_date", "21:44:27 May 05, 2008 PDT");

		bot.go(request);

		/*
		 * Example of real request custom= residence_country=US payment_fee=2.77
		 * first_name=Tawala test_ipn=1 item_number= charset=windows-1252
		 * invoice=6zkdq57arjm0rm2j3rpm shipping=0.00 mc_fee=2.77
		 * business=slilichenko@yahoo.com mc_gross=85.00 payment_type=instant
		 * mc_currency=USD item_name=Dirt Bowl Demo quantity=1
		 * receiver_id=Z5UWTKG3P7S5J txn_type=web_accept
		 * verify_sign=A604txXFPrKoitivbzbzNe45mN-8AkhhzKQuasL0WsU08oxaI8LbUzpA
		 * payer_status=verified payer_id=DBPBHEA9MMS7G payment_gross=85.00
		 * notify_version=2.4 tax=0.00 receiver_email=slilichenko@yahoo.com
		 * payer_email=tester1@tawala.com payment_status=Completed
		 * txn_id=72A97182WX5778347 last_name=Tester payment_date=21\:44\:27 May
		 * 05, 2008 PDT
		 */

		invoice = (ProjectInvoice) TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.get(ProjectInvoice.class, invoiceNumber);
		assertNotNull(invoice);
		assertEquals("Completed", invoice.getStatus());
		assertEquals(new BigDecimal("33.50"), invoice.getPaidAmount());

		List<ProjectInvoiceEvent> events = TawalaSessionFactory.MAIN
				.getHibernateTemplate().find(
						" from " + ProjectInvoiceEvent.class.getName()
								+ " where invoice = ?", invoice);
		assertEquals(1, events.size());
		ProjectInvoiceEvent event = events.get(0);

		assertEquals("Completed", event.getStatus());
		assertContains("invoice=" + invoiceNumber, event.getPayload());
	}

	@SuppressWarnings("unchecked")
	public void testInstantPaymentNotificationWithChargeBack() throws RobotException,
			MalformedURLException, IOException, SAXException {
		Project project = buildProject("100");

		UserProject userProject = new UserProject(project, projectOwner, "test");
		world.domain().projects().put(userProject);


		projectOwner.setPayPalAccountId("test@example.org");
		world.domain().users().addOrSave(projectOwner);

		bot.go(userProject, MAIN_FORM);
		bot.submit();
		assertContains(
				"<input type=\"hidden\" name=\"item_name\" value=\"My item\"/>",
				bot.getPageText());

		Pattern pattern = Pattern.compile(
				"<input type=\"hidden\" name=\"invoice\" value=\"([^\"]+)\"/>",
				Pattern.MULTILINE);
		Matcher matcher = pattern.matcher(bot.getPageText());
		assertTrue(matcher.find());
		String invoiceNumber = matcher.group(1);

		assertNotNull(invoiceNumber);

		ProjectInvoice invoice = (ProjectInvoice) TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.get(ProjectInvoice.class, invoiceNumber);
		assertNotNull(invoice);
		assertEquals("New", invoice.getStatus());
		assertEquals(MAIN_FORM + ":" + "Status", invoice.getStatusFieldName());
		assertEquals(MAIN_FORM + ":" + "Amount", invoice.getAmountFieldName());

		// ---- Simulate IPN with chargeback
		PostMethodWebRequest request = new PostMethodWebRequest(SiteRobot.ROOT
				+ WellKnown.urls.getPayPalInstantPaymentNotification());
		request.setParameter("case_id", "PP-234waw-awdrw34");
		request.setParameter("invoice", invoiceNumber);
		request.setParameter("business", "mybusiness@xy.com");
		request.setParameter("case_type", "chargeback");
		request.setParameter("receiver_id", "@#$ASDF#$@#$");
		request.setParameter("txn_type", "new_case");
		request.setParameter("payer_id", "BQA#ASDRW#R");
		request.setParameter("reason_code", "unauthorized");
		request.setParameter("txn_id", "2aseraw3423adfasdfa");
		request.setParameter("payment_date", "21:44:27 May 05, 2008 PDT");

		bot.go(request);

		/*
		 * Example of real request 
		 */
		// receipt_id=1499-3285-7253-3801
		// custom=
		// case_id=PP-623-555-536
		// charset=windows-1252
		// invoice=w9s10udmuqcnhn33nkni
		// business=mybusiness@gmail.com
		// case_type=chargeback
		// receiver_id=3PMW4KN575AVY
		// txn_type=new_case
		// verify_sign=AFcWxV21C7fd0v3bYYYRCpSSRl31AVMHOHg5tHX34S8qsXjTbVAcUnrn
		// payer_id=2BD4NV5EXXXV96
		// notify_version=2.6
		// receiver_email=mybusiness@gmail.com
		// case_creation_date=22\:24\:05 Jan 23, 2009 PST
		// payer_email=payer@gmail.com
		// reason_code=unauthorized
		// txn_id=7XB10312HX1779514
		// payment_date=15\:05\:34 Dec 16, 2008 PST

		invoice = (ProjectInvoice) TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.get(ProjectInvoice.class, invoiceNumber);
		assertNotNull(invoice);
		assertEquals("chargeback", invoice.getStatus());
		assertNull(invoice.getPaidAmount());

		List<ProjectInvoiceEvent> events = TawalaSessionFactory.MAIN
				.getHibernateTemplate().find(
						" from " + ProjectInvoiceEvent.class.getName()
								+ " where invoice = ?", invoice);
		assertEquals(1, events.size());
		ProjectInvoiceEvent event = events.get(0);

		assertEquals("chargeback", event.getStatus());
		assertContains("invoice=" + invoiceNumber, event.getPayload());
	}
}
