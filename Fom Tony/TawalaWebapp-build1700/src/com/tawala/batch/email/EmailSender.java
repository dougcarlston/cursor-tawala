package com.tawala.batch.email;

import java.util.List;

import org.hibernate.criterion.DetachedCriteria;
import org.hibernate.criterion.Restrictions;
import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.Log;
import com.tawala.email.Email;
import com.tawala.email.EmailService;
import com.tawala.hibernate.TawalaSessionFactory;

public class EmailSender {
	private int maxTimeToRunInSeconds;
	private int batchSize;

	public void run() {
		long startTime = System.currentTimeMillis();
		
		while((System.currentTimeMillis() - startTime)/1000 < maxTimeToRunInSeconds) {
			if (!sendNextEmailBatch(batchSize)) {
				break;
			}
		}
	}

	private static boolean sendNextEmailBatch(final int batchSize) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Boolean) transactionTemplate.execute(new TransactionCallback() {

			@SuppressWarnings("unchecked")
			public Object doInTransaction(TransactionStatus status) {
				long startTime = System.currentTimeMillis();
				DetachedCriteria criteria = DetachedCriteria.forClass(
						Email.class, "email");
				criteria.add(Restrictions.eq("state", Email.State.READY));

				HibernateTemplate hibernateTemplate = TawalaSessionFactory.MAIN
						.getHibernateTemplate();
				List<Email> emails = hibernateTemplate.findByCriteria(criteria, 0, batchSize);
				
				for (Email email : emails) {
					email.setState(Email.State.SENDING);
					hibernateTemplate.flush();
					
					boolean success = sendEmail(email);

					if (success) {
						Log.info(this, "Sent email to " + email.getTo() + " in "
								+ (System.currentTimeMillis() - startTime)
								+ " msecs.");
					} else {
						return false;
					}
				}

				return emails.size() == batchSize;
			}
		});
	}

	public static boolean sendEmail(Email email) {
		boolean success = true;
		try {
			EmailService.sendAndStoreEmail(email);
		} catch (Exception e) {
			Log.error(EmailSender.class, "Unable to send email:", e);
			success = false;
		}
		return success;
	}

	public int getMaxTimeToRunInSeconds() {
		return maxTimeToRunInSeconds;
	}

	public void setMaxTimeToRunInSeconds(int maxTimeToRunInSeconds) {
		this.maxTimeToRunInSeconds = maxTimeToRunInSeconds;
	}

	public int getBatchSize() {
		return batchSize;
	}

	public void setBatchSize(int batchSize) {
		this.batchSize = batchSize;
	}
}
