begin transaction;
	ALTER TABLE user_project ADD COLUMN ysl_league_id varchar(20);
	ALTER TABLE user_project ADD COLUMN ysl_last_updated timestamp;

	ALTER TABLE user_project ADD CONSTRAINT un_ysl_league_id UNIQUE (ysl_league_id);

	update schema_version set version = 63;
commit;