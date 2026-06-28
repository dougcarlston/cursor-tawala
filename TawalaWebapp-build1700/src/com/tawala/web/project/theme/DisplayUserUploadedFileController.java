package com.tawala.web.project.theme;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.project.theme.UserUploadedFile;
import com.tawala.web.CachingContentGenerator;
import com.tawala.web.Request;
import com.tawala.web.Response;

public class DisplayUserUploadedFileController implements Controller {
	public static final String IMAGE_ID = "id";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		String id = request.getParameter(IMAGE_ID);
		if (id == null) {
			Request requestObject = new Request(request);
			if (requestObject.path().size() < 3) {
				Log.warn(this, "project path too short: '"
						+ request.getRequestURI() + "'");
				response.sendError(HttpServletResponse.SC_NOT_FOUND);
				return null;
			}
			id = requestObject.path().element(2).getName();
		}
		UserUploadedFile image = UsersHibernateImpl.getUserUploadedFileById(id);
		if (image == null) {
			response.sendError(HttpServletResponse.SC_NOT_FOUND);
		} else {
			new CachingContentGenerator().cacheResponse(response, 200000);

			Response.addP3PPolicyHeader(response);
			response.setContentType(image.getContentType());
			response.setContentLength(image.getSize());
			response.getOutputStream().write(image.getData());
		}
		return null;
	}
}
