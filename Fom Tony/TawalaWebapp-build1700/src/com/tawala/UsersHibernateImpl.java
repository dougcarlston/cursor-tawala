package com.tawala;

import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Date;
import java.util.HashSet;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

import org.apache.lucene.queryParser.ParseException;
import org.hibernate.Criteria;
import org.hibernate.FetchMode;
import org.hibernate.HibernateException;
import org.hibernate.Session;
import org.hibernate.criterion.Restrictions;
import org.springframework.mail.MailException;
import org.springframework.orm.hibernate3.HibernateCallback;
import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.Log;
import com.tawala.domain.DomainMetadata;
import com.tawala.domain.ProjectGroup;
import com.tawala.domain.Role;
import com.tawala.domain.Status;
import com.tawala.domain.User;
import com.tawala.domain.Visitor;
import com.tawala.domain.notification.PasswordResetNotification;
import com.tawala.domain.notification.UserApprovalNotification;
import com.tawala.email.Emailer;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.Project;
import com.tawala.project.ProjectsHibernateImpl;
import com.tawala.project.UserProject;
import com.tawala.project.theme.UserDefinedTheme;
import com.tawala.project.theme.UserUploadedFile;
import com.tawala.search.Indexers;
import com.tawala.util.RandomTokenGenerator;
import com.tawala.web.user.UserAccessTicket;

public class UsersHibernateImpl implements Users {

	private static final String QUERY_FIND_USER_BY_USER_NAME = "from "
			+ User.class.getName() + " where normalizedUserName = :userName";
	static {
		HibernateTemplate template = new HibernateTemplate();
		template.setQueryCacheRegion("query.findUserByUserName");
		template.setCacheQueries(true);

		TawalaSessionFactory.MAIN.addCustomQueryTemplate(
				QUERY_FIND_USER_BY_USER_NAME, template);
	}

