package com.tawala.project;

import java.io.IOException;
import java.util.HashMap;
import java.util.Map;

import sun.misc.BASE64Decoder;

import com.scissor.xmlconfig.ConfigElement;
import com.scissor.xmlconfig.Factory;

public class Image {
    static final Factory<Data> FACTORY = new Factory<Data>();
    static {
        FACTORY.register("imagedata", Data.class);
    }

    private static Data.Format[] PREFERED_WEB_FORMATS = new Data.Format[] {
            Data.Format.GIF, Data.Format.JPEG, Data.Format.PNG };

    private String id;
    private Map<Data.Format, Data> dataByFormat = new HashMap<Data.Format, Data>();

    public Image(ConfigElement config) {
        this.id = config.attribute("id").stringValue();

        for (ConfigElement dataElement : config.children("imagedata")) {
            Data data = FACTORY.make(dataElement);
            dataByFormat.put(data.getFormat(), data);
        }
    }

    public String getId() {
        return id;
    }

    public Data getTheBestDataForWeb() {
        for (int i = 0; i < PREFERED_WEB_FORMATS.length; i++) {
            Data format = dataByFormat.get(PREFERED_WEB_FORMATS[i]);
            if (format != null)
                return format;
        }

        return null;
    }

    public static class Data {
        public static enum Format {
            PNG {
                public String getMimeType() {
                    return "image/png";
                }
            },
            GIF {
                public String getMimeType() {
                    return "image/gif";
                }
            },
            JPEG {
                public String getMimeType() {
                    return "image/jpeg";
                }
            };

            public abstract String getMimeType();
        }

        public Data(ConfigElement configElement) throws IOException {
            String formatName = configElement.attribute("imageFormat").stringValue();
            this.format = Format.valueOf(formatName);
            if (this.format == null) {
                throw new IllegalArgumentException(
                        "Unable to find image format for name '" + formatName
                                + "'.");
            }
            
            String dataInBase64 = configElement.text();
            this.imageData = new BASE64Decoder().decodeBuffer(dataInBase64);
        }

        private byte[] imageData;
        private Format format;

        public byte[] getImageData() {
            return imageData;
        }

        public Format getFormat() {
            return format;
        }
    }
}
