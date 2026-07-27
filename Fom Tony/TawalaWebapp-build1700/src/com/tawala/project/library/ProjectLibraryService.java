package com.tawala.project.library;

import java.io.IOException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.ListIterator;
import java.util.Map;
import java.util.Set;

import org.apache.lucene.queryParser.ParseException;
import org.hibernate.criterion.DetachedCriteria;
import org.hibernate.criterion.Order;
import org.hibernate.criterion.Restrictions;
import org.springframework.orm.hibernate3.HibernateOptimisticLockingFailureException;
import org.springframework.orm.hibernate3.HibernateTemplate;
import org.springframework.transaction.TransactionStatus;
import org.springframework.transaction.support.TransactionCallback;
import org.springframework.transaction.support.TransactionTemplate;

import com.scissor.Log;
import com.tawala.UsersHibernateImpl;
import com.tawala.domain.User;
import com.tawala.hibernate.TawalaSessionFactory;
import com.tawala.project.Form;
import com.tawala.project.FormSubmission;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.library.event.CategoryAddedEvent;
import com.tawala.project.library.event.CategoryDeletedEvent;
import com.tawala.project.library.event.CategoryNameChangedEvent;
import com.tawala.project.library.event.CategoryParentChangedEvent;
import com.tawala.project.library.event.CommentAdded;
import com.tawala.project.library.event.CommentDeleted;
import com.tawala.project.library.event.CommentRestored;
import com.tawala.project.library.event.LibraryChangeEventBase;
import com.tawala.project.library.event.ProjectAdded;
import com.tawala.project.library.event.ProjectCategoryChanged;
import com.tawala.project.library.event.ProjectChangeEventBase;
import com.tawala.project.library.event.ProjectDeleted;
import com.tawala.project.library.event.ProjectRestored;
import com.tawala.project.library.event.ProjectVersionAdded;
import com.tawala.project.library.event.ProjectVersionDeleted;
import com.tawala.project.library.event.ProjectVersionRestored;
import com.tawala.search.Indexers;
import com.tawala.web.WorldInitializer;

public class ProjectLibraryService {

	private static final String QUERY_GET_TOP_LEVEL_CATEGORIES = "from "
			+ Category.class.getName()
			+ " category where category.parent=null and category.library=?";
	private static final String QUERY_GET_ALL_LIBRARY_PROJECTS = "from "
			+ LibraryProject.class.getName()
			+ " project where project.deleted=false and project.category.library = ?";
	private static final String QUERY_GET_FEATURED_LIBRARY_PROJECTS = "from "
			+ LibraryProject.class.getName()
			+ " where deleted=false and featured=true order by featuredOrder";
	private static final String QUERY_GET_LIBRARY_PROJECTS_ELIGIBLE_FOR_LANDING_PAGES = "from "
			+ LibraryProject.class.getName()
			+ " where deleted=false and category.library.id in ("
			+ ProjectLibrary.SYSTEM_LIBRARY.getId()
			+ ", "
			+ ProjectLibrary.SYSTEM_UNDER_CONTSTRUCTION_LIBRARY.getId() + ")";

	static {
		HibernateTemplate queryTemplate = new HibernateTemplate();
		queryTemplate.setCacheQueries(true);
		queryTemplate.setQueryCacheRegion("query.allProjects");
		TawalaSessionFactory.MAIN.addCustomQueryTemplate(
				QUERY_GET_ALL_LIBRARY_PROJECTS, queryTemplate);

		queryTemplate = new HibernateTemplate(TawalaSessionFactory.MAIN
				.getFactory());
		queryTemplate.setCacheQueries(true);
		queryTemplate.setQueryCacheRegion("query.topLevelCategories");
		TawalaSessionFactory.MAIN.addCustomQueryTemplate(
				QUERY_GET_TOP_LEVEL_CATEGORIES, queryTemplate);

		queryTemplate = new HibernateTemplate(TawalaSessionFactory.MAIN
				.getFactory());
		queryTemplate.setCacheQueries(true);
		queryTemplate.setQueryCacheRegion("query.featuredProjects");
		TawalaSessionFactory.MAIN.addCustomQueryTemplate(
				QUERY_GET_FEATURED_LIBRARY_PROJECTS, queryTemplate);

		queryTemplate = new HibernateTemplate(TawalaSessionFactory.MAIN
				.getFactory());
		queryTemplate.setCacheQueries(true);
		TawalaSessionFactory.MAIN.addCustomQueryTemplate(
				QUERY_GET_LIBRARY_PROJECTS_ELIGIBLE_FOR_LANDING_PAGES,
				queryTemplate);

	}

