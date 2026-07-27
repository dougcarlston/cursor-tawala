package com.tawala.web.projectmanager.projectgroup;

import com.tawala.project.commands.Reference;

public class UpdateCoachStatusMemoController extends UpdateCoachRecordController {
	public static final String MEMO = "memo";

	@Override
	protected Reference getFieldReference() {
		return new Reference("Record:Coach:StatusMemo", true);
	}

	@Override
	protected String getValueParameterName() {
		return MEMO;
	}
	
}
