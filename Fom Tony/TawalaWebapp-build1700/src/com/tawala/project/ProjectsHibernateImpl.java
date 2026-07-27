package com.tawala.project;

import java.math.BigDecimal;
import java.math.BigInteger;
import java.sql.SQLException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.Iterator;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;

import org.hibernate.Criteria;
import org.hibernate.FetchMode;
import org.hibernate.HibernateException;
import org.hibernate.Query;
import org.hibernate.Session;
import org.hibernate.criterion.DetachedCriteria;
import org.hibernate.criterion.Restrictions;
import org.springframework.orm.hibernate3.HibernateCallback;
import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.Log;
import com.tawala.World;
import com.tawala.domain.DomainMetadata;
import com.tawala.domain.User;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.jbpm.sportsdashboards.SportsDashboardStatistics;
import com.tawala.payment.ProjectInvoice;
import com.tawala.payment.ProjectInvoiceEvent;
import com.tawala.payment.paypal.InstantPaymentNotification;
import com.tawala.project.backup.BackupSchedule;
import com.tawala.project.backup.DailyBackup;
import com.tawala.project.caching.ProjectRuntimeCache;
import com.tawala.project.commands.Reference;
import com.tawala.project.data.DataSource;
import com.tawala.project.library.LibraryProjectVersion;
import com.tawala.project.library.ProjectLibrary;
import com.tawala.util.RandomTokenGenerator;
import com.tawala.web.WorldInitializer;

public class ProjectsHibernateImpl implements Projects {
	public final static String CACHED_QUERY_TEMPLATE_ID = "fetch project";

	public static Date NULL_VALUE_PRESENTATION_IN_GET_STAT_QUERY;
	static {
		Calendar calendar = Calendar.getInstance();
		calendar.clear();
		calendar.set(Calendar.YEAR, 1900);
		calendar.set(Calendar.MONTH, 1);
		calendar.set(Calendar.DAY_OF_MONTH, 1);

		NULL_VALUE_PRESENTATION_IN_GET_STAT_QUERY = calendar.getTime();
	}
	private final static String QUERY_GET_PROJECT_STATISTICS = "select p.user_project_id, p.name, p.created_dt, p.last_updated_dt, p.offline, "
			+ "	(select count(*) from submission as s where p.project_id = s.project_id) as responseCount, "
			+ "	coalesce((select max(created_dt) from submission as s where p.project_id = s.project_id), DATE '"
			+ new SimpleDateFormat("yyyy-MM-dd")
					.format(NULL_VALUE_PRESENTATION_IN_GET_STAT_QUERY)
			+ "') as lastAccessed "
			+ "		from user_project as p"
			+ " 		where p.user_id = ?";

	private final static String QUERY_PROJECT_LAST_UPDATED_DATE = "select max(created_dt) from submission where project_id = ?";

	static {
		HibernateTemplate template = new HibernateTemplate();
		template.setCacheQueries(true);
		template.setQueryCacheRegion("query.fetchProject");

		TawalaSessionFactory.MAIN.addCustomQueryTemplate(
				CACHED_QUERY_TEMPLATE_ID, template);
	}

	public static Date getLastDataRecordedDate(final UserProject project) {
		return (Date) TawalaSessionFactory.MAIN.getHibernateTemplate().execute(
				new HibernateCallback() {
					@SuppressWarnings("unchecked")
					public Object doInHibernate(Session session)
							throws HibernateException, SQLException {
						Query query = session
								.createSQLQuery(QUERY_PROJECT_LAST_UPDATED_DATE);
						query.setLong(0, project.getProject().getId());
						List<Object> result = query.list();
						return result.get(0);
					}
				});
	}

	@SuppressWarnings("unchecked")
	public static List<LinkToUserProject> getAllProjectLinks(
			UserProject userProject) {
		return TawalaSessionFactory.MAIN.getHibernateTemplate()
				.findByNamedParam(
						"from " + LinkToUserProject.class.getName()
								+ " where project = :project", "project",
						userProject);
	}

