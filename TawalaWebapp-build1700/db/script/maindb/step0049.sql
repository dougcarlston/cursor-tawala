begin transaction;
    CREATE INDEX ix_user_project_backup_schedule_next_dt
	   ON user_project_backup_schedule (next_dt);

	update schema_version set version = 49;
commit;