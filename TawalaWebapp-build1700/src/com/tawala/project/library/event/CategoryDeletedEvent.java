package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.Category;

@Entity
@DiscriminatorValue("category deleted")
public class CategoryDeletedEvent extends CategoryChangeEventBase {
    @Column(name = "prev_string_value")
    private String previousName;

    private CategoryDeletedEvent() {
        // For Hibernate's use
    }

    public CategoryDeletedEvent(User user, Category category) {
        super(user.getId(), category, new Date());
        this.previousName = category.getName();
    }

    public Message getDescription() {
        return new Message("projectevent.category.deleted",
                previousName);
    }

    public boolean isCapableOfReverting() {
        return false;
    }
}
