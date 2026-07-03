package com.tawala.component.function;

import java.io.IOException;
import java.util.Collections;

import junit.framework.TestCase;

import com.tawala.UsersPersistentBunchImpl;
import com.tawala.World;
import com.tawala.component.runtime.PopularChoiceAlgorithmTest;
import com.tawala.domain.User;
import com.tawala.domain.UserTest;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.Value;
import com.tawala.project.commands.BooleanExpression;
import com.tawala.project.commands.RecordSelector;
import com.tawala.project.commands.RecordSelector.FormDataProvider;

public class PopularChoiceCountFunctionTest extends TestCase {
	private World world;
	private User owner;
	private Project project;
	private UserProject userProject;

	@Override
	protected void setUp() throws Exception {
		super.setUp();
		owner = UserTest.aUser();

		world = new World("/tmp", new UsersPersistentBunchImpl());
		world.domain().users().addOrSave(owner);

		project = PopularChoiceAlgorithmTest.buildGetTogetherProject();
		userProject = new UserProject(project, owner, "test");
	}

	public void testWrongFormFields() {
		try {
			new PopularChoiceCountFunction.Runtime("Some non-existent field",
					1, null).execute(PopularChoiceAlgorithmTest.newContext(
					world, project, owner));
			fail("Should have failed with IllegalArgumentException");
		} catch (IllegalArgumentException e) {
			// Expected
		}
	}

	public void testCorrectExecution() throws IOException {
		// --- No data - 0 results.
		validateResult(1, 0);

		PopularChoiceAlgorithmTest.addData(world, userProject,
				new String[] { "a" }, "a");

		validateResult(1, 1);
		validateResult(2, 0);
		validateResult(3, 0);

		PopularChoiceAlgorithmTest.addData(world, userProject, new String[] { "a",
				"b", "c" }, "b");

		validateResult(1, 2);
		validateResult(2, 1);
		validateResult(3, 1);
	}

	private void validateResult(int rank, int expectedValue) {
		PopularChoiceCountFunction.Runtime function = getFunctionWithRank(rank);

		Value result = function.execute(PopularChoiceAlgorithmTest.newContext(
				world, project, owner));
		assertEquals(String.valueOf(expectedValue), result.toString());
	}

	private PopularChoiceCountFunction.Runtime getFunctionWithRank(int rank) {
		PopularChoiceCountFunction.Runtime function = new PopularChoiceCountFunction.Runtime(
				PopularChoiceAlgorithmTest.FORM_NAME + ":"
						+ PopularChoiceAlgorithmTest.ABLE_TO_ATTEND_FIELD,
				rank,
				new RecordSelector(
						Collections
								.singletonList((FormDataProvider) new RecordSelector.CurrentProjectFormDataProvider(
										PopularChoiceAlgorithmTest.FORM_NAME)),
						BooleanExpression.TRUE, RecordSelector.DEFAULT_RECORD_LIST_NAME));
		return function;
	}
}
