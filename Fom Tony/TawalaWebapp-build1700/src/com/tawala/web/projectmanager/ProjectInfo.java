package com.tawala.web.projectmanager;

import java.text.NumberFormat;

import com.tawala.World;
import com.tawala.domain.User;
import com.tawala.project.Form;
import com.tawala.project.Project;
import com.tawala.project.UserProject;
import com.tawala.project.library.LibraryProject;
import com.tawala.project.library.ProjectLibraryService;

public class ProjectInfo {
    private World world;
    private UserProject project;
    private long responses;
    private String size;
    private LibraryProject submittedProject;
    private boolean triedToLoadLibraryProject = false;

    public ProjectInfo(World world, UserProject project, User user) {
        this.world = world;
        this.project = project;
        this.responses = getProjectResponses(project.getProject(), world);
        this.size = "";
    }

    public UserProject getProject() {
        return project;
    }

    public long getResponses() {
        return (this.responses);
    }

    public String getSize() {
        return (this.size);
    }

    private long getProjectResponses(Project project, World world) {
    	return world.domain().storedData().responseCount(project);
    }

    public boolean isCanUpdateProjectInLibrary() {
        return getSubmittedProject() != null
                && project.getLibraryVersionNumber() == null;
    }

    public boolean isLibraryHasNewerVersion() {
        return getSubmittedProject() != null
                && project.getLibraryVersionNumber() != null
                && project.getLibraryVersionNumber() < getSubmittedProject()
                        .getLatestVersionNumber();
    }

    public LibraryProject getSubmittedProject() {
        if (triedToLoadLibraryProject)
            return submittedProject;

        if (project.getLibraryProjectId() != null) {
            submittedProject = ProjectLibraryService.findProjectById(project
                    .getLibraryProjectId());
            if (submittedProject != null && submittedProject.isDeleted()) {
                submittedProject = null;
            }
        }

        triedToLoadLibraryProject = true;

        return submittedProject;
    }

    public static long getFormResponsesSize(Form form, Project project,
            World world) {
        long result = 0;
        result += world.domain().storedData().sizeOfResponses(project,
                form.getName());
        return result;
    }

    private static long getProjectSize(Project project, World world) {
        return world.domain().projects().fileSize(project, world);
    }

    public static long getProjectResponsesSize(Project project, World world) {
        long result = 0;
        for (Form form : project.getForms()) {
            result += world.domain().storedData().sizeOfResponses(project,
                    form.getName());
        }
        return result;
    }

    public static String convertSize(Long size) {
        String result = "";
        if (size > 1024000) {
            result = size / 1024000 + " M";
        }
        if (size > 1024) {
            result = size / 1024 + " K";
        } else {
            result = "" + size + " B";
        }
        return result;
    }

    public static String getStoragePercent(long size) {
        float totalStorage = 10240000;
        NumberFormat nf = NumberFormat.getPercentInstance();
        nf.setMaximumFractionDigits(2);
        nf.setMaximumIntegerDigits(3);
        String percent = nf.format(((float) size / totalStorage));
        return percent;
    }

    public String getProjectSize() {
        return convertSize(getProjectSize(project.getProject(), world))
                + " ("
                + ProjectInfo.getStoragePercent(ProjectInfo.getProjectSize(
                        project.getProject(), world)) + ")";
    }

    public String getResponsesSize() {
        return ProjectInfo.convertSize(ProjectInfo.getProjectResponsesSize(
                project.getProject(), world))
                + " ("
                + ProjectInfo.getStoragePercent(ProjectInfo
                        .getProjectResponsesSize(project.getProject(), world))
                + ")";
    }

}
