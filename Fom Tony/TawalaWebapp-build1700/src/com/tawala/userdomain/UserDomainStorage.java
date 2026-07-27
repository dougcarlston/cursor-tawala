package com.tawala.userdomain;

import java.util.List;

import org.hibernate.FetchMode;
import org.hibernate.criterion.DetachedCriteria;
import org.hibernate.criterion.Restrictions;
import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;

public class UserDomainStorage {
	private static final String QUERY_FIND_DOMAIN_BY_NAME = "from "
			+ UserDomain.class.getName();
	private static final String QUERY_FIND_ALL_DOMAINS = "from "
			+ UserDomain.class.getName();

	static {
		HibernateTemplate queryTemplate = new HibernateTemplate();
		queryTemplate.setCacheQueries(true);
		queryTemplate.setQueryCacheRegion("query.find.domain.by.name");
		TawalaSessionFactory.MAIN.addCustomQueryTemplate(QUERY_FIND_DOMAIN_BY_NAME,
				queryTemplate);

		queryTemplate = new HibernateTemplate();
		queryTemplate.setCacheQueries(true);
		queryTemplate.setQueryCacheRegion("query.find.all.domains");
		TawalaSessionFactory.MAIN.addCustomQueryTemplate(QUERY_FIND_ALL_DOMAINS,
				queryTemplate);

	}

	public static void createDomain(final UserDomain userDomain,
			final List<Long> libraryProjectIds) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				populateDomainWithProjects(userDomain, libraryProjectIds);

				TawalaSessionFactory.MAIN.getHibernateTemplate().persist(userDomain);
				TawalaSessionFactory.MAIN.getHibernateTemplate().initialize(
						userDomain.getFeaturedProjects());

				return null;
			}
		});
	}

	public static void update(final UserDomain userDomain,
			final List<Long> libraryProjectIds) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				TawalaSessionFactory.MAIN.getHibernateTemplate().update(userDomain);
				userDomain.getFeaturedProjects().clear();
				populateDomainWithProjects(userDomain, libraryProjectIds);

				TawalaSessionFactory.MAIN.getHibernateTemplate().initialize(
						userDomain.getFeaturedProjects());
				return null;
			}
		});
	}

	private static void populateDomainWithProjects(final UserDomain userDomain,
			final List<Long> libraryProjectIds) {
		for (Long libaryProjectId : libraryProjectIds) {
			LibraryProject libraryProject = (LibraryProject) TawalaSessionFactory.MAIN
					.getHibernateTemplate().load(LibraryProject.class,
							libaryProjectId);
			TawalaSessionFactory.MAIN.getHibernateTemplate().initialize(
					libraryProject);
			userDomain.getFeaturedProjects().add(libraryProject);
		}
	}

	public static UserDomain getDomainNamed(final String name) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserDomain) transactionTemplate
				.execute(new TransactionCallback() {
					@SuppressWarnings("unchecked")
					public Object doInTransaction(TransactionStatus status) {

						String domainAlias = "domain";
						DetachedCriteria criteria = DetachedCriteria.forClass(
								UserDomain.class, domainAlias);
						criteria.add(Restrictions.eq("name", name));
						criteria.setFetchMode("featuredProjects",
								FetchMode.JOIN);

						List<UserDomain> result = TawalaSessionFactory.MAIN
								.getCustomTemplate(QUERY_FIND_DOMAIN_BY_NAME)
								.findByCriteria(criteria);

						if (result.size() >= 1) {
							UserDomain userDomain = result.get(0);
							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.initialize(
											userDomain.getFeaturedProjects());
							return userDomain;
						} else {
							return null;
						}
					}
				});
	}

	@SuppressWarnings("unchecked")
	public static List<UserDomain> getAllDomains() {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (List<UserDomain>) transactionTemplate
				.execute(new TransactionCallback() {
					@SuppressWarnings("unchecked")
					public Object doInTransaction(TransactionStatus status) {

						List<UserDomain> result = TawalaSessionFactory.MAIN
								.getCustomTemplate(QUERY_FIND_ALL_DOMAINS)
								.find(QUERY_FIND_ALL_DOMAINS);
						return result;
					}
				});
	}

	public static void deleteDomainNamed(final String name) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				List found = TawalaSessionFactory.MAIN.getHibernateTemplate().find(
						"from " + UserDomain.class.getName()
								+ " where name = ?", name);
				if (found.size() == 1) {
					TawalaSessionFactory.MAIN.getHibernateTemplate().delete(
							found.iterator().next());
				}
				return null;
			}
		});
	}

	public static void deleteDomainById(final long domainId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				UserDomain domain = (UserDomain) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(UserDomain.class, domainId);
				if (domain == null) {
					return null;
				}

				TawalaSessionFactory.MAIN.getHibernateTemplate().delete(domain);

				return null;
			}
		});
	}

	public static UserDomain getDomainById(final long domainId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserDomain) transactionTemplate
				.execute(new TransactionCallback() {
					@SuppressWarnings("unchecked")
					public Object doInTransaction(TransactionStatus status) {

						UserDomain result = (UserDomain) TawalaSessionFactory.MAIN
								.getHibernateTemplate().load(UserDomain.class,
										domainId);
						TawalaSessionFactory.MAIN.getHibernateTemplate().initialize(
								result.getFeaturedProjects());
						return result;
					}
				});
	}

	public static UserDomain getDefaultDomain() {
		UserDomain domain = new UserDomain();
		domain.setShowNotifyBlock(false);
		domain.setShowSuggestionsBlock(false);
		domain.setFeaturedProjects(ProjectLibraryService.getFeaturedProjects());
		return domain;
	}

	public static void createSuggestion(Suggestion suggestion) {
		TawalaSessionFactory.MAIN.getHibernateTemplate().persist(suggestion);
	}

	public static void createNotificationRequest(
			NotificationRequest notificationRequest) {
		TawalaSessionFactory.MAIN.getHibernateTemplate()
				.persist(notificationRequest);
	}
}
