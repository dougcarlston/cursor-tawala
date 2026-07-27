package com.tawala;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.util.Date;
import java.util.List;

import org.apache.lucene.queryParser.ParseException;
import org.springframework.mail.MailException;

import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.Visitor;
import com.tawala.project.Project;

public interface Users {
    public void addOrSave(User user);

    public void delete(User user);

    public void onUserRegistration(User user)
            throws UnsupportedEncodingException, MailException;
    
    public void onUserEmailValidation(User user);

    public void onUserApproval(User user);
    
    public User get(String userName);
    
    public User get(long id);

    public int size();
    
    public List<User> findUsersWithStatus(Status status);
    
    public List<User> search(String query) throws ParseException, IOException ;
    
    public void reindexUsers();

    public void onUserBeingReleasedFromSuspension(User user);

    public void onUserSuspension(User user);

    public void resetPassword(User user);

    public void onLogin(User user);

	public List<User> findUsersRegisteredSince(Date date);

	public User onUserUpgradeToFullyRegistered(User user);

	public User onUserDowngradeToInitiallyRegistered(User user);
	
	public void createVisitor(Visitor visitor);
	
	public Project getSharedStorageForUser(User user);
}
