package com.tawala.project.builder;

import java.util.ArrayList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.scissor.XmlBuffer;
import com.tawala.project.Form;

public class FormBuilder extends TagBuilder {
    private final String name;
    private String startingPointValue = "";
    private String dataEntryOnlyValue = "";
    private String themePath = "";
    private ProcessBlockBuilder postProcess;
    private ProcessBlockBuilder preProcess;
    private final NumericCounter questionLabel = new NumericCounter("Q", 1);
    private final NumericCounter textLabel = new NumericCounter("T", 1);
	private String dataSourceName;

    public FormBuilder() {
        this.name = "aForm";
    }

    public FormBuilder(String name) {
        this.name = name;
    }

    public FormBuilder(String name, Boolean isStartingPoint) {
        this(name);
        startingPointValue = (isStartingPoint ? "true" : "false");
    }

    public FormBuilder(String name, Boolean isStartingPoint, Boolean isDataEntryOnly) {
        this(name, isStartingPoint);
        dataEntryOnlyValue = (isDataEntryOnly ? "true" : "false");
    }

    public FormBuilder(String name, Boolean isStartingPoint, String themePath) {
        this(name);
        startingPointValue = (isStartingPoint ? "true" : "false");
        this.themePath = themePath;
    }

    protected void startTag(XmlBuffer xml) {
        List<String> attributes = new ArrayList<String>();
        attributes.add("name");
        attributes.add(name);
        if (postProcess != null) {
            attributes.add("process");
            attributes.add(postProcess.getName());
        }
        if (preProcess != null) {
            attributes.add("preProcess");
            attributes.add(preProcess.getName());
        }
        if (startingPointValue != "") {
        	attributes.add("startPoint");
        	attributes.add(startingPointValue);
        }
        if (dataEntryOnlyValue != "") {
        	attributes.add("dataEntryOnly");
        	attributes.add(dataEntryOnlyValue);
        }
        if (themePath != "") {
        	attributes.add("themePath");
        	attributes.add(themePath);
        }
        if (dataSourceName != null) {
        	attributes.add("dataSourceName");
        	attributes.add(dataSourceName);
        }
        xml.startTag("form", attributes.toArray(new String[attributes.size()]));
        if (hasContent()) xml.startTag("items");
    }

    protected void endTag(XmlBuffer xml) {
        if (hasContent()) xml.endTag("items");
        xml.endTag("form");
    }

    public void addDeclaredFields(String ... fieldNames) {
    	for (String fieldName : fieldNames) {
			contents().tag("field", "name", fieldName);
		}
    }

    public void setPostProcess(ProcessBlockBuilder process) {
        this.postProcess = process;
    }

    public void setPreProcess(ProcessBlockBuilder process) {
        this.preProcess = process;
    }

    public void addFib(String question, int... blankLengths) {
        FibBuilder builder = addFib();
        builder.addText(question);
        for (int length : blankLengths) {
            builder.addText(" ");
            builder.addBlank(length);
        }
    }

    public void addFib(String question, String alternateLabel, int length) {
        FibBuilder fib = addFib();
        fib.addText(question);
        fib.addText(" ");
        fib.addBlank(alternateLabel, length);
    }

    public void addFib(String question, String altQuestionLabel, String alternateLabel, int length) {
        FibBuilder fib = addFib(altQuestionLabel);
        fib.addText(question);
        fib.addText(" ");
        fib.addBlank(alternateLabel, length);
    }

    public void addFibNoBlankAlt(String question, int length, Boolean required) {
        FibBuilder fib = addFib();
        fib.addText(question);
        fib.addText(" ");
        fib.addBlank(length, required);
    }

    public void addFibNoBlankAlt(String question, String altQuestionLabel, int length) {
        FibBuilder fib = addFib(altQuestionLabel);
        fib.addText(question);
        fib.addText(" ");
        fib.addBlank(length);
    }

    public void addFibNoBlankAlt(String question, String altQuestionLabel, int length, Boolean required) {
        FibBuilder fib = addFib(altQuestionLabel);
        fib.addText(question);
        fib.addText(" ");
        fib.addBlank(length, required);
    }
    
    public void addFibWithPhoneNumberValidatedBlank(String question, int length) {
        FibBuilder builder = addFib();
        builder.addText(" ");
        builder.addBlankWithPhoneNumberValidation(length);
    }
    
