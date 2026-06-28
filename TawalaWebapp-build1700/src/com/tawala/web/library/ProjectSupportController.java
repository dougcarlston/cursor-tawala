package com.tawala.web.library;

import javax.servlet.http.HttpServletRequest;

abstract public class ProjectSupportController {
    public static String PARAMETER_PROJECT_ID = "id";
        
    /**
     * @param request
     * @return
     * @throws IllegalStateException
     */
    public final static long extractProjectId(HttpServletRequest request)
            throws IllegalStateException {
        String projectId = request.getParameter(PARAMETER_PROJECT_ID);
        if (projectId == null) {
            throw new IllegalStateException("Couldn't find parameter '"
                    + PARAMETER_PROJECT_ID + "'.");
        }
        return Long.parseLong(projectId);
    }
}