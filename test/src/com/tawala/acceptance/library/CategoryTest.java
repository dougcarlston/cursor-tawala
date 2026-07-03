package com.tawala.acceptance.library;

import java.util.Collection;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.WebRequest;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.library.Category;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.library.EditCategoryController;

public class CategoryTest extends AcceptanceTestCase {
	private static final String BCD_CATEGORY_NAME = "BCD Category";
	private static final String DCE_CATEGORY_NAME = "DCE Category";
	private static final String ABC_CATEGORY_NAME = "ABC Category";

	private static final String STARTING_ROOT_CATEGORY = "TestRootCategory";
	private Category startCategory;

	public CategoryTest() {
		setCategoryNamesToDelete(new String[] { ABC_CATEGORY_NAME,
				BCD_CATEGORY_NAME, DCE_CATEGORY_NAME, STARTING_ROOT_CATEGORY });
	}

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		startCategory = new Category(ProjectLibrary.COMMUNITY_LIBRARY,
				STARTING_ROOT_CATEGORY, "Test");
		ProjectLibraryService.createCategory(startCategory, projectOwner);
	}

	public void testCategoryCreation() throws RobotException {
		createAndValidateCategory(ABC_CATEGORY_NAME, "", "test description");
		createAndValidateCategory(BCD_CATEGORY_NAME, Long
				.toString(startCategory.getId()), "BCD description");
	}

	private void createAndValidateCategory(String name, String parentId,
			String description) throws RobotException {
		bot.logOut();
		bot.logInAs(projectOwner.getId(), projectOwner.getPassword());

		bot.go(WellKnown.urls.getLibraryEditCategory() + "?"
				+ EditCategoryController.PARAMETER_CATEGORY_ID + "="
				+ startCategory.getId() + "&"
				+ EditCategoryController.PARAMETER_LIBRARY_ID + "="
				+ startCategory.getLibrary().getId());

		WebForm form = bot.getForm("newCategoryForm");
		assertNotNull(form);
		WebRequest request = form.newUnvalidatedRequest();
		request.setParameter("name", name);
		request.setParameter("parent", parentId);
		request.setParameter("description", description);

		request.setParameter(EditCategoryController.PARAMETER_NEW, "xxx");
		bot.go(request);

		Collection<Category> categories = ProjectLibraryService
				.getAllCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY);

		boolean found = false;

		for (Category category : categories) {
			if (category.getName().equals(name)) {
				assertEquals(description, category.getDescription());

				if (parentId.equals("")) {
					assertNull("parent", category.getParent());
				} else {
					assertNotNull("parent", category.getParent());
				}

				found = true;
				break;
			}
		}

		assertTrue("found", found);
	}

	public void testCategoryEditing() throws RobotException {
		createAndValidateCategory(ABC_CATEGORY_NAME, "", "test description");

		editAndValidateCategory(ABC_CATEGORY_NAME, DCE_CATEGORY_NAME, "",
				"New description");

		editAndValidateCategory(DCE_CATEGORY_NAME, DCE_CATEGORY_NAME, Long
				.toString(startCategory.getId()),
				"Now the child of the default category");

		editAndValidateCategory(DCE_CATEGORY_NAME, ABC_CATEGORY_NAME, Long
				.toString(startCategory.getId()), "Revert the name");

	}

	private void editAndValidateCategory(String oldName, String name,
			String parentId, String description) throws RobotException {

		int previousCategoryCount = ProjectLibraryService.getAllCategoriesFor(
				ProjectLibrary.COMMUNITY_LIBRARY).size();

		Category category = findCategoryByName(oldName);
		assertNotNull("category", category);

		bot.logOut();
		bot.logInAs(projectOwner.getId(), projectOwner.getPassword());

		bot.go(WellKnown.urls.getLibraryEditCategory() + "?"
				+ EditCategoryController.PARAMETER_CATEGORY_ID + "="
				+ category.getId() + "&"
				+ EditCategoryController.PARAMETER_LIBRARY_ID + "="
				+ startCategory.getLibrary().getId());

		WebForm form = bot.getForm("editCategoryForm");
		WebRequest request = form.newUnvalidatedRequest();
		request.setParameter("name", name);
		request.setParameter("parent", parentId);
		request.setParameter("description", description);
		request.setParameter(EditCategoryController.PARAMETER_UPDATE, "xxx");
		bot.go(request);

		assertEquals(previousCategoryCount, ProjectLibraryService
				.getAllCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY).size());

		category = ProjectLibraryService.findCategoryById(category.getId());
		assertEquals(name, category.getName());
		assertEquals(description, category.getDescription());

		if (parentId.equals("")) {
			assertNull("parent", category.getParent());
		} else {
			assertNotNull("parent", category.getParent());
		}
	}

	private Category findCategoryByName(String name) {
		for (Category nextCategory : ProjectLibraryService
				.getAllCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY)) {
			if (nextCategory.getName().equals(name)) {
				return nextCategory;
			}
		}
		return null;
	}

	public void testCategoryDeleting() throws RobotException {
		createAndValidateCategory(ABC_CATEGORY_NAME, "", "test description");

		Category category = findCategoryByName(ABC_CATEGORY_NAME);

		bot.logOut();
		bot.logInAs(projectOwner.getId(), projectOwner.getPassword());

		int previousCatetoryCount = ProjectLibraryService.getAllCategoriesFor(
				ProjectLibrary.COMMUNITY_LIBRARY).size();

		bot.go(WellKnown.urls.getLibraryEditCategory() + "?"
				+ EditCategoryController.PARAMETER_CATEGORY_ID + "="
				+ category.getId() + "&"
				+ EditCategoryController.PARAMETER_LIBRARY_ID + "="
				+ startCategory.getLibrary().getId());

		WebForm form = bot.getForm("deleteCategoryForm");

		WebRequest request = form.newUnvalidatedRequest();
		request.setParameter(
				EditCategoryController.PARAMETER_DELETE, "xxx");
		bot.go(request);

		assertEquals(previousCatetoryCount - 1, ProjectLibraryService
				.getAllCategoriesFor(ProjectLibrary.COMMUNITY_LIBRARY).size());
	}
}
