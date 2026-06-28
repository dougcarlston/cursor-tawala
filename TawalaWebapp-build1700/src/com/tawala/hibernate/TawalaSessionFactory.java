package com.tawala.hibernate;

import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Properties;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.Table;

import org.hibernate.SessionFactory;
import org.hibernate.cache.EhCacheProvider;
import org.hibernate.cfg.Configuration;
import org.hibernate.cfg.Environment;
import org.hibernate.dialect.PostgreSQLDialect;
import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.orm.hibernate3.HibernateTransactionManager;
import org.springframework.orm.hibernate3.annotation.AnnotationSessionFactoryBean;
import org.springframework.transaction.PlatformTransactionManager;

import com.scissor.Log;
import com.tawala.domain.ProjectGroup;
import com.tawala.domain.Role;
import com.tawala.domain.User;
import com.tawala.domain.Visitor;
import com.tawala.email.Email;
import com.tawala.email.SportsDashboardContactUsEmail;
import com.tawala.email.SportsDashboardLetterToOrgBoardEmail;
import com.tawala.email.UniqueBodyEmail;
import com.tawala.email.UserProjectEmail;
import com.tawala.event.Event;
import com.tawala.payment.ProjectInvoice;
import com.tawala.payment.ProjectInvoiceEvent;
import com.tawala.project.FormSubmission;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.Project;
import com.tawala.project.ProjectVersion;
import com.tawala.project.UserProject;
import com.tawala.project.backup.BackupSchedule;
import com.tawala.project.backup.DailyBackup;
import com.tawala.project.backup.OnlineUserProjectBackup;
import com.tawala.project.library.Category;
import com.tawala.project.library.Comment;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.Rating;
import com.tawala.project.library.event.CategoryAddedEvent;
import com.tawala.project.library.event.CategoryDeletedEvent;
import com.tawala.project.library.event.CategoryNameChangedEvent;
import com.tawala.project.library.event.CategoryParentChangedEvent;
import com.tawala.project.library.event.CommentAdded;
import com.tawala.project.library.event.CommentDeleted;
import com.tawala.project.library.event.CommentRestored;
import com.tawala.project.library.event.LibraryChangeEventBase;
import com.tawala.project.library.event.ProjectAdded;
import com.tawala.project.library.event.ProjectCategoryChanged;
import com.tawala.project.library.event.ProjectChangeEventBase;
import com.tawala.project.library.event.ProjectDeleted;
import com.tawala.project.library.event.ProjectLongDescriptionChanged;
import com.tawala.project.library.event.ProjectNameChanged;
import com.tawala.project.library.event.ProjectRated;
import com.tawala.project.library.event.ProjectRestored;
import com.tawala.project.library.event.ProjectShortDescriptionChanged;
import com.tawala.project.library.event.ProjectVersionAdded;
import com.tawala.project.library.event.ProjectVersionDeleted;
import com.tawala.project.library.event.ProjectVersionRestored;
import com.tawala.project.theme.UserDefinedTheme;
import com.tawala.project.theme.UserUploadedFile;
import com.tawala.userdomain.NotificationRequest;
import com.tawala.userdomain.Suggestion;
import com.tawala.userdomain.UserDomain;
import com.tawala.web.user.UserAccessTicket;

abstract public class TawalaSessionFactory extends AnnotationSessionFactoryBean {
	public static TawalaSessionFactory MAIN;
	public static TawalaSessionFactory BACKUP;

	private SessionFactory sessionFactory;
	private Configuration configuration;
	private PlatformTransactionManager transactionManager;

	private HibernateTemplate defaultTemplate = null;
	private Map<String, HibernateTemplate> queryTemplateMap = new HashMap<String, HibernateTemplate>();

	public TawalaSessionFactory() {
		setAnnotatedClasses(getClasses());
		setHibernateProperties(getProperties());
	}

	abstract protected Properties getProperties();

	@SuppressWarnings("unchecked")
	public abstract Class[] getClasses();

	abstract protected int getRequiredSchemeVersionNumber();

