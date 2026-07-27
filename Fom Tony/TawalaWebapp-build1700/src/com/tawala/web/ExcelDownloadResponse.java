package com.tawala.web;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;

import com.tawala.World;
import com.tawala.web.admin.UrgentMessage;

public class ExcelDownloadResponse extends Response {
    private HSSFWorkbook workbook;
    private String projectName;

    public ExcelDownloadResponse(HSSFWorkbook workbook, String projectName) {
        this.workbook = workbook;
        this.projectName = projectName;
    }

    public void handle(HttpServletRequest request,
            HttpServletResponse response, World world)
            throws IOException {
        response.setContentType("application/vnd.ms-excel");
        response.setHeader("Content-Disposition", "attachment; filename=\""
                + projectName + ".xls\";");

        workbook.write(response.getOutputStream());
    }

	@Override
	public void handleUrgentNotificationMessage(UrgentMessage urgentMessage) {
		// Do nothing
	}
}
