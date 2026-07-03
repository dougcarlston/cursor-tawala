package com.tawala.hibernate;

import org.springframework.beans.factory.xml.XmlBeanFactory;
import org.springframework.core.io.FileSystemResource;
import org.springframework.core.io.Resource;

import com.tawala.TestCaseLifeCycleListener;

public class HibernateTestSetup implements TestCaseLifeCycleListener {
	private XmlBeanFactory factory;

	public void onSetUp() {
		if (TawalaSessionFactory.MAIN == null || TawalaSessionFactory.MAIN.isClosed()) {
			Resource res = new FileSystemResource(
					"web/WEB-INF/hibernate-config.xml");
			factory = new XmlBeanFactory(res);
			factory.getBean("mainDatabaseSessionFactory");
			factory.getBean("backupDatabaseSessionFactory");
		}
	}

	public void onTearDown() {
		// --- TODO: verify we don't need it.
		// factory.destroySingletons();
	}
}
