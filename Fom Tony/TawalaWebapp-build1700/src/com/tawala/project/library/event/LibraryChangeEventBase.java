package com.tawala.project.library.event;

import java.util.Date;

import javax.persistence.Column;
import javax.persistence.DiscriminatorColumn;
import javax.persistence.DiscriminatorType;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Inheritance;
import javax.persistence.InheritanceType;
import javax.persistence.SequenceGenerator;
import javax.persistence.Table;
import javax.persistence.Temporal;
import javax.persistence.TemporalType;

import com.tawala.domain.User;
import com.tawala.message.Message;
import com.tawala.project.library.LibraryChangeEvent;

@SequenceGenerator(name = "SEQ_GEN", sequenceName = "seq_lib_event_id")

@Entity
@Inheritance(strategy=InheritanceType.SINGLE_TABLE)
@DiscriminatorColumn(
    name="eventtype",
    discriminatorType=DiscriminatorType.STRING,
    length=30
)
@Table(name="lib_event")
abstract public class LibraryChangeEventBase implements LibraryChangeEvent {
    @Id
    @Column(name = "lib_event_id")
    @GeneratedValue(strategy = GenerationType.SEQUENCE, generator = "SEQ_GEN")
    private long id;
    
    @Column(name = "created_dt", nullable = false)
    @Temporal(TemporalType.TIMESTAMP)
    private Date date;

    @Column(name = "user_id", length = 20, nullable = false)
    private String userId;

    protected LibraryChangeEventBase() {
        // For Hibernate's use
    }

    public LibraryChangeEventBase(String userId, Date date) {
        this.userId = userId;
        this.date = date;
    }

    public final long getId() {
        return id;
    }

    public final void setId(long id) {
        this.id = id;
    }

    public final Date getDate() {
        return date;
    }

    public final String getUserId() {
        return userId;
    }

    public void revertChanges(User user) throws Exception {
        throw new IllegalStateException(this.getClass()
                + " is unable to revert the changes.");
    }

    public Message getReversionDescription() {
        throw new IllegalStateException(this.getClass()
                + " is unable to provide reversion description message.");
    }
}
