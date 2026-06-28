package com.scissor.xmlconfig;

import org.dom4j.CDATA;
;

public class ConfigCDATA extends ConfigItem {
    private CDATA cData;

    public ConfigCDATA(ConfigItem parent, CDATA cData) {
        super(parent);
        this.cData = cData;
    }

    public String getName() {
        return "CDATA";
    }

    public String text() {
        markUsed();
        return cData.getText();
    }
}
