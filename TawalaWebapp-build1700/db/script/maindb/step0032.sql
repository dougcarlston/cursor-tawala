begin transaction; 
    alter table lib_project add clone_count int4;

    update lib_project set clone_count = (select count(*) from user_project up, lib_project_version_map lpvm 
		where up.original_lib_project_version_id = lpvm.lib_project_version_id and lpvm.lib_project_id = lib_project.lib_project_id);
		
    alter table lib_project alter clone_count set not null;
    alter table lib_project drop version;

    alter table project_library add show_cloned_count boolean;
    update project_library set show_cloned_count = false;
    update project_library set show_cloned_count = true where project_library_id = 1;
    alter table project_library alter show_cloned_count set not null;

    update schema_version set version = 32;
commit;