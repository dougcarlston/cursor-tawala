package com.tawala.domain.notification;

import java.text.MessageFormat;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.core.io.Resource;

public class UserApprovalNotification extends BaseNotification {
    public static StaticParameters staticParameters;

    private String firstName;
    private String userId;

    public UserApprovalNotification(String to, String firstName, String userId) {
        super(to);
        this.firstName = firstName;
        this.userId = userId;
    }

    @Override
    protected String getText() {
        String text = MessageFormat.format(staticParameters
                .getMessageTemplateText(), new Object[] { firstName,
                staticParameters.getReturnHostName(), userId });
        return text;
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
    
    public static class StaticParameters extends
            BaseNotification.BaseParameters implements InitializingBean {
        private String returnHostName;

        public String getReturnHostName() {
            return returnHostName;
        }

        public void setReturnHostName(String returnHostName) {
            this.returnHostName = returnHostName;
        }

        public void afterPropertiesSet() throws Exception {
            baseValidateProperties();
            if (getReturnHostName() == null) {
                throw new IllegalStateException("Return host name is not set.");
            }
            staticParameters = this;
        }

    }
}
