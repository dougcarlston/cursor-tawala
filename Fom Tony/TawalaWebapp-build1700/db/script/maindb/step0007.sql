begin transaction;
    alter table users add column password_reset bool;
    update users set password_reset = false;
    alter table users alter column password_reset set not null;

    update schema_version set version = 7;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;