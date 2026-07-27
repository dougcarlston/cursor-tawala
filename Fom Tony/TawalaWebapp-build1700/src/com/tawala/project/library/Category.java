package com.tawala.project.library;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Comparator;
import java.util.List;
import java.util.Set;
import java.util.TreeSet;

import javax.persistence.CascadeType;
import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.JoinColumn;
import javax.persistence.ManyToOne;
import javax.persistence.OneToMany;
import javax.persistence.OrderBy;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Version;

import org.hibernate.annotations.AccessType;
import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_lib_category_id")
@Entity
@Table(name = "lib_category")
@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "categories")
public class Category {
	public static class NameComparator implements Comparator<Category> {
		public int compare(Category o1, Category o2) {
			return o1.name.compareToIgnoreCase(o2.name);
		}
	}

	public static final Comparator<Category> DEFAULT_COMPARATOR = new NameComparator();

	@Id
	@Column(name = "category_id")
	@GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
	private long id;

	@Column(name = "name", length = 40, nullable = false)
	private String name;

	@Column(name = "description", length = 60, nullable = true)
	private String description;

	@ManyToOne(cascade = { CascadeType.PERSIST, CascadeType.MERGE }, fetch = FetchType.EAGER)
	@JoinColumn(name = "parent_category_id")
	private Category parent;

