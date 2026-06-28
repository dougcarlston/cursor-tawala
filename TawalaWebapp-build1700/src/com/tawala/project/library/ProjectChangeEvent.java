package com.tawala.project.library;


public interface ProjectChangeEvent extends LibraryChangeEvent {
    long getProjectId();
}
