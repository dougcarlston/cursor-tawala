/**
 * 
 */
package com.tawala.web;

import javax.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.support.WebContentGenerator;

public final class CachingContentGenerator extends WebContentGenerator {
	public void cacheResponse(HttpServletResponse response, int seconds) {
		cacheForSeconds(response, seconds);
	}
}