package com.tawala.project;

import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.Iterator;
import java.util.List;

import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.hibernate.TawalaSessionFactory;

// TODO: needs better name; not all data comes from forms, and later on in lifecycle isn't really a submission
public class FormSubmissionsHibernateImpl implements FormSubmissions {
	public FormSubmissionsHibernateImpl() {
	}

	// It's a bit of a hack - more "appropriate" solution is to use Hibernate
	// options.
	// But it seems pretty harmless, at least now.
	public List<FormSubmission> fullyInitializedResponsesFor(Project project,
			Form form) {
		List<FormSubmission> result = responsesFor(project, form);
		for (FormSubmission submission : result) {
			submission.setProject(project);
		}
		return result;
	}

	@SuppressWarnings("unchecked")
	public List<FormSubmission> responsesFor(Project project, String formName) {
		List<FormSubmission> result = TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.findByNamedParam(
						"from "
								+ FormSubmission.class.getName()
								+ " where project = :project and storedFormName = :formName",
						new String[] { "project", "formName" },
						new Object[] { project, formName });

		Collections.sort(result, new Comparator<FormSubmission>() {
			public int compare(FormSubmission o1, FormSubmission o2) {
				int comparisonResult = o1.getCreationDate().compareTo(
						o2.getCreationDate());
				return comparisonResult == 0 ? (int) (o1.getDatabaseId() - o2
						.getDatabaseId()) : comparisonResult;
			}
		});
		return result;
	}

	public long sizeOfResponses(Project project, String formName) {
		// --- TODO: remove it
		return 0;
	}

	public void record(final FormSubmission submission) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				submission.setUpdatedDate(submission.getCreationDate());
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						submission);

				return null;
			}
		});
	}

	public void update(final FormSubmission submission) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				submission.setUpdatedDate(new Date());
				HibernateTemplate hibernateTemplate = TawalaSessionFactory.MAIN
						.getHibernateTemplate();
				hibernateTemplate.merge(submission);

				return null;
			}
		});
	}

	public void eraseResponsesFor(final Project project, final String name) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				List<FormSubmission> formSubmissions = responsesFor(project,
						name);
				TawalaSessionFactory.MAIN.getHibernateTemplate().deleteAll(
						formSubmissions);

				return null;
			}
		});
	}

	// TODO: this has suboptimal preformance - need to find a way to delete
	// without bringing data in memory.
	public void purgeProjectResponses(final Project project) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			@SuppressWarnings("unchecked")
			public Object doInTransaction(TransactionStatus status) {

				Iterator iterator = TawalaSessionFactory.MAIN
						.getHibernateTemplate().iterate(
								"from " + FormSubmission.class.getName()
										+ " where project = ?", project);
				while (iterator.hasNext()) {
					iterator.next();
					iterator.remove();
				}

				return null;
			}
		});
	}

	public List<FormSubmission> responsesFor(final Project project,
			final Form form) {
		return responsesFor(project, form.getName());
	}

	public long responseCount(final Project project) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Long) transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				return ((Long) TawalaSessionFactory.MAIN.getHibernateTemplate()
						.iterate(
								"select count(*) from "
										+ FormSubmission.class.getName()
										+ " where project = ?", project).next())
						.longValue();
			}
		});
	}

	public long responseCount(final Project project, final String formName) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Long) transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				return ((Long) TawalaSessionFactory.MAIN
						.getHibernateTemplate()
						.iterate(
								"select count(*) from "
										+ FormSubmission.class.getName()
										+ " where project = ? and storedFormName = ?",
								new Object[] { project, formName }).next())
						.longValue();
			}
		});
	}

	public void delete(FormSubmission formSubmission) {
		TawalaSessionFactory.MAIN.getHibernateTemplate().delete(formSubmission);
	}

	public void verifyThatBelongsToUserAndDelete(final long recordId,
			final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			@SuppressWarnings("unchecked")
			public Object doInTransaction(TransactionStatus status) {
				HibernateTemplate hibernateTemplate = TawalaSessionFactory.MAIN
						.getHibernateTemplate();
				FormSubmission record = (FormSubmission) hibernateTemplate.get(
						FormSubmission.class, recordId);
				if (record == null) {
					return null;
				}

				List userList = hibernateTemplate
						.find(
								"from "
										+ User.class.getName()
										+ " as user where exists ( from "
										+ UserProject.class.getName()
										+ " as userProject where userProject.user = user and userProject.project = ?)",
								record.getProject());

				User submissionOwner = null;

				switch (userList.size()) {
				case 1:
					submissionOwner = (User) userList.get(0);
					break;

				default:
					throw new IllegalStateException(
							"Expected exactly one record, instead got "
									+ userList.size());
				}

				if (submissionOwner.equals(user)) {
					hibernateTemplate.delete(record);
				} else {
					Log
							.warn(
									FormSubmissionsHibernateImpl.class
											.getName(),
									"User "
											+ user.getId()
											+ " attempted to delete form submission belonging to "
											+ submissionOwner.getId());
				}

				return null;
			}
		});
	}

	public void deleteSharedDataSourceSubmission(final long recordId,
			final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				HibernateTemplate hibernateTemplate = TawalaSessionFactory.MAIN
						.getHibernateTemplate();
				FormSubmission record = (FormSubmission) hibernateTemplate.get(
						FormSubmission.class, recordId);
				if (record == null) {
					return null;
				}

				// This is somewhat ugly, but this is a very infrequent
				// operation and doesn't have to be optimized.
				User loadedUser = (User) hibernateTemplate.load(User.class,
						user.getDatabaseId());

				if (loadedUser.getSharedStorage().equals(record.getProject())) {
					hibernateTemplate.delete(record);
				} else {
					Log
							.warn(
									FormSubmissionsHibernateImpl.class
											.getName(),
									"User "
											+ user.getId()
											+ " attempted to delete shared data submission belonging to project #"
											+ record.getProject().getId());
				}

				return null;
			}
		});
	}

	public static boolean changeFormSubmissionValueForRegularProject(
			final User user, final long userProjectId,
			final long formSubmissionId, final String fieldName,
			final List<String> values) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Boolean) transactionTemplate.execute(new TransactionCallback() {

			public Object doInTransaction(TransactionStatus status) {
				FormSubmission submission = (FormSubmission) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(FormSubmission.class,
								formSubmissionId);
				if (submission == null) {
					return false;
				}

				UserProject userProject = (UserProject) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(UserProject.class,
								userProjectId);
				if (userProject == null) {
					return false;
				}

				if (!userProject.getUser().equals(user)) {
					return false;
				}

				if (!submission.getProject().equals(userProject.getProject())) {
					return false;
				}

				submission.setValues(fieldName, values);

				return true;
			}
		});
	}

	public static boolean changeFormSubmissionValueForSharedData(
			final User user, final long formSubmissionId,
			final String fieldName, final List<String> values) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Boolean) transactionTemplate.execute(new TransactionCallback() {

			public Object doInTransaction(TransactionStatus status) {
				FormSubmission submission = (FormSubmission) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(FormSubmission.class,
								formSubmissionId);
				if (submission == null) {
					return false;
				}

				User loadedUser = (User) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(User.class,
								user.getDatabaseId());
				if (loadedUser == null) {
					return false;
				}

				if (loadedUser.getSharedStorage() == null) {
					return false;
				}

				if (submission.getProject().getId() != loadedUser.getSharedStorage().getId()) {
					return false;
				}

				submission.setValues(fieldName, values);

				return true;
			}
		});
	}

}
