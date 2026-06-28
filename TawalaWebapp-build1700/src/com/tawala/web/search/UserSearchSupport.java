package com.tawala.web.search;

import com.tawala.search.InProcessUserIndexer;

abstract public class UserSearchSupport extends SerializationSupport {
	private InProcessUserIndexer userIndexer;

	public InProcessUserIndexer getUserIndexer() {
		return userIndexer;
	}

	public void setUserIndexer(InProcessUserIndexer userIndexer) {
		this.userIndexer = userIndexer;
	}
}
