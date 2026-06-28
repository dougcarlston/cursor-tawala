package com.tawala.web.library;

import java.util.Collections;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.message.Message;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.project.library.ProjectLibraryService;

public class ManageLibraryController implements Controller {
    public final static String PARAMETER_REINDEX_LIBRARY = "reindexLibrary";
    public final static String PARAMETER_RESET_CATEGORY_PROJECT_COUNT = "reset_project_count";

    public ModelAndView handleRequest(HttpServletRequest request,
            HttpServletResponse response) throws Exception {
        ModelAndView result = new ModelAndView("library.manage");

        if (request.getParameter(PARAMETER_REINDEX_LIBRARY) != null) {
            ProjectLibraryService.rebuildIndexes();
            result.addObject("messages", Collections.singletonList(new Message(
                    "library.reindexing.done")));
        } else if (request.getParameter(PARAMETER_RESET_CATEGORY_PROJECT_COUNT) != null) {
        	for (ProjectLibrary projectLibrary : ProjectLibrary.getLibrariesAvailableToAdmins()) {
        		ProjectLibraryService.resetCategoryProjectCount(projectLibrary);	
			}
            
            result.addObject("messages", Collections.singletonList(new Message(
                    "library.category.project.count.resetting.done")));
        }

        return result;
    }
}
