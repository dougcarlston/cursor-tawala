package com.tawala.web.tags;

import java.util.List;

import javax.servlet.jsp.JspException;
import javax.servlet.jsp.JspTagException;
import javax.servlet.jsp.JspWriter;
import javax.servlet.jsp.tagext.TagSupport;

import org.springframework.web.util.HtmlUtils;

import com.tawala.project.FormSubmission;
import com.tawala.project.Value;
import com.tawala.project.commands.Reference;

public class DisplayFormSubmissionValueTag extends TagSupport {
	private static final long serialVersionUID = 1L;
	private Reference fieldReference;
	private FormSubmission submission;

	public void setSubmission(FormSubmission submission) {
		this.submission = submission;
	}

	public void setFieldReference(Reference fieldReference) {
		this.fieldReference = fieldReference;
	}

	@Override
	public int doStartTag() throws JspException {
		try {
			JspWriter out = pageContext.getOut();
			out.write(HtmlUtils.htmlEscape(getAllValues(submission, fieldReference)));
		} catch (Exception e) {
			throw new JspTagException(e.getMessage());
		}

		return SKIP_BODY;
	}

	public static String getAllValues(FormSubmission formResponse,
			Reference fieldReference) {
		List<Value> values = formResponse.getValues(fieldReference);
		switch (values.size()) {
		case 0:
			return "";

		case 1:
			return values.get(0).toString();

		default:
			StringBuilder stringBuilder = new StringBuilder(values.get(0)
					.toString());
			for (int i = 1; i < values.size(); i++) {
				stringBuilder.append(", ").append(values.get(i).toString());
			}

			return stringBuilder.toString();
		}
	}
}
