package com.tawala.web;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;

import mock.javax.servlet.http.FakeHttpSession;

import com.tawala.project.FormSegment;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;

public class FakeRequest extends Request {

    public FakeRequest(boolean isPost, String... parameters) {
        super(isPost, new ParameterMap(parameters));
        addRequiredParameters(isPost);
    }

	private void addRequiredParameters(boolean isPost) {
		if(isPost && getParameter(FormSegment.SEGMENT_ID) == null) {
        	setParameter(FormSegment.SEGMENT_ID, "0");
        }
	}

    public FakeRequest(boolean isPost,
            LinkedHashMap<String, List<String>> parameters) {
        super(isPost, parameters);
        addRequiredParameters(isPost);
    }

    public FakeRequest(boolean isPost, UserProject project, String formName,
            String... parameters) {
        super(project.getUrlToForm(EntryPointType.REAL_PROJECT, project
				.getProject().getForm(formName)), isPost, project.getUser()
                .getId(), project.getName(), new ParameterMap(parameters),
                new FakeHttpSession()); // TODO: add _segment & related params
    }

    // TODO: move to main code and use in request?
    @SuppressWarnings("serial")
    private static class ParameterMap extends
            LinkedHashMap<String, List<String>> {
        public ParameterMap(String[] parameters) {
            for (int i = 0; i < parameters.length; i += 2) {
                String name = parameters[i];
                String value = parameters[i + 1];
                add(name, value);
            }
        }

        private void add(String key, String value) {
            if (!containsKey(key)) {
                put(key, new ArrayList<String>());
            }
            get(key).add(value);
        }
    }
}
