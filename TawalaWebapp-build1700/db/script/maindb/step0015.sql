begin transaction;  
    ALTER TABLE lib_project ADD COLUMN icon_url varchar(100);
    ALTER TABLE lib_project ADD COLUMN video_url varchar(100);
	
    update schema_version set version = 15;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;