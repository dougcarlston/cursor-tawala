package com.tawala.dataconversion;

import javax.servlet.ServletException;

import com.tawala.TestCase;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.web.DataConverter;

public class DataConversionTest extends TestCase {
	private HibernateTestSetup hibernateTestSetup = new HibernateTestSetup();
	@Override
	protected void setUp() throws Exception {
		super.setUp();
		hibernateTestSetup.onSetUp();
	}
	
	@Override
	protected void tearDown() throws Exception {
		hibernateTestSetup.onTearDown();
		super.tearDown();
	}
	
	public void testUniqueIdAssignment() throws ServletException {
		new DataConverter().init();
	}

}
