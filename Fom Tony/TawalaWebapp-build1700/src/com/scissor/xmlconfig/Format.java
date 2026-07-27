package com.scissor.xmlconfig;

import javax.persistence.Column;
import javax.persistence.Embeddable;
import javax.persistence.Transient;

@Embeddable
public class Format implements Comparable<Format> {
    public static final Format NULL = new Format("0.0");

    @Transient
    private String rawFormat;
    
    @Column(name="major_version", nullable=false)
    private int major;
    @Column(name="minor_version", nullable=false)
    private int minor;
    
    Format() {
        // For Hibernate's use
    }
    
    public Format(int major, int minor) {
    	this.major = major;
    	this.minor = minor;
    	this.rawFormat = major + "." + minor;
    }

    public Format(String format) {
        this.rawFormat = format;
        String[] items;
        if (format == null) {
            items = new String[0];
        } else {
            items = format.split("\\.");
        }
        if (items.length > 0) {
            major = Integer.parseInt(items[0]);
        } else {
            major = 0;
        }
        if (items.length > 1) {
            minor = Integer.parseInt(items[1]);
        } else {
            minor = 0;
        }
    }

    public String toString() {
        return rawFormat;
    }

    public int major() {
        return major;
    }

    public int minor() {
        return minor;
    }

    public int compareTo(Format other) {
        if (this.major == other.major) {
            if (this.minor == other.minor) {
                return 0;
            }
            return other.minor - this.minor;
        }
        return other.major - this.major;
    }

    public boolean isAtLeast(Format other) {
        return this.compareTo(other) <= 0;
    }

    public boolean isLessThan(Format other) {
        return this.compareTo(other) > 0;
    }
}