    public void addFibWithDollarAmountValidatedBlank(String question, int length) {
        FibBuilder builder = addFib();
        builder.addText(" ");
        builder.addBlankWithDollarAmountValidation(length);
   }

    public void addMc(String question, String... choices) {
        addMcWithAlternateLabel(null, question, false, false, choices);
    }

    public void addMc(String question, boolean onlyOne, boolean required, String... choices) {
        addMcWithAlternateLabel(null, question, onlyOne, required, choices);
    }

    public void addMc(String question, String style, boolean onlyOne, boolean required, String... choices) {
        addMcWithAlternateLabel(null, question, style, onlyOne, required, choices);
    }

    public void addMultiColumnMc(String question, int columnCount, boolean onlyOne, boolean required, String... choices) {
        addMcWithAlternateLabel(null, question, "multicolumn", columnCount, onlyOne, required, choices);
    }

    public void addMcWithCustomDataProvider(String alternateLabel, String question, boolean onlyOne, boolean required, ComponentBuilder componentBuilder) {
        contents().startTag("mc", false, "label", questionLabel.next(), "alternateLabel", alternateLabel,
                "onlyone", "" + onlyOne, "required", "" + required);
        contents().startTag("question", false);
        contents().startTag("paragraph", false);
        contents().startTag("font", false, "face", "Arial", "color", "000000", "size", "200");
        contents().text(question);
        contents().endTag("font", false);
        contents().endTag("paragraph", false);
        contents().endTag("question");
        contents().startTag("data-provider");
        componentBuilder.render(contents());
        contents().endTag("data-provider");
        
        contents().endTag("mc");
    }

    public void addMcWithAlternateLabel(String alternateLabel, String question, boolean onlyOne, boolean required, String... choices) {
        contents().startTag("mc", false, "label", questionLabel.next(), "alternateLabel", alternateLabel,
                "onlyone", "" + onlyOne, "required", "" + required);
        contents().startTag("question", false);
        contents().startTag("paragraph", false);
        contents().startTag("font", false, "face", "Arial", "color", "000000", "size", "200");
        contents().text(question);
        contents().endTag("font", false);
        contents().endTag("paragraph", false);
        contents().endTag("question");
        char id = 'a';
        for (int i = 0; i < choices.length; i++) {
            contents().startTag("choice", false, "label", "" + id++);
            contents().startTag("paragraph", false);
            contents().startTag("font", false, "face", "Arial", "color", "000000", "size", "200");
            contents().text(choices[i]);
            contents().endTag("font", false);
            contents().endTag("paragraph", false);
            contents().endTag("choice");
        }
        contents().endTag("mc");
    }

    public void addMcWithAlternateLabel(String alternateLabel, String question, String style, boolean onlyOne, boolean required, String... choices) {
        contents().startTag("mc", false, "label", questionLabel.next(), "alternateLabel", alternateLabel,
                "onlyone", "" + onlyOne, "required", "" + required, "style", "" + style);
        contents().startTag("question", false);
        contents().startTag("paragraph", false);
        contents().startTag("font", false, "face", "Arial", "color", "000000", "size", "200");
        contents().text(question);
        contents().endTag("font", false);
        contents().endTag("paragraph", false);
        contents().endTag("question");
        char id = 'a';
        for (int i = 0; i < choices.length; i++) {
            contents().startTag("choice", false, "label", "" + id++);
            contents().startTag("paragraph", false);
            contents().startTag("font", false, "face", "Arial", "color", "000000", "size", "200");
            contents().text(choices[i]);
            contents().endTag("font", false);
            contents().endTag("paragraph", false);
            contents().endTag("choice");
        }
        contents().endTag("mc");
    }

    public void addMcWithAlternateLabel(String alternateLabel, String question, String style, int columnCount, boolean onlyOne, boolean required, String... choices) {
        contents().startTag("mc", false, "label", questionLabel.next(), "alternateLabel", alternateLabel,
                "onlyone", "" + onlyOne, "required", "" + required, "style", "" + style, "columnCount", "" + columnCount);
        contents().startTag("question", false);
        contents().startTag("paragraph", false);
        contents().startTag("font", false, "face", "Arial", "color", "000000", "size", "200");
        contents().text(question);
        contents().endTag("font", false);
        contents().endTag("paragraph", false);
        contents().endTag("question");
        char id = 'a';
        for (int i = 0; i < choices.length; i++) {
            contents().startTag("choice", false, "label", "" + id++);
            contents().startTag("paragraph", false);
            contents().startTag("font", false, "face", "Arial", "color", "000000", "size", "200");
            contents().text(choices[i]);
            contents().endTag("font", false);
            contents().endTag("paragraph", false);
            contents().endTag("choice");
        }
        contents().endTag("mc");
    }

