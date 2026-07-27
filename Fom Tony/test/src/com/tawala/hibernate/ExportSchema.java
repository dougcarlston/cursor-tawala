package com.tawala.hibernate;

import java.io.OutputStreamWriter;
import java.sql.Connection;
import java.sql.SQLException;

import org.apache.log4j.ConsoleAppender;
import org.apache.log4j.Level;
import org.apache.log4j.Logger;
import org.apache.log4j.SimpleLayout;
import org.hibernate.HibernateException;
import org.hibernate.Session;
import org.hibernate.cfg.Configuration;
import org.hibernate.dialect.Dialect;
import org.hibernate.tool.hbm2ddl.DatabaseMetadata;
import org.hibernate.tool.hbm2ddl.SchemaExport;
import org.hibernate.tool.hbm2ddl.SchemaValidator;
import org.springframework.beans.factory.xml.XmlBeanFactory;
import org.springframework.core.io.FileSystemResource;
import org.springframework.core.io.Resource;
import org.springframework.orm.hibernate3.HibernateCallback;
import org.springframework.orm.hibernate3.HibernateTemplate;

public class ExportSchema {

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		Logger.getRootLogger().setLevel(Level.INFO);
		ConsoleAppender appender = new ConsoleAppender();
		appender.setName("Console Appender");
		appender.setImmediateFlush(true);
		appender.setWriter(new OutputStreamWriter(System.out));

		SimpleLayout layout = new SimpleLayout();
		appender.setLayout(layout);

		Logger.getRootLogger().addAppender(appender);

		boolean isBackupDatabase = args.length >= 1 && args[0].equals("backup");

		try {
			Resource res = new FileSystemResource(
					"web/WEB-INF/hibernate-config.xml");
			XmlBeanFactory factory = new XmlBeanFactory(res);
			factory.getBean(isBackupDatabase ? "backupDatabaseSessionFactory" : "mainDatabaseSessionFactory");

			TawalaSessionFactory tawalaSessionFactory = isBackupDatabase ? TawalaSessionFactory.BACKUP
					: TawalaSessionFactory.MAIN;
			Configuration configuration = tawalaSessionFactory
					.getHibernateConfiguration();

			if (true) {
				SchemaExport schemaExport = new SchemaExport(configuration);
				schemaExport.setDelimiter(";");

				schemaExport.create(true, false);
			}

			if (false) {
				updateDatabaseSchema(configuration);
			}

			if (false) {
				SchemaValidator validator = new SchemaValidator(configuration);
				validator.validate();
			}
		} catch (Exception e) {
			Logger.getRootLogger().error("Failed:", e);
		}
	}

	public static void updateDatabaseSchema(final Configuration configuration)
			throws HibernateException {
		HibernateTemplate hibernateTemplate = new HibernateTemplate(
				TawalaSessionFactory.MAIN.getFactory());
		hibernateTemplate.setFlushMode(HibernateTemplate.FLUSH_NEVER);
		hibernateTemplate.execute(new HibernateCallback() {
			public Object doInHibernate(Session session)
					throws HibernateException, SQLException {
				Connection con = session.connection();
				final Dialect dialect = Dialect.getDialect(configuration
						.getProperties());
				DatabaseMetadata metadata = new DatabaseMetadata(con, dialect);
				String[] sql = configuration.generateSchemaUpdateScript(
						dialect, metadata);

				for (int i = 0; i < sql.length; i++) {
					System.out.print(sql[i]);
					System.out.println(';');
				}
				return null;
			}
		});
	}
}
