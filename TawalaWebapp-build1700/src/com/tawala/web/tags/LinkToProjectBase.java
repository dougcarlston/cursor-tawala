package com.tawala.web.tags;

import javax.servlet.jsp.JspException;
import javax.servlet.jsp.JspTagException;
import javax.servlet.jsp.JspWriter;
import javax.servlet.jsp.tagext.TagSupport;

import com.scissor.Log;
import com.tawala.project.Form;
import com.tawala.project.UserProject;
import com.tawala.project.UserProject.EntryPointType;

abstract public class LinkToProjectBase extends TagSupport {
	private static final long serialVersionUID = 1L;
	private UserProject project;
	private Form form;
	private String formName;

	public String getFormName() {
		return formName;
	}

	public void setFormName(String formName) {
		this.formName = formName;
	}

	/**
	 * @return Returns the form.
	 */
	public Form getForm() {
		return form;
	}

	/**
	 * @param form
	 *            The form to set.
	 */
	public void setForm(Form form) {
		this.form = form;
	}

	/**
	 * @return Returns the project.
	 */
	public UserProject getProject() {
		return project;
	}

	/**
	 * @param project
	 *            The project to set.
	 */
	public void setProject(UserProject project) {
		this.project = project;
	}

	@Override
	public int doStartTag() throws JspException {
		if (project == null) {
			throw new JspException("project parameter is not set");
		}

		if (form == null) {
			if (formName == null) {
				throw new JspException("Form parameter is not set");
			} else {
				form = project.getProject().getForm(formName);
				if (form == null) {
					Log.warn(this, "Unable to find form named '" + formName
							+ "'");
				}
			}
		}

		if (form != null) {
			try {
				String url = project.getUrlToForm(getEntryPointType(), form);

				JspWriter out = pageContext.getOut();
				out.write(url);
			} catch (Exception e) {
				throw new JspTagException(e.getMessage());
			}
		}

		return SKIP_BODY;
	}

	protected abstract EntryPointType getEntryPointType();
}
