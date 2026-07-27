package com.tawala.project.library;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Collections;
import java.util.Comparator;
import java.util.Date;
import java.util.HashMap;
import java.util.HashSet;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Set;
import java.util.Map.Entry;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.JoinTable;
import javax.persistence.Lob;
import javax.persistence.ManyToOne;
import javax.persistence.OneToMany;
import javax.persistence.OrderBy;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;
import org.hibernate.annotations.CollectionOfElements;
import org.springframework.util.StringUtils;

import com.tawala.domain.User;
import com.tawala.project.UserProject;
import com.tawala.project.library.event.ProjectCategoryChanged;
import com.tawala.project.library.event.ProjectLongDescriptionChanged;
import com.tawala.project.library.event.ProjectNameChanged;
import com.tawala.project.library.event.ProjectShortDescriptionChanged;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_lib_project_id")
@Entity
@Table(name = "lib_project")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "libraryprojects")
public class LibraryProject {
	private static final String DEFAULT_ICON_IMAGE = "/images/webapps/default-icon.gif";

	@Id
	@Column(name = "lib_project_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@Column(name = "author_id", length = 20, nullable = false)
	private String authorId;

	@Column(length = 60, nullable = false, unique = true)
	private String name;

	@Column(name = "short_desc", length = 60, nullable = false)
	private String shortDescription;

	@Lob
	@Column(name = "long_desc", nullable = false)
	private String longDescription;

	@ManyToOne(cascade = { CascadeType.PERSIST, CascadeType.MERGE })
	@JoinColumn(name = "category_id", nullable = false)
	private Category category;

	@OneToMany
	@JoinTable(name = "lib_project_comment", joinColumns = { @JoinColumn(name = "lib_project_id") }, inverseJoinColumns = @JoinColumn(name = "comment_id"))
	@OrderBy("date")
	@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "libraryprojects.comments")
	private List<Comment> comments;

	@Column(name = "next_version", nullable = false)
	private int nextVersionNumber = 1;

