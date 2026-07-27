package com.tawala.web.user;

import com.tawala.domain.User;

public class LoginForm {
    private String userName;
    private String password;
    private User user;
    private boolean keepSignedIn;
    private String redirectTo;
    
    public String getRedirectTo() {
		return redirectTo;
	}
	public void setRedirectTo(String redirectTo) {
		this.redirectTo = redirectTo;
	}
	public boolean isKeepSignedIn() {
		return keepSignedIn;
	}
	public void setKeepSignedIn(boolean keepLoggedOn) {
		this.keepSignedIn = keepLoggedOn;
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
     * @return Returns the userName.
     */
    public String getUserName() {
        return userName;
    }
    /**
     * @param userName The userName to set.
     */
    public void setUserName(String userName) {
        this.userName = userName;
    }
    /**
     * @return Returns the user.
     */
    public User getUser() {
        return user;
    }
    /**
     * @param user The user to set.
     */
    public void setUser(User user) {
        this.user = user;
    }
}
