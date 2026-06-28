begin transaction; 
    alter table users add normal_user_name varchar(40);
    update users set normal_user_name = lower(user_name);
    alter table users  alter normal_user_name set not null;

    update schema_version set version = 39;
commit;