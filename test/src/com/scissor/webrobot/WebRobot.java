package com.scissor.webrobot;

import java.io.IOException;
import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.net.URL;
import java.util.Stack;
import java.util.regex.Pattern;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.xml.sax.SAXException;

import com.meterware.httpunit.Button;
import com.meterware.httpunit.GetMethodWebRequest;
import com.meterware.httpunit.SubmitButton;
import com.meterware.httpunit.WebClient;
import com.meterware.httpunit.WebConversation;
import com.meterware.httpunit.WebForm;
import com.meterware.httpunit.WebLink;
import com.meterware.httpunit.WebRequest;
import com.meterware.httpunit.WebRequestSource;
import com.meterware.httpunit.WebResponse;

/**
 * A project-generic web browser robot.
 */
public class WebRobot {
    public static final String URL_PREFIX = "http://ignored";
	private WebClient client;
    private Stack<WebResponse> responses = new Stack<WebResponse>();
    private PageValidator validator;
    private WebForm selectedForm;
    private Tracker tracker;

    public WebRobot() {
        this(new WebConversation());
    }

    protected WebRobot(WebClient client) {
        this.client = client;
        tracker = new Tracker();
    }

    public WebRobot(Tracker tracker) {
        this();
        this.tracker = tracker;
    }

    public WebResponse lastResponse() {
        return (WebResponse) responses.peek();
    }

    protected void setLastResponse(WebResponse lastResponse) throws RobotException {
        responses.push(lastResponse);
        tracker().gotResponse(this, lastResponse);
        selectedForm = null;
    }

    public WebClient getClient() {
        return client;
    }

    protected WebConversation getConversation() {
        return (WebConversation) client;
    }


    public String pageTitle() {
        try {
            return lastResponse().getTitle();
        } catch (SAXException e) {
            return "couldn't get title: " + e;
        }
    }

    public boolean isVerbose() {
        return tracker.isVerbose();
    }

    public void setVerbose(boolean verbose) {
        tracker.setVerbose(verbose);
    }

    protected Tracker tracker() {
        return tracker;
    }

    public void go(Path path) throws RobotException {
        go(path.toUrlString());
    }


    public void go(String urlOrPath) throws RobotException {
        if (!urlOrPath.matches("\\w+://.*")) urlOrPath = URL_PREFIX + urlOrPath;
        go(new GetMethodWebRequest(urlOrPath));
    }

    public void go(WebRequest request) throws RobotException {
        tracker.goingTo(this, request);
        WebResponse resp;
        try {
            resp = client.getResponse(request);
        } catch (IOException e) {
            throw new RobotException("failure browsing page " + request, e);
        } catch (SAXException e) {
            throw new RobotException("failure browsing page " + request, e);
        }
        gotResponse(resp);
    }

    public void go(URL url) throws RobotException {
        String urlString = url.toExternalForm();
        go(urlString);
    }

    public void back() {
        responses.pop();
        selectedForm = null;
    }

    protected void gotResponse(WebResponse resp) throws RobotException {
        setLastResponse(resp);
        if (validator != null) validator.validate(this);
    }

    public void followLink(String linkLabel) throws RobotException {
        WebLink link = getLink(linkLabel);
        if (link == null) throw new RobotException("no link '" + linkLabel + "'");
        go(link.getRequest());
    }

    /**
     * Follows a link within an element of the identified HTML class or ID
     */
    public void followLink(String linkLabel, String identifier) throws RobotException {
        WebLink link = getLink(linkLabel, identifier);
        if (link == null) throw new RobotException("no link '" + linkLabel + "' in class '" + identifier + "'");
        go(link.getRequest());
    }


    public WebLink getLink(String linkLabel) throws RobotException {
        if (lastResponse() == null) return null;
        WebLink link;
        try {
            link = lastResponse().getLinkWithID(linkLabel);
            if (link == null) link = lastResponse().getLinkWithName(linkLabel);
            if (link == null) link = getLinkUsingAlt(linkLabel);
            if (link == null) link = lastResponse().getLinkWith(linkLabel);
        } catch (SAXException e) {
            throw new RobotException("couldn't parse page " + lastResponse().getURL(), e);
        }
        return link;
    }

    /**
     * Gets a link within an element of the identified HTML class or ID
     */
    public WebLink getLink(String linkSearch, String containingHtml) throws RobotException {
        try {
            WebLink[] links = lastResponse().getLinks();
            for (int i = 0; i < links.length; i++) {
                WebLink link = links[i];
                if (!linkMatches(linkSearch, link)) continue;
                Node current = getRealNode(link);
                while (current != null) {
                    if (current instanceof Element) {
                        Element element = ((Element) current);
                        if (containingHtml.equals(element.getAttribute("class")) ||
                                containingHtml.equals(element.getAttribute("id"))) {
                            return link;
                        }
                    }
                    current = current.getParentNode();
                }
            }
            return null;
        } catch (SAXException e) {
            throw new RobotException("couldn't parse page " + lastResponse().getURL(), e);
        }
    }

