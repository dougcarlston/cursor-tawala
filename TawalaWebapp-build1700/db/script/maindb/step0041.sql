begin transaction; 
	CREATE INDEX user_project_link_authentication_token
		ON user_project_link (authentication_token);

	update schema_version set version = 41;
commit;