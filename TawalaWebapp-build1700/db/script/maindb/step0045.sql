begin transaction;
	ALTER TABLE project_invoice ADD COLUMN paid_amount_field_name character varying(40);

	update schema_version set version = 45;
commit;