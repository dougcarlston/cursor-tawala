begin transaction; 
	ALTER TABLE users  ADD CONSTRAINT users_normal_user_name_key UNIQUE(normal_user_name);
	update schema_version set version = 40;
commit;