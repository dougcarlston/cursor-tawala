package com.tawala.web;

import javax.servlet.http.HttpServletRequest;

import org.dom4j.Element;

import com.tawala.project.Form;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;

public class FormPreviewResponse extends ApiResponse {
    private final String formName;
    private final UserProject project;

    public FormPreviewResponse(UserProject project,
            String formName) {
        super();
        this.formName = formName;
        this.project = project;
    }

    protected void addContents(Element root, HttpServletRequest request) {
        Element formPreview = root.addElement("formPreview");
        Form form = project.getProject().getForm(formName);
        if(form == null) {
        	throw new IllegalArgumentException("Unable to find form \"" + formName + "\" in the project.");
        }
		String urlString = project.getUrlToForm(EntryPointType.FORM_PREVIEW, form);
        formPreview.addAttribute("url", urlString);
    }

    protected String status() {
        return "success";
    }
}
