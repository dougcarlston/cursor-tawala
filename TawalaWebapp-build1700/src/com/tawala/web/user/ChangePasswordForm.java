package com.tawala.web.user;

import java.io.Serializable;

public class ChangePasswordForm implements Serializable {
    private static final long serialVersionUID = 1L;
    private String oldPassword;
    private String password;
	private String repeatedPassword;

	public ChangePasswordForm() {
	}

	public void setRepeatedPassword(String repeatedPassword) {
		this.repeatedPassword = repeatedPassword;
	}

	public String getRepeatedPassword() {
		return repeatedPassword;
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

    /**
     * @return Returns the oldPassword.
     */
    public String getOldPassword() {
        return oldPassword;
    }

    /**
     * @param oldPassword The oldPassword to set.
     */
    public void setOldPassword(String oldPassword) {
        this.oldPassword = oldPassword;
    }
}
