package com.tawala.web;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.Arrays;
import java.util.Collections;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import javax.servlet.RequestDispatcher;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpSession;

import org.springframework.mock.web.MockHttpServletRequest;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.web.multipart.MultipartRequest;

import com.tawala.project.FormSegment;

public class Request {
	private final String method;
	private final LinkedHashMap<String, List<String>> parameters;
	private final String contentType;
	private final String content;
	private String serverName;

	private final Path path;
	private final HttpServletRequest httpServletRequest;
	private final Map<String, MultipartFile> uploadedFiles;

	@SuppressWarnings("unchecked")
	public Request(HttpServletRequest httpReq) throws IOException {
		this.httpServletRequest = httpReq;
		this.method = httpReq.getMethod();
		this.parameters = parseRegularRequest(httpReq);
		this.contentType = httpReq.getContentType();
		// TODO: figure out what to do about binary content
		this.content = extractContentsAsString(httpReq);

		// TODO: there must be a tidier way to do this
		this.path = new Path(httpReq);

		this.serverName = httpReq.getServerName();

		if (MultipartRequest.class.isAssignableFrom(httpReq.getClass())) {
			uploadedFiles = ((MultipartRequest) httpReq).getFileMap();
		} else {
			uploadedFiles = null;
		}
	}

	/**
	 * Used by testing subclasses.
	 */
	protected Request(boolean isPost,
			LinkedHashMap<String, List<String>> parameters) {
		this.httpServletRequest = new MockHttpServletRequest();
		this.method = (isPost ? "POST" : "GET");
		this.parameters = parameters;
		this.contentType = "text/plain";
		this.content = ""; // TODO: verify that this is correct for basic
		// requests
		path = new Path();
		this.uploadedFiles = null;
	}

	/**
	 * Used by testing subclasses.
	 */
	protected Request(String path, boolean isPost, String userId,
			String projectName, LinkedHashMap<String, List<String>> parameters,
			HttpSession session) {
		this.httpServletRequest = null;
		this.method = (isPost ? "POST" : "GET");
		this.parameters = parameters;
		this.contentType = "text/plain";
		this.content = ""; // TODO: verify that this is correct for basic
		// requests
		this.path = new Path(path);
		this.uploadedFiles = null;
	}

	/**
	 * Ignoring character encoding and the possibly binary nature of the data,
	 * get contents as a string.
	 */
	private String extractContentsAsString(HttpServletRequest httpReq)
			throws IOException {
		// TODO: handle character encoding
		BufferedReader reader = new BufferedReader(new InputStreamReader(
				httpReq.getInputStream()));
		char[] cbuf = new char[1024];
		int count;
		StringBuffer content = new StringBuffer();
		while ((count = reader.read(cbuf, 0, cbuf.length)) > -1) {
			content.append(cbuf, 0, count);
		}
		return content.toString();
	}

	public String getMethod() {
		return method;
	}

	public List<String> getParameterValues(String paramName) {
		return parameters.get(paramName);
	}

	public Path path() {
		return path;
	}

	@SuppressWarnings("unchecked")
	private LinkedHashMap<String, List<String>> parseRegularRequest(
			HttpServletRequest httpReq) {
		LinkedHashMap<String, List<String>> params = new LinkedHashMap<String, List<String>>();

		Map httpParams = httpReq.getParameterMap();
		for (Object key : httpParams.keySet()) {
			String[] values = (String[]) httpParams.get(key);
			params.put((String) key, Arrays.asList(values));
		}
		return params;
	}

	public String getContentType() {
		return contentType;
	}

	public String getContentAsString() {
		return content;
	}

	public String getServerName() {
		return serverName;
	}

	public boolean isPost() {
		return "POST".equals(method) || (httpServletRequest.getParameter(FormSegment.SEGMENT_ID) != null);
	}

	public String toString() {
		return method + " " + path;
	}

	public String getParameter(String id) {
		List<String> parameterValues = getParameterValues(id);
		return parameterValues == null ? null : parameterValues.get(0);
	}

	protected void setParameter(String id, String value) {
		parameters.put(id, Collections.singletonList(value));
	}

	/**
	 * @return Returns the session.
	 */
	public HttpSession getSession() {
		HttpSession session = httpServletRequest.getSession();
		if (session == null)
			throw new IllegalStateException("Session is unavailable");

		return session;
	}

	public RequestDispatcher getRequestDispatcher(String resource) {
		return httpServletRequest.getRequestDispatcher(resource);

	}

	public HttpServletRequest getHttpRequest() {
		return httpServletRequest;
	}

	public void clearParameters() {
		parameters.clear();
	}

	public MultipartFile getUploadedFile(String parameterName) {
		return uploadedFiles.get(parameterName);
	}
}
