package com.tawala.domain.notification;

import java.text.MessageFormat;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.core.io.Resource;

public class PasswordResetNotification extends BaseNotification {
    public static StaticParameters staticParameters;

    private String firstName;
    private String newPassword;

    public PasswordResetNotification(String to, String firstName,
            String newPassword) {
        super(to);
        this.firstName = firstName;
        this.newPassword = newPassword;
    }

    @Override
    protected String getText() {
        String text = MessageFormat.format(staticParameters
                .getMessageTemplateText(), new Object[] { firstName,
                newPassword, staticParameters.getReturnHostName() });
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
            staticParameters = this;
        }
    }
}
