package com.tawala.web.controller;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.event.Event;
import com.tawala.event.EventService;
import com.tawala.web.Response;
import com.tawala.web.SendFileResponse;
import com.tawala.web.WorldInitializer;

public class StaticFileHandler implements Controller {
    private String file;

    public StaticFileHandler(String file) {
        this.file = file;
    }

    public ModelAndView handleRequest(HttpServletRequest request, HttpServletResponse response) throws Exception {
        Response responseObject = new SendFileResponse(file);
        responseObject.process(request, response, WorldInitializer.getDefaultWorld());
        
        EventService.createEvent(new Event("FileDownload", request, file));

        return null;
    }
}
