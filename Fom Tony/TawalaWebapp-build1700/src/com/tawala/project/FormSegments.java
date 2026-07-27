package com.tawala.project;

import java.util.ArrayList;

import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.commands.SkipBlock;

public class FormSegments extends ArrayList<FormSegment> {
    private static final long serialVersionUID = 1L;
    private SkipBlocks initialSkips = null;

    public FormSegments(String formId) {
        newSegment(formId);
    }

    public FormSegments(String formId, ConfigElement config) {
        newSegment(formId);
        if (config == null) return;
        boolean newBlockRequired = false;
        for (ConfigElement childConfig : config.childElements()) {
            String name = childConfig.getName();
            if (isBlank()) {
                if (name.equals("break")) {
                	continue;
                } else if (name.equals("skipInstructions")) {
                    if (initialSkips == null) initialSkips = new SkipBlocks();
                    initialSkips.add(new SkipBlock(childConfig));
                    continue;
                }
            }
            if (name.equals("break") || name.equals("skipInstructions")) {
                if (name.equals("skipInstructions")) {
                    getLast().addSkipBlock(new SkipBlock(childConfig));
                }
                newBlockRequired = true;
            } else {
                if (newBlockRequired) {
                	if(name.equals("field")) {
                		continue;
                	}
                    newSegment(formId);
                    newBlockRequired = false;
                }
                FormItem formItem = Form.FACTORY.make(childConfig);
                if (formItem != null) getLast().add(formItem);
            }
        }

    }

    private void newSegment(String formId) {
        add(new FormSegment(lastItemId() + 1, formId));
    }

    private FormSegment getLast() {
        return get(lastItemId());
    }

    private int lastItemId() {
        return size() - 1;
    }

    private boolean isBlank() {
        return size() == 1 && getLast().isEmpty();
    }

    public FormSegments(String formId, FormItem content) {
        this(formId);
        getLast().add(content);
    }

    public boolean hasInitialSkip() {
        return initialSkips != null;
    }

    public SkipBlocks getInitialSkip() {
        return initialSkips;
    }
}