	public void addOrSave(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				TawalaSessionFactory.MAIN.getHibernateTemplate().saveOrUpdate(
						user);

				return null;
			}
		});

		try {
			Indexers.getUserIndexer().index(user);
		} catch (IOException e) {
			Log.warn(this, "Unable to reindex user:", e);
		}
	}

	public void delete(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				TawalaSessionFactory.MAIN.getHibernateTemplate().delete(user);
				TawalaSessionFactory.MAIN
						.evictCachedDataForQuery(QUERY_FIND_USER_BY_USER_NAME);
				TawalaSessionFactory.MAIN
						.evictCachedDataForQuery(ProjectsHibernateImpl.CACHED_QUERY_TEMPLATE_ID);

				return null;
			}
		});

		try {
			Indexers.getUserIndexer().delete(user.getDatabaseId());
		} catch (IOException e) {
			Log.warn(this, "Unable to delete user from the index:", e);
		}
	}

	public void onUserRegistration(final User user)
			throws UnsupportedEncodingException, MailException {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				user.setStatus(Status.REGISTERED);
				/*
				 * TODO: until email woes are resolved.
				 * user.setStatus(Status.EMAIL_UNVALIDATED);
				 * user.generateEmailValidationToken();
				 * user.setRegistrationDate(new Date());
				 * 
				 * EmailVerificationMessage message = new
				 * EmailVerificationMessage( user.getEmail().toString(), user
				 * .constructEmailValidationURI(), user .getFirstName());
				 * 
				 * Emailer.getSender().send(message);
				 */

				addOrSave(user);

				return null;
			}
		});

	}

	public void onUserEmailValidation(User user) {
		if (user.getStatus().equals(Status.EMAIL_UNVALIDATED)) {
			user.setStatus(Status.EMAIL_VALIDATED);
			user.setEmailValidationDate(new Date());
			addOrSave(user);
		}

	}

	public void onUserApproval(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				if (user.getStatus().getCanBeApproved()) {
					user.setStatus(Status.REGISTERED);
					TawalaSessionFactory.MAIN.getHibernateTemplate().update(
							user);

					UserApprovalNotification message = new UserApprovalNotification(
							user.getEmail().toString(), user.getFirstName(),
							user.getId());

					Emailer.getSender().send(message);
				}

				return null;
			}
		});
	}

	/*
	 * When the time comes public void onUserBeingPutOnHold(final User user) {
	 * TransactionTemplate transactionTemplate = new TransactionTemplate(
	 * TawalaSessionFactory.getTransactionManager());
	 * transactionTemplate.execute(new TransactionCallback() { public Object
	 * doInTransaction(TransactionStatus status) {
	 * user.setStatus(Status.ON_HOLD);
	 * TawalaSessionFactory.getHibernateTemplate().update(user);
	 * 
	 * UserPutOnHoldNotification message = new UserPutOnHoldNotification(
	 * user.getEmail().toString(), user.getFirstName());
	 * 
	 * Emailer.getSender().send(message);
	 * 
	 * return null; } }); }
	 */

	@SuppressWarnings("unchecked")
	public User get(final String userName) {
		if (userName == null) {
			throw new IllegalArgumentException("User name is null");
		}

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (User) transactionTemplate.execute(new TransactionCallback() {

			public Object doInTransaction(TransactionStatus arg0) {
				List<User> results = TawalaSessionFactory.MAIN
						.getCustomTemplate(QUERY_FIND_USER_BY_USER_NAME)
						.findByNamedParam(QUERY_FIND_USER_BY_USER_NAME,
								"userName", User.normalizeUserName(userName));

				if (results.size() == 0)
					return null;

				if (results.size() > 1)
					throw new IllegalStateException(
							"More than one user with userId=" + userName);

				User user = (User) results.get(0);

				// --- Fetch roles.
				TawalaSessionFactory.MAIN.getHibernateTemplate().initialize(
						user.getRoles());

				return user;
			}
		});

	}

	public User get(long id) {
		return (User) TawalaSessionFactory.MAIN.getHibernateTemplate().get(
				User.class, id);
	}

	public int size() {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Integer) transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				return ((Long) TawalaSessionFactory.MAIN
						.getHibernateTemplate()
						.iterate("select count(*) from " + User.class.getName())
						.next()).intValue();
			}
		});
	}

	@SuppressWarnings("unchecked")
	public List<User> findUsersWithStatus(Status status) {
		return TawalaSessionFactory.MAIN.getHibernateTemplate()
				.findByNamedParam(
						"from " + User.class.getName()
								+ " where status = :status", "status", status);
	}

	public List<User> search(String query) throws ParseException, IOException {
		List<Long> ids = Indexers.getUserIndexer().searchForUser(query);
		List<User> result = new ArrayList<User>(ids.size());

		for (Long id : ids) {
			User user = get(id);
			if (user == null) {
				Log.warn(UsersHibernateImpl.class, "Can't find user with id="
						+ id + ". Possible timing issue with indexing.");
				continue;
			}
			result.add(user);
		}
		return result;
	}

	// --- TODO: assess whether it's better to reindex all at once inside the
	// indexer and then flush the index.
	public void reindexUsers() {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			@SuppressWarnings("unchecked")
			public Object doInTransaction(TransactionStatus status) {
				Iterator<User> userIterator = TawalaSessionFactory.MAIN
						.getHibernateTemplate().iterate(
								"from " + User.class.getName());
				while (userIterator.hasNext()) {
					try {
						Indexers.getUserIndexer().index(userIterator.next());
					} catch (IOException e) {
						throw new IllegalStateException(
								"Unable to reindex all users:", e);
					}
				}

				return null;
			}
		});
	}

	public void onUserBeingReleasedFromSuspension(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				user.setSuspended(false);
				TawalaSessionFactory.MAIN.getHibernateTemplate().update(user);
				return null;
			}
		});
	}

	public void onUserSuspension(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				user.setSuspended(true);
				TawalaSessionFactory.MAIN.getHibernateTemplate().update(user);
				return null;
			}
		});
	}

	public void resetPassword(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				String newPassword = user.resetPassword();
				TawalaSessionFactory.MAIN.getHibernateTemplate().update(user);

				PasswordResetNotification message = new PasswordResetNotification(
						user.getEmail().toString(), user.getFirstName(),
						newPassword);

				Emailer.getSender().send(message);

				return null;
			}
		});
	}

	public void onLogin(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				User databaseUser = (User) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(User.class,
								user.getDatabaseId());

				databaseUser.setLastLoggedInDate(new Date());

				return null;
			}
		});
	}

	@SuppressWarnings("unchecked")
	public List<User> findUsersRegisteredSince(Date date) {
		return TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.findByNamedParam(
						"from "
								+ User.class.getName()
								+ " where registrationDate > :date order by registrationDate desc",
						"date", date);
	}

	public User onUserUpgradeToFullyRegistered(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (User) transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				User databaseUser = (User) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(User.class,
								user.getDatabaseId());

				databaseUser.setStatus(Status.REGISTERED);

				return databaseUser;
			}
		});
	}

	public User onUserDowngradeToInitiallyRegistered(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (User) transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				User databaseUser = (User) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(User.class,
								user.getDatabaseId());

				databaseUser.setStatus(Status.REGISTERED_INITIAL);

				return databaseUser;
			}
		});
	}

	public void createVisitor(Visitor visitor) {
		TawalaSessionFactory.MAIN.getHibernateTemplate().save(visitor);
	}

	public Project getSharedStorageForUser(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Project) transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				User databaseUser = (User) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(User.class,
								user.getDatabaseId());
				Project sharedStorage = databaseUser.getSharedStorage();
				if (sharedStorage != null) {
					TawalaSessionFactory.MAIN.getHibernateTemplate()
							.initialize(sharedStorage);
				}
				return sharedStorage;
			}
		});
	}

	public static void updateUserRoles(final long userDatabaseId,
			final Set<Role> newRoles) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				HibernateTemplate hibernateTemplate = TawalaSessionFactory.MAIN
						.getHibernateTemplate();
				User user = (User) hibernateTemplate.load(User.class,
						userDatabaseId);
				if (newRoles == null || newRoles.size() == 0) {
					if (user.getRoles().size() == 0) {
						return null;
					} else {
						user.getRoles().clear();
					}
				} else {
					if (!user.getRoles().equals(newRoles)) {
						Set<Role> refreshedNewRoles = new HashSet<Role>();
						for (Role role : newRoles) {
							refreshedNewRoles.add((Role) hibernateTemplate
									.load(Role.class, role.getRoleId()));
						}
						user.setRoles(refreshedNewRoles);
					}
				}

				return null;
			}
		});
	}

	public static UserAccessTicket generateUserAccessTicket(User user) {
		UserAccessTicket result = new UserAccessTicket(
				generateUniqueTicketId(), user);
		TawalaSessionFactory.MAIN.getHibernateTemplate().save(result);
		return result;
	}

	private static String generateUniqueTicketId() {
		String result = null;
		do {
			result = RandomTokenGenerator
					.getRandomToken(DomainMetadata.instance
							.getUserAccessTicketLength());
		} while (!isUniqueUserAccessToken(result));

		return result;
	}

	private static boolean isUniqueUserAccessToken(String token) {
		return retrieveAccessTicket(token) == null;
	}

	public static UserAccessTicket retrieveAccessTicket(final String token) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserAccessTicket) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(TransactionStatus arg0) {
						UserAccessTicket result = (UserAccessTicket) TawalaSessionFactory.MAIN
								.getHibernateTemplate().get(
										UserAccessTicket.class, token);
						if (result != null) {
							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.initialize(result.getUser().getRoles());
						}

						return result;
					}
				});
	}

	public static void deleteAccessTicket(final UserAccessTicket ticket) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				TawalaSessionFactory.MAIN.getHibernateTemplate().delete(ticket);
				return null;
			}
		});
	}

	public static void updateLastTimeAccessTicketUsed(
			final UserAccessTicket ticket) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				ticket.setLastUsed(new Date());
				TawalaSessionFactory.MAIN.getHibernateTemplate().update(ticket);
				return null;
			}
		});
	}

	public static UserDefinedTheme getUserThemeById(final User user,
			final long themeId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserDefinedTheme) transactionTemplate
				.execute(new TransactionCallback() {
					@SuppressWarnings("unchecked")
					public Object doInTransaction(TransactionStatus status) {
						List<UserDefinedTheme> results = TawalaSessionFactory.MAIN
								.getHibernateTemplate().find(
										"from "
												+ UserDefinedTheme.class
														.getName()
												+ " where user = ? and id = ?",
										new Object[] { user, themeId });
						if (results.size() == 1) {
							return results.get(0);
						} else {
							return null;
						}
					}
				});
	}

	public static void saveTheme(final UserDefinedTheme theme) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				TawalaSessionFactory.MAIN.getHibernateTemplate().saveOrUpdate(
						theme);
				return null;
			}
		});
	}

	public static void deleteTheme(final User user, final long themeId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				UserDefinedTheme theme = getUserThemeById(user, themeId);
				if (theme != null) {
					TawalaSessionFactory.MAIN.getHibernateTemplate().delete(
							theme);
				}
				return null;
			}
		});
	}

	@SuppressWarnings("unchecked")
	public static List<UserDefinedTheme> getAllUserThemes(final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (List<UserDefinedTheme>) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						return TawalaSessionFactory.MAIN.getHibernateTemplate()
								.find(
										"from "
												+ UserDefinedTheme.class
														.getName()
												+ " where user = ?",
										new Object[] { user });
					}
				});
	}

	public static UserDefinedTheme getUserThemeById(final long id) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserDefinedTheme) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						return TawalaSessionFactory.MAIN.getHibernateTemplate()
								.get(UserDefinedTheme.class, id);
					}
				});
	}

	public static void saveUserUploadedFile(final UserUploadedFile file) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				if (file.getId() == null) {
					file.setId(generateUniqueRandomFileId());
				}

				TawalaSessionFactory.MAIN.getHibernateTemplate().saveOrUpdate(
						file);
				return null;
			}
		});
	}

	public static String generateUniqueRandomFileId() {
		String result = null;
		do {
			result = RandomTokenGenerator
					.getRandomToken(DomainMetadata.instance
							.getUserFileIdMaxLength());
		} while (!isUniqueFileId(result));

		return result;
	}

	private static boolean isUniqueFileId(String uniqueRandomId) {
		return TawalaSessionFactory.MAIN.getHibernateTemplate().get(
				UserUploadedFile.class, uniqueRandomId) == null;
	}

	public static UserUploadedFile getUserUploadedFileById(final String id) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserUploadedFile) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						return TawalaSessionFactory.MAIN.getHibernateTemplate()
								.get(UserUploadedFile.class, id);
					}
				});
	}

	@SuppressWarnings("unchecked")
	public static List<User> getAllAdminUsers() {
		return (List<User>) TawalaSessionFactory.MAIN.getHibernateTemplate()
				.execute(new HibernateCallback() {
					public Object doInHibernate(Session session)
							throws HibernateException, SQLException {
						Criteria criteria = session.createCriteria(User.class)
								.add(Restrictions.eq("administrator", true))
								.setFetchMode("roles", FetchMode.JOIN);
						return criteria.list();
					}
				});
	}

	public static Role getRoleById(String roleId) {
		return (Role) TawalaSessionFactory.MAIN.getHibernateTemplate().get(
				Role.class, roleId);
	}

	@SuppressWarnings("unchecked")
	public static List<ProjectGroup> getAllUserSportsDashboardGroups(
			final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (List<ProjectGroup>) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						return TawalaSessionFactory.MAIN
								.getHibernateTemplate()
								.find(
										"from " + ProjectGroup.class.getName()
												+ " where groupOwner = ?", user);
					}
				});
	}

	public static ProjectGroup getProjectGroup(final User user,
			final long groupId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (ProjectGroup) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						ProjectGroup group = (ProjectGroup) TawalaSessionFactory.MAIN
								.getHibernateTemplate().get(ProjectGroup.class,
										groupId);
						if(group == null) {
							throw new IllegalStateException("Group with id " + groupId + " doesn't exist.");
						}
						if (!group.getGroupOwner().equals(user)) {
							throw new IllegalStateException(
									"Attempt by user "
											+ user.getId()
											+ " to retrieve a group which belongs to user "
											+ group.getGroupOwner().getId()
											+ ".");
						}
						
						//--- TODO: change to initialize project through the query parameters.
						for (UserProject userProject : group.getUserProjects()) {
							userProject.getProject().getForm("Some Form");
						}

						return group;
					}
				});
	}

	public static void saveProjectGroup(ProjectGroup projectGroup) {
		TawalaSessionFactory.MAIN.getHibernateTemplate().saveOrUpdate(
				projectGroup);
	}

	public static void deleteProjectGroup(final User user,
			final long projectGroupId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				ProjectGroup projectGroup = (ProjectGroup) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(ProjectGroup.class,
								projectGroupId);
				if (projectGroup == null) {
					return null;
				}
				if (!projectGroup.getGroupOwner().equals(user)) {
					throw new IllegalStateException("Attempt by user "
							+ user.getId()
							+ " to delete a group which belongs to user "
							+ projectGroup.getGroupOwner().getId() + ".");
				}
				TawalaSessionFactory.MAIN.getHibernateTemplate().delete(
						projectGroup);
				return null;
			}
		});
	}

	public static void addUserProjectToGroup(final User user,
			final long projectGroupId, final long projectId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				ProjectGroup projectGroup = (ProjectGroup) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(ProjectGroup.class,
								projectGroupId);
				if (projectGroup == null) {
					return null;
				}
				if (!projectGroup.getGroupOwner().equals(user)) {
					throw new IllegalStateException(
							"Attempt by user "
									+ user.getId()
									+ " to add user project to a group which belongs to user "
									+ projectGroup.getGroupOwner().getId()
									+ ".");
				}
				UserProject userProject = (UserProject) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(UserProject.class,
								projectId);
				if (userProject == null) {
					return null;
				}
				projectGroup.addUserProject(userProject);
				return null;
			}
		});
	}

	public static void deleteProjectFromGroup(final User user, final long projectGroupId,
			final long projectId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				ProjectGroup projectGroup = (ProjectGroup) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(ProjectGroup.class,
								projectGroupId);
				if (projectGroup == null) {
					return null;
				}
				if (!projectGroup.getGroupOwner().equals(user)) {
					throw new IllegalStateException(
							"Attempt by user "
									+ user.getId()
									+ " to delete user project from a group which belongs to user "
									+ projectGroup.getGroupOwner().getId()
									+ ".");
				}
				
				Set<UserProject> userProjects = projectGroup.getUserProjects();
				for (UserProject userProject : userProjects) {
					if(userProject.getId() == projectId) {
						userProjects.remove(userProject);
						break;
					}
				}
				return null;
			}
		});
		
	}

	public static UserProject getProjectInGroup(final User user,
			final long groupId, final long userProjectId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserProject) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						ProjectGroup group = getProjectGroup(user, groupId);
						if(group == null) {
							return null;
						}
						
						for (UserProject project : group.getUserProjects()) {
							if(project.getId() == userProjectId) {
								TawalaSessionFactory.MAIN.getHibernateTemplate().initialize(project.getProject());
								return project;
							}
						}
						
						return null;
					}
				});
	}
}
