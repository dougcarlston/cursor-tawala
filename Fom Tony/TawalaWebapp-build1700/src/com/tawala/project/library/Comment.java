package com.tawala.project.library;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Lob;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_lib_comment_id")
@Entity
@Table(name = "lib_comment")
@ Cache( usage=CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region="comments" )
public class Comment {
    @Id
    @Column(name = "comment_id")
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
    private long id;

    @Column(name = "user_id", length = 20, nullable = false)
    private String userId;

    @Column(name = "comment_text", nullable = false)
    @Lob
    private String text;

    @Column(name = "created_dt", nullable = false)
    @Temporal(TemporalType.TIMESTAMP)
    private Date date;

    @Column(name = "deleted", nullable = false)
    private boolean deleted;

    Comment() {
        // For Hibernate's use
    }

    public Comment(String userId) {
        this.userId = userId;
        this.date = new Date();
    }

    public void setText(String text) {
        this.text = text;
    }

    public Date getDate() {
        return date;
    }

    public String getText() {
        return text;
    }

    public String getUserId() {
        return userId;
    }

    @Override
    public boolean equals(Object obj) {
        Comment other = (Comment) obj;
        return this.date.equals(other.date) && this.userId.equals(other.userId)
                && this.text.equals(other.text);
    }

    public long getId() {
        return id;
    }

    public void setId(long id) {
        this.id = id;
    }

    public boolean isDeleted() {
        return deleted;
    }

    public void setDeleted(boolean deleted) {
        this.deleted = deleted;
    }
}
