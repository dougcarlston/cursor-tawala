package com.tawala.web.client;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.dom4j.Element;

import com.tawala.World;
import com.tawala.email.Email;
import com.tawala.email.EmailRuntimeConfig;
import com.tawala.email.EmailService;
import com.tawala.web.ApiResponse;

/** Non-secret outbound-email status for the browser Designer. */
public class EmailStatusResponse extends ApiResponse {
	private final String message;

	public EmailStatusResponse() {
		this(null);
	}

	public EmailStatusResponse(String message) {
		this.message = message;
	}

	protected void addContents(Element root, HttpServletRequest request) {
		EmailRuntimeConfig config = EmailRuntimeConfig.get();
		Element email = root.addElement("emailStatus");
		email.addAttribute("enabled", String.valueOf(config.isEnabled()));
		email.addAttribute("configured", String.valueOf(config.isDeliveryReady()));
		email.addAttribute("host", config.getHost() == null ? "" : config.getHost());
		email.addAttribute("port", String.valueOf(config.getPort()));
		email.addAttribute("auth", String.valueOf(config.isSmtpAuth()));
		email.addAttribute("starttls", String.valueOf(config.isStartTls()));
		email.addAttribute("fromAddress", config.getFromAddress());
		email.addAttribute("fromName", config.getFromName());
		email.addAttribute("workerEnabled", String.valueOf(config.isWorkerEnabled()));
		email.addAttribute("readyCount", String.valueOf(EmailService.countEmailsByState(Email.State.READY)));
		email.addAttribute("sendingCount", String.valueOf(EmailService.countEmailsByState(Email.State.SENDING)));
		email.addAttribute("sentCount", String.valueOf(EmailService.countEmailsByState(Email.State.SENT)));
		email.addAttribute("errorCount", String.valueOf(EmailService.countEmailsByState(Email.State.ERROR)));
		email.addAttribute("lastError", config.getLastError() == null ? "" : config.getLastError());
		email.addAttribute("lastErrorAt", String.valueOf(config.getLastErrorAt()));
		email.addAttribute("lastSuccessAt", String.valueOf(config.getLastSuccessAt()));
		email.addAttribute("lastWorkerRunAt", String.valueOf(config.getLastWorkerRunAt()));
		if (message != null && message.length() > 0) {
			email.addAttribute("message", message);
		}
	}

	protected String status() {
		return "success";
	}

	public void handle(HttpServletRequest request, HttpServletResponse response, World world)
			throws IOException {
		super.handle(request, response, world);
	}
}