    public void addMcWithAlternateLabel(String alternateLabel, String question, String... answers) {
        addMcWithAlternateLabel(alternateLabel, question, false, false, answers);
    }


    public FibBuilder addFib() {
        FibBuilder fib = new FibBuilder(questionLabel.next());
        add(fib);
        return fib;
    }

    public FibBuilder addFib(String alternateLabel) {
        FibBuilder fib = new FibBuilder(questionLabel.next(), alternateLabel);
        add(fib);
        return fib;
    }

    public void addText(String text) {
        addText(null, text);
    }

    public void addText(String alternateLabel, String text) {
        contents().startTag("text", false, "label", textLabel.next(), "alternateLabel", alternateLabel);
        contents().startTag("paragraph");
        contents().startTag("font");
        contents().text(text);
        contents().endTag("font");
        contents().endTag("paragraph");
        contents().endTag("text");
    }

    public void addTextWithFields(String... textAndFields) {
        contents().startTag("text", false, "label", textLabel.next());

        for (int i = 0; i < textAndFields.length; i++) {

            // look for match with field pattern
            Pattern pattern = Pattern.compile("<<(.*)>>");
            Matcher matcher = pattern.matcher(textAndFields[i]);

            if (matcher.matches()) {

                // extract field name and add field to XML
                String fieldName = matcher.group(1);
                addField(fieldName);
            } else {
                // add text to XML
                contents().text(textAndFields[i]);
            }
        }

        contents().endTag("text");
    }

    public void addTextWithImage(String text, ImageInstanceBuilder imageInstanceBuilder) {
        contents().startTag("text", false, "label", textLabel.next());
        
        contents().text(text);
        startParagraph();
        imageInstanceBuilder.render(contents());
        endParagraph();

        contents().endTag("text");
    }

    public void addFontTextWithFields(String fontFace, String fontSizeInTwips, String fontColorInHex, String... textAndFields) {
        contents().startTag("text", false, "label", textLabel.next());

        for (int i = 0; i < textAndFields.length; i++) {

            // look for match with field pattern
            Pattern pattern = Pattern.compile("<<(.*)>>");
            Matcher matcher = pattern.matcher(textAndFields[i]);

        	startParagraph();
        	startFont(fontFace, fontSizeInTwips, fontColorInHex);

            if (matcher.matches()) {

                // extract field name and add field to XML
                String fieldName = matcher.group(1);
                addField(fieldName);
            } else {
                // add text to XML
                contents().text(textAndFields[i]);
            }
            
            endFont();
            endParagraph();
        }

        contents().endTag("text");
    }

    public void addFontTextWithFields(String... textAndFields) {
    	addFontTextWithFields("Microsoft Sans Serif", "180", "000000", textAndFields);
    }

    public void addField(String id) {
        contents().tag("field", false, "name", id);
    }

    public void addBreak() {
        contents().tag("break");
    }

    public SkipBlockBuilder addSkip() {
        SkipBlockBuilder skipBuilder = new SkipBlockBuilder();
        super.add(skipBuilder);
        return skipBuilder;
    }

    public Form build() {
        return new Form(asConfig());
    }

    public String getName() {
        return name;
    }

    private void startParagraph() {
    	contents().startTag("paragraph", "indent", "0", "align", "left");
    }

    private void endParagraph() {
    	contents().endTag("paragraph");
    }

    private void startFont(String face, String size, String color) {
    	contents().startTag("font", "face", face, "size", size, "color", color);
    }

    private void endFont() {
    	contents().endTag("font");
    }

	public void setExternalDataSource(String dataSource) {
		this.dataSourceName = dataSource;
	}

	public void addFileUpload(String fieldName, String text, boolean required) {
        contents().startTag("file", false, "label", "F1", "alternateLabel", fieldName, "required", String.valueOf(required));
        contents().startTag("paragraph");
        contents().startTag("font");
        contents().text(text);
        contents().endTag("font");
        contents().tag("fileNameInput");
        contents().endTag("paragraph");
        contents().endTag("file");
	}
}