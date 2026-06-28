package com.tawala.web.project.theme;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.json.JSONObject;
import org.springframework.web.multipart.MultipartFile;
import org.springframework.web.multipart.MultipartHttpServletRequest;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.tawala.UsersHibernateImpl;
import com.tawala.project.theme.UserUploadedFile;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

public class AddUserUploadedImageController implements Controller {
	public static final String FILE_PARAMETER = "image-file";

	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		if (request.getMethod().equals("GET")) {
			return new ModelAndView("theme.add-image");
		}
		response.setContentType("text/plain");
		JSONObject responseObject = new JSONObject();

		MultipartHttpServletRequest multiPartRequest = (MultipartHttpServletRequest) request;
		MultipartFile file = multiPartRequest.getFile(FILE_PARAMETER);
		if (file.getSize() == 0) {
			responseObject.put("noFileUploaded", true);
		} else {
			UserUploadedFile image = new UserUploadedFile(
					UserInfoPreparationInterceptor.getSessionUser(request),
					file.getBytes(), file.getContentType(), file
							.getOriginalFilename(), (int) file.getSize());

			UsersHibernateImpl.saveUserUploadedFile(image);

			responseObject.put("imageURL", image.getFileURL());
			responseObject.put("imageId", image.getId());
		}
		responseObject.write(response.getWriter());

		return null;
	}
}
