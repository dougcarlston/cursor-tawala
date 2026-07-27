begin transaction;

    ALTER TABLE users ADD COLUMN last_logged_in_dt timestamp;
    update schema_version set version = 12;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;