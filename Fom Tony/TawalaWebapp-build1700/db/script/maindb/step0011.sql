begin transaction;

    ALTER TABLE lib_project ADD COLUMN last_updated_dt timestamp;
    update lib_project set last_updated_dt = 
	(select max(created_dt) from lib_project_version v, lib_project_version_map vm 
		where lib_project.lib_project_id = vm.lib_project_id and vm.lib_project_version_id = v.lib_project_version_id);
    update lib_project set last_updated_dt = created_dt where last_updated_dt is null;

    ALTER TABLE lib_project ALTER COLUMN last_updated_dt SET NOT NULL;

    update schema_version set version = 11;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;