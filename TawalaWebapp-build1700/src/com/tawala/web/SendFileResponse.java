package com.tawala.web;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.SocketException;
import java.util.List;

import javax.servlet.ServletOutputStream;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import com.scissor.Log;
import com.scissor.StreamCopier;
import com.tawala.World;
import com.tawala.web.admin.UrgentMessage;

public class SendFileResponse extends Response {
    private final String resourcePath;

    public SendFileResponse(String resourcePath) {
        super();
        this.resourcePath = resourcePath;
    }

    protected void handle(HttpServletRequest request, HttpServletResponse response, World world) throws IOException {
        String contentType = contentType(resourcePath);

        if (contentType == null) {
            response.sendError(404);
            Log.warn(this, "unknown content type for " + resourcePath);
            return;
        }

        byte[] resource = loadResource(resourcePath, world);

        if (resource == null) {
            response.sendError(404);
            Log.warn(this, "couldn't load contents for " + resourcePath + " from " + world.getContentRoots());
            return;
        }

        response.setContentType(contentType);
        response.setContentLength(resource.length);
        ServletOutputStream output = response.getOutputStream();
        try {
            output.write(resource);
        }
        catch(SocketException e) {
            if(e.getMessage().equals("Connection reset")) {
                Log.warn(this, "Client aborted download of " + resourcePath);
            }
            else {
                throw e;
            }
        } finally {
            output.close();
        }
    }

    public static String contentType(String fullPath) {
        if (fullPath.endsWith(".txt")) {
            return "text/plain";
        } else if (fullPath.endsWith(".css")) {
            return "text/css";
        } else if (fullPath.endsWith(".html")) {
            return "text/html";
        } else if (fullPath.endsWith(".gif")) {
            return "image/gif";
        } else if (fullPath.endsWith(".ico")) {
            return "image/x-icon";
        } else if (fullPath.endsWith(".png")) {
            return "image/png";
        } else if (fullPath.endsWith(".jpg")) {
            return "image/jpeg";
        } else if (fullPath.endsWith(".js")) {
            return "text/javascript";
        } else if (fullPath.endsWith(".exe")) {
            return "application/octet-stream";
        } else if (fullPath.endsWith(".pdf")) {
            return "application/pdf";
        } else if (fullPath.endsWith(".p3p")) {
            return "application/xml";
        } else if (fullPath.endsWith(".xml")) {
            return "application/xml";
        } else {
            return null;
        }
    }
    
    private static byte[] loadResource(String fullPath, World world) throws IOException {
        List<String> contentRoots = world.getContentRoots();
        for (String root : contentRoots) {
            File file = new File(root + "/" + fullPath);
            if (file.exists()) {
                return loadBytes(new FileInputStream(file));
            }
        }
        return null;
    }

    private static byte[] loadBytes(InputStream stream) throws IOException {
        try {
            ByteArrayOutputStream bytes = new ByteArrayOutputStream();
            StreamCopier.copy(stream, bytes);
            return bytes.toByteArray();
        } finally {
            stream.close();
        }
    }

    @Override
	public void handleUrgentNotificationMessage(UrgentMessage urgentMessage) {
		//--- Do nothing
	}
}
