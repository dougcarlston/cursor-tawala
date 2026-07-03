package com.tawala.project.builder;

public class NumericCounter {
    private final String prefix;
    private int number;

    public NumericCounter(String prefix, int number) {
        this.prefix = prefix;
        this.number = number;
    }

    public String next() {
        return prefix + number++;
    }
}
