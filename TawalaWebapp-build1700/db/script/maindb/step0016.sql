begin transaction;  
    ALTER TABLE lib_project ADD COLUMN featured_order int;
	
    update schema_version set version = 16;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;