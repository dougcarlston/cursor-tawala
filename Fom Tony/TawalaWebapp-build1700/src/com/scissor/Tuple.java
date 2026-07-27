package com.scissor;

import java.util.Arrays;

public class Tuple {
    private final Object[] contents;

    public Tuple(Object... contents) {
        this.contents = contents;
    }

    protected Object[] getContents() {
        return contents;
    }

    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        final Tuple tuple = (Tuple) o;

        if (!Arrays.equals(contents, tuple.contents)) return false;

        return true;
    }

    // TODO: will fail for empty contents.
    public int hashCode() {
        int hashCode = contents[0].hashCode();
        for (int i = 1; i < contents.length; i++) {
            hashCode += contents[i].hashCode();
        }
        return hashCode;
    }


    public String toString() {
        StringBuffer result = new StringBuffer();
        result.append("Tuple(");
        for (Object item : contents) {
            result.append(item);
            result.append(",");
        }
        result.deleteCharAt(result.length() - 1);
        result.append(")");
        return result.toString();
    }
}