	public HibernateTemplate getHibernateTemplate() {
		if (defaultTemplate == null) {
			defaultTemplate = new HibernateTemplate(getFactory());
		}
		return defaultTemplate;
	}

	public void addCustomQueryTemplate(String query, HibernateTemplate template) {
		queryTemplateMap.put(query, template);
	}

	public HibernateTemplate getCustomTemplate(String query) {
		HibernateTemplate result = queryTemplateMap.get(query);
		if (result == null) {
			Log.warn(TawalaSessionFactory.class,
					"Unable to find custom template for query '" + query + "'");
			result = getHibernateTemplate();
		} else {
			result.setSessionFactory(getFactory());
		}

		return result;
	}

	public void evictCachedDataForQuery(final String query) {
		getFactory().evictQueries(
				getCustomTemplate(query).getQueryCacheRegion());
	}

	public SessionFactory getFactory() {
		if (sessionFactory == null)
			throw new IllegalStateException("Session factory is not set.");

		if (sessionFactory.isClosed())
			throw new IllegalStateException("Session factory is closed.");

		return sessionFactory;
	}

	public Configuration getHibernateConfiguration() {
		if (configuration == null)
			throw new IllegalStateException("Configuration is null.");

		return configuration;
	}

	public PlatformTransactionManager getTransactionManager() {
		if (transactionManager == null)
			throw new IllegalStateException("Transaction manager is null.");

		return transactionManager;
	}

	public boolean isClosed() {
		return sessionFactory == null;
	}

	public void afterPropertiesSet() throws Exception {
		super.afterPropertiesSet();
		sessionFactory = (SessionFactory) getObject();
		configuration = getConfiguration();
		transactionManager = new HibernateTransactionManager(sessionFactory);

		confirmRequiredSchemaVersion();
	}

	public void destroy() {
		super.destroy();
		sessionFactory = null;
		configuration = null;
		transactionManager = null;
	}

	// Needed for Unit Tests.
	/*
	 * public static void closeSessionFactory() { if (sessionFactory != null) {
	 * sessionFactory.close(); sessionFactory = null; } }
	 */

	private void confirmRequiredSchemaVersion() {
		SchemaVersion databaseSchemaVersion = getDatabaseSchemaVersion();
		if (databaseSchemaVersion.getVersion() != getRequiredSchemeVersionNumber()) {
			throw new IllegalStateException(
					"Required schema version is "
							+ getRequiredSchemeVersionNumber()
							+ " but current database is at version "
							+ databaseSchemaVersion.version
							+ ".\nPlease execute scripts in db/script folder to bring the schema to the required version.");
		}
	}

	@SuppressWarnings("unchecked")
	public SchemaVersion getDatabaseSchemaVersion() {
		List<SchemaVersion> results = getHibernateTemplate().find(
				"from " + SchemaVersion.class.getName());

		if (results.size() == 0)
			throw new IllegalStateException(
					"schema_version table doesn't have any rows.");

		if (results.size() > 1)
			throw new IllegalStateException("schema_version table has "
					+ results.size() + " rows.");

		return results.get(0);
	}

	@Entity
	@Table(name = "schema_version")
	public static class SchemaVersion {
		@Id
		@Column(name = "version")
		private int version;

		public int getVersion() {
			return version;
		}
	}

