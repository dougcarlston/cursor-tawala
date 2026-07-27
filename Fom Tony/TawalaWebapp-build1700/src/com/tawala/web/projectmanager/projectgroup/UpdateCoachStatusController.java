package com.tawala.web.projectmanager.projectgroup;

import com.tawala.project.commands.Reference;

public class UpdateCoachStatusController extends UpdateCoachRecordController {
	public static final String STATUS_ID = "status_id";

	@Override
	protected Reference getFieldReference() {
		return new Reference("Record:Coach:StatusID", true);
	}

	@Override
	protected String getValueParameterName() {
		return STATUS_ID;
	}

}