    @SuppressWarnings("deprecation")
    private boolean linkMatches(String linkSearch, WebLink link) {
        if (linkSearch.equals(link.getID())) return true;
        if (matchesAltText(linkSearch, link)) return true;
        return link.asText().equals(linkSearch);
    }

    private WebLink getLinkUsingAlt(String altText) throws RobotException {
        try {
            WebLink[] links = lastResponse().getLinks();
            for (int i = 0; i < links.length; i++) {
                WebLink link = links[i];
                if (matchesAltText(altText, link)) return link;

            }
            return null;
        } catch (SAXException e) {
            throw new RobotException("couldn't parse page " + lastResponse().getURL(), e);
        }
    }

    private boolean matchesAltText(String altText, WebLink link) {
        Node subtree = link.getDOMSubtree();
        Node possibleImg = subtree.getFirstChild();
        if (possibleImg instanceof Element) {
            String altValue = ((Element) possibleImg).getAttribute("alt");
            if (altText.equals(altValue)) {
                return true;
            }
        }
        return false;
    }

    private boolean matchesAltText(String altText, Button button) {
        Node input = getRealNode(button);
        String altValue = ((Element) input).getAttribute("alt");
        return altText.equals(altValue);
    }

    public void followRegexLink(String regex) throws RobotException {
        WebLink link = getLinkUsingRegex(regex);
        if (link == null) throw new RobotException("no link matching pattern '" + regex + "'");
        go(link.getRequest());
    }

    @SuppressWarnings("deprecation")
    private WebLink getLinkUsingRegex(String regex) throws RobotException {
        Pattern pattern = Pattern.compile(regex);
        try {
            WebLink[] links = lastResponse().getLinks();
            for (int i = 0; i < links.length; i++) {
                WebLink link = links[i];
                if (pattern.matcher(link.getID()).find()) return link;
                if (pattern.matcher(link.getName()).find()) return link;
                if (pattern.matcher(link.asText()).find()) return link;
            }
        } catch (SAXException e) {
            throw new RobotException("couldn't parse page " + lastResponse().getURL(), e);
        }
        return null;
    }


    public URL getUrl() {
        return lastResponse().getURL();
    }

    public WebForm getForm(int index) throws RobotException {
        try {
            if(index >= lastResponse().getForms().length)  {
                throw new RobotException("The response contains only " + lastResponse().getForms().length + " forms (fewer than " + index + " requested):\n" + lastResponse().getText());
            }
            return lastResponse().getForms()[index];
        } catch (Exception e) {
            throw new RobotException("couldn't get form", e);
        }
    }

    public WebForm getForm(String idOrName) throws RobotException {
        WebForm form;
        try {
            form = lastResponse().getFormWithID(idOrName);
            if (form == null) form = lastResponse().getFormWithName(idOrName);
        } catch (SAXException e) {
            throw new RobotException("couldn't get form", e);
        }
        return form;
    }

    public boolean hasForm(String idOrName) throws RobotException {
        return getForm(idOrName) != null;
    }

    public void submit(WebForm form) throws RobotException {
        go(form.getRequest());
    }

    public void submit() throws RobotException {
        go(getSelectedForm().getRequest());
    }

    public void submit(String button) throws RobotException {
        submit(getSelectedForm(), button);
    }

    public void submit(WebForm form, String button) throws RobotException {
        SubmitButton submitButton = form.getSubmitButtonWithID(button);
        if (submitButton == null) submitButton = form.getSubmitButton(button);
        if (submitButton == null) submitButton = getSubmitButtonUsingAlt(form, button);
        if (submitButton == null)
            throw new RobotException("Unable to find a button named '" + button + "'");
        go(form.getRequest(submitButton));
    }

    private SubmitButton getSubmitButtonUsingAlt(WebForm form, String altText) {
        SubmitButton[] buttons = form.getSubmitButtons();
        for (int i = 0; i < buttons.length; i++) {
            SubmitButton button = buttons[i];
            if (matchesAltText(altText, button)) return button;
        }
        return null;
    }

    public void clickButton(String label) throws RobotException {
        WebForm[] forms = getPageForms();
        for (int i = 0; i < forms.length; i++) {
            WebForm form = forms[i];
            SubmitButton[] buttons = form.getSubmitButtons();
            for (int j = 0; j < buttons.length; j++) {
                SubmitButton button = buttons[j];
                if (label.equalsIgnoreCase(button.getID()) ||
                        label.equalsIgnoreCase(button.getName()) ||
                        label.equalsIgnoreCase(button.getValue())) {
                    go(form.getRequest(button));
                    return;
                }
            }
        }
        throw new RobotException("Couldn't find button '" + label + "' in page " + getUrl());
    }

    private WebForm[] getPageForms() throws RobotException {
        try {
            return lastResponse().getForms();
        } catch (SAXException e) {
            throw new RobotException("unable to parse page", e);
        }
    }


    public String getPageText() throws RobotException {
        try {
            return lastResponse().getText();
        } catch (IOException e) {
            throw new RobotException("couldn't load page " + lastResponse().getURL(), e);
        }
    }

