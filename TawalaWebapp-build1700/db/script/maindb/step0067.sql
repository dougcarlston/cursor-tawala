begin transaction;
 	ALTER TABLE user_project ADD COLUMN require_ssl boolean;
 	UPDATE user_project SET require_ssl = false;
 	ALTER TABLE user_project ALTER COLUMN require_ssl SET NOT NULL;
 
	update schema_version set version = 67;
commit;