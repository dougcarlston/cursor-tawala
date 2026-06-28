begin transaction;
	CREATE TRIGGER t_user_project_backup BEFORE DELETE ON user_project_backup
	     FOR EACH ROW EXECUTE PROCEDURE lo_manage(backup_data);

	update schema_version set version = 3;
commit;