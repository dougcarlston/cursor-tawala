package com.tawala.web;

import java.io.UnsupportedEncodingException;
import java.net.MalformedURLException;
import java.net.URL;
import java.net.URLEncoder;

import com.meterware.httpunit.WebClient;
import com.scissor.ImpossibleException;
import com.scissor.webrobot.RobotException;
import com.scissor.webrobot.WebRobot;
import com.tawala.domain.User;
import com.tawala.project.Form;
import com.tawala.project.LinkToUserProject;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;
import com.tawala.web.controller.WellKnown;

public class SiteRobot extends WebRobot {
	public static final String ROOT = "http://ignored";
	private boolean loggedIn;

	public SiteRobot() {
	}

	public SiteRobot(WebClient webClient) {
		super(webClient);
	}

	public void goHome() throws RobotException {
		try {
			go(new URL(ROOT + "/"));
		} catch (MalformedURLException e) {
			throw new RobotException("weird URL construction failure", e);
		}
	}

	public void go(String path) throws RobotException {
		if (!path.startsWith("http://"))
			path = ROOT + path;
		super.go(path);
	}

	public void go(UserProject project) throws RobotException {
		go(project.getUrlToForm(EntryPointType.REAL_PROJECT, project
				.getProject().defaultForm()));
	}

	public void go(UserProject project, Form form) throws RobotException {
		go(project.getUrlToForm(EntryPointType.REAL_PROJECT, form));
	}

	public void go(UserProject project, String formName) throws RobotException {
		Form form = project.getProject().getForm(formName);
		if (form == null) {
			throw new IllegalArgumentException("Form <" + formName
					+ "> not found.");
		}
		go(project, form);
	}

	public void gotoImage(UserProject project, String imagePath)
			throws RobotException {
		go(ROOT + WellKnown.urls.getProjectRunUrlPrefix() + "/"
				+ project.getUniqueRandomId() + "/" + imagePath);
	}

	public void gotoFormPreviewImage(UserProject project, String imagePath)
			throws RobotException {
		go(ROOT + WellKnown.urls.getFormPreviewUrlPrefix() + "/"
				+ urlEncode(project.getUser().getId()) + "-"
				+ urlEncode(project.getName()) + "/" + imagePath);
	}

	private String urlEncode(String string) {
		try {
			return URLEncoder.encode(string, "UTF-8");
		} catch (UnsupportedEncodingException e) {
			throw new ImpossibleException(e);
		}
	}

	public void logInAs(User user) throws RobotException {
		logInAs(user.getId(), user.getPassword());
	}

	public void logInAs(String user, String pass) throws RobotException {
		if (loggedIn)
			logOut();
		go(WellKnown.urls.getLogin());
		setParameter("userName", user);
		setParameter("password", pass);
		submit();
		if (getPageText().indexOf("Welcome" + user) > -1) {
			loggedIn = true;
		}
	}

	public void logOut() throws RobotException {
		go(WellKnown.urls.getLogout());
		loggedIn = false;
	}

	public void go(LinkToUserProject linkToUserProject) throws RobotException {
		go(linkToUserProject.getProject().getUrlToForm(linkToUserProject,
				EntryPointType.REAL_PROJECT,
				linkToUserProject.getProject().getProject().defaultForm()));
	}
}
