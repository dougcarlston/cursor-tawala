package fake.smtp;


import java.io.BufferedReader;
import java.io.CharArrayReader;
import java.io.IOException;
import java.util.LinkedHashMap;
import java.util.List;

import com.scissor.Log;

public class FakeSmtpMessage {

    String sender = null;
    List<String> recipients = new java.util.ArrayList<String>();
    String data = null;
    Headers headers;

    public String getSender() {
        return sender;
    }

    public void setSender(String sender) {
        this.sender = sender;
    }

    public List<String> getRecipients() {
        return recipients;
    }

    public void addRecipient(String recipient) {
        recipients.add(recipient);
    }

    public String getData() {
        return data;
    }

    public void setData(String data) {
        this.data = data;
    }

    public String getHeader(String name) {
        if (headers == null) initHeaders();
        return headers.headerValue(name);
    }

    private void initHeaders() {
        Headers headers = new Headers();
        BufferedReader in = new BufferedReader(new CharArrayReader(data.toCharArray()));
        String line;
        try {
            while ((line = in.readLine()) != null) {
                if ("".equals(line)) break;
                headers.addHeader(line);
            }
        } catch (IOException e) {
            Log.error(this, "failed to read headers", e);
        }
        this.headers = headers;
    }

    public String getBody() {
        String separator = "\n\n";
        int bodyIndex = data.indexOf(separator) + separator.length();
        if (bodyIndex == -1) throw new IllegalStateException("can't get body");
        return data.substring(bodyIndex);
    }

    public void dump() {
        System.out.println(data);
    }

    private class Header {

        String name;
        String value;

        public Header(String name, String value) {
            this.name = name;
            this.value = value;
        }

        public String getName() {
            return name;
        }

        public void setName(String name) {
            this.name = name;
        }

        public String getValue() {
            return value;
        }

        public void setValue(String value) {
            this.value = value;
        }
    }

    @SuppressWarnings("serial")
    private class Headers extends LinkedHashMap<String, Header> {
        public void addHeader(Header header) {
            put(header.getName(), header);
        }

        public void addHeader(String headerline) {
            if (headerline == null) throw new NullPointerException();
            int colon = headerline.indexOf(':');
            if (colon < 0) throw new IllegalArgumentException("Header line must contain colon.");
            String name = headerline.substring(0, colon);
            String value = headerline.substring(colon + 2);
            addHeader(new Header(name, value));
        }

        public String headerValue(String name) {
            Header header = get(name);
            if (header == null) return null;
            return header.getValue();
        }


    }

}
