package com.tawala.web;

import java.io.UnsupportedEncodingException;
import java.net.URLDecoder;
import java.util.ArrayList;
import java.util.List;

import javax.servlet.http.HttpServletRequest;

import com.scissor.ImpossibleException;

public class Path {
    private List<PathElement> elements = new ArrayList<PathElement>();

    public Path(HttpServletRequest req) {
        this(pathForRequest(req));
    }

    private static String pathForRequest(HttpServletRequest req) {
        StringBuffer result = new StringBuffer();
        if (req.getServletPath() != null) result.append(req.getServletPath());
        if (req.getPathInfo() != null) result.append(req.getPathInfo());
        return result.toString();
    }

    public Path() {
    }

    public Path(String pathString) {
        if (pathString == null) return;
        String[] lumps = pathString.split("/+");
        for (int i = 0; i < lumps.length; i++) {
            String lump = decode(lumps, i);
            if (lump.equals("")) {
                elements.add(PathElement.ROOT);
            } else {
                boolean dir = i < lumps.length - 1 || pathString.endsWith("/");
                elements.add(new PathElement(lump, dir));
            }
        }

    }

    private String decode(String[] lumps, int i) {
        try {
            return URLDecoder.decode(lumps[i], "UTF-8");
        } catch (UnsupportedEncodingException e) {
            throw new ImpossibleException(e);
        }
    }

    public boolean startsWith(String pathString) {
        return startsWith(new Path(pathString));
    }

    public boolean startsWith(Path other) {
        if (other.elements.size() > elements.size()) return false;
        for (int i = 0; i < other.elements.size(); i++) {
            if (!other.elements.get(i).getName().equals(elements.get(i).getName())) return false;
        }
        return true;
    }

    public PathElement element(int index) {
        return elements.get(index);
    }

    public int size() {
        return elements.size();
    }

    public String toString() {
        StringBuffer buf = new StringBuffer();
        for (PathElement element : elements) {
            buf.append(element.toString());
        }
        return buf.toString();
    }

    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof Path)) return false;

        final Path path = (Path) o;

        if (elements.size() != path.elements.size()) return false;
        for (int i = 0; i < elements.size(); i++) {
            if (!elements.get(i).equals(path.elements.get(i))) return false;

        }

        return true;
    }

    public int hashCode() {
        return (elements != null ? elements.hashCode() : 0);
    }

}
