package com.tawala.payment.paypal;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.math.BigDecimal;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.Enumeration;
import java.util.Properties;

import javax.servlet.http.HttpServletRequest;

import com.scissor.Log;

public class InstantPaymentNotification {
	private String itemName;
	private String itemNumber;
	private String paymentStatus;
	private BigDecimal paymentAmount;
	private String paymentCurrency;
	private String txnId;
	private String receiverEmail;
	private String payerEmail;
	private String invoiceNumber;
	private Date transactionDate;
	private String originalPayload;

	public InstantPaymentNotification(HttpServletRequest request) {
		this.originalPayload = extractPayload(request);

		boolean errors = false;
		itemName = request.getParameter("item_name");
		itemNumber = request.getParameter("item_number");
		paymentStatus = request.getParameter("payment_status");

		String caseType = request.getParameter("case_type");
		if(caseType != null && caseType.equals("chargeback")) {
			paymentStatus = caseType;
		}
		
		String paymentAmountString = request.getParameter("mc_gross");
		if (paymentAmountString == null) {
			if(caseType == null) {
				errors = true;
			}
		} else {
			paymentAmount = new BigDecimal(paymentAmountString);
		}
		paymentCurrency = request.getParameter("mc_currency");
		txnId = request.getParameter("txn_id");
		receiverEmail = request.getParameter("receiver_email");
		payerEmail = request.getParameter("payer_email");
		invoiceNumber = request.getParameter("invoice");
		String transactionDateString = request.getParameter("payment_date");
		
		if (transactionDateString != null) {
			try {
				transactionDate = new SimpleDateFormat(
						"HH:mm:ss MMM dd, yyyy zzz")
						.parse(transactionDateString);
			} catch (ParseException e) {
				Log.error(this, "Unable to parse the transaction date: ", e);
			}
		}
		if(errors) {
			Log.error(this, "Errors parsing IPN: " + originalPayload);
		}
	}

	@SuppressWarnings("unchecked")
	private static String extractPayload(HttpServletRequest request) {
		Properties properties = new Properties();

		Enumeration<String> parameterNames = request.getParameterNames();
		while (parameterNames.hasMoreElements()) {
			String name = (String) parameterNames.nextElement();
			// --- The assumption is that IPN doesn't send multiple values under
			// the same parameter
			properties.put(name, request.getParameter(name));
		}

		ByteArrayOutputStream stream = new ByteArrayOutputStream();
		try {
			properties.store(stream, null);
		} catch (IOException e) {
			Log.error(InstantPaymentNotification.class,
					"Failed to store properties: ", e);
		} finally {
			try {
				stream.close();
			} catch (IOException e) {
				// Just eat it...
			}
		}
		return stream.toString();
	}

	public String getItemName() {
		return itemName;
	}

	public String getItemNumber() {
		return itemNumber;
	}

	public String getPayerEmail() {
		return payerEmail;
	}

	public BigDecimal getPaymentAmount() {
		return paymentAmount;
	}

	public String getPaymentCurrency() {
		return paymentCurrency;
	}

	public String getPaymentStatus() {
		return paymentStatus;
	}

	public String getReceiverEmail() {
		return receiverEmail;
	}

	public String getTxnId() {
		return txnId;
	}

	public String getInvoiceNumber() {
		return invoiceNumber;
	}

	public Date getTransactionDate() {
		return transactionDate;
	}

	public String getOriginalPayload() {
		return originalPayload;
	}

}
