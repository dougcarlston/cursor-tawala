package com.tawala.web;

import javax.servlet.http.HttpServletRequest;

import org.dom4j.Element;

public class ApiErrorResponse extends ApiResponse {

    public ApiErrorResponse(String id, String message) {
        super();
        addErrorMessage(id, message);
    }

    protected void addContents(Element root, HttpServletRequest request) {
    }

    protected String status() {
        return "failure";
    }
}