	@OneToMany(mappedBy="libraryProject")
	@OrderBy("versionNumber DESC")
	@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "libraryprojects.versions")
	private List<LibraryProjectVersion> versions = new ArrayList<LibraryProjectVersion>();

	@Column(name = "rating", nullable = false)
	private int rating;

	@CollectionOfElements
	@JoinTable(name = "lib_project_user_downloaded", joinColumns = @JoinColumn(name = "lib_project_id"))
	@Column(name = "user_id_and_version", length = 30, nullable = false)
	@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "libraryprojects.versionAndUserIdsWhoDownloaded")
	// --- TODO: store the timestamp too.
	private Set<String> versionAndUserIdsWhoDownloaded = new HashSet<String>();

	@OneToMany
	@JoinTable(name = "lib_project_rating_map", joinColumns = { @JoinColumn(name = "lib_project_id") }, inverseJoinColumns = @JoinColumn(name = "project_rating_id"))
	@org.hibernate.annotations.MapKey(columns = @Column(name = "key"))
	@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "libraryprojects.ratingsByUsers")
	private Map<String, Rating> ratingsByUsers;

	private transient boolean ratingsSorted = false;

	@Column(name = "deleted", nullable = false)
	private boolean deleted;

	@Column(name = "featured", nullable = false)
	private boolean featured;

	@Column(name = "featured_order", nullable = true)
	private Integer featuredOrder;

	@Column(name = "icon_url", nullable = true, length = 100)
	private String iconURL;

	@Column(name = "video_url", nullable = true, length = 100)
	private String videoURL;

	@Column(name = "snapshot_tile", nullable = true, length = 100)
	private String snapshotTile;
	/**
	 * It exists for performance optimization.
	 */
	@Column(name = "download_count", nullable = false)
	private int downloadCount;

	@Column(name = "testdrive_count", nullable = false)
	private int testDriveCount;

	/**
	 * It exists for performance optimization.
	 */
	@Column(name = "comment_count", nullable = false)
	private int commentCount;

	/**
	 * It exists for performance optimization.
	 */
	@Column(name = "clone_count", nullable = false)
	private int cloneCount;

	@Column(name = "created_dt", nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date submittedDate;

	@Column(name = "last_updated_dt", nullable = false)
	@Temporal(TemporalType.TIMESTAMP)
	private Date lastUpdatedDate;

	LibraryProject() {
		// For Hibernate's use
	}

	public LibraryProject(String authorId, UserProject userProject) {
		this.authorId = authorId;

		LibraryProjectVersion version = new LibraryProjectVersion(this, authorId,
				userProject.getProject().makeCopy());

		version.setText("Original version");
		addVersion(version);

		setName(userProject.getName());
	}

	public int getDownloadCount() {
		return downloadCount;
	}

	public void downloadedBy(User user, LibraryProjectVersion version) {
		versionAndUserIdsWhoDownloaded
				.add(version.getId() + "." + user.getId());

		downloadCount = versionAndUserIdsWhoDownloaded.size();
	}

	public void addComment(Comment comment) {
		if (comments == null)
			comments = new ArrayList<Comment>();

		comments.add(comment);
		commentCount = comments.size();
	}

	public void removeComment(Comment comment) {
		comments.remove(comment);
		commentCount = comments.size();
	}

	public void addRating(Rating rating) {
		Rating previousRating = getRatingsByUsers().put(rating.getUserId(),
				rating);
		if (previousRating == null
				|| previousRating.getValue() != rating.getValue()) {
			rerate();
		}
	}

	private void rerate() {
		int totalPoints = 0;
		for (Rating rating : ratingsByUsers.values()) {
			totalPoints += rating.getValue();
		}

		rating = Math.round(((float) totalPoints) / ratingsByUsers.size());
	}

	/**
	 * @return Returns the comments.
	 */
	public List<Comment> getComments() {
		return comments;
	}

	/**
	 * @return Returns the category.
	 */
	public Category getCategory() {
		return category;
	}

	/**
	 * @param category
	 *            The category to set.
	 */
	public void setCategory(Category category) {
		this.category = category;
	}

	/**
	 * @return Returns the longDescription.
	 */
	public String getLongDescription() {
		return longDescription;
	}

	/**
	 * @param longDescription
	 *            The longDescription to set.
	 */
	public void setLongDescription(String longDescription) {
		this.longDescription = longDescription;
	}

	/**
	 * @return Returns the shortDescription.
	 */
	public String getShortDescription() {
		return shortDescription;
	}

	/**
	 * @param shortDescription
	 *            The shortDescription to set.
	 */
	public void setShortDescription(String shortDescription) {
		this.shortDescription = shortDescription;
	}

	/**
	 * @return Returns the submittedDate.
	 */
	public Date getSubmittedDate() {
		return submittedDate;
	}

	/**
	 * @param submittedDate
	 *            The submittedDate to set.
	 */
	public void setSubmittedDate(Date submittedDate) {
		this.submittedDate = submittedDate;
	}

	/**
	 * @return Returns the id.
	 */
	public long getId() {
		return id;
	}

	/**
	 * @param id
	 *            The id to set.
	 */
	public void setId(long id) {
		this.id = id;
	}

	/**
	 * @return Returns the rating.
	 */
	public int getRating() {
		return rating;
	}

	/**
	 * @return Returns the authorId.
	 */
	public String getAuthorId() {
		return authorId;
	}

	@Override
	public boolean equals(Object obj) {
		LibraryProject other = (LibraryProject) obj;

		return this.id == other.id;
	}

	@Override
	public int hashCode() {
		return (int) id;
	}

	public Collection<ProjectChangeEvent> copyChanges(LibraryProject updated,
			User user) {

		Collection<ProjectChangeEvent> result = new ArrayList<ProjectChangeEvent>();

		if (!this.getShortDescription().equals(updated.getShortDescription())) {
			result.add(new ProjectShortDescriptionChanged(user.getId(), this
					.getId(), this.getShortDescription(), updated
					.getShortDescription()));

			this.shortDescription = updated.shortDescription;
		}
		if (!this.getLongDescription().equals(updated.getLongDescription())) {

			result.add(new ProjectLongDescriptionChanged(user.getId(), this
					.getId(), this.getLongDescription(), updated
					.getLongDescription()));

			this.longDescription = updated.longDescription;
		}
		if (!this.getName().equals(updated.getName())) {
			result.add(new ProjectNameChanged(user.getId(), this.getId(), this
					.getName(), updated.getName()));

			this.setName(updated.getName());
		}

		if (!this.getCategory().equals(updated.getCategory())) {
			result.add(new ProjectCategoryChanged(user.getId(), this.getId(),
					this.getCategory(), updated.getCategory()));

			this.category.removeProject(this);
			this.category = updated.category;
			this.category.addProject(this);
		}

		if (user.isAdministrator()) {
			this.featured = updated.featured;
			this.featuredOrder = this.featured ? updated.featuredOrder : null;
			this.iconURL = updated.iconURL;
			this.videoURL = updated.videoURL;
			this.snapshotTile = updated.snapshotTile;
			this.authorId = updated.authorId;
		}

		return result;
	}

	public Map<String, Rating> getRatingsByUsersCollection() {
		return ratingsByUsers;
	}

	/**
	 * @return Returns the ratingsByUsers.
	 */
	public Map<String, Rating> getRatingsByUsers() {
		if (ratingsByUsers == null) {
			ratingsByUsers = new HashMap<String, Rating>();
			return ratingsByUsers;
		}

		if (!ratingsSorted) {
			List<Map.Entry<String, Rating>> entries = new ArrayList<Map.Entry<String, Rating>>(
					ratingsByUsers.entrySet());
			Collections.sort(entries,
					new Comparator<Map.Entry<String, Rating>>() {

						public int compare(Entry<String, Rating> o1,
								Entry<String, Rating> o2) {
							Rating rating1 = o1.getValue();
							Rating rating2 = o2.getValue();

							if (rating1.getValue() == rating2.getValue())
								return rating1.getDate().compareTo(
										rating2.getDate());

							return rating2.getValue() - rating1.getValue();
						}
					});

			Map<String, Rating> sorted = new LinkedHashMap<String, Rating>(
					entries.size() + 1);
			for (Map.Entry<String, Rating> entry : entries) {
				sorted.put(entry.getKey(), entry.getValue());
			}

			ratingsByUsers = sorted;

			ratingsSorted = true;
		}

		return ratingsByUsers;
	}

	/**
	 * @return Returns the deleted.
	 */
	public boolean isDeleted() {
		return deleted;
	}

	/**
	 * @param deleted
	 *            The deleted to set.
	 */
	public void setDeleted(boolean deleted) {
		this.deleted = deleted;
	}

	public void addVersion(LibraryProjectVersion version) {
		version.setVersionNumber(nextVersionNumber++);
		versions.add(0, version);
	}

	/**
	 * @return Returns the versions.
	 */
	public List<LibraryProjectVersion> getVersions() {
		return versions;
	}

	public int getLatestVersionNumber() {
		return versions.size() == 0 ? 0 : versions.get(0).getVersionNumber();
	}

	/**
	 * @return Returns the name.
	 */
	public String getName() {
		return name;
	}

	/**
	 * @param name
	 *            The name to set.
	 */
	public void setName(String name) {
		this.name = name;
	}

	/**
	 * @return Returns the versionAndUserIdsWhoDownloaded.
	 */
	public Set<String> getVersionAndUserIdsWhoDownloaded() {
		return versionAndUserIdsWhoDownloaded;
	}

	public int getCommentCount() {
		return commentCount;
	}

	public int getTestDriveCount() {
		return testDriveCount;
	}

	public void incrementTestDriveCount() {
		testDriveCount += 1;
	}

	public Date getLastUpdatedDate() {
		return lastUpdatedDate;
	}

	public void setLastUpdatedDate(Date lastUpdatedDate) {
		this.lastUpdatedDate = lastUpdatedDate;
	}

	public boolean isVetted() {
		return category.getLibrary().equals(ProjectLibrary.SYSTEM_LIBRARY)
				|| category.getLibrary().equals(
						ProjectLibrary.SYSTEM_UNDER_CONTSTRUCTION_LIBRARY);
	}

	public LibraryProjectVersion getLatestVersion() {
		if (versions.size() < 1) {
			throw new IllegalStateException(
					"No versions found for the library project " + id);
		}

		// --- Versions are sorted in the latest first order.
		return versions.get(0);
	}

	public boolean isFeatured() {
		return featured;
	}

	public void setFeatured(boolean featured) {
		this.featured = featured;
	}

	public String getIconURL() {
		return iconURL == null ? DEFAULT_ICON_IMAGE : iconURL;
	}

	public void setIconURL(String iconImage) {
		this.iconURL = StringUtils.hasText(iconImage) ? iconImage.trim() : null;
	}

	public String getVideoURL() {
		return videoURL;
	}

	public void setVideoURL(String videoFile) {
		this.videoURL = StringUtils.hasText(videoFile) ? videoFile.trim()
				: null;
	}

	public Integer getFeaturedOrder() {
		return featuredOrder;
	}

	public void setFeaturedOrder(Integer featuredOrder) {
		this.featuredOrder = featuredOrder;
	}

	public String getSnapshotTile() {
		return snapshotTile;
	}

	public void setSnapshotTile(String snapshotTile) {
		this.snapshotTile = snapshotTile != null
				&& snapshotTile.trim().length() == 0 ? null : snapshotTile;
	}

	public boolean isUnderConstruction() {
		return getCategory().getLibrary().equals(
				ProjectLibrary.SYSTEM_UNDER_CONTSTRUCTION_LIBRARY);
	}

	public void setAuthorId(String authorId) {
		this.authorId = authorId;
	}

	public int getCloneCount() {
		return cloneCount;
	}

	public void setCloneCount(int cloneCount) {
		this.cloneCount = cloneCount;
	}

	public LibraryProjectVersion getVersionByNumber(int versionNumber) {
		for (LibraryProjectVersion version : getVersions()) {
			if (version.getVersionNumber() == versionNumber) {
				return version;
			}
		}
		throw new IllegalStateException("Unable to find version #" + versionNumber);
	}
}