	@SuppressWarnings("unchecked")
	public static List<LibraryProject> getAllProjectsFrom(
			ProjectLibrary projectLibrary) {
		return (List<LibraryProject>) TawalaSessionFactory.MAIN
				.getCustomTemplate(QUERY_GET_ALL_LIBRARY_PROJECTS).find(
						QUERY_GET_ALL_LIBRARY_PROJECTS, projectLibrary);
	}

	@SuppressWarnings("unchecked")
	public static List<LibraryProject> getFeaturedProjects() {
		return (List<LibraryProject>) TawalaSessionFactory.MAIN
				.getCustomTemplate(QUERY_GET_FEATURED_LIBRARY_PROJECTS).find(
						QUERY_GET_FEATURED_LIBRARY_PROJECTS);
	}

	@SuppressWarnings("unchecked")
	public static List<LibraryProject> getProjectEligibleForInclusionOnLandingPages() {
		return (List<LibraryProject>) TawalaSessionFactory.MAIN
				.getCustomTemplate(
						QUERY_GET_LIBRARY_PROJECTS_ELIGIBLE_FOR_LANDING_PAGES)
				.find(QUERY_GET_LIBRARY_PROJECTS_ELIGIBLE_FOR_LANDING_PAGES);
	}

	@SuppressWarnings("unchecked")
	public static Collection<Category> getTopLevelCategoriesFor(
			ProjectLibrary library) {
		List<Category> sortedResult = (List<Category>) TawalaSessionFactory.MAIN
				.getCustomTemplate(QUERY_GET_TOP_LEVEL_CATEGORIES).find(
						QUERY_GET_TOP_LEVEL_CATEGORIES, library);

		Collections.sort(sortedResult, Category.DEFAULT_COMPARATOR);

		return sortedResult;
	}

	// For tests only. Shouldn't be used in production.
	public static void deleteCategoryLike(final String name) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			@SuppressWarnings("unchecked")
			public Object doInTransaction(TransactionStatus status) {
				List<Category> categories = (List<Category>) TawalaSessionFactory.MAIN
						.getHibernateTemplate()
						.findByNamedParam(
								"from "
										+ Category.class.getName()
										+ " category where category.name = :name",
								"name", name);

				boolean updated = false;
				ProjectLibrary library = null;
				for (Category category : categories) {
					library = category.getLibrary();
					TawalaSessionFactory.MAIN.getFactory()
							.evict(Category.class);
					TawalaSessionFactory.MAIN.getFactory().evict(
							LibraryProject.class);
					TawalaSessionFactory.MAIN.getFactory().evictCollection(
							Category.class.getName() + "." + "projects");
					recursivelyDeleteProjects(category);
					if (category.getParent() != null) {
						category.getParent().removeSubcategory(category);
					}
					TawalaSessionFactory.MAIN.getHibernateTemplate().delete(
							category);
					updated = true;
				}

				if (updated) {
					TawalaSessionFactory.MAIN.getHibernateTemplate().flush();

					doResetCategoryProjectCount(library);
				}
				return null;
			}

