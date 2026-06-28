package com.tawala.web.user;

import java.io.Serializable;

import com.tawala.domain.Status;
import com.tawala.domain.User;

public class UserForm implements Serializable {
    private static final long serialVersionUID = 1L;
    private User user;
    private String emailAddress;
    private String password;
	private String repeatedPassword;
	private boolean seeAdvancedFeatures;

	public UserForm(User user) {
		this.user = user;
        emailAddress = user.getEmail() == null ? null : user.getEmail().toString();
        seeAdvancedFeatures = user.getStatus().equals(Status.REGISTERED);
	}

	public UserForm() {
		this.user = new User(Status.EMAIL_UNVALIDATED);
	}

	public User getUser() {
		return user;
	}

	public void setRepeatedPassword(String repeatedPassword) {
		this.repeatedPassword = repeatedPassword;
	}

	public String getRepeatedPassword() {
		return repeatedPassword;
	}

    /**
     * @return Returns the emailAddress.
     */
    public String getEmailAddress() {
        return emailAddress;
    }

    /**
     * @param emailAddress The emailAddress to set.
     */
    public void setEmailAddress(String emailAddress) {
        this.emailAddress = emailAddress;
    }

    /**
     * @return Returns the password.
     */
    public String getPassword() {
        return password;
    }

    /**
     * @param password The password to set.
     */
    public void setPassword(String password) {
        this.password = password;
    }

	public boolean isSeeAdvancedFeatures() {
		return seeAdvancedFeatures;
	}

	public void setSeeAdvancedFeatures(boolean seeAdvancedFeatures) {
		this.seeAdvancedFeatures = seeAdvancedFeatures;
	}

	public void setUser(User user) {
		this.user = user;
	}
}
