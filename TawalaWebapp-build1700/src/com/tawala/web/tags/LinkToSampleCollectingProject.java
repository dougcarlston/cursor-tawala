package com.tawala.web.tags;

import com.tawala.project.UserProject.EntryPointType;

public class LinkToSampleCollectingProject extends LinkToProjectBase {
    private static final long serialVersionUID = 1L;

	@Override
	protected EntryPointType getEntryPointType() {
		return EntryPointType.SAMPLE_COLLECTION;
	}
}
