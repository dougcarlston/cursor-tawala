package com.tawala.project.builder;

import com.scissor.XmlBuffer;
import com.scissor.XmlRenderable;
import com.tawala.project.Image;

public class ImageBuilder extends TagBuilder {
    public static class ImageTag implements XmlRenderable {
        private String id;
        private ImageData[] data;
        
        public ImageTag(String id, ImageData ... data) {
            this.id = id;
            this.data = data;
        }
        
        public void render(XmlBuffer xml) {
            xml.startTag("imagedef", true, "id", id);
            for (int i = 0; i < data.length; i++) {
                data[i].render(xml);
            }
            xml.endTag("imagedef");
        }
        
    };

    public static class ImageData implements XmlRenderable {
        private Image.Data.Format format;
        private String data;

        public ImageData(Image.Data.Format format, String data) {
            this.format = format;
            this.data = data;
        }
        
        public void render(XmlBuffer xml) {
            xml.startTag("imagedata", true, "imageFormat", format.toString());
            xml.text(data);
            xml.endTag("imagedata");
        }
        
    }
	
    public ImageBuilder() {
    	super();
    }
    
    public void addImage(String id, ImageData ... data) {
        add(new ImageTag(id, data));
    }
    
    protected void startTag(XmlBuffer xml) {
        xml.startTag("images");
    }

    protected void endTag(XmlBuffer xml) {
        xml.endTag("images");
    }
}
