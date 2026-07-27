begin transaction; 
    update project_library set description = 'Tawala Application Library' where project_library_id = 1;
    update project_library set description = 'User Community Library' where project_library_id = 2;
    update project_library set description = 'Under Construction' where project_library_id = 3;

    update schema_version set version = 29;
commit;