package com.tawala.project;

public enum ProjectFilter {
	all {
		@Override
		public String getFilteringClause() {
			return "";
		}
		
		public String getDescription() {
			return "All";
		}
	},
	activeOnly {
		public String getFilteringClause() {
			return " AND p.offline = false";
		}
		public String getDescription() {
			return "Active only";
		}
	},
	inactiveOnly {
		public String getFilteringClause() {
			return " AND p.offline = true";
		}
		
		public String getDescription() {
			return "Inactive only";
		}
	};
	
	abstract public String getDescription();
	abstract public String getFilteringClause();
}
