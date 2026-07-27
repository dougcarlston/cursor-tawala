package com.tawala.domain;

import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.security.MessageDigest;
import java.util.Date;
import java.util.Random;
import java.util.Set;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Embedded;
import javax.persistence.Entity;
import javax.persistence.EnumType;
import javax.persistence.Enumerated;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.JoinTable;
import javax.persistence.ManyToMany;
import javax.persistence.OneToOne;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;
import javax.persistence.Transient;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

import sun.misc.BASE64Encoder;

import com.tawala.project.Project;
import com.tawala.util.RandomTokenGenerator;
import com.tawala.web.controller.WellKnown;
import com.tawala.web.user.EmailVerificationController;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_user_id")
@Entity
@Table(name = "users")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "users")
public class User {
	private static Random randomNumberGenerator = new Random(System
			.currentTimeMillis());
	@Id
	@Column(name = "user_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long databaseId;

	@Column(name = "user_name", unique = true, length = 40, nullable = false)
	private String userName;

	@SuppressWarnings("unused")
	@Column(name = "normal_user_name", unique = true, length = 40, nullable = false)
	private String normalizedUserName;

	@Column(name = "first_name", length = 20, nullable = true)
	private String firstName;

	@Column(name = "last_name", length = 30, nullable = true)
	private String lastName;

	@Column(name = "password", length = 40, nullable = false)
	private String password;

	transient private String realPassword;

	@Embedded
	private EmailAddress email;

	@Column(name = "email_valid_token", length = 40, nullable = true)
	private String emailValidationToken;

	@Column(name = "registration_dt", nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date registrationDate = new Date();

	@Column(name = "last_logged_in_dt", nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date lastLoggedInDate;

	@Column(name = "email_valid_dt", nullable = true)
	@Temporal(TemporalType.TIMESTAMP)
	private Date emailValidationDate;

	@Column(name = "admin", nullable = false)
	private boolean administrator;

	@Enumerated(EnumType.STRING)
	@Column(name = "status", length = 20, nullable = false)
	private Status status = Status.REGISTERED_INITIAL;

	@Column(name = "password_reset", nullable = false)
	private boolean requirePasswordReset;

	@Column(name = "suspended", nullable = false)
	private boolean suspended = false;

	@Column(name = "original_domain", length = 60, nullable = true)
	private String originalDomain;

	@Column(name = "visitor_id", nullable = true)
	private Long originalVisitorId;

	@Transient
	private UserPreferences preferences = new UserPreferences();

	@OneToOne(fetch = FetchType.LAZY, cascade = { CascadeType.ALL })
	@JoinColumn(name = "shared_storage_project_id", nullable = true)
	private Project sharedStorage;

	@Column(name = "enable_ads", nullable = false)
	private boolean enableAds = false;

	@Column(name = "paypal_account", length = 50, nullable = true)
	private String payPalAccountId;

	@ManyToMany(targetEntity=Role.class, cascade={CascadeType.ALL})
	@JoinTable(name="user_role", joinColumns=@JoinColumn(name="user_id"), inverseJoinColumns=@JoinColumn(name="role_id"))
	private Set<Role> roles;

	User() {
		// For Hibernate's use
	}

	public User(Status status) {
		this.status = status;
	}

	public User(String id, String firstName, String lastName,
			EmailAddress email, String password) {
		this(id, firstName, lastName, email, password, false);
	}

	public long getDatabaseId() {
		return databaseId;
	}

	public User(String id, String firstName, String lastName,
			EmailAddress email, String password, boolean isAdmin) {
		this.setId(id);
		this.firstName = firstName;
		this.lastName = lastName;
		this.email = email;
		this.password = password;
		this.setAdministrator(isAdmin);
	}

	public void setEmail(EmailAddress email) {
		this.email = email;
	}

	public void setId(String id) {
		this.userName = id;
		this.normalizedUserName = normalizeUserName(id);
	}

	public static String normalizeUserName(String id) {
		return id.toLowerCase();
	}

	public String getId() {
		return userName;
	}

	public EmailAddress getEmail() {
		return email;
	}

	public String getPassword() {
		if (realPassword == null)
			throw new IllegalStateException(
					"Real password is not set. You can't call this method if the password wasn't set or the object was retrieved from the database.");

		return realPassword;
	}

	public void setPassword(String password) {
		this.realPassword = password;
		this.password = hashPassword(password);
	}

	public String getFirstName() {
		return firstName;
	}

	public void setFirstName(String firstName) {
		this.firstName = firstName;
	}

	public String getLastName() {
		return lastName;
	}

	public void setLastName(String lastName) {
		this.lastName = lastName;
	}

	public Status getStatus() {
		// Needed for proper conversion of older XML files using XStream. It
		// should be removed once we convert to another persistence model.
		if (status == null)
			status = Status.REGISTERED;

		return status;
	}

	public void setStatus(Status status) {
		this.status = status;
	}

	public boolean checkPassword(String perhapsThePassword) {
		return password.equals(hashPassword(perhapsThePassword));
	}

	public String getEmailValidationToken() {
		return emailValidationToken;
	}

	public void generateEmailValidationToken() {
		this.emailValidationToken = String.valueOf(Math
				.abs(randomNumberGenerator.nextLong()));
	}

	public boolean isAdministrator() {
		return administrator;
	}

	public void setAdministrator(boolean administrator) {
		this.administrator = administrator;
	}

	public Date getEmailValidationDate() {
		return emailValidationDate;
	}

	public void setEmailValidationDate(Date emailValidationDate) {
		this.emailValidationDate = emailValidationDate;
	}

	public Date getRegistrationDate() {
		return registrationDate;
	}

	public void setRegistrationDate(Date registrationDate) {
		this.registrationDate = registrationDate;
	}

	@Override
	public String toString() {
		return "User '" + getId() + "'";
	}

	@Override
	public boolean equals(Object obj) {
		User other = (User) obj;
		return this.getId().equals(other.getId());
	}

	@Override
	public int hashCode() {
		return this.getId().hashCode();
	}

	public String constructEmailValidationURI() {
		String returnURI;
		try {
			returnURI = WellKnown.urls.getEmailConfirmation() + '?'
					+ EmailVerificationController.PARAMETER_VALIDATION_TOKEN
					+ '='
					+ URLEncoder.encode(getEmailValidationToken(), "UTF-8")
					+ '&' + EmailVerificationController.PARAMETER_ID + '='
					+ getId();
		} catch (UnsupportedEncodingException e) {
			throw new IllegalStateException("Should never happen, but it did:",
					e);
		}

		return returnURI;
	}

	private static String hashPassword(String password) {
		try {
			MessageDigest messageDigest = MessageDigest.getInstance("MD5");
			messageDigest.update(password.getBytes("UTF-8"));
			byte raw[] = messageDigest.digest();
			String hashedPassword = (new BASE64Encoder()).encode(raw);
			return "MD5:" + hashedPassword;
		} catch (Exception e) {
			throw new IllegalStateException("Failed to hash password:", e);
		}
	}

	public String resetPassword() {
		String newPassword = RandomTokenGenerator.getRandomToken(8);
		password = hashPassword(newPassword);

		setRequirePasswordReset(true);

		return newPassword;
	}

	public boolean isRequirePasswordReset() {
		return requirePasswordReset;
	}

	public void setRequirePasswordReset(boolean requirePasswordReset) {
		this.requirePasswordReset = requirePasswordReset;
	}

	public Date getLastLoggedInDate() {
		return lastLoggedInDate;
	}

	public void setLastLoggedInDate(Date lastLoggedInDate) {
		this.lastLoggedInDate = lastLoggedInDate;
	}

	public boolean isSuspended() {
		return suspended;
	}

	public void setSuspended(boolean suspended) {
		this.suspended = suspended;
	}

	public boolean isAllowedToLogIn() {
		return !suspended && status.isAllowedToLogOn();
	}

	public String getOriginalDomain() {
		return originalDomain;
	}

	public void setOriginalDomain(String originalDomain) {
		this.originalDomain = originalDomain;
	}

	public void setOriginalVisitorId(Long originalVisitorId) {
		this.originalVisitorId = originalVisitorId;
	}

	public Project getSharedStorage() {
		return sharedStorage;
	}

	public void setSharedStorage(Project sharedStorage) {
		this.sharedStorage = sharedStorage;
	}

	public Long getOriginalVisitorId() {
		return originalVisitorId;
	}

	public boolean isEnableAds() {
		return enableAds;
	}

	public void setEnableAds(boolean disableAds) {
		this.enableAds = disableAds;
	}

	public String getPayPalAccountId() {
		return payPalAccountId;
	}

	public void setPayPalAccountId(String payPalAccountId) {
		this.payPalAccountId = payPalAccountId != null
				&& payPalAccountId.trim().length() == 0 ? null
				: payPalAccountId;
	}

	public Set<Role> getRoles() {
		return roles;
	}

	public void setRoles(Set<Role> roles) {
		this.roles = roles;
	}
	
	public UserPreferences getPreferences() {
		return preferences;
	}
}
