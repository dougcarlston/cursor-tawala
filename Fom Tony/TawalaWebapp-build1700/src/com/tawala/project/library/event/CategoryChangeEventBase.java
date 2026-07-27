package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;

import com.tawala.project.library.Category;
import com.tawala.project.library.ProjectLibraryService;

@Entity
public abstract class CategoryChangeEventBase extends LibraryChangeEventBase {
    @Column(name = "category_id")
    private long categoryId;
    
    @Column(name = "category_name")
    private String categoryName;
    
    protected CategoryChangeEventBase() {
        //--- For Hibernate's use.
    }
    
    public CategoryChangeEventBase(String userId, Category category, Date date) {
        super(userId, date);
        this.categoryId = category.getId();
        this.categoryName = category.getFullName();
    }
    
    public boolean isProjectRelated() {
        return false;
    }

    public long getCategoryId() {
        return categoryId;
    }
    
    public Category getCategory() {
        return ProjectLibraryService.findCategoryById(categoryId);
    }
    
    public String getCategoryName() {
        return categoryName;
    }
}
