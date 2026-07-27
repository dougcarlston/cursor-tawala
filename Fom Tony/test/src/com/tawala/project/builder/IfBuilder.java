package com.tawala.project.builder;

import com.scissor.XmlBuffer;

public class IfBuilder extends TagBuilder {
    private ConditionsBuilder conditions;
    private ProcessBuilder trueSet;
    private ProcessBuilder falseSet;

    public IfBuilder(ProcessBuilder parent) {
        conditions = new ConditionsBuilder();
        trueSet = parent.makeChild(ProcessBuilder.Type.trueSet);
        falseSet = parent.makeChild(ProcessBuilder.Type.falseSet);
        add(conditions);
        add(trueSet);
        add(falseSet);
    }

    protected void startTag(XmlBuffer xml) {
        xml.startTag("if");
    }

    protected void endTag(XmlBuffer xml) {
        xml.endTag("if");
    }

    public ConditionsBuilder conditions() {
        return conditions;
    }

    public ProcessBuilder trueSet() {
        return trueSet;
    }

    public ProcessBuilder falseSet() {
        return falseSet;
    }


}
