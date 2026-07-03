package com.scissor.webrobot;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.io.Writer;
import java.text.NumberFormat;
import java.util.HashMap;
import java.util.Map;

import com.meterware.httpunit.WebResponse;

/** Logs the behavior of a robot to the given directory. Note: overwrites previous log files. */
public class LoggingTracker extends Tracker {
    private File dir;
    private String tag = "tracker";
    private Map<String, Integer> counts = new HashMap<String, Integer>();
    private static final NumberFormat NUMFORMAT = NumberFormat.getInstance();
    static {
        NUMFORMAT.setMinimumIntegerDigits(3);
    }

    public LoggingTracker(File outputDir) {
        super();
        dir = outputDir;
        if (!dir.exists()) dir.mkdirs();
    }

    public void gotResponse(WebRobot bot, WebResponse response) {
        super.gotResponse(bot, response);
        try {
            saveResponseText(response.getText());
        } catch (IOException e) {
            grumble("couldn't save response text", e);
        }
    }

    void saveResponseText(String text) throws IOException {
        File file = nextFile("response", "html");
        Writer out = new BufferedWriter(new FileWriter(file));
        out.write(text);
        out.close();
    }

    private File nextFile(String name, String type) {
        return new File(dir, tag + "-" + name + "-" + nextCount(tag, name) + "." + type);
    }

    private String nextCount(String tag, String name) {
        String key = tag + "-" + name;
        if (!counts.containsKey(key)) counts.put(key, new Integer(0));
        int newCount = 1 + ((Integer) counts.get(key)).intValue();
        counts.put(key, new Integer(newCount));
        return NUMFORMAT.format(newCount);
    }
}
