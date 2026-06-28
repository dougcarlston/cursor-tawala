package com.tawala.web;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;


import com.tawala.World;
import com.tawala.project.Image;
import com.tawala.web.admin.UrgentMessage;

public class ImageResponse extends Response {
    private Image.Data imageData;
    
    public ImageResponse(Image.Data imageData) {
        this.imageData = imageData;
    }

    @Override
    protected void handle(HttpServletRequest request,
            HttpServletResponse response, World world) throws IOException {
        
        response.setContentType(imageData.getFormat().getMimeType());
        response.setContentLength(imageData.getImageData().length);
        
        //--- Set up the caching headers.
        int seconds = 20000;
        CachingContentGenerator contentGenerator = new CachingContentGenerator();
        contentGenerator.cacheResponse(response, seconds);

        response.getOutputStream().write(imageData.getImageData());
    }

	@Override
	public void handleUrgentNotificationMessage(UrgentMessage urgentMessage) {
		//--- Do nothing
	}
}
