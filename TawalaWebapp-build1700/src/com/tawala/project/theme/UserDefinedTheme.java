package com.tawala.project.theme;

import java.util.ArrayList;
import java.util.Date;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.Lob;
import javax.persistence.ManyToOne;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.persistence.UniqueConstraint;
import javax.servlet.http.HttpServletRequest;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;
import org.json.JSONObject;

import com.scissor.Log;
import com.tawala.domain.User;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.oldhtml.Image;
import com.tawala.web.project.theme.RenderThemeController;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_user_theme_id")
@Entity
@Table(name = "user_theme", uniqueConstraints = { @UniqueConstraint(columnNames = {
		"user_id", "name" }) })
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE)
public class UserDefinedTheme implements ProjectTheme {
	@Id
	@Column(name = "user_theme_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@Column(name = "name", nullable = false, length = 40)
	private String name;

	@Column(name = "parent_theme", nullable = false, length = 40)
	private String parentThemeId;

	@ManyToOne(fetch = FetchType.LAZY)
	@JoinColumn(name = "user_id", nullable = false)
	private User user;

	@Lob
	@Column(name = "style_definitions", nullable = false)
	private String styleDefinitions;

	@ManyToOne(fetch = FetchType.EAGER)
	@JoinColumn(name = "header_image_id", nullable = true)
	private UserUploadedFile headerImage;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "created_dt", nullable = false)
	private Date createdDate;

	@Temporal(TemporalType.TIMESTAMP)
	@Column(name = "updated_dt", nullable = false)
	private Date lastUpdatedDate;

	public UserDefinedTheme() {
		// For Hibernate's use
	}

	public UserDefinedTheme(User user) {
		this.user = user;
		this.createdDate = new Date();
		this.lastUpdatedDate = this.createdDate;
	}

	private ProjectTheme getParentTheme() {
		CommonTheme result = CommonTheme.getThemeByPath(this.parentThemeId);
		if (result == null) {
			return CommonTheme.DEFAULT_THEME;
		}
		return result;
	}

	public String getStyleDefinitions() {
		return styleDefinitions;
	}

	public User getUser() {
		return user;
	}

	public long getId() {
		return id;
	}

	public String getName() {
		return name;
	}

	public Date getLastUpdatedDate() {
		return lastUpdatedDate;
	}

	public void setLastUpdatedDate(Date lastUpdatedDate) {
		this.lastUpdatedDate = lastUpdatedDate;
	}

	public String getParentThemeId() {
		return parentThemeId;
	}

	public void setParentThemeId(String parentThemeId) {
		this.parentThemeId = parentThemeId;
	}

	public Date getCreatedDate() {
		return createdDate;
	}

	public void setName(String name) {
		this.name = name;
	}

	public void setStyleDefinitions(String styleDefinitions) {
		this.styleDefinitions = styleDefinitions;
	}

	public List<String> getScreenStylesheetURLs() {
		List<String> result = new ArrayList<String>(getParentTheme()
				.getScreenStylesheetURLs());
		result.add(WellKnown.urls.getRenderUserDefinedTheme() + '?'
				+ RenderThemeController.THEME_ID + '=' + id);
		return result;
	}

	public List<String> getPrintStylesheetURLs() {
		return getParentTheme().getPrintStylesheetURLs();
	}

	public Image getHeaderImageHTML() {
		return headerImage == null ? getParentTheme().getHeaderImageHTML()
				: new Image(headerImage.getFileURL(), "Header Image");
	}

	public String getCSSContents(HttpServletRequest request) {
		ProjectTheme parentTheme = getParentTheme();
		return parentTheme.getCSSContents(request) + "\n"
				+ getStyleDefinitions();
	}

	public String getGoogleAdSenseCode() {
		return getParentTheme().getGoogleAdSenseCode();
	}

	public String getMainBackgroundColor() {
		return getParentTheme().getMainBackgroundColor();
	}

	public String getThemeId() {
		return String.valueOf(id);
	}

	public void setHeaderImage(UserUploadedFile image) {
		this.headerImage = image;
	}

	public UserUploadedFile getHeaderImage() {
		return headerImage;
	}

	public Map<String, Map<String, String>> getStyleDetails() {
		Map<String, Map<String, String>> result = null;
		if (styleDefinitions != null) {
			try {
				result = CSSDocumentHandler.parseCSS(styleDefinitions);
			} catch (Exception e) {
				Log.error(this, "Error parsing the stylesheet:", e);
			}
		}
		if (result == null) {
			result = new HashMap<String, Map<String, String>>();
		}
		return result;
	}

	public JSONObject getStyleDefailsAsJSONObject() throws Exception {
		return CSSDocumentHandler.convertStyleDetailsToJSONObject(getStyleDetails());
	}
}