	// --- Setup for the default database.
	public static class MainDatabase extends TawalaSessionFactory {
		public static final int REQUIRED_SCHEMA_VERSION = 68;
		@SuppressWarnings("unchecked")
		private final static Class[] annotatedClasses = new Class[] {
				SchemaVersion.class, LibraryProject.class, Category.class,
				Comment.class, LibraryProjectVersion.class, Rating.class,
				LibraryChangeEventBase.class, ProjectChangeEventBase.class,
				CommentAdded.class, CommentDeleted.class,
				CommentRestored.class, ProjectAdded.class,
				ProjectCategoryChanged.class, ProjectDeleted.class,
				ProjectLongDescriptionChanged.class, ProjectNameChanged.class,
				ProjectRated.class, ProjectRestored.class,
				ProjectShortDescriptionChanged.class,
				ProjectVersionAdded.class, ProjectVersionDeleted.class,
				ProjectVersionRestored.class, CategoryAddedEvent.class,
				CategoryNameChangedEvent.class,
				CategoryParentChangedEvent.class, CategoryDeletedEvent.class,
				User.class, Project.class, ProjectVersion.class,
				UserProject.class, FormSubmission.class, UserDomain.class,
				Suggestion.class, NotificationRequest.class, Visitor.class,
				Event.class, LinkToUserProject.class, ProjectLibrary.class,
				UserAccessTicket.class, ProjectInvoice.class,
				ProjectInvoiceEvent.class, BackupSchedule.class,
				DailyBackup.class, OnlineUserProjectBackup.class, Email.class,
				UniqueBodyEmail.class, UserProjectEmail.class,
				UserDefinedTheme.class, UserUploadedFile.class,
				SportsDashboardContactUsEmail.class,
				SportsDashboardLetterToOrgBoardEmail.class, Role.class,
				ProjectGroup.class };

		@SuppressWarnings("unchecked")
		@Override
		public Class[] getClasses() {
			return annotatedClasses;
		}

		@Override
		protected Properties getProperties() {
			Properties properties = new Properties();
			properties.setProperty(Environment.DIALECT, PostgreSQLDialect.class
					.getName());
			properties.setProperty(Environment.FORMAT_SQL, Boolean
					.toString(true));
			properties.setProperty(Environment.USE_SQL_COMMENTS, Boolean
					.toString(false));
			properties.setProperty(Environment.SHOW_SQL, Boolean
					.toString(false));
			properties.setProperty(Environment.CACHE_PROVIDER,
					EhCacheProvider.class.getName());
			properties.setProperty(Environment.USE_QUERY_CACHE, Boolean
					.toString(true));
			properties.setProperty(Environment.USE_SECOND_LEVEL_CACHE, Boolean
					.toString(true));
			return properties;
		}

		@Override
		protected int getRequiredSchemeVersionNumber() {
			return REQUIRED_SCHEMA_VERSION;
		}

		@Override
		public void afterPropertiesSet() throws Exception {
			super.afterPropertiesSet();
			TawalaSessionFactory.MAIN = this;
		}

		@Override
		public void destroy() {
			super.destroy();
			TawalaSessionFactory.MAIN = null;
		}
	}

	// --- Database to store backups
	public static class BackupDatabase extends TawalaSessionFactory {
		public static final int REQUIRED_SCHEMA_VERSION = 3;
		@SuppressWarnings("unchecked")
		private final static Class[] annotatedClasses = new Class[] {
				SchemaVersion.class, OnlineUserProjectBackup.class };

		@SuppressWarnings("unchecked")
		@Override
		public Class[] getClasses() {
			return annotatedClasses;
		}

		@Override
		protected Properties getProperties() {
			Properties properties = new Properties();
			properties.setProperty(Environment.DIALECT, PostgreSQLDialect.class
					.getName());
			properties.setProperty(Environment.FORMAT_SQL, Boolean
					.toString(true));
			properties.setProperty(Environment.USE_SQL_COMMENTS, Boolean
					.toString(false));
			properties.setProperty(Environment.SHOW_SQL, Boolean
					.toString(false));
			properties.setProperty(Environment.CACHE_PROVIDER,
					EhCacheProvider.class.getName());
			properties.setProperty(Environment.USE_QUERY_CACHE, Boolean
					.toString(true));
			properties.setProperty(Environment.USE_SECOND_LEVEL_CACHE, Boolean
					.toString(true));
			return properties;
		}

		@Override
		protected int getRequiredSchemeVersionNumber() {
			return REQUIRED_SCHEMA_VERSION;
		}

		@Override
		public void afterPropertiesSet() throws Exception {
			super.afterPropertiesSet();
			TawalaSessionFactory.BACKUP = this;
		}

		@Override
		public void destroy() {
			super.destroy();
			TawalaSessionFactory.BACKUP = null;
		}
	}

}
