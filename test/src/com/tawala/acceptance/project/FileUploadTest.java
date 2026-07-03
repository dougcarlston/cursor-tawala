package com.tawala.acceptance.project;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.Arrays;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.protocol.UploadFileSpec;
import com.scissor.StreamCopier;
import com.scissor.webrobot.RobotException;
import com.tawala.acceptance.AcceptanceTestCase;
import com.tawala.project.FormSegment;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.tawala.web.WorldInitializer;

public class FileUploadTest extends AcceptanceTestCase {
	public void testFileUpload() throws RobotException, IOException {
		ProjectBuilder projectBuilder = new ProjectBuilder();

		FormBuilder form = projectBuilder.addForm("Form1");
		form.addFileUpload("File", "Select the file to upload: ", true);

		form.addBreak();
		form.addTextWithFields("Here's the link to the file: ",
				"<<Form1:File>>");

		UserProject userProject = new UserProject(projectBuilder.build(),
				projectOwner, "test of file upload");

		WorldInitializer.getDefaultWorld().domain().projects().put(userProject);

		//--- Test required parameter.
		bot.go(userProject);

		WebForm webForm = bot.getForm(FormSegment.TAWALA_PROJECT_FORM_NAME);
		bot.submit(webForm);
		
		assertContains("Please answer the marked questions before continuing.", bot.getPageText());

		//--- Test successful upload.
		webForm = bot.getForm(FormSegment.TAWALA_PROJECT_FORM_NAME);
		byte[] data = new byte[] { 23, 12, 54, 99, 34, 12, 94 };
		webForm.setParameter("File", new UploadFileSpec[] { new UploadFileSpec(
				"c:\\temp\\photo.jpg", new ByteArrayInputStream(data),
				"image/jpeg") });
		bot.submit(webForm);

		Pattern pattern = Pattern
				.compile("Here's the link to the file: (/i/.{15}/photo.jpg)");
		Matcher matcher = pattern.matcher(bot.getPageText());

		assertTrue(matcher.find());
		String link = matcher.group(1);

		bot.go(link);

		assertEquals("image/jpeg", bot.getContentType());
		assertEquals(data.length, bot.getContentLength());

		ByteArrayOutputStream output = new ByteArrayOutputStream();
		StreamCopier.copy(bot.lastResponse().getInputStream(), output);
		assertTrue(Arrays.equals(data, output.toByteArray()));
	}
}
