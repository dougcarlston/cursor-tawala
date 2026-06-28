package com.tawala.web;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.ConfigItem;
import com.tawala.World;
import com.tawala.domain.User;

public class ApiRequest {
    private final ConfigElement xml;
    private final String type;
    private final String userId;
    private final String password;
    private final String protocol;
    private final String formName;

    public ApiRequest(ConfigElement xml) {
        this.xml = xml;
        this.type = xml.attribute("type").stringValue();
        this.protocol = xml.attribute("protocol").stringValue(); // TODO: turn into verison like other config versions?
        this.userId = xml.child("credentials").attribute("user").stringValue();
        this.password = xml.child("credentials").attribute("password").stringValue();
        this.formName = xml.attribute("form").stringValue();
    }

    public String getType() {
        return type;
    }

    public String getUserId() {
        return userId;
    }

    public void addUnusedXmlWarnings(ApiResponse response) {
        for (ConfigItem item : xml.getUnusedItems()) {
            response.addDebugMessage("Unused xml: " + item.path());
        }
    }

    public ConfigElement getXml() {
        return xml;
    }

    public User retrieveUserAndValidateCredentials(World world) {
        User user = world.domain().users().get(userId);
        return user != null && user.checkPassword(password) ? user : null;
    }
    
    // TODO: never used - validation should be done in tests and production code.
    // Also, protocol should be external to the message - too late to to check it; 
    // we already are parsing the message.
    public String getProtocol() {
        return protocol;
    }

    /**
     * @return Returns the formName.
     */
    public String getFormName() {
        return formName;
    }
}
