package com.tawala.web;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.util.List;
import java.util.Map;

import javax.servlet.http.HttpServletRequest;

import org.dom4j.Element;

import com.scissor.ImpossibleException;
import com.tawala.project.Form;
import com.tawala.project.UserProject;

public class DeploymentQueryResponse extends ApiResponse {
    protected final String userId;
    protected final List<UserProject> projects;

    public DeploymentQueryResponse(String userId, List<UserProject> projects) {
        super();
        this.userId = userId;
        this.projects = projects;
    }

    protected void addContents(Element root, HttpServletRequest request) {
        Element deployments = root.addElement("deployments");
        deployments.addAttribute("user", userId);
        for (UserProject project : projects) {
            Element deployment = deployments.addElement("deployment");
            deployment.addAttribute("project", project.getName());
            for (Map.Entry<Form, String> entry: project.getEntryPointURLs().entrySet()) {
                Element startpoint = deployment.addElement("startpoint");
                startpoint.addAttribute("form", entry.getKey().getName());
                startpoint.addAttribute("url", entry.getValue());
			}
        }
    }

    protected String status() {
        return "success";
    }

    protected String urlEncode(String userId) {
        try {
            return URLEncoder.encode(userId, "UTF-8");
        } catch (UnsupportedEncodingException e) {
            throw new ImpossibleException(e);
        }
    }
}
