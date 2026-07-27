package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.message.Message;
import com.tawala.project.library.Comment;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("comment restored")
public class CommentRestored extends ProjectChangeEventBase {
    @Column(name = "comment_by_id", length = 20)
    private String commentUserId;

    @Column(name = "comment_dt")
    private Date commentDate;

    private CommentRestored() {
        // For Hibernate's use
    }

    public CommentRestored(LibraryProject project, String userId,
            Comment comment) {
        super(userId, project.getId(), new Date());
        this.commentDate = comment.getDate();
        this.commentUserId = comment.getUserId();
    }

    public Message getDescription() {
        return new Message("projectevent.comment.restored", commentUserId,
                commentDate);
    }

    public boolean isCapableOfReverting() {
        return false;
    }
}