    public Document getDocument() throws RobotException {
        try {
            return lastResponse().getDOM();
        } catch (SAXException e) {
            throw new RobotException("problem getting DOM", e);
        }
    }

    public String getContentType() {
        return lastResponse().getContentType();
    }

    public int getContentLength() {
        return lastResponse().getContentLength();
    }

    public boolean isAt(Path path) {
        if (path == null) return false;
        if (lastResponse() == null) return false;
        return path.equals(lastResponse().getURL());
    }

    public boolean hasLink(String label) throws RobotException {
        return getLink(label) != null;
    }

    public boolean pageContains(String text) throws RobotException {
        return getPageText().indexOf(text) >= 0;
    }

    public Path getPath() {
        return new Path(lastResponse().getURL());
    }

    public int responseCode() {
        if (lastResponse() == null) return 0;
        return lastResponse().getResponseCode();
    }

    public void setVaildator(PageValidator pageValidator) {
        this.validator = pageValidator;
    }

    public void selectForm(String idOrName) throws RobotException {
        WebForm form = getForm(idOrName);
        if (form == null)
            throw new RobotException("Cannot find form with id or name '" + idOrName + "'");
        setSelectedForm(form);
    }

    public void selectForm(int position) throws RobotException {
        setSelectedForm(getForm(position));
    }

    public String getParameter(String name) throws RobotException {
        return getSelectedForm().getParameterValue(name);
    }

    public String[] getParameters(String name) throws RobotException {
        return getSelectedForm().getParameterValues(name);
    }


    public void setParameter(String name, String value) throws RobotException {
        getSelectedForm().setParameter(name, value);
    }

    public void setParameters(String name, String[] values) throws RobotException {
        getSelectedForm().setParameter(name, values);
    }

    public void setCheckbox(String name, boolean value) throws RobotException {
        getSelectedForm().setCheckbox(name, value);
    }

    public WebForm getSelectedForm() throws RobotException {
        if (selectedForm == null) {
            try {
                selectedForm = getForm(0);
            } catch (RobotException e) {
                throw new RobotException("No forms to use", e);
            }
        }
        return selectedForm;
    }

    public void setSelectedForm(WebForm selectedForm) {
        this.selectedForm = selectedForm;
    }


    protected void setAuthorization(String id, String pass) {
        client.setAuthorization(id, pass);
    }

    public void setErrorsExpected(boolean errorsExpected) {
        client.setExceptionsThrownOnErrorStatus(!errorsExpected);
    }

    public Table getTable(int pos) throws RobotException {
        try {
            return new Table(lastResponse().getTables()[pos]);
        } catch (SAXException e) {
            throw new RobotException("couldn't get tables", e);
        }
    }

    public Table getTableStartingWith(String text) throws RobotException {
        try {
            return new Table(lastResponse().getTableStartingWith(text));
        } catch (SAXException e) {
            throw new RobotException("couldn't get tables", e);
        }
    }

    public Table getTableWithId(String id) throws RobotException {
        try {
            return new Table(lastResponse().getTableWithID(id));
        } catch (SAXException e) {
            throw new RobotException(e);
        }
    }

    public boolean hasCookie(String name) {
        if (name == null) return false;
        String[] names = client.getCookieNames();
        for (int i = 0; i < names.length; i++) {
            if (name.equals(names[i])) return true;
        }
        return false;
    }

    public String[] getCookieNames() {
        return client.getCookieNames();
    }

    public String getCookieValue(String name) {
        return client.getCookieValue(name);
    }

    public void copyCookies(WebRobot other) {
        String[] cookieNames = other.client.getCookieNames();
        for (int i = 0; i < cookieNames.length; i++) {
            String cookieName = cookieNames[i];
            setCookie(cookieName, other.client.getCookieValue(cookieName));
        }
    }

    public void setCookie(String cookieName, String cookieValue) {
        client.putCookie(cookieName, cookieValue);
    }

    // TODO: better cookie API: check other attributes, whether just added or just deleted

    private Node getRealNode(WebRequestSource item) {
        return getNode(WebRequestSource.class, item);
    }

    private Node getRealNode(Button item) {
        Class baseClass = Button.class.getSuperclass().getSuperclass();
        return getNode(baseClass, item);
    }

    private Node getNode(Class theClass, Object item) {
        try {
            Method getNode = theClass.getDeclaredMethod("getNode", new Class[0]);
            getNode.setAccessible(true);
            return (Node) getNode.invoke(item, new Object[0]);
        } catch (IllegalAccessException e) {
            throw new RuntimeException("Unexpected problem", e);
        } catch (InvocationTargetException e) {
            throw new RuntimeException("Unexpected problem", e);
        } catch (NoSuchMethodException e) {
            throw new RuntimeException("Unexpected problem", e);
        }
    }

    public void dumpPage() {
        System.out.println("----------------");
        System.out.println("conversation = " + client);
        WebPageDumper dumper = new WebPageDumper(System.out);
        dumper.dump(lastResponse());
        System.out.println("----------------");
    }
    
    public void clearResponses() {
    	responses.clear();
    }
}
