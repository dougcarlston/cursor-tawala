package com.tawala.domain.notification;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;

import javax.mail.internet.MimeMessage;

import org.springframework.core.io.FileSystemResource;
import org.springframework.core.io.Resource;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.mail.javamail.MimeMessagePreparator;

import com.scissor.StreamCopier;

abstract public class BaseNotification implements MimeMessagePreparator {
    private String to;

    public BaseNotification(String to) {
        this.to = to;
    }

    public final void prepare(MimeMessage mimeMessage) throws Exception {
        MimeMessageHelper message = new MimeMessageHelper(mimeMessage, false,
                "UTF-8");
        message.setFrom(getFrom());
        message.setTo(to);
        message.setSubject(getSubject());
        message.setText(getText(), isHtmlMessage());
        // message.addInline("tawalaLogo", getLogoImage());
    }

    protected boolean isHtmlMessage() {
		return true;
	}

	abstract protected Resource getLogoImage();

    abstract protected String getSubject();

    abstract protected String getFrom();

    abstract protected String getText();

    public static class BaseParameters {
        private String messageTemplateText;
        private Resource logoImage;
        private String from;
        private String subject;

        /**
         * @return Returns the from.
         */
        public String getFrom() {
            return from;
        }

        /**
         * @param from
         *            The from to set.
         */
        public void setFrom(String from) {
            this.from = from;
        }

        /**
         * @return Returns the logoImage.
         */
        public Resource getLogoImage() {
            return logoImage;
        }

        /**
         * @param logoImage
         *            The logoImage to set.
         */
        public void setLogoImage(Resource logoImage) {
            if (!logoImage.exists())
                throw new IllegalArgumentException(logoImage
                        + " doesn't exist.");

            this.logoImage = logoImage;
        }

        /**
         * @param messageTemplate
         *            The messageTemplate to set.
         * @throws IOException
         */
        public void setMessageTemplate(Resource messageTemplate)
                throws IOException {
        	messageTemplateText = getResourceContentsAsString(messageTemplate);
        }

        /**
         * @return Returns the subject.
         */
        public String getSubject() {
            return subject;
        }

        /**
         * @param subject
         *            The subject to set.
         */
        public void setSubject(String subject) {
            this.subject = subject;
        }

        public void baseValidateProperties() throws Exception {
            if (from == null) {
                throw new IllegalStateException("'from' is not set");
            }
            if (logoImage == null) {
                throw new IllegalStateException("'logoImage' is not set");
            }
            if (logoImage == null) {
                throw new IllegalStateException("'from' is not set");
            }
            if (messageTemplateText == null) {
                throw new IllegalStateException("'messageTemplate' is not set");
            }
            if (subject == null) {
                throw new IllegalStateException("'subject' is not set");
            }
        }

        /**
         * @return Returns the messageTemplateText.
         */
        public String getMessageTemplateText() {
            return messageTemplateText;
        }
    }

	public static String getResourceContentsAsString(Resource messageTemplate) throws IOException {
        return new String(getResourceAsByteArray(messageTemplate).toByteArray());
	}

	public static ByteArrayOutputStream getResourceAsByteArray(
			Resource messageTemplate) throws IOException {
		if (!messageTemplate.exists()) {
            messageTemplate = new FileSystemResource("web/"
                    + messageTemplate.getFile().getAbsolutePath());
        }

        if (!messageTemplate.exists()) {
            throw new IllegalArgumentException("Resource for "
                    + messageTemplate.getFilename() + " doesn't exist!");
        }

        ByteArrayOutputStream bytes = new ByteArrayOutputStream();
        InputStream stream = messageTemplate.getInputStream();
        try {
            StreamCopier.copy(stream, bytes);
        } finally {
            stream.close();
        }
		return bytes;
	}

}
