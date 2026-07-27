package com.tawala.project.data;

import java.util.List;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.TestCase;
import com.tawala.domain.Domain;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.WorldInitializer;

public class SharedStorageTest extends TestCase {
	private User user = UserTest.aRegisteredUser();
	private Domain domain;

	public SharedStorageTest() {
		setUserNamesToDelete(user.getId());
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
		domain = WorldInitializer.getDefaultWorld().domain();
		domain.users().addOrSave(user);
	}

	public void testSharedStorageCreation() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("test");
		formBuilder.setExternalDataSource("MyDataSource");
		formBuilder.addFib("My FIB Question:", "fibfield", 20);
		formBuilder.addMcWithAlternateLabel("mcqfield", "My Question",
				new String[] { "apple", "oranges", "bananas" });
		formBuilder.addDeclaredFields("declared1", "declared2");

		UserProject userProject = new UserProject(projectBuilder.build(), user,
				"testproject");
		domain.projects().put(userProject);

		Project sharedStorage = domain.users().getSharedStorageForUser(user);
		assertNotNull(sharedStorage);
		List<DataSource> dataSources = sharedStorage.getDataSources();
		assertNotNull(dataSources);
		assertEquals(1, dataSources.size());

		DataSource dataSource = dataSources.get(0);
		assertEquals("MyDataSource", dataSource.getName());
		assertNotNull(dataSource.getFields());
		assertEquals(4, dataSource.getFields().size());

		assertEquals(StringField.class, ((List<StoredField>) dataSource
				.getFields()).get(0).getClass());
		StringField field1 = (StringField) ((List<StoredField>) dataSource
				.getFields()).get(0);
		assertEquals("fibfield", field1.getName());

		assertEquals(MultiChoiceField.class, ((List<StoredField>) dataSource
				.getFields()).get(1).getClass());
		MultiChoiceField field2 = (MultiChoiceField) ((List<StoredField>) dataSource
				.getFields()).get(1);
		assertEquals("mcqfield", field2.getName());

		assertEquals(StringField.class, ((List<StoredField>) dataSource
				.getFields()).get(2).getClass());
		StringField field3 = (StringField) ((List<StoredField>) dataSource
				.getFields()).get(2);
		assertEquals("declared1", field3.getName());

		assertEquals(StringField.class, ((List<StoredField>) dataSource
				.getFields()).get(3).getClass());
		StringField field4 = (StringField) ((List<StoredField>) dataSource
				.getFields()).get(3);
		assertEquals("declared2", field4.getName());
	}

	public void testSharedStorageUpdate() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("test");
		formBuilder.setExternalDataSource("MyDataSource");
		formBuilder.addFib("My FIB Question:", "fibfield", 20);
		formBuilder.addMcWithAlternateLabel("mcqfield", "My Question",
				new String[] { "apple", "oranges", "bananas" });
		formBuilder.addDeclaredFields("declared1", "declared2");

		UserProject userProject = new UserProject(projectBuilder.build(), user,
				"testproject");
		domain.projects().put(userProject);

		Project sharedStorage = domain.users().getSharedStorageForUser(user);
		assertNotNull(sharedStorage);
		List<DataSource> dataSources = sharedStorage.getDataSources();
		assertNotNull(dataSources);
		assertEquals(1, dataSources.size());

		DataSource dataSource = dataSources.get(0);
		assertEquals("MyDataSource", dataSource.getName());
		assertNotNull(dataSource.getFields());
		assertEquals(4, dataSource.getFields().size());

		// --- Modify the form and redeploy.
		projectBuilder = new ProjectBuilder();
		formBuilder = projectBuilder.addForm("test");
		formBuilder.setExternalDataSource("MyDataSource");
		formBuilder.addFib("My FIB Question:", "fibfield", 20);
		formBuilder.addFib("My Second FIB", "fibfield2", 20);

		userProject = new UserProject(projectBuilder.build(), user,
				"testproject");
		domain.projects().put(userProject);

		sharedStorage = domain.users().getSharedStorageForUser(user);
		assertNotNull(sharedStorage);
		dataSources = sharedStorage.getDataSources();
		assertNotNull(dataSources);
		assertEquals(1, dataSources.size());

		dataSource = dataSources.get(0);
		assertEquals("MyDataSource", dataSource.getName());
		assertNotNull(dataSource.getFields());
		assertEquals(2, dataSource.getFields().size());

		assertEquals(StringField.class, ((List<StoredField>) dataSource
				.getFields()).get(0).getClass());
		StringField field1 = (StringField) ((List<StoredField>) dataSource
				.getFields()).get(0);
		assertEquals("fibfield", field1.getName());

		assertEquals(StringField.class, ((List<StoredField>) dataSource
				.getFields()).get(1).getClass());
		StringField field2 = (StringField) ((List<StoredField>) dataSource
				.getFields()).get(1);
		assertEquals("fibfield2", field2.getName());
	}

	public void testMultipleDataSources() {
		ProjectBuilder projectBuilder = new ProjectBuilder();
		FormBuilder formBuilder = projectBuilder.addForm("test");
		formBuilder.setExternalDataSource("MyDataSource");
		formBuilder.addFib("My FIB Question:", "fibfield", 20);
		formBuilder.addMcWithAlternateLabel("mcqfield", "My Question",
				new String[] { "apple", "oranges", "bananas" });
		formBuilder.addDeclaredFields("declared1", "declared2");

		UserProject userProject = new UserProject(projectBuilder.build(), user,
				"testproject");
		domain.projects().put(userProject);

		Project sharedStorage = domain.users().getSharedStorageForUser(user);
		assertNotNull(sharedStorage);
		List<DataSource> dataSources = sharedStorage.getDataSources();
		assertNotNull(dataSources);
		assertEquals(1, dataSources.size());

		DataSource dataSource = dataSources.get(0);
		assertEquals("MyDataSource", dataSource.getName());
		assertNotNull(dataSource.getFields());
		assertEquals(4, dataSource.getFields().size());

		user = domain.users().get(user.getDatabaseId());

		// --- Modify the form and redeploy.
		projectBuilder = new ProjectBuilder();
		formBuilder = projectBuilder.addForm("test");
		formBuilder.setExternalDataSource("MyDataSource 2");
		formBuilder.addFib("My FIB Question:", "fibfield", 20);
		formBuilder.addFib("My Second FIB", "fibfield2", 20);

		userProject = new UserProject(projectBuilder.build(), user,
				"testproject");
		domain.projects().put(userProject);

		sharedStorage = domain.users().getSharedStorageForUser(user);
		assertNotNull(sharedStorage);
		dataSources = sharedStorage.getDataSources();
		assertNotNull(dataSources);
		assertEquals(2, dataSources.size());

		dataSource = dataSources.get(0);
		assertEquals("MyDataSource", dataSource.getName());
		assertNotNull(dataSource.getFields());
		assertEquals(4, dataSource.getFields().size());

		dataSource = dataSources.get(1);
		assertEquals("MyDataSource 2", dataSource.getName());
		assertNotNull(dataSource.getFields());
		assertEquals(2, dataSource.getFields().size());

		assertEquals(StringField.class, ((List<StoredField>) dataSource
				.getFields()).get(0).getClass());
		StringField field1 = (StringField) ((List<StoredField>) dataSource
				.getFields()).get(0);
		assertEquals("fibfield", field1.getName());

		assertEquals(StringField.class, ((List<StoredField>) dataSource
				.getFields()).get(1).getClass());
		StringField field2 = (StringField) ((List<StoredField>) dataSource
				.getFields()).get(1);
		assertEquals("fibfield2", field2.getName());
	}
}
