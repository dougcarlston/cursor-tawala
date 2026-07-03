package com.tawala.project.builder;

import com.scissor.XmlBuffer;
import com.scissor.xmlconfig.ConfigElement;
import com.tawala.project.commands.SkipBlock;

public class SkipBlockBuilder extends ProcessBuilder {
    protected SkipBlockBuilder() {
        super(Type.skipInstructions);
    }

    protected SkipBlockBuilder(Type type) {
        super(type);
    }

    // TODO: rename type or change type to Enum
    public void addIfSkip(String field, String comparison,
                          String type, String nameOrValue,
                          final String trueTarget, String falseTarget) {
        IfBuilder ifBuilder = addIf();
        ifBuilder.conditions().addComparison(comparison, field, type, nameOrValue);
        ((SkipBlockBuilder) ifBuilder.trueSet()).addSkip(trueTarget);
        if (falseTarget != null) ((SkipBlockBuilder) ifBuilder.falseSet()).addSkip(falseTarget);
    }

    public SkipBlock makeSkipBlock() {
        XmlBuffer xml = new XmlBuffer();
        render(xml);
        return new SkipBlock(new ConfigElement(xml.toString()));
    }

    public ProcessBuilder makeChild(Type type) {
        return new SkipBlockBuilder(type);
    }

    public void addSkip(String target) {
        contents().tag("skip", "to", target);
    }


}
