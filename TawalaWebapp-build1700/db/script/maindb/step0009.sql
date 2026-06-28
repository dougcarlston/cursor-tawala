begin transaction;
    ALTER TABLE user_project ADD COLUMN deployed_version_id int8;
    update user_project set deployed_version_id = 
	(select project_version_id from project_version v 
		where v.user_project_id = user_project.user_project_id 
			and v.version_number = user_project.next_version - 1);

    update schema_version set version = 9;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;