package com.tawala.web.projectmanager;

import java.io.IOException;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.dom4j.Attribute;
import org.springframework.web.servlet.ModelAndView;
import org.springframework.web.servlet.mvc.Controller;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.domain.User;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.web.Request;
import com.tawala.web.WorldInitializer;
import com.tawala.web.controller.UserInfoPreparationInterceptor;

/**
 * Push from Designer on this server. The signed-in account owns the project.
 * The request body is the same upload XML Designer already sends. Credentials
 * inside that XML are ignored.
 *
 * A name this account already has is updated when the upload carries that
 * project's address. The address and the stored responses stay. A new name
 * creates a project. The public Library copies (Horses and Penguins Test and
 * the other six) are refused, so a Push of those files is saved under a new
 * name. An address that already belongs to a project is refused.
 */
public class UploadSignedInProjectController implements Controller {
	public ModelAndView handleRequest(HttpServletRequest request,
			HttpServletResponse response) throws Exception {
		if (!"POST".equals(request.getMethod())) {
			response.sendError(HttpServletResponse.SC_METHOD_NOT_ALLOWED);
			return null;
		}
		User user = UserInfoPreparationInterceptor.getSessionUser(request);
		if (user == null) {
			response.sendError(HttpServletResponse.SC_UNAUTHORIZED);
			return null;
		}

		String xml = new Request(request).getContentAsString();
		if (xml == null || xml.trim().length() == 0) {
			response.sendError(HttpServletResponse.SC_BAD_REQUEST);
			return null;
		}

		ConfigElement root;
		try {
			root = new ConfigElement(xml);
		} catch (RuntimeException e) {
			response.sendError(HttpServletResponse.SC_BAD_REQUEST);
			return null;
		}
		if (!root.hasChild("project")) {
			response.sendError(HttpServletResponse.SC_BAD_REQUEST);
			return null;
		}
		ConfigElement projectElement = root.child("project");
		String name = projectElement.attribute("name").stringValue();
		if (name != null) {
			name = name.trim();
		}
		if (name == null || name.length() == 0 || name.length() > 100) {
			response.sendError(HttpServletResponse.SC_BAD_REQUEST);
			return null;
		}

		String incomingId = projectElement.attribute("uniqueId").stringValue();
		if (incomingId != null) {
			incomingId = incomingId.trim();
		}
		if (incomingId != null && incomingId.length() == 0) {
			incomingId = null;
		}
		if (isPublicCatalogId(incomingId)) {
			Attribute attr = projectElement.getRawElement().attribute("uniqueId");
			if (attr != null) {
				projectElement.getRawElement().remove(attr);
			}
			incomingId = null;
		}

		UserProject existing = WorldInitializer.getDefaultWorld().domain()
				.projects().get(user.getId(), name);
		if (existing != null) {
			if (isPublicCatalogId(existing.getUniqueRandomId())) {
				writeFailure(response, "library-project");
				return null;
			}
			if (incomingId == null
					|| !incomingId.equals(existing.getUniqueRandomId())) {
				writeFailure(response, "name-taken");
				return null;
			}
		}

		UserProject saved;
		try {
			UserProject incoming = new UserProject(projectElement, user);
			saved = WorldInitializer.getDefaultWorld().domain().projects()
					.put(incoming);
		} catch (IllegalArgumentException e) {
			writeFailure(response, "uniqueId");
			return null;
		}
		if (saved == null || saved.getUniqueRandomId() == null) {
			response.sendError(HttpServletResponse.SC_BAD_REQUEST);
			return null;
		}

		TawalaSessionFactory.MAIN.evictCachedDataForQuery(
				ProjectsHibernateImpl.CACHED_QUERY_TEMPLATE_ID);
		writeSuccess(response, saved);
		return null;
	}

	private static boolean isPublicCatalogId(String uniqueId) {
		return "gy1zssbrwm4fgfm".equals(uniqueId)
				|| "52ozm3kqd58zlss".equals(uniqueId)
				|| "b6do4s50iq64vl8".equals(uniqueId)
				|| "wg77ytn0bgq1x70".equals(uniqueId)
				|| "grniytf6dvmobqe".equals(uniqueId)
				|| "u3hkqgwtrepjlur".equals(uniqueId)
				|| "bx44wpdnspbsi3k".equals(uniqueId);
	}

	private static void writeFailure(HttpServletResponse response, String code)
			throws IOException {
		response.setStatus(HttpServletResponse.SC_CONFLICT);
		response.setContentType("text/xml; charset=UTF-8");
		response.getWriter().print(
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
						+ "<response status=\"failure\" code=\"" + escape(code)
						+ "\"/>\n");
	}

	private static void writeSuccess(HttpServletResponse response,
			UserProject saved) throws IOException {
		response.setContentType("text/xml; charset=UTF-8");
		response.getWriter().print(
				"<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n"
						+ "<response status=\"success\">\n"
						+ "  <project name=\"" + escape(saved.getName())
						+ "\" uniqueId=\"" + escape(saved.getUniqueRandomId())
						+ "\"/>\n" + "</response>\n");
	}

	private static String escape(String value) {
		if (value == null) {
			return "";
		}
		return value.replace("&", "&amp;").replace("\"", "&quot;")
				.replace("<", "&lt;");
	}
}
