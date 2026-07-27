package com.scissor;

import java.text.SimpleDateFormat;
import java.util.TimeZone;

@SuppressWarnings("serial")
public class HttpDateFormat extends SimpleDateFormat {

    public HttpDateFormat(boolean brokenLikeHttpUnit) {
        super(brokenLikeHttpUnit ? "EEE, dd MMM yyyy hh:mm:ss z" : "EEE, dd MMM yyyy HH:mm:ss z");
        setTimeZone(TimeZone.getTimeZone("GMT"));
    }

}
