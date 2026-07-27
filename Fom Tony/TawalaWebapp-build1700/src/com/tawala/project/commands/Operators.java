package com.tawala.project.commands;

import java.util.ArrayList;
import java.util.List;

import com.scissor.xmlconfig.ConfigElement;

public class Operators extends ArrayList<Operator> implements List<Operator> {
    private static final long serialVersionUID = 1L;

    public static Operators parseChildren(ConfigElement config) {
        Operators operators = new Operators();
        operators.addAll(Operator.FACTORY.makeChildren(config));
        return operators;
    }
}
