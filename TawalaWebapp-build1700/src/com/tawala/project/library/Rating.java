package com.tawala.project.library;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import org.hibernate.annotations.Cache;
import org.hibernate.annotations.CacheConcurrencyStrategy;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_lib_project_rating_id")
@Entity
@Table(name = "lib_project_rating")
@ Cache( usage=CacheConcurrencyStrategy.NONSTRICT_READ_WRITE, region="ratings" )
public class Rating {
    @Id
    @Column(name = "project_rating_id")
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
    private long id;

    @Column(name = "user_id", length = 20, nullable = false)
    private String userId;

    @Column(name = "note", length = 100, nullable = true)
    private String text;

    @Column(name = "created_dt", nullable = false)
    @Temporal(TemporalType.TIMESTAMP)
    private Date date;

    @Column(name = "value", nullable = false)
    private int value;

    private Rating() {
        // For Hibernate's use
    }

    public Rating(String userId) {
        this.userId = userId;
        this.date = new Date();
    }

    /**
     * @param rating
     *            The rating to set.
     */
    public void setValue(int rating) {
        this.value = rating;
    }

    /**
     * @param text
     *            The text to set.
     */
    public void setText(String text) {
        this.text = text;
    }

    /**
     * @return Returns the date.
     */
    public Date getDate() {
        return date;
    }

    /**
     * @return Returns the text.
     */
    public String getText() {
        return text;
    }

    /**
     * @return Returns the userId.
     */
    public String getUserId() {
        return userId;
    }

    /**
     * @return Returns the rating.
     */
    public int getValue() {
        return value;
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
}
