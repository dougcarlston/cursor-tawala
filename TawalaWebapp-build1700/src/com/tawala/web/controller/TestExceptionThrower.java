package com.tawala.web.controller;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

public class TestExceptionThrower implements Controller {
    public ModelAndView handleRequest(HttpServletRequest request,
            HttpServletResponse response) throws Exception {
        throw new IllegalStateException(
                "This is a test exception being thrown."
                        + " You can safely ignore it.");
    }
}
