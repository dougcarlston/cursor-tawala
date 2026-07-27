begin transaction;

	ALTER TABLE lib_project ADD COLUMN testdrive_count int4;
	UPDATE lib_project SET testdrive_count = 0;
	ALTER TABLE lib_project ALTER COLUMN testdrive_count SET NOT NULL;

    update schema_version set version = 5;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;