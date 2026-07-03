package com.tawala.acceptance.projectmanager;

import java.io.IOException;
import java.util.Collection;
import java.util.Iterator;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.WebRequest;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.Project;
import com.tawala.project.data.DataSource;
import com.tawala.project.data.StoredField;
import com.tawala.web.controller.WellKnown;

public class AddSharedDataSourceTest extends AcceptanceTestCase {
	private static final String DATASOURCE_NAME = "My Datasource";

	public void testAuthenticatedAccessOnly() throws RobotException, IOException {
		bot.logOut();
		bot.go(WellKnown.urls.getAddNewDataSource());
		assertContains(WellKnown.urls.getLogin(), bot.getPath().localPart());
	}
	
	public void testValidation() throws RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("addDataSourceLink");
		
		//--- Blank data source name and no fields.
		WebForm form = bot.getForm("addDataSourceForm");
		
		bot.submit(form);
		assertEquals(WellKnown.urls.getAddNewDataSource(), bot.getPath().localPart());
		assertContains("Name cannot be blank", bot.getPageText());
		assertContains("A data source should have least one field", bot.getPageText());
		
		//--- Invalid field values
		form = bot.getForm("addDataSourceForm");
		WebRequest request = form.newUnvalidatedRequest();
		request.setParameter("dataSourceName", DATASOURCE_NAME);
		String[] fieldNames = new String[] {"field:with illegal colon."};
		request.setParameter("field", fieldNames);
		bot.go(request);

		assertContains("Invalid field name", bot.getPageText());

		//--- Duplicate field names
		form = bot.getForm("addDataSourceForm");
		request = form.newUnvalidatedRequest();
		request.setParameter("dataSourceName", DATASOURCE_NAME);
		fieldNames = new String[] {"field1", "field1"};
		request.setParameter("field", fieldNames);
		bot.go(request);

		assertContains("This field name already exists", bot.getPageText());
	}

	public void testGoRight() throws RobotException {
		bot.logInAs(projectOwner);
		bot.go(WellKnown.urls.getProjectManagerView());
		bot.followLink("addDataSourceLink");
		
		WebForm form = bot.getForm("addDataSourceForm");
		WebRequest request = form.newUnvalidatedRequest();
		
		request.setParameter("dataSourceName", DATASOURCE_NAME);
		String[] fieldNames = new String[] {"field1", "field2", "field3"};
		request.setParameter("field", fieldNames);
		
		bot.go(request);
		assertEquals(WellKnown.urls.getViewSharedDatasources(), bot.getPath().localPart());
		
		Project sharedData = world.domain().users().getSharedStorageForUser(projectOwner);
		assertNotNull(sharedData);
		DataSource dataSource = sharedData.getDataSourceNamed(DATASOURCE_NAME);
		assertNotNull(dataSource);
		Collection<StoredField> fields = dataSource.getFields();
		assertEquals(fieldNames.length, fields.size());
		
		Iterator<StoredField> iterator = fields.iterator();
		for (int i = 0; i < fieldNames.length; i++) {
			assertEquals(fieldNames[i], iterator.next().getName());
		}
	}
}
