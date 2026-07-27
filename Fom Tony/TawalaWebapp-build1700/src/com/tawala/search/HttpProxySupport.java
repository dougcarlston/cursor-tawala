package com.tawala.search;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.OutputStream;
import java.io.Serializable;
import java.net.HttpURLConnection;
import java.net.MalformedURLException;
import java.net.URL;

import com.scissor.Log;

abstract public class HttpProxySupport {

	protected void handlePut(URL url, Serializable ... objects)
			throws MalformedURLException, IOException {
		handleConnection(url, false, objects);
	}

	protected Object handleGet(URL url, Serializable ... objects)
			throws MalformedURLException, IOException {
		return handleConnection(url, true, objects);
	}

	@SuppressWarnings("unchecked")
	private<T> T handleConnection(URL url, boolean extractObjectFromInput, Serializable ... objectsToSend) throws MalformedURLException,
			IOException {
		long start = System.currentTimeMillis();
		
		HttpURLConnection connection = (HttpURLConnection) url.openConnection();
		connection.setDoOutput(true);
		connection.setDoInput(true);
		connection.connect();
		try {
			if (objectsToSend != null) {
				OutputStream outputStream = connection.getOutputStream();
				ObjectOutputStream objectOutputStream = new ObjectOutputStream(
						outputStream);
				try {
					for (int i = 0; i < objectsToSend.length; i++) {
						objectOutputStream.writeObject(objectsToSend[i]);	
					}
				} finally {
					objectOutputStream.close();
				}
			}

			T result = null;
			if (extractObjectFromInput) {
				ObjectInputStream objectInputStream = new ObjectInputStream(
						connection.getInputStream());
				try {
					result = (T) objectInputStream.readObject();
				} catch (ClassNotFoundException e) {
					throw new RuntimeException(e);
				}
			}
			if (connection.getResponseCode() != HttpURLConnection.HTTP_OK) {
				throw new IllegalStateException(
						"Connection failed with response code "
								+ connection.getResponseCode() + " "
								+ connection.getResponseMessage());
			}

			Log.info(this, "Connection to " + url + " in " + (System.currentTimeMillis() - start)  + " msecs.");
			return result;
		} finally {
			connection.disconnect();
		}
	}

}
