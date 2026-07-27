package com.tawala.web.oldhtml;


public class HtmlParagraph extends Block {

    public HtmlParagraph(Html... contents) {
        this();
        for (Html item : contents) {
            add(item);
        }
    }

    public HtmlParagraph(String contents) {
        this(new HtmlString(contents));
    }

    public HtmlParagraph() {
        super("p");
    }

    public HtmlParagraph(boolean newline) {
        super("p", newline);
    }

}
