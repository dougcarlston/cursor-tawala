package com.tawala.acceptance;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.InputStream;

import org.xml.sax.SAXException;

import com.meterware.httpunit.PostMethodWebRequest;
import com.meterware.httpunit.WebImage;
import com.meterware.httpunit.WebRequest;
import com.meterware.httpunit.WebResponse;
import com.meterware.servletunit.ServletUnitClient;
import com.scissor.webrobot.RobotException;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.Image;
import com.tawala.project.ImageInstance;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ImageBuilder;
import com.tawala.project.builder.ImageInstanceBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.project.builder.SkipBlockBuilder;
import com.tawala.project.commands.SkipBlock;
import com.tawala.web.controller.WellKnown;
import com.thoughtworks.xstream.core.util.Base64Encoder;

public class PreviewFormTest extends AcceptanceTestCase {

	public void testPreviewFormBasics() throws RobotException, IOException,
			SAXException {

		String projectName = "abc";
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm("Hello");
		formBuilder.addText("Hello, World!");

		requestApi("previewForm", builder.toStringAsElement(projectName),
				"Hello");

		assertEquals("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n"
				+ "<response status=\"success\">\n"
				+ "  <formPreview url=\"" +
				
						"http://ignored"
				+ WellKnown.urls.getFormPreviewUrlPrefix() + "/"
				+ projectOwner.getId() + "-" + projectName + "/Hello" +
						"\"/>\n"
				+ "</response>\n", response.getText());

		String urlString = new PreviewUrl(parseConfig(response.getText()))
				.getUrlString();
		bot.go(urlString);

		assertContains("Hello, World!", bot.getPageText());
		assertContains("/css/project/default/project.css", bot.getPageText());

	}

	public void testMultisegmentPreview() throws RobotException, IOException,
			SAXException {

		String projectName = "abc";
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm("Hello");
		formBuilder.addFib("FIB on the first page", "question", 25);

		formBuilder.addBreak();

		formBuilder.addMc("An MCQ on the second page", "apples", "oranges");

		SkipBlockBuilder skipBuilder = formBuilder.addSkip();
		skipBuilder.addSkip(SkipBlock.SKIP_TO_END);

		formBuilder.addText("Third page");

		requestApi("previewForm", builder.toStringAsElement(projectName),
				"Hello");

		assertEquals("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n"
				+ "<response status=\"success\">\n"
				+ "  <formPreview url=\"http://ignored"
				+ WellKnown.urls.getFormPreviewUrlPrefix() + "/"
				+ projectOwner.getId() + "-" + projectName + "/Hello\"/>\n"
				+ "</response>\n", response.getText());

		String urlString = new PreviewUrl(parseConfig(response.getText()))
				.getUrlString();
		bot.go(urlString);

		assertContains("FIB on the first page", bot.getPageText());
		assertContains("An MCQ on the second page", bot.getPageText());
		assertContains("Third page", bot.getPageText());

		assertContains(">Page Break<", bot.getPageText());
		assertContains(
				"<input class=\"button\" type=\"submit\" value=\"submit\" disabled=\"true\"></input>",
				bot.getPageText());
	}

	public void testBlankFormPreview() throws RobotException, IOException,
			SAXException {

		String projectName = "abc";
		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm("Hello");
		SkipBlockBuilder skipBuilder = formBuilder.addSkip();
		skipBuilder.addSkip(SkipBlock.SKIP_TO_END);

		requestApi("previewForm", builder.toStringAsElement(projectName),
				"Hello");

		assertEquals("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n\n"
				+ "<response status=\"success\">\n"
				+ "  <formPreview url=\"http://ignored"
				+ WellKnown.urls.getFormPreviewUrlPrefix() + "/"
				+ projectOwner.getId() + "-" + projectName + "/Hello\"/>\n"
				+ "</response>\n", response.getText());

		String urlString = new PreviewUrl(parseConfig(response.getText()))
				.getUrlString();
		bot.go(urlString);

		assertDoesntContain(">Page Break<", bot.getPageText());
		assertContains("This form didn't produce any output!", bot
				.getPageText());
	}

	public void testPreviewFormWithImage() throws RobotException, IOException,
			SAXException {
		byte[] imageData = new byte[] { 01, 34, 90, 23, 00, 00, 12, 45, 23 };

		ProjectBuilder builder = new ProjectBuilder();
		FormBuilder formBuilder = builder.addForm("Hello");
		formBuilder.addTextWithImage("Here comes the image:",
				new ImageInstanceBuilder("image1", 40, 60));

		ImageBuilder imageBuilder = new ImageBuilder();
		imageBuilder.addImage("image1", new ImageBuilder.ImageData(
				Image.Data.Format.PNG, new Base64Encoder().encode(imageData)));
		builder.add(imageBuilder);

		requestApi("previewForm",
				builder.toStringAsElement("projectWithImage"), "Hello");

		String urlString = new PreviewUrl(parseConfig(response.getText()))
				.getUrlString();
		bot.go(urlString);

		assertContains("Here comes the image:", bot.getPageText());

		WebImage image = bot.lastResponse().getImageWithAltText(
				ImageInstance.DEFAULT_IMAGE_ALT_NAME);
		assertNotNull(image);

		bot.gotoFormPreviewImage(new UserProject(builder.build(), projectOwner,
				"projectWithImage"), image.getSource());

		assertEquals(Image.Data.Format.PNG.getMimeType(), bot.lastResponse()
				.getContentType());

		assertEquals(imageData.length, bot.lastResponse().getContentLength());
		InputStream stream = bot.lastResponse().getInputStream();

		byte[] retrievedData = new byte[imageData.length];
		stream.read(retrievedData);

		for (int i = 0; i < retrievedData.length; i++) {
			assertEquals(imageData[i], retrievedData[i]);
		}

	}

	private class PreviewUrl {

		private String urlString;

		public PreviewUrl(ConfigElement config) {
			this(config.child("formPreview").attribute("url").stringValue());
		}

		private PreviewUrl(String urlString) {
			this.urlString = urlString;
		}

		public String getUrlString() {
			return urlString;
		}
	}

	private void requestApi(String command, String contents, String formName)
			throws IOException, SAXException {
		requestApi(command, contents, formName, projectOwner.getId(),
				projectOwner.getPassword());
	}

	private void requestApi(String command, String contents, String formName,
			String userId, String password) throws IOException, SAXException {
		getResponse(postXml("<request type=\"" + command + "\" form=\""
				+ formName + "\" protocol=\"1.0\">\n" + "<credentials user=\""
				+ userId + "\" password=\"" + password + "\" />\n" + contents
				+ "</request>"), client);
		assertEquals(200, response.getResponseCode());
	}

	protected WebResponse getResponse(WebRequest request,
			ServletUnitClient client) throws IOException, SAXException {
		response = client.getResponse(request);
		return response;
	}

	private PostMethodWebRequest postXml(String xml) {
		return new PostMethodWebRequest("http://ignored/client",
				new ByteArrayInputStream(xml.getBytes()), "text/xml");
	}
}
