package fake.smtp;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.io.PrintWriter;
import java.io.StringWriter;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;

/**
 * A simple receive-only SMTP server meant for use as part of a test harness.
 */
public class FakeSmtpServer implements Runnable {

    private static int counter = 0;

    private final ServerSocket serverSocket;
    private final int serverPort;
    private volatile boolean keepRunning = true;
    private List<FakeSmtpMessage> messages = new ArrayList<FakeSmtpMessage>();
    private List<SmtpConnection> connections = new ArrayList<SmtpConnection>();

    public FakeSmtpServer(int port) throws IOException {
        serverSocket = new ServerSocket(port);
        serverPort = serverSocket.getLocalPort();
        Thread thread = new Thread(this, "MockSmtpServer-" + (counter++));
        thread.setDaemon(true);
        thread.start();

    }

    public FakeSmtpServer() throws IOException {
        this(0);
    }

    public int getPort() {
        return serverPort;
    }

    public void run() {
        try {
            String serverName = Thread.currentThread().getName();
            int connectionNumber = 1;
            while (keepRunning) {
                Socket client = serverSocket.accept();
                SmtpConnection handler = new SmtpConnection(this, client);
                childStarted(handler);
                Thread clientThread = new Thread(handler, serverName + "-connection-" + connectionNumber++);
                clientThread.start();
            }
        } catch (Exception exception) {
            reportException(exception);
        }
    }

    public static void reportException(Exception exception) {
        StringWriter buffer = new StringWriter();
        buffer.write("Exception in thread " + Thread.currentThread().getName() + ": ");
        exception.printStackTrace(new PrintWriter(buffer));
        System.err.print(buffer.toString());
    }


    public synchronized void addMessage(FakeSmtpMessage message) {
        messages.add(message);
    }

    public synchronized int getMessageCount() {
        return messages.size();
    }

    public synchronized FakeSmtpMessage getMessage(int index) {
        return (FakeSmtpMessage) messages.get(index);
    }

    public void shutDown() {
        keepRunning = false;
    }

    public synchronized void waitForAllConnectionsToClose() throws InterruptedException {
        while (connections.size() > 0) {
            wait();
        }
    }

    private synchronized void childStarted(SmtpConnection child) {
        connections.add(child);
    }

    private synchronized void childDone(SmtpConnection child) {
        connections.remove(child);
        notifyAll();
    }


    /**
     * Handles the SMTP conversation with a single client connection.
     */
    private class SmtpConnection implements Runnable {

        private FakeSmtpServer parent;
        private Socket socket;
        private BufferedReader in;
        private PrintWriter out;

        public SmtpConnection(FakeSmtpServer parent, Socket socket) throws IOException {
            this.parent = parent;
            this.socket = socket;
            in = new BufferedReader(new InputStreamReader(socket.getInputStream()));
            out = new PrintWriter(new OutputStreamWriter(socket.getOutputStream()));
        }

        public void run() {
            try {
                say("220 " + this.getClass());

                boolean done = false;
                FakeSmtpMessage message = null;
                while (!done) {
                    String command = in.readLine();
                    if (command == null) {
                        done = true;
                    } else if (command.equalsIgnoreCase("QUIT")) {
                        done = true;
                        say("221 Later, dude.");
                    } else if (command.startsWith("HELO")) {
                        say("250 Howdy, pardner.");
                    } else if (command.startsWith("MAIL FROM:")) {
                        String sender = extractTarget(command);
                        if (sender == null) {
                            say("501 Don't talk crazy");
                        } else {
                            // create message, add sender
                            message = new FakeSmtpMessage();
                            message.setSender(sender);
                            say("250 Sending from '" + sender + "'");
                        }
                    } else if (command.startsWith("RCPT TO:")) {
                        String recipient = extractTarget(command);
                        if (message == null) {
                            say("503 Need sender first");
                        } else if (recipient == null) {
                            say("501 Don't talk crazy");
                        } else {
                            message.addRecipient(recipient);
                            say("250 Sending to '" + recipient + "'");
                        }
                    } else if (command.equalsIgnoreCase("DATA")) {
                        if (message == null) {
                            say("503 Need sender first");
                        } else if (message.getRecipients().size() == 0) {
                            say("503 Need recipient first");
                        } else {
                            say("354 Go ahead; end with '.' on blank line");
                            StringBuffer data = new StringBuffer();
                            String line = null;
                            do {
                                line = in.readLine();
                                if (".".equals(line)) {
                                    message.setData(data.toString());
                                    parent.addMessage(message);
                                    say("250 " + message.getData().length() + " characters on their way");
                                    break;
                                }
                                data.append(line);
                                data.append('\n');
                            } while (line != null);
                        }
                    } else {
                        say("500 I dunno how to '" + command + "'");
                    }
                }
            } catch (Exception e) {
                reportException(e);
            } finally {
                out.close();
                try {
                    in.close();
                } catch (IOException e) {
                    reportException(e);
                }
                try {
                    socket.close();
                } catch (IOException e) {
                    reportException(e);
                }
            }

            parent.childDone(this);
        }

        private String extractTarget(String command) {
            if (command == null) return null;

            // find markers
            int firstMarker = command.indexOf('<');
            int lastMarker = command.lastIndexOf('>');
            if (firstMarker < 0) return null;
            if (lastMarker < 0) return null;
            if (lastMarker - firstMarker < 2) return null;

            return command.substring(firstMarker + 1, lastMarker);
        }

        private void say(String message) {
            out.println(message);
            out.flush();
        }

    }

    public String toString() {
        return "SmtpServer running on " + getPort() + " with " + getMessageCount() + " messages received";
    }

}
