package com.tawala.web;

public class PathElement {
    private final String name;
    private final boolean dir;
    public static final PathElement ROOT = new RootElement();

    public PathElement(String name, boolean isDir) {
        this.name = name;
        dir = isDir;
    }

    public String getName() {
        return name;
    }

    public boolean isDir() {
        return dir;
    }

    public boolean isRoot() {
        return false;
    }

    public String toString() {
        if (isDir()) {
            return name + "/";
        } else {
            return name;
        }
    }

    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof PathElement)) return false;

        final PathElement pathElement = (PathElement) o;

        if (dir != pathElement.dir) return false;
        if (name != null ? !name.equals(pathElement.name) : pathElement.name != null) return false;

        return true;
    }

    public int hashCode() {
        int result;
        result = (name != null ? name.hashCode() : 0);
        result = 29 * result + (dir ? 1 : 0);
        return result;
    }

    private static class RootElement extends PathElement {
        public RootElement() {
            super("/", true);
        }

        public boolean isRoot() {
            return true;
        }

        public String toString() {
            return "/";
        }
    }
}
