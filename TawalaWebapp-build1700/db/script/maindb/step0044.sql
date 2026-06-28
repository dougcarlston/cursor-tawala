begin transaction;
	ALTER TABLE project_invoice ALTER project_invoice_id TYPE character varying(20);
	DROP SEQUENCE project_invoice_id;
	update schema_version set version = 44;
commit;