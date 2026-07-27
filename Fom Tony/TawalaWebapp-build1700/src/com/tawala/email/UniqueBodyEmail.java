package com.tawala.email;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.util.zip.GZIPInputStream;
import java.util.zip.GZIPOutputStream;

import javax.mail.MessagingException;
import javax.mail.internet.MimeMessage;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.Lob;
import javax.persistence.Transient;

@Entity
abstract public class UniqueBodyEmail extends Email {
	public static enum Type {
		TEXT {
			public String getContentType() {
				return "text/plain";
			}
		},
		HTML {
			public String getContentType() {
				return "text/html";
			}
		};

		abstract public String getContentType();
	}

	@Lob
	@Column(name = "body", nullable = true)
	private byte[] body;

	@Column(name = "compressed", nullable = true)
	private Boolean compressed;
	
	@Column(name = "orig_size", nullable = true)
	private int originalBodySize;

	@Column(name = "compressed_size", nullable = true)
	private int compressedBodySize;

	@Transient
	private byte[] uncompressedBody;

	@Column(name = "type", length = 100, nullable = true)
	@Enumerated(EnumType.STRING)
	private Type type;

	public UniqueBodyEmail() {
		// --- For Hibernate's use
	}

	UniqueBodyEmail(String from, String to, String cc, String subject,
			Type type, String messageText) throws IOException {
		super(from, to, cc, subject);
		setBody(messageText.getBytes());
		this.type = type;
	}

	byte[] getBody() {
		return body;
	}

	void setBody(byte[] newBody) throws IOException {
		this.originalBodySize = newBody.length;
		this.compressed = true;
		this.uncompressedBody = newBody;
		this.body = compress(newBody);
		this.compressedBodySize = this.body.length;
	}

	private static byte[] compress(byte[] body) throws IOException {
		ByteArrayOutputStream output = new ByteArrayOutputStream();
		GZIPOutputStream gzipOutput = new GZIPOutputStream(output);
		try {
			gzipOutput.write(body);
		} finally {
			gzipOutput.close();
			output.close();
		}

		return output.toByteArray();
	}

	public String getMessageText() throws IOException {
		if (body == null) {
			throw new IllegalStateException(
					"Message body hasn't been retrived from the database.");
		}
		if (!compressed) {
			return new String(body);
		} else {
			if (uncompressedBody == null) {
				uncompressedBody = uncompress(body);
			}
			return new String(uncompressedBody);
		}
	}

	private static byte[] uncompress(byte[] body) throws IOException {
		byte[] buffer = new byte[2048];
		ByteArrayOutputStream output = new ByteArrayOutputStream();
		GZIPInputStream input = new GZIPInputStream(new ByteArrayInputStream(
				body));
		try {
			int readBytes = 0;
			while ((readBytes = input.read(buffer)) > 0) {
				output.write(buffer, 0, readBytes);
			}
		} finally {
			input.close();
		}
		return output.toByteArray();
	}

	public Type getType() {
		return type;
	}

	void setType(Type type) {
		this.type = type;
	}

	@Override
	protected void createBody(MimeMessage mimeMessage)
			throws MessagingException, IOException {
		mimeMessage.setContent(getMessageText(), type.getContentType());
	}

	public int getCompressedBodySize() {
		return compressedBodySize;
	}

	public int getOriginalBodySize() {
		return originalBodySize;
	}
}
