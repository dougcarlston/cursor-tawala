package com.tawala.web.controller;

import java.text.DecimalFormat;
import java.text.NumberFormat;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.springframework.web.multipart.MaxUploadSizeExceededException;
import org.springframework.web.servlet.HandlerExceptionResolver;
import org.springframework.web.servlet.ModelAndView;

import com.scissor.Log;

public class ExceptionResolver implements HandlerExceptionResolver {
	private static final NumberFormat numberFormat = new DecimalFormat(
			"###,###,###,##0");

	public ModelAndView resolveException(HttpServletRequest request,
			HttpServletResponse response, Object handler, Exception ex) {
		ModelAndView result = new ModelAndView("standalone-error");

		if (ex.getClass().equals(MaxUploadSizeExceededException.class)) {
			MaxUploadSizeExceededException actualException = (MaxUploadSizeExceededException) ex;
			Log.error(this, "Attempt to upload file wich is too large: "
					+ actualException.getLocalizedMessage(), ex);
			result.addObject("detailedMessage",
					"The file(s) you want to upload exceed maximum allowed size of "
							+ numberFormat.format(actualException
									.getMaxUploadSize()) + " bytes.");
		} else {
			Log.error(this, "Unhandled exception: " + ex, ex);
		}
		return result;
	}
}
