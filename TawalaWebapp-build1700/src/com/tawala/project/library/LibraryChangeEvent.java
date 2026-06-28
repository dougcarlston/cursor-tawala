package com.tawala.project.library;

import java.util.Date;

import com.tawala.domain.User;
import com.tawala.message.Message;

public interface LibraryChangeEvent {

    void setId(long id);

    long getId();

    Date getDate();

    Message getDescription();

    Message getReversionDescription();

    String getUserId();

    boolean isCapableOfReverting();

    void revertChanges(User user) throws Exception;
    
    boolean isProjectRelated();
}
