begin transaction;

    ALTER TABLE users ALTER user_name TYPE varchar(40);

    update schema_version set version = 3;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;