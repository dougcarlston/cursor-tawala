package com.scissor.webrobot;


public class BasicValidator implements PageValidator {
    public void validate(WebRobot bot) throws ValidationFaliedException {
        if (bot.responseCode() != 200) {
            fail("strange problem loading page " + bot.getUrl() + ":" + bot.responseCode());
        }

        if (bot.getContentType().equals("text/html")) {
            checkHtml(bot);
        }
    }

    private void checkHtml(WebRobot bot) {
        String pageText = null;
        try {
            pageText = bot.getPageText();
        } catch (RobotException e) {
            fail("problem reading response: " + e);
        }
        if (pageText == null) fail("no page text");
        if (pageText.indexOf("<html>") < 0) fail("missing <html> start tag");
        if (pageText.indexOf("<body") < 0) fail("missing <html> start tag");
        if (pageText.indexOf("</body>") < 0) fail("missing <html> start tag");
        if (pageText.indexOf("</html>") < 0) fail("missing <html> end tag");
    }

    private void fail(String problem) {
        throw new ValidationFaliedException(problem);
    }
}


