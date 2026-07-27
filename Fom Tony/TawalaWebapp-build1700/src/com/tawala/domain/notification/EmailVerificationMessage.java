package com.tawala.domain.notification;

import java.text.MessageFormat;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.core.io.Resource;

public class EmailVerificationMessage extends BaseNotification {
    public static StaticParameters staticParameters;

    private String returnURI;
    private String firstName;

    public EmailVerificationMessage(String to, String returnURI,
            String firstName) {
        super(to);
        this.returnURI = returnURI;
        this.firstName = firstName;
    }

    @Override
    protected String getText() {
        String result = MessageFormat.format(staticParameters
                .getMessageTemplateText(), new Object[] { firstName,
                staticParameters.getReturnHostName() + returnURI });
        return result;
    }

    public static class StaticParameters extends
            BaseNotification.BaseParameters implements InitializingBean {
        private String returnHostName;

        public String getReturnHostName() {
            return returnHostName;
        }

        public void setReturnHostName(String returnHostName) {
            this.returnHostName = returnHostName;
        }

        /* (non-Javadoc)
         * @see org.springframework.beans.factory.InitializingBean#afterPropertiesSet()
         */
        public void afterPropertiesSet() throws Exception {
            baseValidateProperties();
            if(getReturnHostName() == null) {
                throw new IllegalStateException("Return host name is not set.");
            }
            staticParameters = this;
        }
    }

    @Override
    protected Resource getLogoImage() {
        return staticParameters.getLogoImage();
    }

    @Override
    protected String getSubject() {
        return staticParameters.getSubject();
    }

    @Override
    protected String getFrom() {
        return staticParameters.getFrom();
    }
}
