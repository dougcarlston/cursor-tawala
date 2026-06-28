begin transaction;
	ALTER TABLE user_project ADD COLUMN po_number varchar(20);
	ALTER TABLE user_project ADD COLUMN invoice_number varchar(20);

	update schema_version set version = 61;
commit;