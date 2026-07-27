package com.tawala;

import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Date;
import java.util.HashMap;
import java.util.List;

import org.springframework.mail.MailException;

import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.Visitor;
import com.tawala.domain.notification.EmailVerificationMessage;
import com.tawala.domain.notification.UserApprovalNotification;
import com.tawala.email.Emailer;
import com.tawala.persistence.InMemoryPersistenceStrategy;
import com.tawala.persistence.PersistenceStrategy;
import com.tawala.persistence.PersistentBunch;
import com.tawala.project.Project;

public class UsersPersistentBunchImpl extends PersistentBunch implements Users {
    private final HashMap<String, User> usersByName = new HashMap<String, User>();

    public UsersPersistentBunchImpl() {
        this(new InMemoryPersistenceStrategy());
    }

    public UsersPersistentBunchImpl(PersistenceStrategy persistenceStrategy) {
        super(persistenceStrategy);
        List<User> users = persistenceStrategy.loadAll(User.class);
        for (User user : users) {
            justAdd((User) user);

        }
    }

    public void addOrSave(User user) {
        justAdd(user);
        getPersistenceStrategy().save(user);
    }

    public void delete(User user) {
        justDelete(user);
        getPersistenceStrategy().delete(user);
    }

    public void onUserRegistration(User user)
            throws UnsupportedEncodingException, MailException {
        user.generateEmailValidationToken();
        user.setRegistrationDate(new Date());

        EmailVerificationMessage message = new EmailVerificationMessage(user
                .getEmail().toString(), user.constructEmailValidationURI(),
                user.getFirstName());

        Emailer.getSender().send(message);

        addOrSave(user);
    }

    public void onUserEmailValidation(User user) {
        if (user.getStatus().equals(Status.EMAIL_UNVALIDATED)) {
            user.setStatus(Status.EMAIL_VALIDATED);
            user.setEmailValidationDate(new Date());
            addOrSave(user);
        }
    }

    public void onUserApproval(User user) {
        if (user.getStatus().equals(Status.EMAIL_VALIDATED)) {
            user.setStatus(Status.REGISTERED);
            addOrSave(user);

            UserApprovalNotification message = new UserApprovalNotification(
                    user.getEmail().toString(), user.getFirstName(), user
                            .getId());

            Emailer.getSender().send(message);
        }
    }

    private void justAdd(User user) {
        usersByName.put(user.getId(), user);
    }

    private void justDelete(User user) {
        usersByName.remove(user.getId());
    }

    public User get(String key) {
        User user = usersByName.get(key);
        if (user == null) {
            user = getPersistenceStrategy().load(User.class, key);
            if (user != null)
                usersByName.put(key, user);
        }
        return user;
    }

    public int size() {
        return usersByName.size();
    }

    public List<User> findUsersWithStatus(Status status) {
        List<User> result = new ArrayList<User>();
        List<User> allUsers = getPersistenceStrategy().loadAll(User.class);
        for (User user : allUsers) {
            if (user.getStatus().equals(Status.EMAIL_VALIDATED))
                result.add(user);
        }
        return result;
    }

    public Collection<User> getAllUsers() {
        return usersByName.values();
    }

    public List<User> search(String query) {
        throw new UnsupportedOperationException(this.getClass().getName()
                + " doesn't support search()");
    }

    public User get(long id) {
        throw new UnsupportedOperationException(this.getClass().getName()
                + " doesn't support get(long)");
    }

    public void reindexUsers() {
        throw new UnsupportedOperationException(this.getClass().getName()
                + " doesn't support reindexUsers()");
    }

    public void onUserBeingReleasedFromSuspension(User user) {
        throw new UnsupportedOperationException(this.getClass().getName()
                + " doesn't support onUserBeingReleasedFromSuspension(User user)");
    }

    public void onUserSuspension(User user) {
        throw new UnsupportedOperationException(this.getClass().getName()
                + " doesn't support onUserSuspension(User user)");
    }

    public void resetPassword(User user) {
        throw new IllegalStateException(this.getClass().getName() + " doesn't support resetPassword() method." );
    }

	public void onLogin(User user) {
        throw new IllegalStateException(this.getClass().getName() + " doesn't support onLogin method." );
	}

	public List<User> findUsersRegisteredSince(Date date) {
		throw new IllegalStateException(this.getClass().getName() + " doesn't support findUsersRegisteredSince method.");
	}

	public User onUserUpgradeToFullyRegistered(User user) {
		throw new IllegalStateException(this.getClass().getName() + " doesn't support onUserUpgradeToFullyRegistered method.");
	}

	public User onUserDowngradeToInitiallyRegistered(User user) {
		throw new IllegalStateException(this.getClass().getName() + " doesn't support onUserDowngradeToInitiallyRegistered method.");
	}

	public void createVisitor(Visitor visitor) {
		throw new IllegalStateException(this.getClass().getName() + " doesn't support createVisitor method.");
	}

	public Project getSharedStorageForUser(User user) {
		//--- TODO: verify it works.
		return user.getSharedStorage();
	}
}
