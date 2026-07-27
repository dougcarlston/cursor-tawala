begin transaction; 
    alter table project add designer_version varchar(10);

    update schema_version set version = 37;
commit;