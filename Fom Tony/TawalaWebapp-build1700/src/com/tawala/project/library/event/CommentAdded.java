package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorValue;
import javax.persistence.Entity;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.Comment;
import com.tawala.project.library.ProjectLibraryService;
import com.tawala.project.library.LibraryProject;

@Entity
@DiscriminatorValue("comment added")
public class CommentAdded extends ProjectChangeEventBase {
    @Column(name = "lib_comment_id")
    private long commentId;
    private static Message message = new Message("projectevent.comment.added");

    private CommentAdded() {
        // For Hibernate's use
    }

    public CommentAdded(LibraryProject project, Comment comment) {
        super(comment.getUserId(), project.getId(), new Date());
        commentId = comment.getId();
    }

    public Message getDescription() {
        return message;
    }

    public boolean isCapableOfReverting() {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        if (project == null || project.isDeleted())
            return false;

        Comment comment = ProjectLibraryService.findCommentById(commentId);
        return comment != null && !comment.isDeleted();
    }

    @Override
    public void revertChanges(User user) throws Exception {
        LibraryProject project = ProjectLibraryService
                .findProjectById(getProjectId());
        if (project == null || project.isDeleted())
            return;

        Comment comment = ProjectLibraryService.findCommentById(commentId);
        if (comment == null)
            return;

        ProjectLibraryService.onCommentDeletion(project, comment, user.getId());
    }

    @Override
    public Message getReversionDescription() {
        return new Message("projectevent.comment.added.reverted", getUserId(),
                getDate());
    }
}
