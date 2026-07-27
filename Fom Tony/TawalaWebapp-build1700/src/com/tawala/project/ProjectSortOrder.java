package com.tawala.project;

public enum ProjectSortOrder {
	responseCountAscending {
		public String getSortOrderString() {
			return "responseCount asc, name asc";
		}
		public String getDescription() {
			return "response count in ascending order";
		}
	},
	responseCountDescending {
		public String getSortOrderString() {
			return "responseCount desc, name asc";
		}
		public String getDescription() {
			return "response count in descending order";
		}
	},
	lastAccessedDateAscending{
		public String getSortOrderString() {
			return "lastAccessed asc, name asc";
		}
		public String getDescription() {
			return "last accessed date in ascending order";
		}
	},
	lastAccessedDescending {
		public String getSortOrderString() {
			return "lastAccessed desc, name asc";
		}
		public String getDescription() {
			return "last accessed date in descending order";
		}
	},

	lastUpdatedDateAscending{
		public String getSortOrderString() {
			return "last_updated_dt asc, name asc";
		}
		public String getDescription() {
			return "last updated date in ascending order";
		}
	},
	lastUpdatedDateDescending {
		public String getSortOrderString() {
			return "last_updated_dt desc, name asc";
		}
		public String getDescription() {
			return "last updated date in descending order";
		}
	},
	nameAscending {
		public String getSortOrderString() {
			return "name asc";
		}
		public String getDescription() {
			return "name in ascending order";
		}
	},
	nameDescending{
		public String getSortOrderString() {
			return "name desc";
		}
		public String getDescription() {
			return "name in descending order";
		}
	},
	createdDateAscending{
		public String getSortOrderString() {
			return "created_dt asc, name asc";
		}
		public String getDescription() {
			return "date created in ascending order";
		}
	},
	createdDateDescending {
		public String getSortOrderString() {
			return "created_dt desc, name asc";
		}
		public String getDescription() {
			return "date created in descending order";
		}
	};
	
	abstract public String getSortOrderString();
	abstract public String getDescription();
}
