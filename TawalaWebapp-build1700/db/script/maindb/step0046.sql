begin transaction;
	ALTER TABLE project_invoice ALTER status_field_name TYPE character varying(100);
	ALTER TABLE project_invoice ALTER paid_amount_field_name TYPE character varying(100);
	update schema_version set version = 46;
commit;