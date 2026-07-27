package com.tawala.domain.notification;

import java.text.MessageFormat;

import org.springframework.beans.factory.InitializingBean;
import org.springframework.core.io.Resource;

public class UserPutOnHoldNotification extends BaseNotification {
    public static StaticParameters staticParameters;

    private String firstName;

    public UserPutOnHoldNotification(String to, String firstName) {
        super(to);
        this.firstName = firstName;
    }

    @Override
    protected String getText() {
        String text = MessageFormat.format(staticParameters
                .getMessageTemplateText(), new Object[] { firstName });
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
        public void afterPropertiesSet() throws Exception {
            baseValidateProperties();
            staticParameters = this;
        }

    }
}
