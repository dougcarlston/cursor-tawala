package com.tawala.batch.email;

import java.sql.SQLException;

import org.hibernate.HibernateException;
import org.hibernate.Query;
import org.hibernate.Session;
import org.springframework.orm.hibernate3.HibernateCallback;

import com.scissor.Log;
import com.tawala.hibernate.TawalaSessionFactory;

public class OldEmailPurger {
	private int numberOfDaysOld = 120;
	
	public void run() {
		TawalaSessionFactory.MAIN.getHibernateTemplate().execute(new HibernateCallback() {
			public Object doInHibernate(Session session)
					throws HibernateException, SQLException {
				Log.info(this, "Deleting emails older than " + numberOfDaysOld + " days.");
				Query query = session.createSQLQuery("delete from email where create_dt < now() - interval '" + numberOfDaysOld + " days' ");
				int deletedEmails = query.executeUpdate();
				Log.info(this, "Deleted " + deletedEmails + " email(s).");
				return null;
			}
		});
		
	}

	public int getNumberOfDaysOld() {
		return numberOfDaysOld;
	}

	public void setNumberOfDaysOld(int numberOfDaysOld) {
		this.numberOfDaysOld = numberOfDaysOld;
	}
}
