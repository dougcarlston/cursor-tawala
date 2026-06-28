package com.tawala.web;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.dom4j.Document;
import org.dom4j.DocumentFactory;
import org.dom4j.Element;
import org.dom4j.io.OutputFormat;
import org.dom4j.io.XMLWriter;

import com.tawala.World;
import com.tawala.web.admin.UrgentMessage;

public abstract class ApiResponse extends Response {
    private final List<Message> messages = new ArrayList<Message>();

    public void handle(HttpServletRequest request, HttpServletResponse response, World world) throws IOException {
        response.setContentType("text/xml");
        Document doc = DocumentFactory.getInstance().createDocument();
        Element root = doc.addElement("response");
        root.addAttribute("status", status());
        for (Message message : messages) {
            message.addTo(root);
        }
        addContents(root, request);
        XMLWriter writer = new XMLWriter(response.getWriter(), OutputFormat.createPrettyPrint());
        writer.write(doc);
        writer.flush();

    }

    @Override
    public void handleUrgentNotificationMessage(UrgentMessage urgentMessage) {
    	//--- Do nothing (yet)
    }
    
    public void addDebugMessage(String message) {
        messages.add(new Message(MessageType.DEBUG, message));
    }

    public void addErrorMessage(String message) {
        messages.add(new Message(MessageType.ERROR, message));
    }

    public void addErrorMessage(String id, String message) {
        messages.add(new Message(MessageType.ERROR, id, message));
    }


    protected abstract void addContents(Element root, HttpServletRequest request);

    protected abstract String status();

    private static enum MessageType {
        DEBUG, INFO, WARNING, ERROR
    }

    private static class Message {
        private final MessageType type;
        private final String id;
        private final String message;

        public Message(MessageType type, String id, String message) {
            this.type = type;
            this.id = id;
            this.message = message;
        }

        public Message(MessageType type, String message) {
            this(type, null, message);
        }

        public void addTo(Element root) {
            Element element = root.addElement(type.name().toLowerCase());
            if (id != null) element.addAttribute("id", id);
            element.addAttribute("message", message);
        }
    }
}
