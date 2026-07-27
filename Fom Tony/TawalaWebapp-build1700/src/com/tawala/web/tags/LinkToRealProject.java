package com.tawala.web.tags;

import com.tawala.project.UserProject.EntryPointType;

public class LinkToRealProject extends LinkToProjectBase {
    private static final long serialVersionUID = 1L;

	@Override
	protected EntryPointType getEntryPointType() {
		return EntryPointType.REAL_PROJECT;
	}

}
