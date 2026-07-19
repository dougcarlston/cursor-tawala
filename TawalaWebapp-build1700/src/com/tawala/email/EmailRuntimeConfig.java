package com.tawala.email;

import com.scissor.Log;

/**
 * Server-owned outbound email settings. Credentials stay in Spring/mail.properties
 * (or a runtime-generated classpath override); never in project XML/JSON.
 */
public class EmailRuntimeConfig {
	private static volatile EmailRuntimeConfig instance = new EmailRuntimeConfig();

	private boolean enabled;
	private String host = "";
	private int port = 587;
	private String username = "";
	private boolean smtpAuth;
	private boolean startTls;
	private String fromAddress = "noreply@localhost";
	private String fromName = "Tawala";
	private boolean workerEnabled = true;
	private int workerIntervalSeconds = 15;
	private int workerBatchSize = 20;
	private int workerMaxSecondsPerRun = 30;
	private int staleSendingMinutes = 15;
	private volatile String lastError = "";
	private volatile long lastErrorAt;
	private volatile long lastSuccessAt;
	private volatile long lastWorkerRunAt;

	public static EmailRuntimeConfig get() {
		return instance;
	}

	/** Spring wiring — replaces the singleton after properties are injected. */
	public void setAsActive() {
		instance = this;
		Log.info(this, "Email runtime configured: " + statusSummary());
	}

	public String statusSummary() {
		return "enabled=" + enabled + " host=" + (host == null || host.length() == 0 ? "(none)" : host)
				+ " port=" + port + " auth=" + smtpAuth + " starttls=" + startTls
				+ " from=" + fromAddress + " worker=" + workerEnabled;
	}

	public boolean isDeliveryReady() {
		return enabled && host != null && host.trim().length() > 0;
	}

	public void recordError(String message) {
		if (message == null) {
			message = "unknown error";
		}
		if (message.length() > 500) {
			message = message.substring(0, 500);
		}
		lastError = message;
		lastErrorAt = System.currentTimeMillis();
	}

	public void recordSuccess() {
		lastSuccessAt = System.currentTimeMillis();
		lastError = "";
	}

	public void recordWorkerRun() {
		lastWorkerRunAt = System.currentTimeMillis();
	}

	public boolean isEnabled() {
		return enabled;
	}

	public void setEnabled(boolean enabled) {
		this.enabled = enabled;
	}

	public String getHost() {
		return host;
	}

	public void setHost(String host) {
		this.host = host == null ? "" : host.trim();
	}

	public int getPort() {
		return port;
	}

	public void setPort(int port) {
		this.port = port;
	}

	public String getUsername() {
		return username;
	}

	public void setUsername(String username) {
		this.username = username == null ? "" : username;
	}

	public boolean isSmtpAuth() {
		return smtpAuth;
	}

	public void setSmtpAuth(boolean smtpAuth) {
		this.smtpAuth = smtpAuth;
	}

	public boolean isStartTls() {
		return startTls;
	}

	public void setStartTls(boolean startTls) {
		this.startTls = startTls;
	}

	public String getFromAddress() {
		return fromAddress;
	}

	public void setFromAddress(String fromAddress) {
		if (fromAddress != null && fromAddress.trim().length() > 0) {
			this.fromAddress = fromAddress.trim();
		}
	}

	public String getFromName() {
		return fromName;
	}

	public void setFromName(String fromName) {
		if (fromName != null && fromName.trim().length() > 0) {
			this.fromName = fromName.trim();
		}
	}

	public boolean isWorkerEnabled() {
		return workerEnabled;
	}

	public void setWorkerEnabled(boolean workerEnabled) {
		this.workerEnabled = workerEnabled;
	}

	public int getWorkerIntervalSeconds() {
		return workerIntervalSeconds;
	}

	public void setWorkerIntervalSeconds(int workerIntervalSeconds) {
		this.workerIntervalSeconds = Math.max(5, workerIntervalSeconds);
	}

	public int getWorkerBatchSize() {
		return workerBatchSize;
	}

	public void setWorkerBatchSize(int workerBatchSize) {
		this.workerBatchSize = Math.max(1, workerBatchSize);
	}

	public int getWorkerMaxSecondsPerRun() {
		return workerMaxSecondsPerRun;
	}

	public void setWorkerMaxSecondsPerRun(int workerMaxSecondsPerRun) {
		this.workerMaxSecondsPerRun = Math.max(5, workerMaxSecondsPerRun);
	}

	public int getStaleSendingMinutes() {
		return staleSendingMinutes;
	}

	public void setStaleSendingMinutes(int staleSendingMinutes) {
		this.staleSendingMinutes = Math.max(1, staleSendingMinutes);
	}

	public String getLastError() {
		return lastError;
	}

	public long getLastErrorAt() {
		return lastErrorAt;
	}

	public long getLastSuccessAt() {
		return lastSuccessAt;
	}

	public long getLastWorkerRunAt() {
		return lastWorkerRunAt;
	}
}
