package com.tawala.web.search;

import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.Serializable;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

public class SerializationSupport {

	@SuppressWarnings("unchecked")
	protected <T> T readObject(HttpServletRequest request) throws IOException, ClassNotFoundException {
		ObjectInputStream objectInputStream = new ObjectInputStream(request
				.getInputStream());
		try {
			return (T) objectInputStream.readObject();
		} finally {
			objectInputStream.close();
		}
	}

	protected Object[] readObjects(HttpServletRequest request, int expectedCount) throws IOException, ClassNotFoundException {
		ObjectInputStream objectInputStream = new ObjectInputStream(request
				.getInputStream());
		try {
			Object[] result = new Object[expectedCount];
			for (int i = 0; i < result.length; i++) {
				result[i] = objectInputStream.readObject();
			}
			return result;
		} finally {
			objectInputStream.close();
		}
	}

	protected void writeObject(HttpServletResponse response, Serializable result) throws IOException {
		ObjectOutputStream outputStream = new ObjectOutputStream(response
				.getOutputStream());
		try {
			outputStream.writeObject(result);
		} finally {
			outputStream.close();
		}
	}
	
}
