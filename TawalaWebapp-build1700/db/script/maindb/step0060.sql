begin transaction;
	ALTER TABLE user_project ADD COLUMN reg_start_dt date;
	ALTER TABLE user_project ADD COLUMN reg_close_dt date;
	ALTER TABLE user_project ADD COLUMN reg_closed boolean;
	ALTER TABLE user_project ADD COLUMN reg_fee numeric(6,2);
	ALTER TABLE user_project ADD COLUMN reg_invoice_dt date;
	ALTER TABLE user_project ADD COLUMN project_type varchar(20);

	CREATE INDEX ix_user_project_project_type
   		ON user_project (project_type);

	update schema_version set version = 60;
commit;