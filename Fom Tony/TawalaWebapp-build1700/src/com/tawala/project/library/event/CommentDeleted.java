package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.Comment;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("comment deleted")
public class CommentDeleted extends ProjectChangeEventBase {
    @Column(name = "lib_comment_id")
    private long commentId;

    @Column(name = "comment_by_id", length = 20)
    private String commentUserId;

    @Column(name = "comment_dt")
    @Temporal(TemporalType.TIMESTAMP)
    private Date commentDate;

    private CommentDeleted() {
        // For Hibernate's use
    }

    public CommentDeleted(LibraryProject project, String userId,
            Comment comment) {
        super(userId, project.getId(), new Date());
        this.commentDate = comment.getDate();
        this.commentUserId = comment.getUserId();
        this.commentId = comment.getId();
    }

    public Message getDescription() {
        return new Message("projectevent.comment.deleted", commentUserId,
                commentDate);
    }

    public boolean isCapableOfReverting() {
        Comment comment = ProjectLibraryService.findCommentById(commentId);
        return comment != null && comment.isDeleted();
    }

    @Override
    public void revertChanges(User user) throws Exception {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        if (project == null)
            return;

        Comment comment = ProjectLibraryService.findCommentById(commentId);
        if (comment == null || !comment.isDeleted())
            return;

        ProjectLibraryService.onCommentRestoration(project, comment, user.getId());
    }

    @Override
    public Message getReversionDescription() {
        return new Message("projectevent.comment.deleted.reverted",
                commentUserId, commentDate);
    }
}
