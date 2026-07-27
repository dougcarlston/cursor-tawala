package com.tawala.integration.ysl;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.ExcludeFromTests;
import com.tawala.TestCase;
import com.tawala.hibernate.HibernateTestSetup;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.UserProject;
import com.tawala.web.WorldInitializer;

@ExcludeFromTests
public class LeagueDataExchangeTest extends TestCase {
	HibernateTestSetup hibernateTestSetup = new HibernateTestSetup();

	@Override
	protected void setUp() throws Exception {
		hibernateTestSetup.onSetUp();
		super.setUp();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
	}

	public void testPayloadCreation() {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				UserProject userProject = WorldInitializer.getDefaultWorld()
						.domain().projects().get("sergei",
								"SportsDashboardsTemplateVersion2");
				if (userProject == null) {
					return null;
				}
				League league = LeagueDataExchange.extractDataFrom(userProject);

				String payload = LeagueDataExchange.generatePayload(
						userProject, league);
				assertNotNull(payload);
				return null;
			}
		});
	}
}
