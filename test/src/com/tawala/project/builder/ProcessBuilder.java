package com.tawala.project.builder;

import com.scissor.XmlBuffer;

public abstract class ProcessBuilder extends TagBuilder {
    private Type type;

    public enum OperandType {
        VALUE, FIELD}

    ;


    protected ProcessBuilder(Type type) {
        this.type = type;
    }

    protected void startTag(XmlBuffer xml) {
        if (shouldShowTag()) xml.startTag(type.name(), tagAttributes());
    }

    protected void endTag(XmlBuffer xml) {
        if (shouldShowTag()) xml.endTag(type.name());
    }

    private boolean shouldShowTag() {
        return !(type.hideifEmpty() && isEmpty());
    }

    protected String[] tagAttributes() {
        return new String[0];
    }


    public IfBuilder addIf() {
        IfBuilder ifBuilder = new IfBuilder(this);
        add(ifBuilder);
        return ifBuilder;
    }

    public void addSet(String field, OperandType type, String nameOrValue) {
        contents().startTag("set", "field", field);
        addFieldOrValue(contents(), type, nameOrValue);
        contents().endTag("set");
    }

    public GetBuilder addGet(String recordList, String... forms) {
    	GetBuilder getbuilder = new GetBuilder(recordList, forms);
    	add(getbuilder);
    	return getbuilder;    	
    }

    public GetBuilder addGet(String recordList, Object[]... forms) {
    	GetBuilder getbuilder = new GetBuilder(recordList, forms);
    	add(getbuilder);
    	return getbuilder;    	
    }

    public ForEachBuilder addForEach(String record, String recordList) {
    	ForEachBuilder foreachbuilder = new ForEachBuilder(record, recordList);
    	add(foreachbuilder);
    	return foreachbuilder;
    }
    
    public ForEachMcBuilder addForEachMc() {
    	ForEachMcBuilder forEachMcBuilder = new ForEachMcBuilder();
    	add(forEachMcBuilder);
    	return forEachMcBuilder;
    }
    
    public void addAddTo(String field, OperandType type, String nameOrValue) {
        mathOp("addTo", field, type, nameOrValue);
    }

    public void addSubtractFrom(String field, OperandType type, String nameOrValue) {
        mathOp("subtractFrom", field, type, nameOrValue);
    }

    public void addMultiplyBy(String field, OperandType type, String nameOrValue) {
        mathOp("multiplyBy", field, type, nameOrValue);
    }

    public void addDivideBy(String field, OperandType type, String nameOrValue) {
        mathOp("divideBy", field, type, nameOrValue);
    }

    public void addShow(String document) {
        addShow(document, false);
    }
    
    public void addShow(String document, boolean reset) {
        contents().tag("show", "document", document, "reset", Boolean.toString(reset));
    }

    public void addShow(DocumentBuilder document) {
        addShow(document.getName(), false);
    }

    public void addShow(DocumentBuilder document, boolean reset) {
        addShow(document.getName(), reset);
    }

    private void mathOp(String operation, String field, OperandType type, String nameOrValue) {
        contents().startTag(operation, "field", field);
        addOperand(contents(), type, nameOrValue);
        contents().endTag(operation);
    }

    private void addFieldOrValue(XmlBuffer buf, OperandType type, String nameOrValue) {
        if (type.equals(OperandType.FIELD)) {
            addField(buf, nameOrValue);
        } else {
            addValue(buf, nameOrValue);
        }
    }

    private void addOperand(XmlBuffer buf, OperandType type, String nameOrValue) {
        buf.tag("operand", type.toString().toLowerCase(), nameOrValue);
    }

    private void addValue(XmlBuffer buf, String value) {
        buf.tag("string", "value", value);
    }

    private void addField(XmlBuffer buf, String name) {
//        buf.tag("field", "name", name);
        buf.tag("string", "field", name);
    }

    public abstract ProcessBuilder makeChild(Type type);

    public static enum Type {
        process, skipInstructions, forEach, forEachMc, get, trueSet, falseSet(true), edit;

        private final boolean hideifEmpty;

        Type(boolean hideifEmpty) {
            this.hideifEmpty = hideifEmpty;
        }

        Type() {
            this(false);
        }

        public boolean hideifEmpty() {
            return hideifEmpty;
        }
    }

}
