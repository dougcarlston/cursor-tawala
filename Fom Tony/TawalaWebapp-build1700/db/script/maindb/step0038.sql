begin transaction; 
    alter table users add enable_ads boolean;
    update users set enable_ads = false;
    alter table users  alter enable_ads set not null;

    update schema_version set version = 38;
commit;