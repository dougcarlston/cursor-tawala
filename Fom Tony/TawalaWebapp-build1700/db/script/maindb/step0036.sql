begin transaction; 
    alter table user_project add offline boolean;
    update user_project set offline = false;
    alter table user_project alter offline set not null;

    update schema_version set version = 36;
commit;