package com.scissor.webrobot;

import java.io.PrintStream;
import java.io.PrintWriter;
import java.io.Writer;

class Dumper {
    PrintWriter out;
    int indent = 0;

    public Dumper(Writer writer) {
        out = new PrintWriter(writer);
    }

    public Dumper(PrintStream out) {
        this.out = new PrintWriter(out);
    }

    public void dump(String s) {
        dump(new LineEntry(s));
    }

    public void dump(String name, Object value) {
        dump(new VariableEntry(name, value));
    }

    private void dump(Entry entry) {
            reallyDump(entry);
    }

    public void maybeDump(String s) {
        if (isInteresting(s)) dump(s);
    }

    public void maybeDump(String name, Object value) {
        if (isInteresting(value)) dump(name, value);
    }

    private boolean isInteresting(Object value) {
        if (value==null) return false;
        if ("".equals(String.valueOf(value).trim())) return false;
        return true;
    }


    private void reallyDump(Entry entry) {
        for (int j = 0; j < indent; j++) {
            out.print("  ");
        }
        out.println(entry.render());
        out.flush();
    }


    public void push() {
        indent++;
    }

    public void pop() {
        indent--;
    }


    private static interface Entry {
        public String render();
    }

    private static class LineEntry implements Entry {
        String line;

        public LineEntry(String line) {
            this.line = line;
        }

        public String render() {
            return line;
        }
    }

    private static class VariableEntry implements Entry {
        private String name;
        private Object value;

        public VariableEntry(String name, Object value) {
            this.name = name;
            this.value = value;
        }

        public String render() {
            return name + " = " + "'" + value + "'";
        }

    }

}
