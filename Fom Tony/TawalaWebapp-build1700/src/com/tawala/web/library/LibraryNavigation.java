package com.tawala.web.library;

import java.io.IOException;

import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;

import com.tawala.web.controller.WellKnown;

public class LibraryNavigation {
    public static ModelAndView navigateToProjectDetailPage(HttpServletResponse response, long projectId) throws IOException {
        response.sendRedirect(WellKnown.urls.getLibraryProjectDetailView()
                + "?" + ViewProjectDetailsController.PARAMETER_ID + "="
                + projectId);
    
        return null;
    }
    
    public static ModelAndView navigateToSearchPage(
            HttpServletResponse response) throws IOException {
        response.sendRedirect(WellKnown.urls.getLibrarySearch());
        return null;
    }
}