			private void recursivelyDeleteProjects(Category category) {
				Collection<LibraryProject> projects = new ArrayList<LibraryProject>(
						category.getProjects());
				for (LibraryProject project : projects) {
					TawalaSessionFactory.MAIN.getHibernateTemplate().delete(
							project);
					TawalaSessionFactory.MAIN.getHibernateTemplate().flush();
				}
				for (Category subcategory : category.getSubcategories()) {
					recursivelyDeleteProjects(subcategory);
				}
			}
		});
	}

	public static void createCategory(final Category category, final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(category);
				if (category.getParent() != null)
					TawalaSessionFactory.MAIN.getHibernateTemplate().update(
							category.getParent());

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new CategoryAddedEvent(category, user));

				return null;
			}
		});
	}

	public static Category updateCategory(final User user,
			final long categoryId, final String name, final String description,
			final Long parentCategoryId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Category) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(TransactionStatus status) {
						Category category = ProjectLibraryService
								.findCategoryById(categoryId);
						if (category == null)
							return null;

						if (category.isReadOnly())
							return category;

						Category newParentCategory = null;
						if (parentCategoryId != null) {
							newParentCategory = ProjectLibraryService
									.findCategoryById(parentCategoryId);
						}

						long newParentId = newParentCategory == null ? -1
								: newParentCategory.getId();
						long existingParentId = category.getParent() == null ? -1
								: category.getParent().getId();

						boolean categoryChanged = newParentId != existingParentId;

						if (categoryChanged) {
							Category oldParentCategory = category.getParent();
							category.setParent(newParentCategory);

							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.save(
											new CategoryParentChangedEvent(
													user, category,
													oldParentCategory));
						}

						if (!category.getName().equals(name)) {
							String previousName = category.getName();

							category.setName(name);
							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.save(
											new CategoryNameChangedEvent(user,
													category, previousName));
						}

						category.setDescription(description);

						doResetCategoryProjectCount(category.getLibrary());

						try {
							rebuildIndexes();
						} catch (Exception e) {
							Log.error(ProjectLibraryService.class,
									"failed to rebuild indexes", e);
						}

						return category;
					}

				});
	}

	public static void deleteCategory(final Category detachedCategory,
			final User user) throws Exception {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				// --- Let's work with persistent object from the start.
				Category category = (Category) TawalaSessionFactory.MAIN
						.getHibernateTemplate().merge(detachedCategory);

				Category parent = category.getParent();

				Category projectDestination = parent;
				if (projectDestination == null) {
					projectDestination = getOrCreateDefaultCategory(category
							.getLibrary());
				}

				if (category.equals(projectDestination)) {
					// --- this is the case of deleting default directory.
					return null;
				}

				if (parent != null) {
					parent.removeSubcategory(category);
					TawalaSessionFactory.MAIN.getHibernateTemplate().update(
							parent);
				}

				deleteAllChildren(category, projectDestination, user);
				doDeleteCategory(category, projectDestination, user);

				TawalaSessionFactory.MAIN.getHibernateTemplate().update(
						projectDestination);

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new CategoryDeletedEvent(user, category));

				doResetCategoryProjectCount(category.getLibrary());

				return null;
			}
		});
	}

	/**
	 * @param category
	 * @param container
	 * @throws Exception
	 */
	private static void deleteAllChildren(final Category category,
			final Category projectDestination, final User user) {
		for (Category child : category.getSubcategories()) {
			deleteAllChildren(child, projectDestination, user);

			doDeleteCategory(child, projectDestination, user);
		}

		category.getSubcategories().clear();
	}

	// TODO: move reindexing out of transaction.
	private static void doDeleteCategory(final Category category,
			final Category projectDestination, final User user) {
		Collection<LibraryProject> projects = findAllProjectsByCategory(category);
		for (LibraryProject project : projects) {
			ProjectCategoryChanged event = new ProjectCategoryChanged(user
					.getId(), project.getId(), category, projectDestination);
			TawalaSessionFactory.MAIN.getHibernateTemplate().save(event);

			project.setCategory(projectDestination);
			TawalaSessionFactory.MAIN.getHibernateTemplate().update(project);

			try {
				Indexers.getProjectIndexer().index(project);
			} catch (IOException e) {
				Log.error(ProjectLibraryService.class,
						"Failed to reindex the project", e);
			}
		}

		TawalaSessionFactory.MAIN.getHibernateTemplate().delete(category);
	}

	public static void onProjectSubmission(final LibraryProject project,
			final UserProject userProject) throws IOException {

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				project.setSubmittedDate(new Date());
				project.setLastUpdatedDate(project.getSubmittedDate());

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(project);

				LibraryProjectVersion projectVersion = project.getVersions()
						.get(0);
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						projectVersion.getProject());
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						projectVersion);
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new ProjectAdded(project));

				Category category = (Category) TawalaSessionFactory.MAIN
						.getHibernateTemplate().merge(project.getCategory());
				category.addProject(project);

				try {
					Indexers.getProjectIndexer().index(project);
				} catch (IOException e) {
					// --- TODO: Shall we fail?
					Log.error(this, "Failed to reindex the project:", e);
				}

				updateProjectWithLinkToLibrary(userProject, project,
						projectVersion);

				return null;
			}

		});
	}

	public static void onProjectUpdate(final LibraryProject project,
			final User user) throws IOException {

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				LibraryProject original = findProjectById(project.getId());
				if (original == null) {
					throw new IllegalStateException(
							"Unable to find the project with id "
									+ project.getId());
				}

				verifyUserAllowedToModifyProject(original, user);

				Category originalCategory = original.getCategory();

				if (!originalCategory.equals(project.getCategory())) {
					// --- Needs to be loaded inside transaction
					project.setCategory(findCategoryById(project.getCategory()
							.getId()));
				}

				Collection<ProjectChangeEvent> changeEvents = original
						.copyChanges(project, user);
				for (ProjectChangeEvent event : changeEvents) {
					TawalaSessionFactory.MAIN.getHibernateTemplate()
							.save(event);
				}

				original.setLastUpdatedDate(new Date());

				if (!originalCategory.equals(project.getCategory())) {
					doResetCategoryProjectCount(originalCategory.getLibrary());
				}

				try {
					Indexers.getProjectIndexer().index(project);
				} catch (IOException e) {
					Log.error(this, "unable to reindex the project:", e);
				}

				return null;
			}
		});
	}

	public static void onProjectDownloadByUser(final LibraryProject project,
			final LibraryProjectVersion version, final User user) {

		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				project.downloadedBy(user, version);
				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(project);

				return null;
			}
		});
	}

	public static void onAddingProjectComments(final LibraryProject project,
			final Comment comment) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(comment);

				project.addComment(comment);
				project.setLastUpdatedDate(new Date());

				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(project);

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new CommentAdded(project, comment));

				return null;
			}
		});
	}

	public static void onAddingProjectVersion(final UserProject userProject,
			final LibraryProject project, final LibraryProjectVersion version) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				verifyUserAllowedToModifyProject(project, userProject.getUser());

				Project runnableProject = version.getProject();

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						runnableProject);
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(version);

				project.addVersion(version);
				project.setLastUpdatedDate(new Date());

				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(project);

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new ProjectVersionAdded(project.getId(), version));

				updateProjectWithLinkToLibrary(userProject, project, version);

				return null;
			}
		});
	}

	private static void updateProjectWithLinkToLibrary(UserProject userProject,
			LibraryProject project, LibraryProjectVersion version)
			throws IllegalStateException {
		if (userProject == null) {
			throw new IllegalArgumentException("Deployed project is null");
		}

		userProject = (UserProject) TawalaSessionFactory.MAIN
				.getHibernateTemplate().load(UserProject.class,
						userProject.getId());

		userProject.setLibraryProjectId(project.getId());
		userProject.setLibraryVersionNumber(version.getVersionNumber());
	}

	public static void onRatingProject(final LibraryProject project,
			final Rating rating) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(rating);

				project.addRating(rating);
				project.setLastUpdatedDate(new Date());
				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(project);

				return null;
			}
		});
	}

	public static void onProjectTestDrive(final LibraryProject project) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		try {
			transactionTemplate.execute(new TransactionCallback() {
				public Object doInTransaction(TransactionStatus status) {
					project.incrementTestDriveCount();
					TawalaSessionFactory.MAIN.getHibernateTemplate().update(
							project);
					return null;
				}
			});
		} catch (HibernateOptimisticLockingFailureException e) {
			Log.warn(ProjectLibraryService.class,
					"Unable to update test drive count of " + project.getName()
							+ " due to concurrent updates.");
		}
	}

	public static boolean projectNameExists(final String name) {
		return null != findProjectByName(name);
	}

	@SuppressWarnings("unchecked")
	public static LibraryProject findProjectByName(final String name) {
		List<LibraryProject> result = TawalaSessionFactory.MAIN
				.getHibernateTemplate().findByNamedParam(
						"from " + LibraryProject.class.getName()
								+ " where name = :name", "name", name);

		if (result.size() == 0)
			return null;
		if (result.size() == 1)
			return result.get(0);
		throw new IllegalStateException(
				"There are projects with duplicate name '" + name + "'");
	}

	public static LibraryProject findProjectById(final long id) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return ((LibraryProject) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						LibraryProject result = (LibraryProject) TawalaSessionFactory.MAIN
								.getHibernateTemplate().get(
										LibraryProject.class, id);

						if (result == null)
							return null;

						TawalaSessionFactory.MAIN.getHibernateTemplate()
								.initialize(
										result.getRatingsByUsersCollection());
						TawalaSessionFactory.MAIN.getHibernateTemplate()
								.initialize(result.getComments());
						TawalaSessionFactory.MAIN.getHibernateTemplate()
								.initialize(result.getVersions());
						TawalaSessionFactory.MAIN
								.getHibernateTemplate()
								.initialize(
										result
												.getVersionAndUserIdsWhoDownloaded());

						return result;
					}
				}));
	}

	public static Comment findCommentById(long id) {
		return (Comment) TawalaSessionFactory.MAIN.getHibernateTemplate().get(
				Comment.class, id);
	}

	@SuppressWarnings("unchecked")
	public static Collection<ProjectChangeEvent> getProjectHistory(final long id) {
		List<ProjectChangeEvent> result = (List<ProjectChangeEvent>) TawalaSessionFactory.MAIN
				.getHibernateTemplate().findByNamedParam(
						"from " + ProjectChangeEventBase.class.getName()
								+ " where projectId = :id", "id", id);
		Collections.sort(result, new Comparator<ProjectChangeEvent>() {
			public int compare(ProjectChangeEvent o1, ProjectChangeEvent o2) {
				return o2.getDate().compareTo(o1.getDate());
			}
		});

		return result;
	}

	@SuppressWarnings("unchecked")
	public static Collection<LibraryChangeEvent> getChangesSince(final Date date) {
		List<LibraryChangeEvent> result = (List<LibraryChangeEvent>) TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.findByNamedParam(
						"from " + LibraryChangeEvent.class.getName()
								+ " where date > :startDate", "startDate", date);

		Collections.sort(result, new Comparator<LibraryChangeEvent>() {
			public int compare(LibraryChangeEvent o1, LibraryChangeEvent o2) {
				return o2.getDate().compareTo(o1.getDate());
			}
		});

		return result;
	}

	@SuppressWarnings("unchecked")
	public static Collection<LibraryChangeEvent> getChangesByUserSince(
			final User user, final Date date) {
		List<LibraryChangeEvent> result = (List<LibraryChangeEvent>) TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.findByNamedParam(
						"from "
								+ LibraryChangeEvent.class.getName()
								+ " where date > :startDate and userId = :userId",
						new String[] { "startDate", "userId" },
						new Object[] { date, user.getId() });

		Collections.sort(result, new Comparator<LibraryChangeEvent>() {
			public int compare(LibraryChangeEvent o1, LibraryChangeEvent o2) {
				return o2.getDate().compareTo(o1.getDate());
			}
		});

		return result;
	}

	@SuppressWarnings("unchecked")
	public static Collection<LibraryProject> findUndeletedProjectsByCategory(
			final Category category) {
		return TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.findByNamedParam(
						"from "
								+ LibraryProject.class.getName()
								+ " where category = :category and deleted = false",
						"category", category);
	}

	@SuppressWarnings("unchecked")
	private static Collection<LibraryProject> findAllProjectsByCategory(
			final Category category) {
		return TawalaSessionFactory.MAIN.getHibernateTemplate()
				.findByNamedParam(
						"from " + LibraryProject.class.getName()
								+ " where category = :category", "category",
						category);
	}

	public static void onProjectDelete(final long id, final User user)
			throws IOException {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				LibraryProject project = findProjectById(id);
				if (project == null)
					return null;

				verifyUserAllowedToModifyProject(project, user);

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new ProjectDeleted(project, user.getId()));

				project.setDeleted(true);

				Category category = (Category) TawalaSessionFactory.MAIN
						.getHibernateTemplate().merge(project.getCategory());
				category.decrementProjectCount();

				try {
					Indexers.getProjectIndexer().delete(project.getId());
				} catch (IOException e) {
					Log.error(this, "Failed to reindex the project", e);
				}

				return null;
			}
		});
	}

	public static void onCommentDeletion(final LibraryProject project,
			final Comment comment, final String userId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				project.removeComment(comment);

				comment.setDeleted(true);
				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(comment);

				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(project);
				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new CommentDeleted(project, userId, comment));

				return null;
			}
		});
	}

	public static void onProjectVersionDeletion(final LibraryProject project,
			final LibraryProjectVersion version, final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				verifyUserAllowedToModifyProject(project, user);

				project.getVersions().remove(version);
				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(project);

				version.setDeleted(true);
				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(version);

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new ProjectVersionDeleted(project, user.getId(),
								version));

				return null;
			}
		});
	}

	public static void onCommentRestoration(final LibraryProject project,
			final Comment comment, final String userId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				project.addComment(comment);

				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(project);

				comment.setDeleted(false);
				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(comment);

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new CommentRestored(project, userId, comment));

				return null;
			}
		});
	}

	public static void onVersionRestoration(final LibraryProject project,
			final LibraryProjectVersion version, final User user) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				verifyUserAllowedToModifyProject(project, user);

				version.setDeleted(false);
				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(version);

				project.getVersions().add(version);
				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(project);

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new ProjectVersionRestored(project, user.getId(),
								version));

				return null;
			}
		});
	}

	public static Category findCategoryById(long id) {
		return findCategoryById(id, false);
	}

	public static Category findCategoryById(final long id,
			final boolean fetchProjects) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Category) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						Category result = (Category) TawalaSessionFactory.MAIN
								.getHibernateTemplate().get(Category.class, id);
						if (fetchProjects) {
							TawalaSessionFactory.MAIN.getHibernateTemplate()
									.initialize(result.getProjects());
						}
						return result;
					}
				});
	}

	private static Category findCategoryByName(final ProjectLibrary library,
			final String name) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Category) transactionTemplate
				.execute(new TransactionCallback() {
					@SuppressWarnings("unchecked")
					public Object doInTransaction(TransactionStatus status) {
						DetachedCriteria criteria = DetachedCriteria
								.forClass(Category.class);
						criteria.add(Restrictions.eq("library", library));
						criteria.add(Restrictions.eq("name", name));

						List<Category> categories = TawalaSessionFactory.MAIN
								.getHibernateTemplate()
								.findByCriteria(criteria);
						if (categories.size() == 0) {
							return null;
						} else {
							return categories.get(0);
						}
					}
				});
	}

	public static List<Category> getAllCategoriesFor(ProjectLibrary library) {
		List<Category> flattenedCategories = new ArrayList<Category>();
		for (Category category : getTopLevelCategoriesFor(library)) {
			category.flattenOnto(flattenedCategories);
		}
		return flattenedCategories;
	}

	public static Collection<LibraryProject> search(
			ProjectLibrary projectLibrary, String query) throws ParseException,
			IOException {
		return retrieveProjectsById(Indexers.getProjectIndexer().search(
				projectLibrary.getId(), query));
	}

	private static Collection<LibraryProject> retrieveProjectsById(
			List<Long> ids) {
		Collection<LibraryProject> result = new ArrayList<LibraryProject>(ids
				.size());

		for (Long id : ids) {
			LibraryProject project = findProjectById(id);
			if (project == null) {
				Log.warn(UsersHibernateImpl.class,
						"Can't find library project with id=" + id
								+ ". Possible timing issue with indexing.");
				continue;
			}
			result.add(project);
		}
		return result;
	}

	@SuppressWarnings("unchecked")
	public static void rebuildIndexes() throws IOException {
		List<LibraryProject> projects = TawalaSessionFactory.MAIN
				.getHibernateTemplate().find(
						"from " + LibraryProject.class.getName()
								+ " as project where project.deleted = false");
		for (LibraryProject project : projects) {
			Indexers.getProjectIndexer().index(project);
		}
	}

	public static LibraryChangeEvent findEventById(long id) {
		return (LibraryChangeEvent) TawalaSessionFactory.MAIN
				.getHibernateTemplate().get(LibraryChangeEventBase.class, id);
	}

	public static void onRestoreProject(final long projectId,
			final User sessionUser) throws IOException {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				LibraryProject project = (LibraryProject) TawalaSessionFactory.MAIN
						.getHibernateTemplate().load(LibraryProject.class,
								projectId);

				TawalaSessionFactory.MAIN.getHibernateTemplate().save(
						new ProjectRestored(project, sessionUser.getId()));

				project.setDeleted(false);
				project.getCategory().incrementProjectCount();

				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.update(project);

				try {
					Indexers.getProjectIndexer().index(project);
				} catch (IOException e) {
					Log.error(this, "Failed to reindex the project", e);
				}

				return null;
			}
		});
	}

	public static void resetCategoryProjectCount(final ProjectLibrary library) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {

				doResetCategoryProjectCount(library);

				return null;
			}
		});
	}

	private static void doResetCategoryProjectCount(ProjectLibrary library) {
		Collection<Category> categories = getTopLevelCategoriesFor(library);

		for (Category category : categories) {
			resetCategoryProjectCount(category);
		}
	}

	private static void resetCategoryProjectCount(Category category) {
		int totalProjectsInSubcategories = 0;
		for (Category subCategory : category.getSubcategories()) {
			resetCategoryProjectCount(subCategory);
			totalProjectsInSubcategories += subCategory.getProjectCount();
		}

		// --- TODO: Optimize by selecting counts only.
		Collection<LibraryProject> projects = findUndeletedProjectsByCategory(category);
		category
				.setProjectCount(projects.size() + totalProjectsInSubcategories);
	}

	public static LibraryProjectVersion findProjectVersionById(final long id) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (LibraryProjectVersion)transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				LibraryProjectVersion libraryProjectVersion = (LibraryProjectVersion) TawalaSessionFactory.MAIN
								.getHibernateTemplate().get(LibraryProjectVersion.class, id);
				if(libraryProjectVersion == null) {
					return null;
				}
				
				//--- Force retrieval of the parent project.
				libraryProjectVersion.getLibraryProject().getLatestVersion();
				return libraryProjectVersion;
			}
		});
	}

	public static void permanentlyDeleteProject(final LibraryProject project) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		transactionTemplate.execute(new TransactionCallback() {
			public Object doInTransaction(TransactionStatus status) {
				TawalaSessionFactory.MAIN.getHibernateTemplate()
						.delete(project);

				Category category = (Category) TawalaSessionFactory.MAIN
						.getHibernateTemplate().merge(project.getCategory());
				category.decrementProjectCount();

				try {
					Indexers.getProjectIndexer().delete(project.getId());
				} catch (IOException e) {
					Log.error(this, "Failed to delete project from the index",
							e);
				}

				return null;
			}
		});

	}

	@SuppressWarnings("unchecked")
	public static List<Category> getCategoriesWithProjectsFrom(
			ProjectLibrary projectLibrary) {
		List<Category> result = (List<Category>) TawalaSessionFactory.MAIN
				.getHibernateTemplate()
				.find(
						"from "
								+ Category.class.getName()
								+ " as category"
								+ " left join fetch category.projects"
								+ " where category.projectCount > 0 and category.library = ?",
						projectLibrary);

		ListIterator<Category> resultIterator = result.listIterator();

		List<Category> cleanedResult = new ArrayList<Category>(10);
		while (resultIterator.hasNext()) {
			Category nextCategory = resultIterator.next();
			if (!nextCategory.isTopLevelCategory()) {
				continue;
			}

			if (cleanedResult.contains(nextCategory)) {
				continue;
			}

			removeEmptyCategories(nextCategory);
			removeDeletedProjects(nextCategory);

			cleanedResult.add(nextCategory);
		}

		Collections.sort(cleanedResult, Category.DEFAULT_COMPARATOR);

		result = new ArrayList<Category>();

		for (Category category : cleanedResult) {
			category.flattenOnto(result);
		}

		return result;
	}

	private static void removeEmptyCategories(Category nextCategory) {
		Collection<Category> toBeRemoved = new ArrayList<Category>();
		for (Category subCategory : nextCategory.getSubcategories()) {
			if (subCategory.getProjectCount() == 0) {
				toBeRemoved.add(subCategory);
			} else {
				removeEmptyCategories(subCategory);
			}
		}
		nextCategory.getSubcategories().removeAll(toBeRemoved);
	}

	private static void removeDeletedProjects(Category category) {
		Collection<LibraryProject> deletedProjects = new ArrayList<LibraryProject>();
		for (LibraryProject project : category.getProjects()) {
			if (project.isDeleted()) {
				deletedProjects.add(project);
			}
		}

		if (deletedProjects.size() > 0) {
			category.getProjects().removeAll(deletedProjects);
		}

		for (Category subcategory : category.getSubcategories()) {
			removeDeletedProjects(subcategory);
		}
	}

	@SuppressWarnings("unchecked")
	public static List<LibraryProject> getAllProjectsWithinCategory(
			final long categoryId) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (List<LibraryProject>) transactionTemplate
				.execute(new TransactionCallback() {
					public Object doInTransaction(TransactionStatus status) {
						List<LibraryProject> result = new ArrayList<LibraryProject>();
						Category category = (Category) TawalaSessionFactory.MAIN
								.getHibernateTemplate().load(Category.class,
										categoryId);
						addProjectsToList(result, category);
						return result;
					}

					private void addProjectsToList(List<LibraryProject> result,
							Category category) {
						for (LibraryProject project : category.getProjects()) {
							if (!project.isDeleted()) {
								result.add(project);
							}
						}
						for (Category subcategory : category.getSubcategories()) {
							addProjectsToList(result, subcategory);
						}
					}
				});
	}

	@SuppressWarnings("unchecked")
	public static List<LibraryProject> getClonedProjects() {
		DetachedCriteria criteria = DetachedCriteria
				.forClass(LibraryProject.class);
		criteria.add(Restrictions.gt("cloneCount", 0));
		criteria.addOrder(Order.desc("cloneCount"));
		return TawalaSessionFactory.MAIN.getHibernateTemplate().findByCriteria(
				criteria);
	}

	private static void verifyUserAllowedToModifyProject(
			final LibraryProject project, final User user) {
		if (!getLibrariesUpdatableByUser(user).contains(
				project.getCategory().getLibrary())) {
			throw new IllegalStateException("User #" + user.getDatabaseId()
					+ ", user name='" + user.getId() + "' with status "
					+ user.getStatus().toString()
					+ " attempted to modify project #" + project.getId() + " ("
					+ project.getName() + ") in "
					+ project.getCategory().getLibrary() + ".");
		}
	}

	public static Category getOrCreateDefaultCategory(ProjectLibrary library) {
		Category result = findCategoryByName(library,
				ProjectLibrary.DEFAULT_CATEGORY_NAME);
		if (result != null) {
			return result;
		}
		result = new Category(library, ProjectLibrary.DEFAULT_CATEGORY_NAME,
				ProjectLibrary.DEFAULT_CATEGORY_NAME);
		result.setReadOnly(true);
		TawalaSessionFactory.MAIN.getHibernateTemplate().save(result);
		return result;
	}

	public static List<ProjectLibrary> getLibrariesReadableByUser(User user) {
		if (user != null && user.isAdministrator()) {
			return ProjectLibrary.getLibrariesAvailableToAdmins();
		} else if (user != null
				&& user.getStatus().isAllowedToViewCompleteLibrary()) {
			return ProjectLibrary.getLibrariesReadableByFullyRegisteredUsers();
		} else {
			return ProjectLibrary.getLibrariesReadableByAnonymousUsers();
		}
	}

	@SuppressWarnings("unchecked")
	public static List<ProjectLibrary> getLibrariesUpdatableByUser(User user) {
		if (user != null && user.isAdministrator()) {
			return ProjectLibrary.getLibrariesAvailableToAdmins();
		} else if (user != null
				&& user.getStatus().isAllowedToUpdateLibraryProjects()) {
			return ProjectLibrary
					.getlibrariesUpdateableByFullyRegisteredUsers();
		} else {
			return ProjectLibrary.getlibrariesUpdateableByAnonymousUsers();
		}
	}

	public static UserProject cloneProjectToUserAccount(
			LibraryProject libraryProject, User user,
			boolean makeProjectNameUnique, String projectName, String themePath, String versionDescription) {
		LibraryProjectVersion latestVersion = libraryProject.getLatestVersion();
		Project project = latestVersion.getProject().makeCopy();
		if (themePath != null) {
			project.setThemePath(themePath);
		}
		UserProject userProject = new UserProject(project, user,
				projectName == null ? libraryProject.getName() : projectName);
		userProject.getVersions().get(0).setDescription(versionDescription);
		userProject.setOriginalLibraryProjectVersionId(latestVersion.getId());

		if (makeProjectNameUnique) {
			userProject.makeProjectNameUnique();
		}

		userProject = WorldInitializer.getDefaultWorld().domain().projects()
				.put(userProject);

		for (Form form : userProject.getProject().getForms()) {
			List<FormSubmission> submissions = WorldInitializer
					.getDefaultWorld().domain().storedData()
					.fullyInitializedResponsesFor(latestVersion.getProject(),
							form);
			for (FormSubmission submission : submissions) {
				FormSubmission newSubmission = new FormSubmission(userProject,
						form);
				newSubmission.copyFieldsFrom(submission);

				WorldInitializer.getDefaultWorld().domain().storedData()
						.record(newSubmission);
			}
		}

		onProjectCloning(libraryProject);

		return userProject;
	}

	private static void onProjectCloning(final LibraryProject project) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		try {
			transactionTemplate.execute(new TransactionCallback() {
				public Object doInTransaction(TransactionStatus status) {
					project.setCloneCount(project.getCloneCount() + 1);
					TawalaSessionFactory.MAIN.getHibernateTemplate().update(
							project);
					return null;
				}
			});
		} catch (HibernateOptimisticLockingFailureException e) {
			Log.warn(ProjectLibraryService.class,
					"Unable to update test drive count of " + project.getName()
							+ " due to concurrent updates.");
		}
	}

	public static Map<Category, Set<LibraryProject>> getExampleProjects() {
		return getAllProjectsFromALibrary(ProjectLibrary.SYSTEM_EXAMPLES_LIBRARY);
	}

	public static Map<Category, Set<LibraryProject>> getReadyToRunProjects() {
		return getAllProjectsFromALibrary(ProjectLibrary.SYSTEM_LIBRARY);
	}

	@SuppressWarnings("unchecked")
	private static Map<Category, Set<LibraryProject>> getAllProjectsFromALibrary(
			final ProjectLibrary projectLibrary) {
		TransactionTemplate transactionTemplate = new TransactionTemplate(
				TawalaSessionFactory.MAIN.getTransactionManager());
		return (Map<Category, Set<LibraryProject>>) transactionTemplate
				.execute(new TransactionCallback() {

					public Object doInTransaction(final TransactionStatus status) {
						final Map<Category, Set<LibraryProject>> result = new LinkedHashMap<Category, Set<LibraryProject>>();
						for (final Category subCategory : getCategoriesWithProjectsFrom(projectLibrary)) {
							Set<LibraryProject> projects = subCategory
									.getProjects();
							for (LibraryProject project : projects) {
								project.getName();
								project.getLatestVersion().getProject()
										.getForms();
							}
							result.put(subCategory, projects);
						}

						return result;
					}
				});
	}
}