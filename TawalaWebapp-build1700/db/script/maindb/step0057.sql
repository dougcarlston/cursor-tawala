begin transaction;
	ALTER TABLE user_image RENAME TO user_file;
	ALTER TABLE user_file ALTER user_image_id TYPE character varying(15);
	ALTER TABLE user_file RENAME user_image_id  TO user_file_id;
	ALTER TABLE user_file RENAME suffix  TO file_name;
	ALTER TABLE user_file ALTER file_name TYPE character varying(200);
	update user_file set file_name = 'file.' || file_name;
	ALTER TABLE user_file ADD COLUMN user_project_id bigint;
	ALTER TABLE user_file ADD FOREIGN KEY (user_project_id) REFERENCES user_project (user_project_id) ON DELETE CASCADE;

	ALTER TABLE user_theme ALTER header_image_id TYPE character varying(15);
	
	update schema_version set version = 57;
commit;