	public static UserProject getUserProjectById(final User user,
			final long projectId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserProject) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(TransactionStatus status) {
						UserProject result = (UserProject) TawalaSessionFactory.MAIN
								.getHibernateTemplate().get(UserProject.class,
										projectId);
						if (result != null && user != null
								&& !user.equals(result.getUser())) {
							throw new IllegalStateException(
									"Attempt to retrieve user project #"
											+ projectId
											+ " which doesn't belong to user #"
											+ user.getDatabaseId());
						}

						return result;
					}
				});
	}

	public static UserProject upgradeWithLibraryProjectVersion(
			final UserProject userProject,
			final LibraryProjectVersion libraryVersion,
			final String versionDescription) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserProject) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						UserProject originalProject = (UserProject) TawalaSessionFactory.MAIN
								.getHibernateTemplate().load(UserProject.class,
										userProject.getId());
						Project copyOfLibraryProject = libraryVersion
								.getProject().makeCopy();
						TawalaSessionFactory.MAIN.getHibernateTemplate().save(
								copyOfLibraryProject);
						ProjectVersion newVersion = new ProjectVersion(
								originalProject, copyOfLibraryProject);
						newVersion.setDescription(versionDescription);
						originalProject.addVersion(newVersion);
						TawalaSessionFactory.MAIN.getHibernateTemplate().save(
								newVersion);

						originalProject.deployVersion(newVersion, true /*
																		 * preserve
																		 * the
																		 * theme
																		 * .
																		 */);
						// This will indicate the project version is no
						// longer applicable.
						originalProject.setLibraryVersionNumber(null);
						originalProject
								.setOriginalLibraryProjectVersionId(libraryVersion
										.getId());

						originalProject.setLastUpdatedDate(new Date());

						originalProject.getProject().generateRandomFormTokens();

						updateUserGlobalStorageWithNewDataSources(
								originalProject.getUser(), originalProject
										.getProject());

						return originalProject;
					}
				});
	}

	public UserProject put(final UserProject project) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserProject) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						UserProject originalProject = get(project.getUser()
								.getId(), project.getName());
						if (originalProject == null) {
							boolean newLinkRequired = project
									.getUniqueRandomId() == null;
							LinkToUserProject linkToUserProject = null;
							if (newLinkRequired) {
								String uniqueId = generateUniqueRandomProjectId();
								project.setUniqueRandomId(uniqueId);

								project.getProject().generateRandomFormTokens();

								linkToUserProject = LinkToUserProject
										.createUnauthenticatedLink(project,
												uniqueId);
							}
							project.setLastUpdatedDate(new Date());

							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.save(project);
							if (newLinkRequired) {
								TawalaSessionFactory.MAIN
										.getHibernateTemplate().save(
												linkToUserProject);
							}
							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.flush();
							project.deployVersion(project.getVersions().get(0),
									false /*
										 * don't preserve the current theme.
										 */);

							updateUserGlobalStorageWithNewDataSources(project
									.getUser(), project.getProject());

							// --- This gets the project initialized and the
							// field ready to be used when detached.
							// --- Would be great to find a cleaner way of doing
							// it.
							project.getProject().getId();

							return project;
						} else {
							ProjectVersion projectVersion = project
									.getVersions().get(0);

							originalProject.addVersion(projectVersion);
							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.save(projectVersion);

							originalProject
									.deployVersion(projectVersion, false);
							// This will indicate the project version is no
							// longer applicable.
							originalProject.setLibraryVersionNumber(null);

							originalProject.setLastUpdatedDate(new Date());

							originalProject.getProject()
									.generateRandomFormTokens();

							updateUserGlobalStorageWithNewDataSources(project
									.getUser(), project.getProject());

							return originalProject;
						}
					}
				});
	}

	private static void updateUserGlobalStorageWithNewDataSources(User user,
			Project project) {
		List<DataSource> newDataSources = project
				.getDataSourcesDefinedInForms();
		if (newDataSources == null) {
			return;
		}

		addSharedDataSources(user, newDataSources);
	}

	public static void addSharedDataSources(final User user,
			final List<DataSource> newDataSources) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				User loadedUser = (User) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(User.class,
								user.getDatabaseId());
				if (loadedUser.getSharedStorage() == null) {
					loadedUser.setSharedStorage(new Project(
							Project.CURRENT_VERSION));
				}
				loadedUser.getSharedStorage().addDataSources(newDataSources);

				return null;
			}
		});

	}

	private static boolean isUnique(String uniqueRandomId) {
		return TawalaSessionFactory.MAIN.getHibernateTemplate().get(
				LinkToUserProject.class, uniqueRandomId) == null;
	}

	public void delete(final UserProject project, final World world) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.delete(project);

				TawalaSessionFactory.MAIN
						.evictCachedDataForQuery(CACHED_QUERY_TEMPLATE_ID);
				return null;
			}
		});

	}

	// TODO: revisit it.
	public long fileSize(Project project, World world) {
		return 0;
	}

	public UserProject get(String userId, String projectName) {
		return fetchProject(userId, projectName, false, false);
	}

	public UserProject getWithProjectRuntime(String userId, String projectName) {
		UserProject userProject = fetchProject(userId, projectName, true, false);

		if (userProject != null) {
			replaceProjectRuntimeWithCachedObject(userProject);
		}

		return userProject;
	}

	private void replaceProjectRuntimeWithCachedObject(UserProject userProject) {
		Project cachedProject = ProjectRuntimeCache.instance
				.getProject(userProject.getProject().getId());
		if (cachedProject == null) {
			ProjectRuntimeCache.instance.addProject(userProject.getProject());
			Log.info(this, "Cached project #"
					+ userProject.getProject().getId() + ".");
		} else {
			Log.info(this, "Retrieved project #"
					+ userProject.getProject().getId() + " from cache.");
			TawalaSessionFactory.MAIN.getHibernateTemplate().evict(
					userProject.getProject());
			userProject.setProject(cachedProject);
		}
	}

	public UserProject getWithVersions(String userId, String projectName) {
		return fetchProject(userId, projectName, true, true);
	}

	@SuppressWarnings("unchecked")
	private UserProject fetchProject(final String userId,
			final String projectName, final boolean isRuntimeNeeded,
			final boolean isVersionNeeded) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (UserProject) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {

						User user = WorldInitializer.getDefaultWorld().domain()
								.users().get(userId);
						if (user == null)
							return null;

						String projectAlias = "userProject";
						DetachedCriteria criteria = DetachedCriteria.forClass(
								UserProject.class, projectAlias);
						criteria.add(Restrictions.eq("user", user));
						criteria.add(Restrictions.eq("name", projectName));
						if (isRuntimeNeeded) {
							criteria.setFetchMode("project", FetchMode.JOIN);
						}

						List<UserProject> result = TawalaSessionFactory.MAIN
								.getCustomTemplate(CACHED_QUERY_TEMPLATE_ID)
								.findByCriteria(criteria);

						if (result.size() == 0)
							return null;

						UserProject userProject = result.get(0);
						if (isRuntimeNeeded) {
							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.initialize(userProject.getProject());
						}
						if (isVersionNeeded) {
							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.initialize(userProject.getVersions());
						}

						return userProject;
					}
				});

	}

	public int size() {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Integer) transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				return ((Long) TawalaSessionFactory.MAIN.getHibernateTemplate()
						.iterate(
								"select count(*) from "
										+ UserProject.class.getName()).next())
						.intValue();
			}
		});
	}

	@SuppressWarnings("unchecked")
	public List<UserProject> getAllForUserId(String userId) {
		User user = WorldInitializer.getDefaultWorld().domain().users().get(
				userId);
		if (user == null)
			return Collections.EMPTY_LIST;

		DetachedCriteria criteria = DetachedCriteria
				.forClass(UserProject.class);
		criteria.setFetchMode("project", FetchMode.JOIN);
		criteria.setFetchMode("user", FetchMode.JOIN);
		criteria.add(Restrictions.eq("user", user));

		List<UserProject> result = TawalaSessionFactory.MAIN
				.getHibernateTemplate().findByCriteria(criteria);

		Collections.sort(result, new Comparator<UserProject>() {

			public int compare(UserProject o1, UserProject o2) {
				return o1.getName().compareTo(o2.getName());
			}
		});

		return result;
	}

	public void deleteAllProjectsForUser(final World world, final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			@SuppressWarnings("unchecked")
			public Object doInTransaction(TransactionStatus status) {
				for (UserProject project : (List<UserProject>) TawalaSessionFactory.MAIN
						.getHibernateTemplate().findByNamedParam(
								"from " + UserProject.class.getName()
										+ " where user = :user", "user", user)) {
					delete(project, world);
				}

				return null;
			}
		});
	}

	@SuppressWarnings("unchecked")
	public ProjectVersion findProjectVersion(long versionId) {
		DetachedCriteria criteria = DetachedCriteria
				.forClass(ProjectVersion.class);
		criteria.setFetchMode("parent", FetchMode.JOIN);
		criteria.setFetchMode("parent.user", FetchMode.JOIN);
		criteria.setFetchMode("project", FetchMode.JOIN);
		criteria.add(Restrictions.idEq(versionId));

		List<ProjectVersion> results = TawalaSessionFactory.MAIN
				.getHibernateTemplate().findByCriteria(criteria);
		switch (results.size()) {
		case 0:
			return null;
		case 1:
			return results.get(0);
		default:
			throw new IllegalStateException(this.getClass().getName()
					+ ".findProjectVersion returned " + results.size()
					+ " rows.");
		}
	}

	public UserProject deployProjectVersion(final User user,
			final long projectVersionId) {
		return (UserProject) new TransactionTemplate(TawalaSessionFactory.MAIN
				.getTransactionManager()).execute(new TransactionCallback() {

			public Object doInTransaction(TransactionStatus status) {
				ProjectVersion projectVersion = (ProjectVersion) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(ProjectVersion.class,
								projectVersionId);

				if (!projectVersion.getParent().getUser().equals(user)) {
					throw new IllegalArgumentException("User " + user.getId()
							+ " attempted to deploy project version "
							+ projectVersionId + " owned by "
							+ projectVersion.getParent().getUser().getId());
				}

				projectVersion.getParent().deployVersion(projectVersion, false);

				projectVersion.getParent().setLastUpdatedDate(new Date());

				return projectVersion.getParent();
			}
		});
	}

	public UserProject deleteProjectVersion(final User user,
			final long projectVersionId) {
		return (UserProject) new TransactionTemplate(TawalaSessionFactory.MAIN
				.getTransactionManager()).execute(new TransactionCallback() {

			public Object doInTransaction(TransactionStatus status) {
				ProjectVersion projectVersion = (ProjectVersion) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(ProjectVersion.class,
								projectVersionId);

				if (!projectVersion.getParent().getUser().equals(user)) {
					throw new IllegalArgumentException("User " + user.getId()
							+ " attempted to delete project version "
							+ projectVersionId + " owned by "
							+ projectVersion.getParent().getUser().getId());
				}

				if (projectVersion.getParent().getDeployedVersion().equals(
						projectVersion)) {
					throw new IllegalArgumentException(
							"Attempt to delete the deployed project version.");
				}

				projectVersion.getParent().removeVersion(projectVersion);
				TawalaSessionFactory.MAIN.getHibernateTemplate().delete(
						projectVersion);

				Log.info(this, "Deleted version "
						+ projectVersion.getVersionNumber() + " of project '"
						+ projectVersion.getParent().getName() + "'");

				return projectVersion.getParent();
			}
		});
	}

	public static String generateUniqueRandomProjectId() {
		String result = null;
		do {
			result = RandomTokenGenerator
					.getRandomToken(DomainMetadata.instance
							.getProjectUniqueTokenLength());
		} while (!isUnique(result));

		return result;
	}

	public LinkToUserProject getWithProjectRuntime(final String randomProjectId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		LinkToUserProject result = (LinkToUserProject) transactionTemplate
				.execute(new TransactionCallback() {
					@SuppressWarnings("unchecked")
					public Object doInTransaction(TransactionStatus status) {

						String linkAlias = "link";
						DetachedCriteria criteria = DetachedCriteria.forClass(
								LinkToUserProject.class, linkAlias);
						criteria.add(Restrictions.idEq(randomProjectId));
						criteria.setFetchMode("link.project.project",
								FetchMode.JOIN);
						criteria.setFetchMode("link.project.user",
								FetchMode.JOIN);

						List<LinkToUserProject> result = TawalaSessionFactory.MAIN
								.getCustomTemplate(CACHED_QUERY_TEMPLATE_ID)
								.findByCriteria(criteria);

						if (result.size() == 0)
							return null;

						LinkToUserProject link = result.get(0);
						TawalaSessionFactory.MAIN.getHibernateTemplate()
								.initialize(link.getProject().getProject());

						TawalaSessionFactory.MAIN.getHibernateTemplate()
								.initialize(link.getProject().getUser());

						return link;
					}
				});

		if (result != null) {
			replaceProjectRuntimeWithCachedObject(result.getProject());
		}

		return result;
	}

	public void changeProjectNameAndOwnership(final long projectId,
			final String newName, final User newOwner) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				UserProject project = (UserProject) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(UserProject.class,
								projectId);
				project.setUser(newOwner);
				project.setName(newName);
				return null;
			}
		});
	}

	public long projectCountFor(User user) {
		return (Long) (TawalaSessionFactory.MAIN.getHibernateTemplate()
				.findByNamedParam(
						"select count(*) from " + UserProject.class.getName()
								+ " where user = :user", "user", user).get(0));
	}

	public long inactiveProjectCountFor(User user) {
		return (Long) (TawalaSessionFactory.MAIN.getHibernateTemplate()
				.findByNamedParam(
						"select count(*) from " + UserProject.class.getName()
								+ " where offline=true AND user = :user",
						"user", user).get(0));
	}

	public void addLinkToProject(final LinkToUserProject linkToProject) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			@SuppressWarnings("unchecked")
			public Object doInTransaction(TransactionStatus status) {
				if (linkToProject.isAuthenticated()) {
					List<LinkToUserProject> existingLinks = TawalaSessionFactory.MAIN
							.getCustomTemplate(CACHED_QUERY_TEMPLATE_ID)
							.findByNamedParam(
									"from "
											+ LinkToUserProject.class.getName()
											+ " where project = :project and authenticationToken = :token",
									new String[] { "project", "token" },
									new Object[] {
											linkToProject.getProject(),
											linkToProject
													.getAuthenticationToken() });
					if (existingLinks.size() > 0) {
						linkToProject.setId(existingLinks.get(0).getId());
						return null;
					}
				}
				if (linkToProject.getId() == null) {
					linkToProject.setId(generateUniqueRandomProjectId());
				}
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						linkToProject);
				return null;
			}
		});
	}

	@SuppressWarnings("unchecked")
	public List<ProjectStatistics> getProjectStatistics(final User user,
			final ProjectFilter filter, final ProjectSortOrder sortOrder,
			final int startRow, final int maxCount) {
		return TawalaSessionFactory.MAIN.getHibernateTemplate().executeFind(
				new HibernateCallback() {

					public Object doInHibernate(Session session)
							throws HibernateException, SQLException {
						Query query = session
								.createSQLQuery(QUERY_GET_PROJECT_STATISTICS
										+ filter.getFilteringClause()
										+ " order by "
										+ sortOrder.getSortOrderString());
						query.setLong(0, user.getDatabaseId());
						query.setFirstResult(startRow);
						query.setMaxResults(maxCount);

						List<ProjectStatistics> result = new ArrayList<ProjectStatistics>(
								maxCount);
						Iterator<Object[]> iterator = query.list().iterator();
						while (iterator.hasNext()) {
							Object[] nextRow = iterator.next();
							ProjectStatistics statistics = new ProjectStatistics();
							statistics.setId(((BigInteger) nextRow[0])
									.longValue());
							statistics.setName((String) nextRow[1]);
							statistics.setCreated((Date) nextRow[2]);
							statistics.setLastUpdated((Date) nextRow[3]);
							statistics.setOffline((Boolean) nextRow[4]);
							BigInteger responseCount = (BigInteger) nextRow[5];
							statistics
									.setResponseCount(responseCount == null ? 0
											: responseCount.longValue());
							Date lastAccessedDate = (Date) nextRow[6];
							if (lastAccessedDate.getTime() == NULL_VALUE_PRESENTATION_IN_GET_STAT_QUERY
									.getTime()) {
								lastAccessedDate = null;
							}
							statistics.setLastAccessed(lastAccessedDate);

							result.add(statistics);
						}
						return result;
					}

				});
	}

	public void removeDataSource(final World world, final Project sharedData,
			final String dataSourceName) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				Project realProject = (Project) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(Project.class,
								sharedData.getId());
				if (realProject.removeDataSource(dataSourceName)) {
					TawalaSessionFactory.MAIN.getHibernateTemplate().update(
							realProject);

					world.domain().storedData().eraseResponsesFor(sharedData,
							dataSourceName);
				}

				return null;
			}
		});
	}

	public void changeProjectTheme(final String projectId, final String theme) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				LinkToUserProject link = getWithProjectRuntime(projectId);
				if (link == null) {
					return null;
				}

				link.getProject().getProject().setThemePath(theme);

				TawalaSessionFactory.MAIN.getHibernateTemplate().update(
						link.getProject().getProject());

				return null;
			}
		});

	}

	/**
	 * If "offline" is true the project will be taken offline and vice versa.
	 * 
	 * @see com.tawala.project.Projects#takeProjectOffline(com.tawala.domain.User,
	 *      long, boolean)
	 * @return true if project is offline
	 */
	public boolean takeProjectOffline(final User sessionUser,
			final long projectId, final boolean offline) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Boolean) transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				UserProject userProject = (UserProject) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(UserProject.class,
								projectId);
				if (userProject == null) {
					throw new IllegalStateException(
							"Unable to get user project #" + projectId);
				}

				if (!userProject.getUser().equals(sessionUser)) {
					throw new IllegalStateException(
							"Attempt to change online status of user project#"
									+ projectId
									+ " which doesn't belong to the current user.");
				}

				if (userProject.isOffline() && offline) {
					return offline;
				}

				userProject.setOffline(offline);
				return userProject.isOffline();
			}
		});
	}

	public static void saveFormSelectionForProject(final User user,
			final long projectId, final String formName, final boolean selected) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				UserProject userProject = getUserProjectById(user, projectId);
				if (userProject != null) {
					if (selected) {
						userProject.getProject().selectFormInProjectManager(
								formName);
					} else {
						userProject.getProject().unselectFormInProjectManager(
								formName);
					}
				}
				return null;
			}

		});
	}

	public static void hideSelectedFormsInProjectManager(final User user,
			final long projectId, final boolean doHide) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				UserProject userProject = getUserProjectById(user, projectId);
				if (userProject != null) {
					if (doHide) {
						userProject.getProject().showSelectedFormsByDefault();
					} else {
						userProject.getProject().showAllFormsByDefault();
					}
				}
				return null;
			}
		});
	}

	public static void createProjectInvoice(final ProjectInvoice invoice) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {

			public Object doInTransaction(TransactionStatus status) {
				invoice.setId(generateUniqueRandomInvoiceId());
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(invoice);
				return null;
			}
		});

	}

	private static String generateUniqueRandomInvoiceId() {
		String result = null;
		do {
			result = RandomTokenGenerator
					.getRandomToken(DomainMetadata.instance
							.getProjectInvoiceIdLength());
		} while (!isUniqueProjectInvoiceId(result));

		return result;
	}

	private static boolean isUniqueProjectInvoiceId(String invoiceId) {
		return TawalaSessionFactory.MAIN.getHibernateTemplate().get(
				ProjectInvoice.class, invoiceId) == null;
	}

	public static void processPaypalNotification(
			final InstantPaymentNotification notification) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {

			public Object doInTransaction(TransactionStatus status) {
				ProjectInvoice invoice = (ProjectInvoice) TawalaSessionFactory.MAIN
						.getHibernateTemplate().get(ProjectInvoice.class,
								notification.getInvoiceNumber());
				if (invoice == null) {
					Log.error(this, "Unable to find invoice by number '"
							+ notification.getInvoiceNumber());
					return null;
				}

				invoice.setStatus(notification.getPaymentStatus());
				invoice.setPaidAmount(notification.getPaymentAmount());
				invoice.setPaidDate(notification.getTransactionDate());

				if (invoice.getStatusFieldName() != null
						&& invoice.getStatusFieldName().length() != 0) {
					FormSubmission formSubmission = invoice.getFormSubmission();
					if (formSubmission != null
							&& notification.getPaymentStatus() != null) {
						formSubmission.setValue(new Reference(invoice
								.getStatusFieldName(), true).getFieldName(),
								notification.getPaymentStatus());

						if (invoice.getAmountFieldName() != null
								&& invoice.getStatusFieldName().length() != 0
								&& invoice.getPaidAmount() != null) {
							formSubmission
									.setValue(new Reference(invoice
											.getAmountFieldName(), true)
											.getFieldName(), notification
											.getPaymentAmount().toPlainString());
						}
					}
				}

				ProjectInvoiceEvent projectInvoiceEvent = new ProjectInvoiceEvent(
						invoice, notification);
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						projectInvoiceEvent);

				return null;
			}
		});
	}

	@SuppressWarnings("unchecked")
	public static List<BackupSchedule> getBackupSchedules(UserProject project) {
		return TawalaSessionFactory.MAIN.getHibernateTemplate()
				.findByNamedParam(
						"from " + BackupSchedule.class.getName()
								+ " where project = :project", "project",
						project);
	}

	public static void saveBackupSchedule(DailyBackup backupSchedule) {
		TawalaSessionFactory.MAIN.getHibernateTemplate().saveOrUpdate(
				backupSchedule);
	}

	public static void deleteScheduledBackup(BackupSchedule schedule) {
		TawalaSessionFactory.MAIN.getHibernateTemplate().delete(schedule);
	}

	/**
	 * This method is currently used in the project manager to display the list
	 * of user projects. For the most users it will display all the projects and
	 * it's good enough. For the users with a large number of projects the
	 * sorting is not that relevant, but they would most likely go to the
	 * project manager where the sorting is done correctly.
	 * 
	 * @param user
	 * @return
	 */
	@SuppressWarnings("unchecked")
	public static List<UserProject> getFirstXUserProjects(User user) {
		DetachedCriteria criteria = DetachedCriteria
				.forClass(UserProject.class);
		criteria.setFetchMode("project", FetchMode.JOIN);
		criteria.setFetchMode("user", FetchMode.JOIN);
		criteria.add(Restrictions.eq("user", user));

		List<UserProject> result = TawalaSessionFactory.MAIN
				.getHibernateTemplate().findByCriteria(criteria, 0,
						user.getPreferences().getNumberOfProjectsInBlock());

		Collections.sort(result, new Comparator<UserProject>() {
			public int compare(UserProject o1, UserProject o2) {
				return o1.getName().compareTo(o2.getName());
			}
		});

		return result;
	}

	public static UserProject getUserProjectById(long projectId) {
		return getUserProjectById(null, projectId);
	}

	@SuppressWarnings("unchecked")
	public static Map<UserProject, LibraryProjectVersion> findAllObsoleteProjects(
			final ProjectLibrary library) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Map<UserProject, LibraryProjectVersion>) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(TransactionStatus status) {
						Map<UserProject, LibraryProjectVersion> result = new TreeMap<UserProject, LibraryProjectVersion>(
								new Comparator<UserProject>() {
									public int compare(UserProject o1,
											UserProject o2) {
										int userComparisonResult = o1.getUser()
												.getId().compareTo(
														o2.getUser().getId());

										return userComparisonResult == 0 ? o1
												.getName().compareTo(
														o2.getName())
												: userComparisonResult;
									}
								});

						Iterator<Object[]> iterator = TawalaSessionFactory.MAIN
								.getHibernateTemplate()
								.iterate(
										"select user_project, project_version from "
												+ UserProject.class.getName()
												+ " as user_project, "
												+ LibraryProjectVersion.class
														.getName()
												+ " as project_version "
												+ "where user_project.originalLibraryProjectVersionId = project_version.id"
												+ " AND user_project.offline = false"
												+ " AND project_version.libraryProject.category.library = ?"
												+ " AND"
												+ " exists( from "
												+ LibraryProjectVersion.class
														.getName()
												+ " another_version where project_version.libraryProject = another_version.libraryProject and "
												+ " project_version.versionNumber < another_version.versionNumber)",
										library);
						while (iterator.hasNext()) {
							Object[] tuple = iterator.next();
							UserProject userProject = (UserProject) tuple[0];
							userProject.getUser().getId();
							LibraryProjectVersion libraryProjectVersion = (LibraryProjectVersion) tuple[1];
							libraryProjectVersion.getLibraryProject()
									.getLatestVersion().getVersionNumber();

							result.put(userProject, libraryProjectVersion);
						}

						return result;
					}
				});
	}

	public static UserProject getUserProjectWithRuntimeById(
			final long userProjectId) {
		return getUserProjectWithRuntimeById(null, userProjectId);
	}
	


	public static UserProject getUserProjectWithRuntimeById(final User user,
			final long userProjectId) {
		return (UserProject) TawalaSessionFactory.MAIN.getHibernateTemplate()
				.execute(new HibernateCallback() {

					public Object doInHibernate(Session session)
							throws HibernateException, SQLException {
						Criteria criteria = session.createCriteria(
								UserProject.class).add(
								Restrictions.idEq(userProjectId)).setFetchMode(
								"project", FetchMode.JOIN);
						UserProject result = (UserProject) criteria.uniqueResult();
						if (result != null && user != null
								&& !user.equals(result.getUser())) {
							throw new IllegalStateException(
									"Attempt to retrieve user project #"
											+ userProjectId
											+ " which doesn't belong to user #"
											+ user.getDatabaseId());
						}

						return result;
					}
				});
	}

	public static void updateProjectDetails(final UserProject userProject) {
		TawalaSessionFactory.MAIN.getHibernateTemplate().merge(userProject);
	}

	private final static String QUERY_GET_SPORTS_DASHBOARD_STATISTICS = "select "
			+ "user_project_id, u.user_name, up.name, reg_start_dt, reg_close_dt, reg_invoice_dt, reg_fee, "
			+ "(select count(*) from submission s where s.project_id = up.project_id and form = 'Registration') as submission_count, "
			+ "(select count(*) from submission s where s.project_id = up.project_id and form = 'Registration' and age(s.created_dt) < interval '5 days' ) as submission_count_last_5, "
			+ "(select max(created_dt) from submission as s where s.project_id = up.project_id)"
			+ " from user_project up, users u  "
			+ "where reg_start_dt is not null "
			+ " and project_type = 'sports-dashboard'"
			+ " and reg_invoice_dt is null" + " and up.user_id = u.user_id";

	@SuppressWarnings("unchecked")
	public static List<SportsDashboardStatistics> getSportsDashboardOpenProjectsStats() {
		return TawalaSessionFactory.MAIN.getHibernateTemplate().executeFind(
				new HibernateCallback() {

					public Object doInHibernate(Session session)
							throws HibernateException, SQLException {
						Query query = session
								.createSQLQuery(QUERY_GET_SPORTS_DASHBOARD_STATISTICS);

						List<SportsDashboardStatistics> result = new ArrayList<SportsDashboardStatistics>(
								100);
						Iterator<Object[]> iterator = query.list().iterator();
						while (iterator.hasNext()) {
							Object[] nextRow = iterator.next();
							SportsDashboardStatistics statistics = new SportsDashboardStatistics();
							statistics
									.setUserProjectId(((BigInteger) nextRow[0])
											.longValue());
							statistics.setUserId((String) nextRow[1]);
							statistics.setProjectName((String) nextRow[2]);
							statistics.setRegistrationOpen((Date) nextRow[3]);
							statistics.setRegistrationClose((Date) nextRow[4]);
							statistics.setInvoiceDate((Date) nextRow[5]);
							statistics
									.setRegistrationFee((BigDecimal) nextRow[6]);
							statistics
									.setRegistrationCount(extractInteger((BigInteger) nextRow[7]));
							statistics
									.setRegistrationCountLast5Days(extractInteger((BigInteger) nextRow[8]));
							statistics
									.setLastRegistrationDate((Date) nextRow[9]);

							result.add(statistics);
						}
						return result;
					}
				});
	}

	private static int extractInteger(BigInteger responseCount) {
		return responseCount == null ? 0 : responseCount.intValue();
	}

	public static Map<Date, Integer> getSportsDashboardOpenProjectsRegistrationTrend(
			final int daysBack) {

		final String queryString = "SELECT date_trunc('day', s.created_dt), count(*)"
				+ " from submission s, user_project up"
				+ " where up.project_type = 'sports-dashboard'"
				+ "  and up.reg_start_dt <= s.created_dt"
				+ "  and (up.reg_invoice_dt is null OR age(up.reg_invoice_dt) <= interval '"
				+ daysBack
				+ " days')"
				+ "  and s.project_id = up.project_id"
				+ "  and s.form = 'Registration'"
				+ "  and age(s.created_dt) <= interval '"
				+ daysBack
				+ " days'"
				+ " group by date_trunc('day', s.created_dt)";

		return runCountByDaysQuery(daysBack, queryString);
	}

	public static Map<Date, Integer> getSportsDashboardOpenProjectsEmailTrend(
			final int daysBack) {

		final String queryString = "SELECT date_trunc('day', e.create_dt), count(*)"
				+ " from email e, user_project up"
				+ " where up.project_type = 'sports-dashboard'"
				+ "  and up.reg_start_dt <= e.create_dt"
				+ "  and (up.reg_invoice_dt is null OR age(up.reg_invoice_dt) <= interval '"
				+ daysBack
				+ " days')"
				+ "  and e.user_project_id = up.user_project_id"
				+ "  and age(e.create_dt) <= interval '"
				+ daysBack
				+ " days'"
				+ " group by date_trunc('day', e.create_dt)";

		return runCountByDaysQuery(daysBack, queryString);
	}

	public static Map<Date, Integer> getFormSubmissionTrendForProject(
			final long userProjectId, String formName, final int daysBack) {
		final String queryString = "SELECT date_trunc('day', s.created_dt), count(*)"
				+ " from submission s, user_project up"
				+ " where up.user_project_id = "
				+ userProjectId
				+ "  and s.project_id = up.project_id"
				+ "  and s.form = '"
				+ formName
				+ "'"
				+ "  and age(s.created_dt) <= interval '"
				+ daysBack
				+ " days'"
				+ " group by date_trunc('day', s.created_dt)";

		return runCountByDaysQuery(daysBack, queryString);
	}

	public static Map<Date, Integer> getEmailTrendForProject(
			final long userProjectId, final int daysBack) {

		final String queryString = "SELECT date_trunc('day', e.create_dt), count(*)"
				+ " from email e, user_project up"
				+ " where up.user_project_id = "
				+ userProjectId
				+ "  and e.user_project_id = up.user_project_id"
				+ "  and age(e.create_dt) <= interval '"
				+ daysBack
				+ " days'"
				+ " group by date_trunc('day', e.create_dt)";

		return runCountByDaysQuery(daysBack, queryString);
	}

	private static Map<Date, Integer> runCountByDaysQuery(final int daysBack,
			final String queryString) {
		final Map<Date, Integer> result = new LinkedHashMap<Date, Integer>();
		Calendar now = Calendar.getInstance();
		for (int i = daysBack; i > 0; i--) {
			Calendar nextDay = Calendar.getInstance();
			nextDay.clear();
			nextDay.set(now.get(Calendar.YEAR), now.get(Calendar.MONTH), now
					.get(Calendar.DATE));
			nextDay.add(Calendar.DAY_OF_YEAR, -i);
			result.put(nextDay.getTime(), new Integer(0));
		}

		TawalaSessionFactory.MAIN.getHibernateTemplate().execute(
				new HibernateCallback() {
					@SuppressWarnings("unchecked")
					public Object doInHibernate(Session session)
							throws HibernateException, SQLException {

						Query sqlQuery = session.createSQLQuery(queryString);

						Iterator<Object[]> iterator = sqlQuery.list()
								.iterator();

						while (iterator.hasNext()) {
							Object[] nextRow = iterator.next();
							Calendar nextDate = Calendar.getInstance();
							nextDate.setTime((Date) nextRow[0]);

							Calendar date = Calendar.getInstance();
							date.clear();
							date.set(nextDate.get(Calendar.YEAR), nextDate
									.get(Calendar.MONTH), nextDate
									.get(Calendar.DATE));

							Integer count = extractInteger((BigInteger) nextRow[1]);
							if (result.containsKey(date.getTime())) {
								result.put(date.getTime(), count);
							}
						}

						return null;
					}
				});

		return result;
	}
}
