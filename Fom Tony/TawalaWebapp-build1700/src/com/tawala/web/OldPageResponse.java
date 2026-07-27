package com.tawala.web;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.tawala.World;
import com.tawala.web.admin.UrgentMessage;
import com.tawala.web.oldhtml.OldPage;
import com.tawala.web.oldhtml.RenderingContext;

public class OldPageResponse extends Response {
    private final OldPage page;

    public OldPageResponse(OldPage page) {
        this.page = page;
    }

    public OldPage getPage() {
        return page;
    }
    
	@Override
	public void handleUrgentNotificationMessage(UrgentMessage urgentMessage) {
		page.setUrgentMessage(urgentMessage);
	}


    public void handle(HttpServletRequest request, HttpServletResponse response, World world) throws IOException {
		new CachingContentGenerator().cacheResponse(response, 0);

        response.setStatus(200);
        response.setContentType("text/html");
        page.render(response.getWriter(), new RenderingContext());
    }
}
