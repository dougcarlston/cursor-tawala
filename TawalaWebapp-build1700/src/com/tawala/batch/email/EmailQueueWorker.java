package com.tawala.batch.email;

import java.util.Calendar;
import java.util.List;

import org.hibernate.criterion.DetachedCriteria;
import org.hibernate.criterion.Restrictions;
import org.springframework.beans.factory.DisposableBean;
import org.springframework.beans.factory.InitializingBean;
import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.Log;
import com.tawala.email.Email;
import com.tawala.email.EmailRuntimeConfig;
import com.tawala.hibernate.TawalaSessionFactory;

/**
 * In-process queue consumer for READY emails. The historical batch WAR / Quartz
 * schedule is missing from this repo, so the main webapp owns delivery.
 */
public class EmailQueueWorker implements InitializingBean, DisposableBean, Runnable {
	private Thread thread;
	private volatile boolean running = true;

	public void afterPropertiesSet() throws Exception {
		thread = new Thread(this, "tawala-email-queue");
		thread.setDaemon(true);
		thread.start();
		Log.info(this, "Email queue worker started");
	}

	public void destroy() throws Exception {
		running = false;
		if (thread != null) {
			thread.interrupt();
		}
	}

	public void run() {
		while (running) {
			EmailRuntimeConfig config = EmailRuntimeConfig.get();
			try {
				if (config.isDeliveryReady() && config.isWorkerEnabled()) {
					config.recordWorkerRun();
					recoverStaleSending(config.getStaleSendingMinutes());
					EmailSender sender = new EmailSender();
					sender.setBatchSize(config.getWorkerBatchSize());
					sender.setMaxTimeToRunInSeconds(config.getWorkerMaxSecondsPerRun());
					sender.run();
				}
			} catch (Throwable t) {
				Log.error(this, "Email queue worker iteration failed", t);
				config.recordError(t.getMessage());
			}
			sleepQuietly(config.getWorkerIntervalSeconds() * 1000L);
		}
	}

	/** Re-queue emails left in SENDING after a crash / killed JVM. */
	@SuppressWarnings("unchecked")
	static int recoverStaleSending(final int staleMinutes) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		Integer recovered = (Integer) transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				Calendar cutoff = Calendar.getInstance();
				cutoff.add(Calendar.MINUTE, -staleMinutes);
				DetachedCriteria criteria = DetachedCriteria.forClass(Email.class, "email");
				criteria.add(Restrictions.eq("state", Email.State.SENDING));
				criteria.add(Restrictions.lt("createdDate", cutoff.getTime()));
				HibernateTemplate hibernateTemplate = TawalaSessionFactory.MAIN
						.getHibernateTemplate();
				List<Email> emails = hibernateTemplate.findByCriteria(criteria);
				for (Email email : emails) {
					email.setState(Email.State.READY);
					email.setErrorReason(null);
					email.setCustomerErrorReason(null);
					hibernateTemplate.update(email);
				}
				return emails.size();
			}
		});
		if (recovered != null && recovered.intValue() > 0) {
			Log.warn(EmailQueueWorker.class, "Re-queued " + recovered
					+ " stale SENDING email(s) older than " + staleMinutes + " minutes");
		}
		return recovered == null ? 0 : recovered.intValue();
	}

	private static void sleepQuietly(long ms) {
		try {
			Thread.sleep(ms);
		} catch (InterruptedException e) {
			Thread.currentThread().interrupt();
		}
	}

	/** One-shot drain used by the test-send API and unit tests. */
	public static void processOnce() {
		EmailRuntimeConfig config = EmailRuntimeConfig.get();
		if (!config.isDeliveryReady()) {
			throw new IllegalStateException("Outbound email is not configured");
		}
		recoverStaleSending(config.getStaleSendingMinutes());
		EmailSender sender = new EmailSender();
		sender.setBatchSize(config.getWorkerBatchSize());
		sender.setMaxTimeToRunInSeconds(config.getWorkerMaxSecondsPerRun());
		sender.run();
	}
}
