package com.scissor.webrobot;

import java.io.IOException;

import javax.servlet.ServletConfig;
import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

public class SampleServlet extends HttpServlet {
    private static final long serialVersionUID = 1L;

    private String text;
    private String contentType;

    public void init(ServletConfig servletConfig) throws ServletException {
        super.init(servletConfig);
        this.text = servletConfig.getInitParameter("text");
        this.contentType = servletConfig.getInitParameter("contentType");
        if (contentType == null) contentType = "text/html";

    }

    protected void doGet(HttpServletRequest request, HttpServletResponse response)
            throws ServletException, IOException {
        response.setContentType(contentType);
        response.getWriter().print(text);
    }
}
