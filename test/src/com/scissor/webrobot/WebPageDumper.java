package com.scissor.webrobot;

import java.io.IOException;
import java.io.PrintStream;

import org.xml.sax.SAXException;

import com.meterware.httpunit.WebLink;
import com.meterware.httpunit.WebResponse;

public class WebPageDumper {
    private PrintStream out;

    public WebPageDumper(PrintStream out) {
        this.out = out;
    }

    public void dump(WebResponse response) {
        try {
            dumpResponse(response);
        } catch (IOException e) {
            out.print("Couldn't read response");
            e.printStackTrace(out);
        } catch (SAXException e) {
            out.print("Couldn't parse response");
            e.printStackTrace(out);
        }
    }

    @SuppressWarnings("deprecation")
    private void dumpResponse(WebResponse response) throws IOException, SAXException {
        WebLink[] links = response.getLinks();
        for (int i = 0; i < links.length; i++) {
            WebLink link = links[i];
            startLine("link " + i);
            item("name", link.getName());
            item("href", link.getURLString());
            item("text", link.asText());
            out.println();
        }
        out.print(response.getText());

    }

    private void startLine(String name) {
        out.print(name + ": ");
    }

    private void item(String key, String value) {
        out.print(key + " = '" + value + "' ");
    }
}
