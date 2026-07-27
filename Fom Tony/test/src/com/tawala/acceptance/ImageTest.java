package com.tawala.acceptance;

import java.io.IOException;
import java.io.InputStream;

import org.xml.sax.SAXException;

import com.meterware.httpunit.WebImage;
import com.scissor.webrobot.RobotException;
import com.tawala.project.Image;
import com.tawala.project.ImageInstance;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.builder.FormBuilder;
import com.tawala.project.builder.ImageBuilder;
import com.tawala.project.builder.ImageInstanceBuilder;
import com.tawala.project.builder.ProjectBuilder;
import com.thoughtworks.xstream.core.util.Base64Encoder;

public class ImageTest extends AcceptanceTestCase {

    public void testImageRetrievalInRealProject() throws RobotException,
            SAXException, IOException {
        byte[] imageData = new byte[] { 01, 34, 90, 23, 00, 00, 12, 45 };

        ProjectBuilder builder = new ProjectBuilder();

        FormBuilder formBuilder = builder.addForm("FirstForm");
        formBuilder.addTextWithImage("Here comes the image:",
                new ImageInstanceBuilder("image1", 40, 60));

        ImageBuilder imageBuilder = new ImageBuilder();
        imageBuilder.addImage("image1", new ImageBuilder.ImageData(
                Image.Data.Format.PNG, new Base64Encoder().encode(imageData)));
        builder.add(imageBuilder);

        Project project = builder.build();

        UserProject userProject = new UserProject(project, projectOwner,
                "imagetest");
        world.domain().projects().put(userProject);

        bot.go(userProject);

        WebImage image = bot.lastResponse().getImageWithAltText(
                ImageInstance.DEFAULT_IMAGE_ALT_NAME);
        assertNotNull(image);

        bot.gotoImage(userProject, image.getSource());

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
}