	@OneToMany(mappedBy = "parent", fetch = FetchType.EAGER, cascade = {
			CascadeType.REMOVE, CascadeType.REFRESH })
	@OrderBy("name")
	@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "categories.subcategories")
	private Set<Category> subcategories = new TreeSet<Category>(
			DEFAULT_COMPARATOR);

	@Column(name = "read_only", nullable = false)
	private boolean readOnly;

	@Column(name = "project_count", nullable = false)
	private int projectCount;

	@OneToMany(mappedBy = "category")
	@Cache(usage = CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region = "categories.projects")
	private Set<LibraryProject> projects;

	@SuppressWarnings("unused")
	@Version
	@Column(name = "version", nullable = false)
	@AccessType(value = "field")
	private int version;

	@ManyToOne()
	@JoinColumn(name = "project_library_id")
	private ProjectLibrary library;

	Category() {
		// --- For Hibernate's use.
	}

	public Category(ProjectLibrary library, String name, String description) {
		this(library, null, name, description);
	}

	public Category(Category parent, String name, String description) {
		this(parent.getLibrary(), parent, name, description);
	}

	private Category(ProjectLibrary library, Category parent, String name,
			String description) {
		this.library = library;
		this.parent = parent;
		this.name = name;
		this.description = description;

		if (parent != null)
			parent.addSubcategory(this);
	}

	/**
	 * @return Returns the parent.
	 */
	public Category getParent() {
		return parent;
	}

	/**
	 * @return Returns the description.
	 */
	public String getDescription() {
		return description;
	}

	/**
	 * @return Returns the name.
	 */
	public String getName() {
		return name;
	}

	/**
	 * @return Returns the subcategories.
	 */
	public Collection<Category> getSubcategories() {
		return subcategories;
	}

	/**
	 * @param category
	 * @return
	 */
	private boolean addSubcategory(Category category) {
		return subcategories.add(category);
	}

	/**
	 * @param category
	 * @return
	 */
	public boolean removeSubcategory(Category category) {
		return subcategories.remove(category);
	}

	/**
	 * @param id
	 *            The id to set.
	 */
	public void setId(long id) {
		this.id = id;
	}

	/**
	 * @return Returns the id.
	 */
	public long getId() {
		return id;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see java.lang.Object#equals(java.lang.Object)
	 */
	@Override
	public boolean equals(Object obj) {
		Category other = (Category) obj;

		if (!this.getLibrary().equals(other.getLibrary())) {
			return false;
		}

		if ((this.parent != null && other.parent == null)
				|| (this.parent == null && other.parent != null))
			return false;

		if (this.parent != null && !this.parent.equals(other.parent)) {
			return false;
		}

		return this.name.equals(other.name);
	}

	public boolean isLeaf() {
		return subcategories == null || subcategories.size() == 0;
	}

	/**
	 * Recursively adds each node in the branch to the result.
	 * 
	 * @param result
	 */
	public void flattenOnto(Collection<Category> result) {
		result.add(this);
		if (!isLeaf()) {
			for (Category subcategory : subcategories) {
				subcategory.flattenOnto(result);
			}
		}
	}

	public List<Category> getAllParents() {
		List<Category> result = new ArrayList<Category>();

		Category previousParent = parent;
		while (previousParent != null) {
			result.add(0, previousParent);
			previousParent = previousParent.parent;
		}

		return result;
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see java.lang.Object#toString()
	 */
	@Override
	public String toString() {
		return "Category "
				+ name
				+ (parent == null ? "; no parent" : "; with parent of "
						+ parent.name);
	}

	/*
	 * (non-Javadoc)
	 * 
	 * @see java.lang.Object#hashCode() @Override public int hashCode() { return
	 *      name.hashCode(); }
	 */

	/**
	 * @param description
	 *            The description to set.
	 */
	public void setDescription(String description) {
		this.description = description;
	}

	/**
	 * @param name
	 *            The name to set.
	 */
	public void setName(String name) {
		if (parent != null)
			parent.subcategories.remove(this);

		this.name = name;

		if (parent != null)
			parent.subcategories.add(this);
	}

	/**
	 * @param newParent
	 *            The parent to set.
	 */
	public void setParent(Category newParent) {
		if(newParent != null && newParent.getClass() == TopLevelCategory.class) {
			setLibrary(newParent.getLibrary());
			this.parent = null;
			return;
		}
		
		if (newParent != null
				&& (newParent.isChildOf(this) || this.equals(newParent))) {
			throw new IllegalArgumentException(
					"Attempt to create a circular tree of categories.");
		}

		if (this.parent != null) {
			this.parent.removeSubcategory(this);
		}

		this.parent = newParent;
		if (this.parent != null) {
			this.parent.addSubcategory(this);
			this.library = this.parent.getLibrary();
		}
	}

	/**
	 * @return Returns the readOnly.
	 */
	public boolean isReadOnly() {
		return readOnly;
	}

	public void incrementProjectCount() {
		projectCount++;
		if (parent != null)
			parent.incrementProjectCount();
	}

	public void decrementProjectCount() {
		projectCount--;
		if (parent != null)
			parent.decrementProjectCount();
	}

	public int getProjectCount() {
		return projectCount;
	}

	public void setProjectCount(int projectCount) {
		this.projectCount = projectCount;
	}

	public boolean isChildOf(Category category) {
		Category nextParent = parent;
		while (nextParent != null) {
			if (nextParent.equals(category))
				return true;

			nextParent = nextParent.getParent();
		}
		return false;
	}

	public boolean isTopLevelCategory() {
		return parent == null;
	}

	public Set<LibraryProject> getProjects() {
		return projects;
	}

	public String getFullName() {
		StringBuffer buffer = new StringBuffer();

		for (Category parent : getAllParents()) {
			buffer.append(parent.getName());
			buffer.append(" -> ");
		}

		buffer.append(getName());

		return buffer.toString();
	}

	public int getDepth() {
		int depth = 0;
		if (parent == null) {
			return depth;
		} else {
			return parent.getDepth() + 1;
		}
	}

	public void addProject(LibraryProject project) {
		if (projects == null)
			projects = new TreeSet<LibraryProject>();
		if (projects.add(project) && !project.isDeleted())
			incrementProjectCount();
	}

	public void removeProject(LibraryProject project) {
		projects.remove(project);
		decrementProjectCount();
	}

	public Category getTopLevelCategoryInHierarchy() {
		Category result = this;
		while (result.getParent() != null) {
			result = result.getParent();
		}
		return result;
	}

	public ProjectLibrary getLibrary() {
		return library;
	}

	public void setLibrary(ProjectLibrary library) {
		this.library = library;
	}

	/**
	 * TODO: this is a hack around the problem with creating new categories. We
	 * need to assign a new top level category, but there is no such persistent
	 * thing, and we need pass the reference to the right library. A better way
	 * should exist.
	 */
	public static class TopLevelCategory extends Category {
		public TopLevelCategory(ProjectLibrary library) {
			setLibrary(library);
		}
	}

	public void setReadOnly(boolean readOnly) {
		this.readOnly = readOnly;
	}
}
