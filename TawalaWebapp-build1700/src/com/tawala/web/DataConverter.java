package com.tawala.web;

import java.util.List;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;

import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.Log;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.Project;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;

@SuppressWarnings("serial")
public class DataConverter extends HttpServlet {
	private static final String QUERY = "select project.id from "
			+ UserProject.class.getName()
			+ " as project where project.id = project.uniqueRandomId";

	@Override
	public void init() throws ServletException {
		if (true) {
			// --- We currently don't need to do the conversion. But we keep the
			// code around - one day we are going to need it.
			return;
		}
		int batchSize = 100;
		int converted = 0;

		HibernateTemplate template = new HibernateTemplate();
		template.setMaxResults(batchSize);
		TawalaSessionFactory.MAIN.addCustomQueryTemplate(QUERY, template);

		do {
			converted = convertNextBatch(batchSize);
			Log.info(this, "Updated unique ids of " + converted + " records.");
		} while (converted == batchSize);
	}

	private int convertNextBatch(final int batchSize) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());

		return (Integer) transactionTemplate.execute(new TransactionCallback() {
			@SuppressWarnings("unchecked")
			public Object doInTransaction(TransactionStatus status) {
				List<Long> allIds = TawalaSessionFactory.MAIN.getCustomTemplate(
						QUERY).find(QUERY);

				// --- TODO: this won't work now; the update needs to be tested.
				for (Long id : allIds) {
					TawalaSessionFactory.MAIN.getHibernateTemplate().bulkUpdate(
							"update "
									+ Project.class.getName()
									+ " project set project.uniqueRandomId = '"
									+ ProjectsHibernateImpl
											.generateUniqueRandomProjectId()
									+ "' where project.id =" + id);
				}

				return allIds.size();
			}
		});
	}
}
