begin transaction;

    ALTER TABLE lib_event ADD COLUMN category_id int8;
    ALTER TABLE lib_event ADD COLUMN new_category_id int8;
    ALTER TABLE lib_event ADD COLUMN category_name varchar(255);

    update schema_version set version = 4;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;