begin transaction; 
	ALTER TABLE users ADD COLUMN paypal_account character varying(50);

	update schema_version set version = 42;
commit;