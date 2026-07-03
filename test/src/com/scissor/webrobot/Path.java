package com.scissor.webrobot;

import java.net.MalformedURLException;
import java.net.URL;

public class Path {
    private String path;


    public Path(String absolutePath) {
        this.path = absolutePath;
    }

    public Path(String base, String relativePath) {
        this(pathJoin(base, relativePath));
    }

    public Path(Path base, String relativePath) {
        this(pathJoin(base.path, relativePath));
    }

    public Path(URL url) {
        this.path = url.toExternalForm();
    }

    public String toUrlString() {
        return path;
    }

    public boolean equals(URL url) {
        return toUrlString().equals(url.toString());
    }

    public int hashCode() {
        return path.hashCode();
    }

    public boolean equals(Object obj) {
        if (obj == null) return false;
        if (!(obj instanceof Path)) return false;
        Path other = (Path) obj;
        return path.equals(other.path);
    }

    public String toString() {
        return path;
    }

    private static String pathJoin(String p1, String p2) {
        if (p1 == null && p2 == null) return null;
        if (p2 == null) return p1;
        if (p1 == null) return p2;

        if (p1.endsWith("/") && p2.startsWith("/")) {
            return p1 + p2.substring(1);
        } else if (p1.endsWith("/") || p2.startsWith("/")) {
            return p1 + p2;
        } else {
            return p1 + "/" + p2;
        }
    }

    public String localPart() {
        int pos = path.indexOf("//");
        pos = path.indexOf("/", pos + 2);
        return path.substring(pos);
    }

    public URL toUrl() throws MalformedURLException {
        return new URL(toUrlString());
    }


}
