package com.tawala.domain;

import java.io.UnsupportedEncodingException;

import javax.mail.Address;
import javax.mail.internet.AddressException;
import javax.mail.internet.InternetAddress;
import javax.persistence.Column;
import javax.persistence.Embeddable;
import javax.persistence.Transient;

@Embeddable
public class EmailAddress {
	@Column(name = "email", length = 100, nullable = true)
	private String address;

	@Transient
	private String alias;

	EmailAddress() {
		// For Hibernate's use
	}

	public EmailAddress(String address) {
		this.address = address;
	}

	public boolean equals(Object o) {
		if (this == o)
			return true;
		if (!(o instanceof EmailAddress))
			return false;

		final EmailAddress emailAddress = (EmailAddress) o;

		if (address != null ? !address.equals(emailAddress.address)
				: emailAddress.address != null)
			return false;

		return true;
	}

	public int hashCode() {
		return (address != null ? address.hashCode() : 0);
	}

	public String toString() {
		return address;
	}

	public String getAlias() {
		return alias;
	}

	public void setAlias(String alias) {
		this.alias = alias;
	}

	public Address asInternetAddress() throws AddressException,
			UnsupportedEncodingException {
		if (!address.contains("@")) {
			throw new AddressException(
					"Email can't be addressed to the local server; it must include the host name: "
							+ address);
		}
		if(address.contains("..")) {
			throw new AddressException("Two dots together are not allowed in email address.");
		}
				
		InternetAddress result = new InternetAddress(address);
		if (alias != null) {
			result.setPersonal(alias);
		}
		return result;
	}

	public boolean isEmpty() {
		return this.address == null || this.address.length() == 0;
	}
}
