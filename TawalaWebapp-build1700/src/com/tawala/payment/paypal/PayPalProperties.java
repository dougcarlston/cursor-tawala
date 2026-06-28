package com.tawala.payment.paypal;

public class PayPalProperties {
	private static PayPalProperties singleton;
	
	private String paypalSiteURL;
	private String instantPaymentNotificationCallBackSiteURL;

	public String getInstantPaymentNotificationCallBackSiteURL() {
		if(instantPaymentNotificationCallBackSiteURL == null) {
			throw new IllegalStateException("instantPaymentNotificationCallBackSiteURL is not set. " +
					"Make sure paypal.properties file is on the server classpath and it has 'payPalProperties.instantPaymentNotificationCallBackSiteURL' property defined.");
		}
		return instantPaymentNotificationCallBackSiteURL;
	}

	public void setInstantPaymentNotificationCallBackSiteURL(
			String instantPaymentNotificationCallBackSiteURL) {
		this.instantPaymentNotificationCallBackSiteURL = instantPaymentNotificationCallBackSiteURL;
	}

	public String getPaypalSiteURL() {
		return paypalSiteURL;
	}

	public void setPaypalSiteURL(String siteURL) {
		this.paypalSiteURL = siteURL;
	}
	
	public void setSingleton(PayPalProperties singleton) {
		PayPalProperties.singleton = singleton;
	}
	
	public static PayPalProperties getSingleton() {
		if(singleton == null)  {
			throw new IllegalStateException("PayPalProperties singleton hasn't been set.");
		}
		return singleton;
	}
}
