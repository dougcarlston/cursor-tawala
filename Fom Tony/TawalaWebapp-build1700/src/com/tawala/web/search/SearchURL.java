package com.tawala.web.search;

public enum SearchURL {
	INDEX_PROJECT {
		public String url() {
			return "/project/index";
		}
	},
	SEARCH_PROJECT {
		public String url() {
			return "/project/search";
		}
	},
	DELETE_PROJECT {
		public String url() {
			return "/project/delete";
		}
	},
	DELETE_USER {
		public String url() {
			return "/user/delete";
		}
	},
	INDEX_USER {
		public String url() {
			return "/user/index";
		}
	},
	SEARCH_USER {
		public String url() {
			return "/user/search";
		}
	};

	public abstract String url();
}
