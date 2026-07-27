package com.tawala.web;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.tawala.World;
import com.tawala.web.admin.UrgentMessage;

public class CSVDownloadResponse extends Response {
    private Download file;
    private String projectName;

    public CSVDownloadResponse(Download file, String projectName) {
        this.file = file;
        this.projectName = projectName;
    }

    public void handle(HttpServletRequest request,
            HttpServletResponse response, World world)
            throws IOException {
        response.setContentType("application/octet-stream");
        response.setHeader("Content-Disposition", "attachment; filename=\""
                + projectName + ".csv\";");

        file.render(response.getWriter());
    }

	@Override
	public void handleUrgentNotificationMessage(UrgentMessage urgentMessage) {
		// Do nothing
	}
}
