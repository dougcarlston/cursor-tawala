begin transaction; 
    alter table user_project add last_updated_dt timestamp;
    update user_project set 
	last_updated_dt = (select created_dt from project_version where project_version_id = user_project.deployed_version_id);
    alter table user_project alter last_updated_dt set not null;

    update schema_version set version = 30;
commit;