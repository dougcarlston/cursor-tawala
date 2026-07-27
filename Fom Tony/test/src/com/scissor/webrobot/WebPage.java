package com.scissor.webrobot;

import org.xml.sax.SAXException;

import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.WebResponse;


// TODO: Move the functionality from WebRobot into here
public class WebPage {
    WebResponse response;

    public WebPage(WebResponse response) {
        this.response = response;
    }

    protected WebResponse getResponse() {
        return response;
    }

    public WebForm getForm(int i) throws RobotException {
        try {
            return response.getForms()[0];
        } catch (SAXException e) {
            throw new RobotException("Can't parse page");
        }
    }

}
