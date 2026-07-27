package com.tawala.project.theme;

import java.util.Date;
import java.text.CharacterIterator;
import java.text.StringCharacterIterator;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.Lob;
import javax.persistence.ManyToOne;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

import com.tawala.domain.User;
import com.tawala.project.UserProject;
import com.tawala.web.controller.WellKnown;

@Entity
@Table(name = "user_file")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE)
public class UserUploadedFile {
	@Id
	@Column(name = "user_file_id", length = 15, nullable = false)
	private String id;

	@ManyToOne(fetch = FetchType.LAZY)
	@JoinColumn(name = "user_id", nullable = false)
	private User user;

	@SuppressWarnings("unused")
	@ManyToOne(fetch = FetchType.LAZY)
	@JoinColumn(name = "user_project_id", nullable = true)
	private UserProject userProject;

	@Lob
	@Column(name = "image_data", nullable = false)
	private byte[] data;

	@Column(name = "content_type", nullable = false, length = 100)
	private String contentType;

	@Column(name = "file_name", nullable = false, length = 20)
	private String fileName;

	@Column(name = "size", nullable = false)
	private int size;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "created_dt", nullable = false)
	private Date createdDate;

	UserUploadedFile() {
		// For Hibernate's use
	}

	public UserUploadedFile(User user, UserProject userProject, byte[] data,
			String contentType, String fileName, int size) {
		this(user, data, contentType, fileName, size);
		this.userProject = userProject;
	}

	public UserUploadedFile(User user, byte[] data, String contentType,
			String fileName, int size) {
		this.user = user;
		this.data = data;
		this.contentType = contentType;
		this.fileName = forHTML(fileName);
		this.size = size;
		this.createdDate = new Date();
	}

	public User getUser() {
		return user;
	}

	public String getId() {
		return id;
	}

	public Date getCreatedDate() {
		return createdDate;
	}

	public byte[] getData() {
		return data;
	}

	public String getContentType() {
		return contentType;
	}

	public int getSize() {
		return size;
	}

	public String getFileURL() {
		if (id == null) {
			throw new IllegalStateException(
					"Can't create file URL until file id is created.");
		}

		return WellKnown.urls.getUserFileDownloadPrefix() + "/" + id + "/"
				+ fileName;
	}

	public void setId(String id) {
		this.id = id;
	}
	
	public static String forHTML(String aText){
	     final StringBuilder result = new StringBuilder();
	     final StringCharacterIterator iterator = new StringCharacterIterator(aText);
	     char character =  iterator.current();
	     while (character != CharacterIterator.DONE ){
	       if (character == '<') {
	         result.append("&lt;");
	       }
	       else if (character == '>') {
	         result.append("&gt;");
	       }
	       else if (character == '%') {
	         result.append("&percnt;");
	       }
	       else {
	         //the char is not a special one
	         //add it to the result as is
	         result.append(character);
	       }
	       character = iterator.next();
	     }
	     return result.toString();
	  }	
}
