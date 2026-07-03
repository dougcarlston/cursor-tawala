package com.tawala.project.builder;

import java.util.ArrayList;
import java.util.List;

import com.scissor.XmlBuffer;

public class FibBuilder extends TagBuilder {
    private String label;
    private String alternateLabel;
    private LetterCounter blankId = new LetterCounter();
    private ConditionsBuilder displayCondition;

    public FibBuilder(String label) {
        this.label = label;
    }

    public FibBuilder(String label, String alternateLabel) {
        this.label = label;
        this.alternateLabel = alternateLabel;
    }

    protected void startTag(XmlBuffer xml) {
        xml.startTag("fib", false, attributes());
        xml.startTag("paragraph", false);
    }

    private String[] attributes() {
        List<String> attribs = new ArrayList<String>();
        attribs.add("label");
        attribs.add(label);
        if (alternateLabel != null) {
            attribs.add("alternateLabel");
            attribs.add(alternateLabel);
        }
        return attribs.toArray(new String[0]);
    }

    protected void endTag(XmlBuffer xml) {
        xml.endTag("paragraph", false);
        if(displayCondition != null) {
        	xml.startTag("displayConditions");
        	displayCondition.render(xml);
        	xml.endTag("displayConditions");
        }
        xml.endTag("fib");
    }

    public ConditionsBuilder getDisplayConditions() {
    	if(displayCondition == null) {
    		displayCondition = new ConditionsBuilder();
    		displayCondition.setOmitEnclosingTag(true);
    	}
    	return displayCondition;
    }
    
    public void addText(String text) {
    	contents().startTag("font", false, "face", "Arial", "size", "200", "color", "000000");
        contents().text(text);
        contents().endTag("font", false);
    }

    public void addBlank() {
        addBlank(nextBlankId(), null, 30, false);
    }

    public void addBlank(int length) {
        addBlank(nextBlankId(), null, length, false);
    }

    public void addBlank(int length, Boolean required) {
        addBlank(nextBlankId(), null, length, required);
    }

    public void addBlank(String alternateLabel, int length) {
        addBlank(nextBlankId(), alternateLabel, length, false);
    }

    public void addBlank(String alternateLabel) {
        addBlank(nextBlankId(), alternateLabel, 30, false);
    }

    public void addBlank(boolean required) {
        addBlank(nextBlankId(), null, 30, required);
    }

    public void addBlank(String label, String alternateLabel, int length, boolean required) {
        contents().tag("blank", false,
                "label", label,
                "length", "" + length,
                "alternateLabel", alternateLabel,
                "required", "" + required);
    }

    public void addBlankWithPhoneNumberValidation(int length) {
        contents().startTag("blank", false,
                "label", nextBlankId(),
                "length", "" + length,
                "alternateLabel", null,
                "required", "" + false);
    	contents().startTag("validator", false);
    	contents().startTag("phone-number-validator", false, "version", "1");
    	contents().startTag("error-message", false);
    	contents().tag("string", false, "value", "Please enter a valid phone number including area code.");
        contents().endTag("error-message", false);
        contents().endTag("phone-number-validator", false);
        contents().endTag("validator", false);
        contents().endTag("blank", false);
    }

    public void addBlankWithDollarAmountValidation(int length) {
        contents().startTag("blank", false,
                "label", nextBlankId(),
                "length", "" + length,
                "alternateLabel", null,
                "required", "" + false);
    	contents().startTag("validator", false);
    	contents().startTag("us-dollar-amount-validator", false, "version", "1");
    	contents().startTag("error-message", false);
    	contents().tag("string", false, "value", "Please enter a valid dollar amount.");
        contents().endTag("error-message", false);
        contents().endTag("us-dollar-amount-validator", false);
        contents().endTag("validator", false);
        contents().endTag("blank", false);
    }

    private String nextBlankId() {
        return blankId.next();
    }

}
