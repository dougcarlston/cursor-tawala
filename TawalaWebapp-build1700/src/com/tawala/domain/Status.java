package com.tawala.domain;

import com.thoughtworks.xstream.annotations.XStreamAlias;

@XStreamAlias(value = "status", impl = Status.class)
public enum Status {
    EMAIL_UNVALIDATED {
        public boolean isAllowedToLogOn() {
            return false;
        }

        public boolean hasEmailBeenValidated() {
            return false;
        }
        public boolean getCanBeApproved() {
            return true;
        }
        public boolean isOnlyAllowedToViewVettedLibraryProjects() {
        	throw new IllegalStateException("Should never be called");
        }
        public boolean isAllowedToViewCompleteLibrary() {
        	throw new IllegalStateException("Should never be called");
        }
        public boolean isAllowedToViewDesigner() {
        	throw new IllegalStateException("Should never be called");
        }
        public boolean isAllowedToUpdateLibraryProjects() {
        	throw new IllegalStateException("Should never be called");
        }
    },
    EMAIL_VALIDATED {
        public boolean isAllowedToLogOn() {
            return false;
        }
        public boolean hasEmailBeenValidated() {
            return true;
        }
        public boolean getCanBeApproved() {
            return true;
        }
        public boolean isOnlyAllowedToViewVettedLibraryProjects() {
        	throw new IllegalStateException("Should never be called");
        }
        public boolean isAllowedToViewCompleteLibrary() {
        	throw new IllegalStateException("Should never be called");
        }
        public boolean isAllowedToViewDesigner() {
        	throw new IllegalStateException("Should never be called");
        }
        public boolean isAllowedToUpdateLibraryProjects() {
        	throw new IllegalStateException("Should never be called");
        }
    },
    REGISTERED {
        public boolean isAllowedToLogOn() {
            return true;
        }
        public boolean hasEmailBeenValidated() {
            return true;
        }
        public boolean getCanBeApproved() {
            return false;
        }
        public boolean isOnlyAllowedToViewVettedLibraryProjects() {
        	return true;
        }
        public boolean isAllowedToViewCompleteLibrary() {
        	return true;
        }
        public boolean isAllowedToViewDesigner() {
        	return true;
        }
        public boolean isAllowedToUpdateLibraryProjects() {
        	return true;
        }
    },
    REGISTERED_INITIAL {
        public boolean isAllowedToLogOn() {
            return true;
        }
        public boolean hasEmailBeenValidated() {
            return true;
        }
        public boolean getCanBeApproved() {
            return false;
        }
        public boolean isOnlyAllowedToViewVettedLibraryProjects() {
        	return true;
        }
        public boolean isAllowedToViewCompleteLibrary() {
        	return false;
        }
        public boolean isAllowedToViewDesigner() {
        	return false;
        }
        public boolean isAllowedToUpdateLibraryProjects() {
        	return false;
        }
    };
    
    abstract public boolean isAllowedToLogOn();
    abstract public boolean hasEmailBeenValidated();
    abstract public boolean getCanBeApproved();
    abstract public boolean isOnlyAllowedToViewVettedLibraryProjects();
    abstract public boolean isAllowedToViewCompleteLibrary();
    abstract public boolean isAllowedToViewDesigner();
    abstract public boolean isAllowedToUpdateLibraryProjects();
}
