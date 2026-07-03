package com.tawala.project;

import java.io.PrintWriter;
import java.io.StringWriter;

import org.springframework.mock.web.MockServletConfig;
import org.springframework.mock.web.MockServletContext;

import com.tawala.TestCase;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ImageBuilder;
import com.tawala.project.builder.ImageInstanceBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.commands.FakeExecutionContext;
import com.tawala.web.WorldInitializer;
import com.tawala.web.oldhtml.Html;
import com.tawala.web.oldhtml.OldPage;
import com.tawala.web.oldhtml.RenderingContext;
import com.tawala.web.project.ProjectController;

public class ImageTest extends TestCase {
	@Override
	protected void setUp() throws Exception {
		super.setUp();
		new WorldInitializer().init(new MockServletConfig(
				new MockServletContext()));
	}

	public void testImageBasics() {
		ProjectBuilder builder = new ProjectBuilder();
		ImageBuilder imageBuilder = new ImageBuilder();
		imageBuilder.addImage("image1", new ImageBuilder.ImageData(
				Image.Data.Format.PNG, "aabb"));
		builder.add(imageBuilder);

		Project project = builder.build();
		assertNotNull(project.getImage("image1"));
	}

	public void testImageReferenceWithinParagraph() {
		ProjectBuilder builder = new ProjectBuilder();

		FormBuilder formBuilder = builder.addForm("First Form");
		formBuilder.addTextWithImage("Here comes the image:",
				new ImageInstanceBuilder("image1", 40, 60));

		ImageBuilder imageBuilder = new ImageBuilder();
		imageBuilder.addImage("image1", new ImageBuilder.ImageData(
				Image.Data.Format.PNG, "aabb"));
		builder.add(imageBuilder);

		Project project = builder.build();
		Form form = project.getForm("First Form");

		UserProject userProject = new UserProject(project, null, "test");
		userProject.setUniqueRandomId("123456");

		FakeExecutionContext context = new FakeExecutionContext(
				WorldInitializer.getDefaultWorld().domain(), userProject, form);

		// --- Test short style URL generation to be use on all the pages.
		OldPage page = form.firstPage(context);
		assertContains("<img src=\"" + ProjectController.IMAGE_PATH + "image1"
				+ Project.IMAGE_SUFFIX
				+ "\" alt=\"Project Image\" width=\"40px\" height=\"60px\" />",
				renderAsHtml(page));
	}

	private String renderAsHtml(Html page) {
		StringWriter output = new StringWriter();
		page.render(new PrintWriter(output), new RenderingContext());
		return output.toString();
	}
}
