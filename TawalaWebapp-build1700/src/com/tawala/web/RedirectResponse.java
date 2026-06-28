package com.tawala.web;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.tawala.World;
import com.tawala.web.admin.UrgentMessage;

public class RedirectResponse extends Response {
	private final String redirectTo;
	
	public RedirectResponse(String redirectTo) {
		this.redirectTo = redirectTo;
	}

	@Override
	protected void handle(HttpServletRequest request,
			HttpServletResponse response, World world) throws IOException {
		response.sendRedirect(redirectTo);
	}

	@Override
	public void handleUrgentNotificationMessage(UrgentMessage urgentMessage) {
		//--- Do nothing
	}
}
