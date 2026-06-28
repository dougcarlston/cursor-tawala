begin transaction;
 	ALTER TABLE user_project ADD COLUMN roster_template_id varchar(40);
 
	update schema_version set version = 66;
commit;